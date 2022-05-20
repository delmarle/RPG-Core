using UnityEngine;

namespace Station
{
    public class SceneTrigger : MonoBehaviour
    {
	    [SerializeField] private string _sceneName;
	    [SerializeField] private bool _onlyOnce = true;

	    private bool _loaded;
	    
	    public void Invoke()
	    {
		    if (_onlyOnce && _loaded)
			    return;

		    _loaded = true;
			//SceneLoader.Instance.LoadScene(_sceneName);
	    }
	
    }

}

