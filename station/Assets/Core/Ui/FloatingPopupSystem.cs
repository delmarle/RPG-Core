using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
	public class FloatingPopupSystem : BaseSystem
	{
		public const string TYPE_DAMAGE = "damage";
		public const string TYPE_DAMAGE_CRITICAL = "damage_critical";
		public const string TYPE_MISS = "miss";
		public const string TYPE_EVADE = "evade";
		
		private Dictionary<string, FloatingPopupModel> _runtimeCache;
		private List<FloatingPopup> _activePopups = new List<FloatingPopup>();
		private Camera _camera;
		private GameObject _canvas;
		private static FloatingPopupSystem _instance;
		private static FloatingPopupDb _db;
		private SceneSystem _sceneSystem;

		private bool CheckCameraExist()
		{
			if (_camera == null)
			{
				_camera = Camera.main;
				return _camera != null;
			}

			return true;
		}
		
		protected override void OnInit()
		{
			GameGlobalEvents.OnBeforeLeaveScene.AddListener(OnBeforeLeaveScene);
		}

		protected override void OnDispose()
		{
			GameGlobalEvents.OnBeforeLeaveScene.RemoveListener(OnBeforeLeaveScene);
		}

		protected override void OnDataBaseReady()
		{
			_instance = this;

			_sceneSystem = GameInstance.GetSystem<SceneSystem>();
			_db = GameInstance.GetDb<FloatingPopupDb>();
			_runtimeCache = new Dictionary<string, FloatingPopupModel>();
			foreach (var entry in _db.Db.Values)
			{
				if (entry.Prefab != null)
				{
					_runtimeCache.Add(entry.Name, entry);
					PoolSystem.PopulatePool(entry.Prefab, entry.PoolSize);
				}
			}
			_canvas = GameObject.Find("canvas_floating_text");
		}
		
		
		private void OnBeforeLeaveScene()
		{
			var popups = _activePopups.ToArray();
			foreach (var popup in popups)
			{
				DeSpawnFloatingPopup(popup, 0);
			}
		}

		private void SpawnPopup(string popupType, string text, FloatingPopupAnchor anchor)
		{
			if (_sceneSystem.IsLoadingScene) return;
			if (_runtimeCache.ContainsKey(popupType) == false) return;
			if (CheckCameraExist() == false) return;
			
			var meta = _runtimeCache[popupType];
			var instance = PoolSystem.Spawn(meta.Prefab);
			instance.transform.SetParent(_canvas.transform, false);
			var floatingParams = new FloatingParams(text, anchor);
			FloatingPopup popupInstance = (FloatingPopup) instance;
			if (popupInstance == null)
			{
							
				instance.GetComponent<FloatingPopup>().Setup(floatingParams);
			}
			else
			{
				popupInstance.Setup(floatingParams);
			}
			_activePopups.Add(popupInstance);
		}

		public static void SpawnObject(string popupType, string text, FloatingPopupAnchor anchor, CoreCharacter source, CoreCharacter target, Sprite visual = null)
		{
			if (_instance)
			{
				if (ShouldShowPopup(popupType, source, target))
				{
					_instance.SpawnPopup(popupType, text, anchor);
				}
			}
		}

		public void DeSpawnFloatingPopup(FloatingPopup instance, float despawnTime)
		{
			_activePopups.Remove(instance);
			PoolSystem.Despawn(instance, despawnTime);
		}

		public static bool ShouldShowPopup(string popupType, CoreCharacter source, CoreCharacter owner)
		{
			if (owner == null)
			{
				return false;
			}

			var entry = _db.GetEntryByName(popupType);
			if (entry == null)
			{
				return false;
			}

			var ownerIdentity = (string)owner.GetMeta("identity");
			var sourceIdentity = (string)source.GetMeta("identity");
	
			if (ownerIdentity == IdentityType.ControlledCharacter.ToString())
			{
				var currentRule = entry.Rule.LeaderRules;
				if (currentRule.ByAny)
				{
					return true;
				}

				if (currentRule.ByLeader && sourceIdentity == IdentityType.ControlledCharacter.ToString())
				{
					return true;
				}
			}

			
			
			return true;
		}

	}

	
}

