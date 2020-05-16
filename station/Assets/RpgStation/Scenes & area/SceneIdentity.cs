
using Station;
using Station.Data;
using UnityEngine;

public class SceneIdentity : MonoBehaviour
{
    [SerializeField] private SceneType _sceneType = SceneType.Area;

    private void Awake()
    {
        var sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
        if (sceneSystem)
        {
            sceneSystem.SetCurrentSceneType(_sceneType);
        }
    }
}
