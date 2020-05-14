using System.Collections.Generic;
using UnityEngine;
namespace Station
{
  public class InteractionHandler : MonoBehaviour
  {
    #region [[ FIELDS ]] 

    private static InteractionHandler _instance;
    [SerializeField] private LayerMask _interactibleLayers = 0;
    [SerializeField] private AreaTracker _interactionTracker = null;

    private BaseCharacter _leader;
    private bool _interactionEnabled;
    private Interactible _lastHovered;
    
    //Hide hints checks
    private Dictionary<Interactible,float> _hideDelay = new Dictionary<Interactible, float>();
    private List<Interactible> _hideByDistance = new List<Interactible>();
    private List<Interactible> _removeFromChecks = new List<Interactible>();
    #endregion

    private void Awake()
    {
      _instance = this;
      GameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);
      GameGlobalEvents.OnBeforeLeaveScene.AddListener(OnBeforeLeaveScene);
    }

 

    private void Start()
    {
      EnableInteractions();
    }

    private void Update()
    { 
      OnHover();
      CheckDistances();
      ProcessRemoved();
    }

    private void OnDestroy()
    {
      _instance = null;
      GameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
    }

    private void EnableInteractions()
    {
      TouchManager.OnTap += OnTap;
      _interactionEnabled = true;
      _interactionTracker.AddOnNewDetected(OnDetect);
      _interactionTracker.AddOnNotDetected(OnUnDetect);
    }

    private void DisableInteractions()
    {
      TouchManager.OnTap -= OnTap;
      _interactionEnabled = false;
    }
    
    private void OnTap(Finger obj)
    {
      Interactible tapped;
      RaycastUtils.RaycastTarget(obj.PreviousScreenPosition ,100, _interactibleLayers, out tapped);
      if (obj.StartedOverGui)
      {
        return;
      }

      if(tapped) tapped.OnTap(_leader);
    }

    private void OnHover()
    {
      if (_interactionEnabled == false)
        return;
      
      Interactible hovered;
      RaycastUtils.RaycastTarget(Input.mousePosition , 100,_interactibleLayers, out hovered);

      if (_lastHovered != hovered)
      {
        if (_lastHovered)
        {
          _lastHovered.OnStopHover();
        }

        if (hovered && hovered.CanBeHovered())
        {
          _lastHovered = hovered;
          if (_lastHovered)
          {
            _lastHovered.OnStartHover();
          }
        }
        else
        {
          _lastHovered = null;
        }

        
      }
    }
    
    #region CHECKS 
    
    #region [[TRACKER CALLBACK]]

    private bool OnDetect(Tracker tracker, Target target)
    {
      //print("detect");
      return true;
    }

    private void OnUnDetect(Tracker tracker, Target target)
    {
      //print("detect");
    }

    #endregion
    
#region ADD
    public void AddDelayHintCheck(Interactible interactible)
    {
      float delay = interactible.Config.HideHintOptions.Delay;
      if (_hideDelay.ContainsKey(interactible))
      {
        _hideDelay[interactible] = delay;
      }
      else
      {
        _hideDelay.Add(interactible,delay);
      }
    }

    private void AddDistanceHintCheck(Interactible interactible)
    {
      if (_hideByDistance.Contains(interactible) == false)
      {
        _hideByDistance.Add(interactible);
      }
    }
#endregion

    private void HideInteractible(Interactible interactible)
    {
      if (_hideDelay.ContainsKey(interactible))
      {
        _hideDelay.Remove(interactible);
      }

      if (_hideByDistance.Contains(interactible))
      {
        _hideByDistance.Remove(interactible);
      }
      
      interactible.HideVisual();
    }

    private void CheckDistances()
    {
      if (_leader == null) return;
      foreach (var interactible in _hideByDistance)
      {
        float distanceMax = interactible.Config.HideHintOptions.Distance;
        if (Vector3.Distance(_leader.transform.position, interactible.transform.position) > distanceMax)
        {
          if (_removeFromChecks.Contains(interactible) == false)
          {
            _removeFromChecks.Add(interactible);
          }
        }
      }
    }

    private void ProcessRemoved()
    {
      foreach (var interactible in _removeFromChecks)
      {
        if (_hideByDistance.Contains(interactible)) _hideByDistance.Remove(interactible);
          
        if (_hideDelay.ContainsKey(interactible))  _hideDelay.Remove(interactible);
        
        interactible.HideVisual();
      }
      
      _removeFromChecks.Clear();
    }

    #endregion
    
    #region STATIC

    public static void AddDistanceCheck(Interactible interactible)
    {
      if(_instance) _instance.AddDistanceHintCheck(interactible);
    }

    #endregion
    
    private void OnLeaderChanged(BaseCharacter leader)
    {
      _leader = leader;
      if (leader != null && _interactionTracker != null)
      {
        var transf = _interactionTracker.transform;
        transf.SetParent(_leader.transform);
        transf.localPosition = Vector3.zero;
      }

    }
    
    private void OnBeforeLeaveScene()
    {
      if (_interactionTracker != null)
      {
        var transf = _interactionTracker.transform;
        transf.SetParent(transform);
        transf.localPosition = Vector3.zero;
      }

    }

  }
}


