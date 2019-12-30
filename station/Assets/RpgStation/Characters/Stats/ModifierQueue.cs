using System.Collections.Generic;

namespace Station
{
  public class ModifierQueue : BaseSystem 
  {
    #region [[ FIELDS ]]

    private static ModifierQueue _instance;
    private readonly List<StatsHandler> _characters = new List<StatsHandler>();
    #endregion
    #region [[ ADD / REMOVE ]]

    public static void AddCharacter(StatsHandler character)
    {
      if(_instance)_instance._AddCharacter(character);
    }
    
    public static void RemoveCharacter(StatsHandler character)
    {
      if(_instance)_instance._RemoveCharacter(character);
    }

    private void _AddCharacter(StatsHandler character)
    {
      if (_characters.Contains(character) == false)
      {
        _characters.Add(character);
      }
    }
    
    private void _RemoveCharacter(StatsHandler character)
    {
      if (_characters.Contains(character))
      {
        _characters.Remove(character);
      }
    }

    #endregion
    #region [[ MONOBEHAVIOR ]]

    private void Update()
    {
      for (int i = 0; i < _characters.Count; i++)
      {
        _characters[i].CycleModifier();
      }
    }
    
    #endregion

    protected override void OnInit()
    {
      _instance = this;
    }

    protected override void OnDispose()
    {
      _instance = null;
    }
  }
}

