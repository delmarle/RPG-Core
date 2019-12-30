using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
  public class Area : MonoBehaviour, IList<Target>
  {
    private readonly ListTarget _targets = new ListTarget();
    public AreaTracker Tracker;
    public Collider Coll;
    public Collider2D Coll2D;

    protected void Awake()
    {
      Coll2D = GetComponent<Collider2D>();
      Coll = GetComponent<Collider>();
    }

    private bool ValidateLayer(GameObject obj)
    {
      LayerMask objMask = 1 << obj.layer;
      var targetMask = Tracker.TargetLayers;
      return (targetMask.value & objMask.value) != 0;
    }

    #region [[ UNITY TRIGGERS ]]

    protected void OnTriggerEnter(Collider other)
    {
      OnTriggerEntered(other);
    }

    protected void OnTriggerExit(Collider other)
    {
      OnTriggerExited(other);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
      OnTriggerEntered(other);
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
      OnTriggerExited(other);
    }

    protected void OnTriggerEntered(Component other)
    {
      var targetable = other.GetComponent<Trackable>();
      if (targetable == null) return;
      Add(targetable);
    }

    protected void OnTriggerExited(Component other)
    {
      Remove(other.transform);
    }

    #endregion

    #region [[ ILIST ]]

    public void Add(Target target)
    {
      if (!target.Trackable.IsTrackable || !gameObject.activeInHierarchy || _targets.Contains(target)) return;
      if (!ValidateLayer(target.Trackable.gameObject)) return;
      if (Tracker.IsAlreadyDetectedDelegate(target)) return;

      _targets.Add(target);
      target.Trackable.DoDetected(Tracker);
      Tracker.ListTargets = _targets;
    }

    public void Add(Trackable trackable)
    {
      var target = new Target(trackable, Tracker);
      Add(target);
    }

    public bool Remove(Transform xform)
    {
      return Remove(new Target(xform, Tracker));
    }

    public bool Remove(Trackable trackable)
    {
      return Remove(new Target(trackable, Tracker));
    }

    public bool Remove(Target target)
    {
      if (!_targets.Remove(target)) return false;
      if (target.Transform == null || transform == null || transform.parent == null) return false;
        

      if (Tracker.OnRemoveDetected != null) Tracker.OnRemoveDetected(Tracker, target);

      target.Trackable.DoNotDetected(Tracker);		
      Tracker.ListTargets = _targets;

      return true;
    }

    public Target this[int index]
    {
      get { return _targets[index]; }
      set {}
    }

    public void Clear()
    {
      foreach (Target target in _targets)
      {
        if (Tracker.OnRemoveDetected != null) Tracker.OnRemoveDetected(Tracker, target);

        target.Trackable.DoNotDetected(Tracker);
      }

      _targets.Clear();
      Tracker.ListTargets = _targets;
    }

    public bool Contains(Transform target)
    {
      return _targets.Contains(new Target(target, Tracker));
    }

    public bool Contains(Target target)
    {
      return _targets.Contains(target);
    }

    public IEnumerator<Target> GetEnumerator()
    {
      for (int i = 0; i < _targets.Count; i++) yield return _targets[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < _targets.Count; i++)  yield return _targets[i]; 
    }

    public void CopyTo(Target[] array, int arrayIndex)
    {
      _targets.CopyTo(array, arrayIndex);
    }

    public int IndexOf(Target item)
    {
      return -1;
    }

    public void Insert(int index, Target item)
    {
    }

    public void RemoveAt(int index)
    {
    }

    public bool IsReadOnly
    {
      get { return true; }
    }
    
    public int Count
    {
      get { return _targets.Count; }
    }

    #endregion
  }
}