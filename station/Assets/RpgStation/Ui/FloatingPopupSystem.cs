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
		private Camera _camera;
		private GameObject _canvas;
		private static FloatingPopupSystem _instance;
		private static FloatingPopupDb _db;

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
			GameGlobalEvents.OnDataBaseLoaded.AddListener(OnDbReady);
		}

		protected override void OnDispose()
		{
			GameGlobalEvents.OnDataBaseLoaded.RemoveListener(OnDbReady);
		}

		private void OnDbReady()
		{
			_instance = this;
			var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
			_db = dbSystem.GetDb<FloatingPopupDb>();
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

		private void SpawnPopup(string popupType, string text, FloatingPopupAnchor anchor)
		{
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
		}

		public static void SpawnObject(string popupType, string text, FloatingPopupAnchor anchor, BaseCharacter source, BaseCharacter target, Sprite visual = null)
		{
			if (_instance)
			{
				if (ShouldShowPopup(popupType, source, target))
				{
					_instance.SpawnPopup(popupType, text, anchor);
				}
			}
		}

		public static bool ShouldShowPopup(string popupType, BaseCharacter source, BaseCharacter owner)
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

			var ownerIdentity = owner.GetMeta("identity");
			var sourceIdentity = source.GetMeta("identity");
	
			if (ownerIdentity == IdentityType.MainPlayer.ToString())
			{
				var currentRule = entry.Rule.LeaderRules;
				if (currentRule.ByAny)
				{
					return true;
				}

				if (currentRule.ByLeader && sourceIdentity == IdentityType.MainPlayer.ToString())
				{
					return true;
				}
			}

			
			
			return true;
		}

	}

	[Serializable]
	public class ShowPopupRule
	{
		public ShowRuleEntry LeaderRules = new ShowRuleEntry();
		public ShowRuleEntry TeamMemberRules= new ShowRuleEntry();
		public ShowRuleEntry NpcRules = new ShowRuleEntry();
		public ShowRuleEntry PetRules = new ShowRuleEntry();

		public bool ShowOnlyVisible;
		public float ShowRangeLimit = -1;
	}

	public class ShowRuleEntry
	{
		public bool ByAny = true;
		public bool ByLeader;
		public bool ByTeamMember;
		public bool ByNpc;
		public bool ByPet;
	}
}

