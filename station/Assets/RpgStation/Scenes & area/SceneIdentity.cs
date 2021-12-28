
using Station;
using Station.Data;
using UnityEngine;

public class SceneIdentity : MonoBehaviour
{
    [SerializeField] private SceneType _sceneType = SceneType.Area;

    private void Awake()
    {
        var sceneSystem = GameInstance.GetSystem<SceneSystem>();
        if (sceneSystem)
        {
            sceneSystem.SetCurrentSceneType(_sceneType);
        }
    }

    public bool IsArea()
    {
        return _sceneType == SceneType.Area;
    }
    
    public bool IsCharacterCreation()
    {
        return _sceneType == SceneType.CharacterCreation;
    }
}
