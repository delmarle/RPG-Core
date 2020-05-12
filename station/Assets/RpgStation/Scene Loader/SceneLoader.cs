
#if DOTWEEN
	using DG.Tweening;
	#endif
using System;
using System.Collections;
using Station.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Station
{
	public class SceneLoader : MonoBehaviour
	{
		public static SceneLoader Instance;
		[SerializeField] private CanvasGroup _canvas = null;
		private const string BlankScene = "BlankScene";
		private const float FadeTime = 0.25f;
		private const float DelayBeforeFadeOut = 1.5f;


		private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
#if DOTWEEN
		private Tween _tweener;
		#endif
		private bool _isLoading;
		private string _currentScene;
		private float _progress;

		public float Progress => _progress;

		private void MakeSingleton()
		{
			if (Instance)
			{
				Debug.LogError("single scene loader ws created, it should not happen");
			}

			Instance = this;
		}

		#region Delegates

	
		public event Action<float> OnLoadingProgress;

		#endregion

		#region Monobehaviour

		private void Awake()
		{
			MakeSingleton();
			DontDestroyOnLoad(gameObject);
			_canvas.alpha = 0;
			_canvas.gameObject.SetActive(false);
		}

		#endregion

		public void LoadScene(string sceneName)
		{
			if (_isLoading) return;
			_currentScene = sceneName;
			_isLoading = true;
			var sceneSystem  = RpgStation.GetSystemStatic<SceneSystem>();
			if (sceneSystem.GetCurrentSceneType() == SceneType.Area)
			{
				GameGlobalEvents.OnTriggerSceneSave?.Invoke();
			}

			GameGlobalEvents.OnSceneStartLoad?.Invoke();
			UpdateProgressEvent(0);
			_canvas.gameObject.SetActive(true);
#if DOTWEEN
			_tweener = UiTween.SetCanvasFade(Ease.Linear, _canvas, true, FadeTime, 0);
			_tweener.OnComplete(() => { StartCoroutine(LoadSceneSequence()); }
			);
	#else
			_canvas.alpha = 1;
			_canvas.interactable = true;
			_canvas.blocksRaycasts = true;
			StartCoroutine(LoadSceneSequence());
#endif


		}

		private IEnumerator LoadSceneSequence()
		{
			yield return _waitForEndOfFrame;
			yield return LoadSceneAsync(BlankScene);
			yield return _waitForEndOfFrame;
			CleanMemory();
			UpdateProgressEvent(0.01f);
			yield return _waitForEndOfFrame;
			var asyncOperation = LoadSceneAsync(_currentScene);
			while (asyncOperation.isDone == false)
			{
				yield return _waitForEndOfFrame;
				UpdateProgressEvent(asyncOperation.progress* 0.75f);
			}
			
			yield return _waitForEndOfFrame;
			GameGlobalEvents.OnSceneInitialize?.Invoke();

			while (_progress<1f)
			{
				//waiting for initialize event
				yield return null;
			}
			yield return new WaitForSeconds(0.2f);
			GameGlobalEvents.OnSceneLoadObjects?.Invoke();

#if DOTWEEN
			_tweener = UiTween.SetCanvasFade(Ease.Linear, _canvas, false, FadeTime, DelayBeforeFadeOut);
			_tweener.OnComplete(() =>
				{
					_canvas.gameObject.SetActive(false);
					if (OnFinishLoadScene != null)
						OnFinishLoadScene();
					_isLoading = false;
				}
	
			);
			#else
			yield return new WaitForSeconds(DelayBeforeFadeOut);
			_canvas.alpha = 0;
			_canvas.interactable = false;
			_canvas.blocksRaycasts = false;
			#endif
			
			_isLoading = false;
			GameGlobalEvents.OnSceneReady?.Invoke();
		}

		public void UpdateProgressEvent(float progress)
		{
			_progress = progress;
			if (OnLoadingProgress != null)
			{
				OnLoadingProgress.Invoke(progress);
			}
		}

		private AsyncOperation LoadSceneAsync(string sceneName)
		{
			return SceneManager.LoadSceneAsync(sceneName);
		}

		private void CleanMemory()
		{
			GC.Collect();
			Resources.UnloadUnusedAssets();
		}

		private void OnDestroy()
		{
#if DOTWEEN
			_tweener.SafeKill();
	#endif
		}
	}
}

