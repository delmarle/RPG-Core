using UnityEngine;
using System.Collections.Generic;

namespace Station
{
  public class Trackable : MonoBehaviour
  {
    #region [[ FIELDS ]]
    [HideInInspector] public float Strength;
    [HideInInspector] public List<Vector3> Path = new List<Vector3>();
    private readonly List<Tracker> _trackers = new List<Tracker>();
    public delegate void OnDetected(Tracker source);
    public delegate void OnUnDetected(Tracker source);
    private OnDetected _onDetected;
    private OnUnDetected _onUnDetected;
    private Collider _collider;
    private Collider2D _collider2D;
    private bool _isTrackable = true;
    
    public bool IsTrackable
    {
      get { return gameObject.activeInHierarchy && enabled && _isTrackable; }
      set
      {
        if (_isTrackable == value) return;
        _isTrackable = value;
        if (!gameObject.activeInHierarchy || !enabled) return;
        if (!_isTrackable) Reset();
        else ForceTrackable();
      }
    }
    #endregion
    
    protected void Awake()
    {
      _collider = GetComponent<Collider>();
      _collider2D = GetComponent<Collider2D>();
      if (_collider)
      {
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
          rigidBody = gameObject.AddComponent<Rigidbody>();
          rigidBody.isKinematic = true;
          rigidBody.useGravity = false;
        }
      }
    }

    protected void OnDisable()
    {
      Reset();
    }

    protected void OnDestroy()
    {
      Reset();
    }

    protected void Reset()
    {
      if (!Application.isPlaying) return;

      var copy = new List<Tracker>(_trackers);
      foreach (Tracker tt in copy)
      {
        if (tt == null || tt.Area == null || tt.Area.Count == 0) continue;
        
        tt.Area.Remove(this);
      }

      _trackers.Clear();
    }

    private void ForceTrackable()
    {
      if (_collider != null && _collider.enabled)
      {
        _collider.enabled = false;
        _collider.enabled = true;
      }
      else if (_collider2D != null && _collider2D.enabled)
      {
        _collider2D.enabled = false;
        _collider2D.enabled = true;
      }
    }

    public void DoDetected(Tracker source)
    {
      _trackers.Add(source);
      if (_onDetected != null) _onDetected(source);
    }

    public void DoNotDetected(Tracker source)
    {
      _trackers.Remove(source);
      if (_onUnDetected != null) _onUnDetected(source);
    }

    public float PathLength
    {
      get
      {
        if (Path.Count == 0) return 0;
        float dist = GetSqrDistToPos(Path[0]);
        for (int i = 0; i < Path.Count - 2; i++)  dist += (Path[i] - Path[i + 1]).sqrMagnitude;
        return dist;
      }
    }

    public float GetSqrDistToPos(Vector3 other)
    {
      return (transform.position - other).sqrMagnitude;
    }

    public float GetDistToPos(Vector3 other)
    {
      return (transform.position - other).magnitude;
    }

    public void AddOnDetected(OnDetected callback)
    {
      _onDetected -= callback; 
      _onDetected += callback;
    }

    public void UniqueOnDetected(OnDetected callback)
    {
      _onDetected = callback;
    }

    public void RemoveOnDetected(OnDetected callback)
    {
      _onDetected -= callback;
    }

    public void AddOnNotDetected(OnUnDetected callback)
    {
      _onUnDetected -= callback; 
      _onUnDetected += callback;
    }

    public void UniqueOnNotDetected(OnUnDetected callback)
    {
      _onUnDetected = callback;
    }

    public void RemoveOnNotDetected(OnUnDetected callback)
    {
      _onUnDetected -= callback;
    }
  }
}