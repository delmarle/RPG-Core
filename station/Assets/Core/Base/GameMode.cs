using Station;
using UnityEngine;

public class GameMode : MonoBehaviour
{

    #region FIELDS

    public SceneSaveSettingsModel SaveSettings = new SceneSaveSettingsModel();

    #endregion
    
    #region VIRTUALS
    protected virtual void OnEnterScene(){Debug.LogError($"enter default GM");}

    protected virtual void OnExitScene(){Debug.LogError($"exit default GM");}
    #endregion

    public void DoEnterScene()
    {
        OnEnterScene();
    }

    public void DoExitScene()
    {
        OnExitScene();
    }
}
