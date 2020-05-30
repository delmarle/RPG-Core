using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    
    public class BaseCharacter : MonoBehaviour
    {
        #region FIELDS

        private DbSystem _dbSystem;
        private GameSettingsDb _gameSettingsDb;
        private StationMechanics _mechanics;

        public FloatingPopupAnchor FloatingPopupAnchor;
        public CharacterUpdate OnCharacterInitialized;
        public CharacterUpdate OnVitalsUpdated;
        public CharacterUpdate OnStatisticUpdated;
        public CharacterUpdate OnAttributesUpdated;
        public CharacterVitalChange OnDamaged;
        public CharacterVitalChange OnHealed;
        public CharacterVitalChange OnEnergyChange;
        public CharacterUpdate OnDie;
        public CharacterUpdate OnRevived;
        public CharacterTargetUpdated OnTargetChanged;

        public delegate void CharacterTargetUpdated(BaseCharacter target);
        public delegate void CharacterUpdate(BaseCharacter character);
        public delegate void CharacterVitalChange(BaseCharacter character, VitalChangeData data);
       
        [SerializeField] private CharacterInputHandler _inputHandler = null;
        public CharacterInputHandler GetInputHandler => _inputHandler;

        private CharacterControl _control;
        public CharacterControl Control => _control;

        private StatsHandler _stats = null;
        public StatsHandler Stats => _stats;

        private CharacterCalculation _calculatorInstance = null;
        public CharacterCalculation Calculator => _calculatorInstance;

        private ActionHandler _action = null;
        public ActionHandler Action => _action;

        private CharacterBrain _brain;

        private bool _isDead;

        public bool IsDead
        {
            get 
            { 
                return _isDead;
            }
            set
            {
                if (value && OnDie != null)
                {
                    if (_isDead)
                    {
                        Debug.LogError("died twice");
                    }

                    OnDie.Invoke(this);
                }

                _isDead = value;
            }
        }
        
        private BaseCharacter _target;
        public BaseCharacter Target
        {
            get { return _target; }
            set
            {
                _target = value;
                OnTargetChanged?.Invoke(value);
            }
        }

        private string _raceId;

        public string GetRace()
        {
            return _raceId;
        }

        private string _genderId;

        public string GetGender()
        {
            return _genderId;
        }

        
        private string _factionId;

        public string GetFaction()
        {
            return _factionId;
        }
        
        private string _characterId;

        public string GetCharacterId()
        {
            return _characterId;
        }

        private Dictionary<string, object> _meta = new Dictionary<string, object>();

        public object GetMeta(string key)
        {
            if (_meta.ContainsKey(key))
            {
                return _meta[key];
            }

            return "";
        }

        [SerializeField]private Renderer _characterVisual;
        #endregion

        private void Awake()
        {
            _dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            _gameSettingsDb = _dbSystem.GetDb<GameSettingsDb>();
            _control = GetComponent<CharacterControl>();
            _mechanics = _gameSettingsDb.Get().Mechanics;
            FloatingPopupAnchor = GetComponentInChildren<FloatingPopupAnchor>();
            if (FloatingPopupAnchor == null)
            {
                GameObject anchor = new GameObject("anchor");
                FloatingPopupAnchor = anchor.AddComponent<FloatingPopupAnchor>();
                anchor.transform.SetParent(transform);
                anchor.transform.position = GetTop();
            }
        }

        private void OnDestroy()
        {
            _action.Unsubscribe();
        }

        public void Init(
            string characterId, 
            string raceId,
            string factionId, 
            string genderId, 
            CharacterCalculation instance, 
            string characterName, 
            CharacterBrain brain)
        {

            _characterId = characterId;
            _raceId = raceId;
            _factionId = factionId;
            _genderId = genderId;
            AddMeta("name", characterName);

            _calculatorInstance = instance;
            
            _calculatorInstance.Setup(this);
            if (brain)
            {
                _brain = brain;
                brain.Setup(this);
            }
        }

        public void SetRenderer(Renderer cache)
        {
            _characterVisual = cache;
        }

        public void SetupStats(IdIntegerValue health, IdIntegerValue secondaryHealth, IdIntegerValue[] energies)
        {
            _stats = gameObject.AddComponent<StatsHandler>();
            _stats.Setup(this,health, secondaryHealth,energies);
        }
        
        public void AddMeta(string key, object value)
        {
            if (_meta.ContainsKey(key))
            {
                _meta[key] = value;
            }
            else
            {
                _meta.Add(key, value);
            }
        }

        public void SetupAction(AttackData defaultAttack)
        {
           _action = new RpgActionHandler();
          
           _action.SetupDefaultAttack(defaultAttack);
           _action.SetAbilities(new List<RuntimeAbility>(), this);
           _action.Subscribe();
        }

        #region [[ FACTION & TARGETING ]]

        public Stance ResolveStance(BaseCharacter requester)
        {
            var factionHandler = _mechanics.FactionHandler();
  
            var stance = factionHandler.ResolveStance(requester._factionId, _factionId);

            if (stance < 2)
            {
                return Stance.Ally;
            }
            else if (stance == 0)
            {
                return Stance.Neutral;
            }
            else
            {
                return Stance.Enemy;
            }
        }

        #endregion
        
        private void Update()
        {
            if (_brain && _inputHandler.UseAi)
            {
                _brain.TickBrain();
            }
            _action?.UpdateCombat();
            _action?.UpdateLoop();
        }
        
        #region effect related

        public Vector3 GetFeet()
        {
            if (_characterVisual == null)
            {
                return transform.position;
            }
            
            return _characterVisual.bounds.min;
        }

        public Vector3 GetCenter()
        {
            if (_characterVisual == null)
            {
                return transform.position;
            }

            return _characterVisual.bounds.center;
        }
        
        public Vector3 GetTop()
        {
            if (_characterVisual == null)
            {
                return transform.position;
            }

            var bounds = _characterVisual.bounds;
            Vector3 top = bounds.center;
            top.y += bounds.extents.y;
            return top;
        }
        #endregion

    }

    public enum Stance
    {
        Ally,
        Neutral,
        Enemy
    }
}
