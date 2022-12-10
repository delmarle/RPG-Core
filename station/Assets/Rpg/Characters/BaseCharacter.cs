using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    
    public partial class BaseCharacter : CoreCharacter, IEntityState
    {
        #region FIELDS

        private AreaContainerSystem _containerSystem;
        private ItemsDb _itemDb;
        private DbSystem _dbSystem;
        private RpgSettingsDb _gameSettingsDb;
        private StationMechanics _mechanics;

        public FloatingPopupAnchor FloatingPopupAnchor;
        public CharacterUpdate OnVitalsUpdated;
        public CharacterUpdate OnStatisticUpdated;
        public CharacterUpdate OnAttributesUpdated;
        public CharacterVitalChange OnDamaged;
        public CharacterVitalChange OnHealed;
        public CharacterVitalChange OnEnergyChange;
   
        public CharacterUpdate OnRevived;
        public CharacterTargetUpdated OnTargetChanged;
        public CharacterSkillUpdate OnSkillUpdated;
        public CharacterSkillUpdate OnSkillGained;
        public CharacterSkillUpdate OnSkillRemoved;

        private string _instancedLootContainedId;
        private Interactible _activeLootInteraction;
        public delegate void CharacterTargetUpdated(BaseCharacter target);
 
        public delegate void CharacterSkillUpdate(BaseCharacter character, RankProgression progress, int data);
        public delegate void CharacterVitalChange(BaseCharacter character, VitalChangeData data);
       
        [SerializeField] private CharacterInputHandler _inputHandler = null;
        public CharacterInputHandler GetInputHandler => _inputHandler;

        private CharacterControl _control;
        public CharacterControl Control => _control;

        private StatsHandler _stats = null;
        public StatsHandler Stats => _stats;
        
        private SkillHandler _skills = null;
        public SkillHandler Skills => _skills;

        private CharacterCalculation _calculatorInstance = null;
        public CharacterCalculation Calculator => _calculatorInstance;

        private ActionHandler _action = null;
        public ActionHandler Action => _action;

        private CharacterBrain _brain;
        protected CharacterMemoryHandler _memoryHandler;
        public CharacterMemoryHandler Memory => _memoryHandler;

        private LimbsReference _limbs;

        public Transform GetLimbs(string limbName)
        {
            if (_limbs == null)
            {
                return transform;
            }

            return _limbs.GetLimb(limbName);
        }

        private EntityInteraction _interaction;

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

                    GenerateLootInteractable();
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

        public string GetRaceID()
        {
            return _raceId;
        }

        private string _genderId;

        public string GetGender()
        {
            return _genderId;
        }

        private string _factionId;

        public string GetFactionID()
        {
            return _factionId;
        }
        
        private string _characterId;

        public string GetCharacterId()
        {
            return _characterId;
        }

    

        [SerializeField]private Renderer _characterVisual;
        private EquipmentHandler _equipmentHandler;
        public EquipmentHandler GetEquipment => _equipmentHandler;
        #endregion

        private void Awake()
        {
            _dbSystem = GameInstance.GetSystem<DbSystem>();
            _containerSystem = GameInstance.GetSystem<AreaContainerSystem>();
            _itemDb = GameInstance.GetDb<ItemsDb>();
            _gameSettingsDb = GameInstance.GetDb<RpgSettingsDb>();
            _control = GetComponent<CharacterControl>();
            _mechanics = _gameSettingsDb.Get().Mechanics;
            FloatingPopupAnchor = GetComponentInChildren<FloatingPopupAnchor>();
            _limbs = GetComponentInChildren<LimbsReference>();
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
            _memoryHandler?.DesActivate();
        }

        public void Init(
            string characterId, 
            string raceId,
            string factionId, 
            string genderId, 
            CharacterCalculation instance, 
            string characterName, 
            CharacterBrain brain,
            CharacterMemoryHandler memory)
        {
            SetTracked();
            _characterId = characterId;
            _raceId = raceId;
            _factionId = factionId;
            _genderId = genderId;
            AddMeta(StationConst.CHARACTER_NAME, characterName);
            AddMeta(StationConst.RACE_ID, raceId);
            AddMeta(StationConst.GENDER_ID, genderId);
            AddMeta(StationConst.FACTION_ID, factionId);
            _skills = new SkillHandler();
            _calculatorInstance = instance;
            _equipmentHandler = gameObject.GetComponent<EquipmentHandler>();
            if (_equipmentHandler)
            {
                _equipmentHandler.Setup(this);
            }
           
            _calculatorInstance.Setup(this);
            if (brain)
            {
                _brain = brain;
                brain.Setup(this);
            }

            if (memory)
            {
                _memoryHandler = memory;
                _memoryHandler.Activate(this);
            }
        }

        private void SetTracked()
        {
            Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
            if (rigidBody == null)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }

            Trackable component = gameObject.AddComponent<Trackable>();
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
        


        public void SetupAction(AttackData defaultAttack)
        {
           _action = new RpgActionHandler();
          
           _action.SetupDefaultAttack(defaultAttack);
           _action.SetAbilities(new List<RuntimeAbility>(), this);
           _action.Subscribe();
           _control.Setup();
        }

        public void SetupInteraction(EntityInteraction interaction, List<InteractionLine> lines)
        {
            _interaction = interaction;
            interaction.SetCharacterOwner(this);
            interaction.SetTopics(lines);
        }

        #region [[ FACTION & TARGETING ]]

        public Stance ResolveStance(BaseCharacter target)
        {
            var factionHandler = _mechanics.FactionHandler();
  
            var stance = factionHandler.ResolveStance(target._factionId, _factionId);

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
        
        #region [[ LOOTS ]]

        private LootInteractable _lootInteractable;
        private void GenerateLootInteractable()
        {
            if (_lootInteractable == null)
            {
                var settings = GameInstance.GetDb<ItemsSettingsDb>();
                var prefab = settings.Get().ContainerSettings.LootInteractionPrefab;
                if (prefab == null) return;
                
                _lootInteractable = Instantiate(prefab, transform);
            }
            TryGenerateLoot();
           _lootInteractable.SetContainerReference(_instancedLootContainedId);

        }
        private void TryGenerateLoot()
        {
            ClearPreviousLootContainer();
            var lootTableId = GetMeta<string>(StationConst.LOOT_TABLE_KEY);
            if (string.IsNullOrEmpty(lootTableId) == false)
            {
                GenerateLoots(lootTableId);
            }
        }
        private void ClearPreviousLootContainer()
        {
            if (string.IsNullOrEmpty(_instancedLootContainedId) == false)
            {
                _containerSystem.CleanContainer(_instancedLootContainedId);
            }
        }
        public void GenerateLoots(string lootTable)
        {
            var defaultItems = LootUtils.GenerateLootStack(lootTable);
            var currencies = LootUtils.GenerateCurrencies(lootTable);
            var containerState = new ContainerState(8, defaultItems);
            _instancedLootContainedId = Guid.NewGuid().ToString();
            var container = new ItemContainer(_instancedLootContainedId, containerState, _itemDb);
            _containerSystem.AddContainer(container, false);
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

        public EntityState GetState()
        {
            var st = new EntityState();

            return st;
        }
    }

    public enum Stance
    {
        Ally,
        Neutral,
        Enemy
    }
}
