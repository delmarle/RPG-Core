# RpgStationUnity
# What is in progress: 
- Items with different types and functionalities.
- Container, for players inventories or chests..
- Equipment
# Next features: 
- Status Effects extended
- Hate system
- Npc Basic Ai
- Counters system
- Quest system
- integrations with other assets
- Crafting

# Feature list:
- Saving system

- hotswappable Mechanics:
  - Swappable character creation
  - Swappable Ui
  - Swapable character Controller / input / camera
- Stats: Attributes, Vitals, Statistics, Elements
- Characters: Races, Player classes, Npcs
- Party, switch Character
- Factions
- Abilities
- Skills
- World, Scenes Data, Spawners, Portals
- Interaction: Button, harvestNode, portal
- Spawning for characters and any objects, use rules and state can be saved 

# Later features: 
-Pets 
-Difficulty
-Localization

![alt text](https://i.gyazo.com/f4625456c7d35ba03a5c93b438253aaa.png)
![alt text](https://i.gyazo.com/b9b99aa6f4469ff422696f89bcc6c983.png)

https://gyazo.com/5fa3bf2209a5a69a2ade7363c9ecb21b

# What is Rpg Station?

its a framework that want to simplify creation of rpg games but still provide flexibility, so you can make any type of game with included functionality. If the game you want to make is not possible out of the box, it will be very easy to add specific logic.

# How flexible it is?

- none of the stats / skills / names / vitals / Character creation, Ui whatever are hardcoded, they care configured through editor.
- each character will instantiate a scriptable object "calculator" that will be responsible for calculation, tell other component what to do.
- hot swappable Mechanics feature, that allow you to change controls, component, inputs of your game in a click of a button. that mean you can use ClassicRpgMechanics to have a team based rpg, then change to FPSRpgMechanics and your game will play like skyrim. this does not need to reconfigure your data or your scenes



How to:
How to create my Own mechanics:

``` csharp
namespace Station
{
    [CreateAssetMenu]
    public class DefaultMechanics : StationMechanics
    {
        private DefaultFactionHandler _factionHandler;
     

        public override AsyncOperationHandle<GameObject>? OnCreateCharacter(PlayerCharacterType typeHandler, object[] data, Action<GameObject> onPlayerInstanced, string PrefabId)
        {
            _factionHandler = new DefaultFactionHandler();
            string raceId = (string) data[0];
            string secondId = (string) data[1];
            string genderId = (string) data[2];
            
            var dbSystem = _station.GetSystem<DbSystem>();
            var raceDb = dbSystem.GetDb<RaceDb>();
   
            if (typeHandler.GetType() == typeof(PlayerCharacterType))
            {
                if (data.Length != 3)
                {
                    Debug.Log("need update");
                }

                if (string.IsNullOrEmpty(PrefabId))
                {
                    Debug.Log("MISSING CHARACTER address");
               
                }
                else
                {
                    var op = Addressables.InstantiateAsync(PrefabId);
                    return op;
                }
            }

            return null;
        }

        public override void OnBuildPlayer(BaseCharacter character, PlayersData save, PlayerClassModel classData)
        {
            var dbSystem = _station.GetSystem<DbSystem>();
            var classDb = dbSystem.GetDb<PlayerClassDb>();
            var ActiveAbilityDb = dbSystem.GetDb<ActiveAbilitiesDb>();
            var PassiveAbilityDb = dbSystem.GetDb<PassiveAbilitiesDb>();
            var model = classDb.GetEntry(save.ClassId);
            if (model.StatsCalculator)
            {
                var calculatorInstance = Instantiate(model.StatsCalculator) as PlayerCalculations;
                calculatorInstance.PreSetup(classData);
                            
                character.Init(save.RaceId, save.FactionId, save.GenderId, calculatorInstance, save.Name);
                character.SetupAction(model.Attack);     
                character.AddMeta("classId", save.ClassId);
                character.gameObject.name = "[player] "+save.Name;
                character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
                character.GetInputHandler.InitializePlayerInput(PlayerInput.Instance);
                
                #region ABILITIES
                //load from save
                List<RuntimeAbility> tempList = new List<RuntimeAbility>();
                foreach (var ab in save.LearnedActiveAbilitiesList)
                {
                    var ability = new RuntimeAbility();
                    ability.Initialize(ActiveAbilityDb.GetEntry(ab.Id),ab.Rank ,ab.CoolDown, character,ab.Id);
                    tempList.Add(ability);
                }
                character.Action.SetAbilities(tempList, character);
                
               //set binds
               var binds = new Dictionary<string,List<BarSlotState>>();
               var mainBarBinds = new List<BarSlotState>();
               foreach (var barState in save.BarStates)
               {
                   if (barState.Id == "main")
                   {
                       mainBarBinds = barState.Slots;
                   }
               }
               binds.Add("main", mainBarBinds);
                character.Action.BuildBinds(binds);
                //passive Abilities
                List<RuntimePassiveAbility> passiveList = new List<RuntimePassiveAbility>();
                foreach (var ab in save.LearnedPassiveAbilitiesList)
                {
                    var ability = new RuntimePassiveAbility();
                    ability.Initialize(PassiveAbilityDb.GetEntry(ab.Id),ab.Rename, character);
                    passiveList.Add(ability);
                }
                
                character.Action.SetPassiveAbilities(passiveList, character);

                //skills
                
                
                #endregion
            }
            else
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
            }

        }

        public override void OnBuildNpc(BaseCharacter character, NpcModel model, string npcId)
        {
            if (model.StatsCalculator == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
                return;
            }

            var calculatorInstance = Instantiate(model.StatsCalculator) as NpcCalculation;
            if (calculatorInstance == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR INSTANCE");
                return;
            }

            calculatorInstance.PreSetup(model);
                            
            character.Init(model.RaceId, model.FactionId, "Male", calculatorInstance, model.Name);
            character.SetupAction(model.Attack);     
            character.AddMeta("npc_id", npcId);
            character.gameObject.name = "[npc] "+model.Name;
            character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
               
            #region ABILITIES
            //load from save
            List<RuntimeAbility> tempList = new List<RuntimeAbility>();
            foreach (var ab in model.OwnedAbilities)
            {
                var ability = new RuntimeAbility();
                ability.Initialize(ab,0 ,0, character, "");
                tempList.Add(ability);
            }
            character.Action.SetAbilities(tempList, character);
               
            //skills
                
                
            #endregion
        }

        public override void OnReceiveEvent(string eventName, object[] localParams)
        {
            
        }

        public override IFactionHandler FactionHandler()
        {
            return _factionHandler;
        }
        
        public override string Description()
        {
            return "this is the default mechanics used as demo";

        }
    }
}

```

You are free to use this in any comercial game but do not resell the framework
