
using Station;
using Station.Data;
using UnityEngine;

public class SceneIdentity : MonoBehaviour
{
    [SerializeField] private SceneType _sceneType;

    private void Awake()
    {
        var sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
        if (sceneSystem)
        {
            sceneSystem.SetCurrentSceneType(_sceneType);
        }
    }
}
