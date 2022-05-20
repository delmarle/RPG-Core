using UnityEngine;
using System.Collections.Generic;
using System;



namespace Station
{
	public class TouchManager : MonoBehaviour
	{
		[SerializeField] private LayerMask _pickedObjectLayers = new LayerMask();
		private static readonly List<Finger> PooledFingers = new List<Finger>(MaxTouch);
		private static readonly List<Finger> ActiveFingers = new List<Finger>(MaxTouch);

		public static Action<Finger> OnTouchDown;
		public static Action<Finger> OnTouch;
		public static Action<Finger> OnTouchUp;
		public static Action<Finger> OnTap;
		public static Action<Finger> OnSwipe;

		private const int MaxTouch = 12;
		private const float TapMinTime = 0.5f;
		private const float SwipeMinDistance = 50.0f;
		private const int AverageDpi = 200;
		private const KeyCode PinchingKey = KeyCode.LeftControl;
		private const KeyCode SecondFingerKey = KeyCode.LeftAlt;
		
		private Vector2 ScreenCenter 
		{
			get { return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f); }
		}

		public static float ScaleMultiplier
		{
			get{ return Screen.dpi > 0 ? Mathf.Sqrt(AverageDpi) / Mathf.Sqrt(Screen.dpi) : 1;}
		}

		private static bool IsMousePressed
		{
			get
			{
				for (var i = 0; i < 4; i++) if (Input.GetMouseButton(i)) return true;

				return false;
			}
		}

		protected virtual void Update()
		{
			RefreshLists();
			CatchInputs();
			ProcessFingers();
			SendEvents();
		}

		private void RefreshLists()
		{
			for (var i = PooledFingers.Count - 1; i >= 0; i--) PooledFingers[i].TimeAlive += Time.unscaledDeltaTime;
	
			for (var i = ActiveFingers.Count - 1; i >= 0; i--) TryRefreshFinger(i);
		}

		private void TryRefreshFinger(int index)
		{
			var finger = ActiveFingers[index];
			if (finger.IsFingerReleased == false)
			{
				finger.PreviousTouching = finger.Touching;
				finger.PreviousScreenPosition = finger.ScreenPosition;

				finger.Tap = false;
				finger.Swipe = false;
				finger.Touching = false;
			}
			else
			{
				ActiveFingers.RemoveAt(index);
				PooledFingers.Add(finger);
				finger.TimeAlive = 0.0f;
			}
		}

		private void ProcessFingers()
		{
			for (var i = ActiveFingers.Count - 1; i >= 0; i--)
			{
				var finger = ActiveFingers[i];

				if (finger.IsFingerReleased)
				{
					if (finger.TimeAlive > TapMinTime)
					{
						finger.TapCount = 0;
					}
					else
					{
						if (finger.SwipeScreenDelta.magnitude * ScaleMultiplier < SwipeMinDistance)
						{
							finger.TapCount += 1;
							finger.Tap = true;
						}
						else
						{
							finger.TapCount = 0;
							finger.Swipe = true;
						}
					}
				}
				else if (finger.IsFingerDown == false) finger.TimeAlive += Time.unscaledDeltaTime;
			}
		}

		private void CatchInputs()
		{
			#if (UNITY_IOS || UNITY_ANDROID || UNITY_TVOS) && !UNITY_EDITOR
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{
						AddFinger(touch.fingerId, touch.position);
					}
				}
			}
			#endif
			#if UNITY_STANDALONE || UNITY_EDITOR
			if (IsMousePressed)
			{
				var screenBoundary = new Rect(0, 0, Screen.width, Screen.height);
				var cursorPosition = (Vector2) Input.mousePosition;
				if (!screenBoundary.Contains(cursorPosition)) return;
				
				AddFinger(0, cursorPosition);
				if (Input.GetKey(PinchingKey)) AddFinger(1, ScreenCenter - (cursorPosition - ScreenCenter));
				else if (Input.GetKey(SecondFingerKey)) AddFinger(1, cursorPosition);
			}
			#endif
		}

		private void SendEvents()
		{
			var fingerCount = ActiveFingers.Count;
			if (fingerCount <= 0) return;
			
			for (var i = 0; i < fingerCount; i++)
			{
				var finger = ActiveFingers[i];
				if (finger.IsFingerDown && OnTouchDown != null) OnTouchDown(finger);
				if (finger.Touching && OnTouch != null) OnTouch(finger);
				if (finger.IsFingerReleased && OnTouchUp != null) OnTouchUp(finger);
				if (finger.Tap && OnTap != null) OnTap(finger);
				if (finger.Swipe && OnSwipe != null) OnSwipe(finger);
			}

			//TODO check other gestures here
		}

		private void AddFinger(int index, Vector2 screenPosition)
		{
			var finger = FindFinger(index);
			if (finger == null)
			{
				var inactiveIndex = GetInactiveFingerId(index);

				if (inactiveIndex >= 0)
				{
					finger = PooledFingers[inactiveIndex];
					PooledFingers.RemoveAt(inactiveIndex);
					if (finger.TimeAlive > TapMinTime) finger.TapCount = 0;
				
					finger.Reset();
				}
				else
				{
					finger = new Finger {FingerIndex = index};
				}

				finger.StartScreenPosition = screenPosition;
				finger.PreviousScreenPosition = screenPosition;
				finger.ScreenPosition = screenPosition;
				finger.StartedOverGui = finger.IsOverUi();
			  RaycastUtils.RaycastTarget(screenPosition,100, _pickedObjectLayers, out finger.PickedGameobject);
				ActiveFingers.Add(finger);
			}

			finger.Touching = true;
			finger.ScreenPosition = screenPosition;
		}

		private Finger FindFinger(int index)
		{
			for (var i = ActiveFingers.Count - 1; i >= 0; i--)
			{
				var finger = ActiveFingers[i];

				if (finger.FingerIndex == index) return finger;
			}

			return null;
		}

		private int GetInactiveFingerId(int index)
		{
			for (var i = PooledFingers.Count - 1; i >= 0; i--)if (PooledFingers[i].FingerIndex == index) return i;
			
			return -1; //invalid
		}
	}
}