using UnityEngine;
using UnityEngine.EventSystems;

namespace Station
{
	public class Finger
	{
		#region FIELDS
		public Vector2 StartScreenPosition;
		public Vector2 PreviousScreenPosition;
		public Vector2 ScreenPosition;
		public bool StartedOverGui;
		public GameObject PickedGameobject;
		public int FingerIndex;
		public float TimeAlive;
		public bool Touching;
		public bool PreviousTouching;
		public bool Swipe;
		public bool Tap;
		public int TapCount;
		#endregion
		
		public bool IsFingerDown{ get{ return Touching && !PreviousTouching; }}
		public bool IsFingerReleased{ get{ return !Touching && PreviousTouching; } }
		public Vector2 ScreenDelta{ get{ return ScreenPosition - PreviousScreenPosition;} }
		public Vector2 ScaledDelta{ get{ return ScreenDelta * TouchManager.ScaleMultiplier;} }
		public Vector2 SwipeScreenDelta{ get{ return ScreenPosition - StartScreenPosition;} }
		public Vector2 SwipeScaledDelta{ get{ return SwipeScreenDelta * TouchManager.ScaleMultiplier;} }
		public Ray GetRay(){ return Camera.main? Camera.main.ScreenPointToRay(ScreenPosition) : default(Ray); }
		public Ray GetStartRay(){ return Camera.main? Camera.main.ScreenPointToRay(StartScreenPosition):default(Ray); }

		public Vector3 GetPreviousWorldPosition(float distance)
		{
			return !Camera.main ? default(Vector3) : Camera.main.ScreenToWorldPoint(new Vector3(PreviousScreenPosition.x, PreviousScreenPosition.y, distance));
		}
		
		public Vector3 GetWorldPosition(float distance)
		{
			return !Camera.main ? default(Vector3) : Camera.main.ScreenToWorldPoint(new Vector3(ScreenPosition.x, ScreenPosition.y, distance));
		}

		public Vector3 GetWorldDelta(float distance)
		{
			return GetWorldDelta(distance, distance);
		}

		public Vector3 GetWorldDelta(float previousDistance, float distance)
		{

			return GetWorldPosition(distance) - GetPreviousWorldPosition(previousDistance);
		}
		
		public Vector3 GetStartWorldPosition(float distance)
		{
			return !Camera.main ? default(Vector3) : Camera.main.ScreenToWorldPoint(new Vector3(StartScreenPosition.x, StartScreenPosition.y, distance));
		}

		public bool IsOverUi()
		{
			if (EventSystem.current == null)
			{
				return false;
			}

			bool isPointerOverGameObject = false;

			if((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) && !Application.isEditor)
			{

				var touch = Input.touches[FingerIndex];
				if( touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended) {
					if(EventSystem.current.IsPointerOverGameObject( Input.touches[FingerIndex].fingerId )) {
						isPointerOverGameObject = true;
					}
				}
			} 
			else 
			{
				isPointerOverGameObject = EventSystem.current.currentSelectedGameObject != null;
			}
		    
			return isPointerOverGameObject;
		}

		public float GetScreenDistance(Vector2 point){ return Vector2.Distance(ScreenPosition, point); }

		public float GetScaledDistance(Vector2 point){ return GetScreenDistance(point) * TouchManager.ScaleMultiplier; }

		public float GetPreviousScreenDistance(Vector2 point){ return Vector2.Distance(PreviousScreenPosition, point); }

		public float GetPreviousScaledDistance(Vector2 point)
		{
			return GetPreviousScreenDistance(point) * TouchManager.ScaleMultiplier;
		}
		
		#region ANGLES
		public float GetRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(ScreenPosition.x - referencePoint.x, ScreenPosition.y - referencePoint.y);
		}
		public float GetDegrees(Vector2 referencePoint){ return GetRadians(referencePoint) * Mathf.Rad2Deg; }

		public float GetPreviousRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(PreviousScreenPosition.x - referencePoint.x, PreviousScreenPosition.y - referencePoint.y);
		}

		public float GetPreviousDegrees(Vector2 referencePoint)
		{
			return GetPreviousRadians(referencePoint) * Mathf.Rad2Deg;
		}

		public float GetDeltaRadians(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint);
		}

		public float GetDeltaRadians(Vector2 referencePoint, Vector2 previousReferencePoint)
		{
			var gprad = GetPreviousRadians(previousReferencePoint);
			var rad = GetRadians(referencePoint);
			var delta = Mathf.Repeat(gprad - rad, Mathf.PI * 2.0f);
			if (delta > Mathf.PI) delta -= Mathf.PI * 2.0f;
			
			return delta;
		}

		public float GetDeltaDegrees(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint) * Mathf.Rad2Deg;
		}

		public float GetDeltaDegrees(Vector2 referencePoint, Vector2 previousReferencePoint)
		{
			return GetDeltaRadians(referencePoint, previousReferencePoint) * Mathf.Rad2Deg;
		}
		#endregion

		public void Reset()
		{
			TimeAlive = 0.0f;
			Touching = false;
			PreviousTouching = false;
			Tap = false;
			Swipe = false;
		}
	}
}