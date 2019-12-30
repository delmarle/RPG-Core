using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Station
{
  public delegate bool OnAddDetected(Tracker source, Target target);
  public delegate void OnRemoveDetected(Tracker source, Target target);
  public delegate void OnPostSort(Tracker source, ListTarget targets);
  public delegate void OnTargetsUpdate(Tracker source);

  public abstract class Tracker : MonoBehaviour, ITracker
  {
    [SerializeField] private float _sortingCycle = 0.1f;
    public int NumberOfTargets = 1;
    public LayerMask TargetLayers;
    [SerializeField] private SortingStyles _sortingStyle = SortingStyles.Nearest;
    public SortingStyles SortingStyle
    {
      get { return _sortingStyle; }
      set
      {
        if (_sortingStyle != value)
        {
          _sortingStyle = value;
          Dirty = true;
        }
      }
    }
    public virtual bool Dirty
    {
      get { return false; }
      set
      {
#if UNITY_EDITOR
        if (!Application.isPlaying)
          return;
#endif
        if (!value)
          return;

        ListTargets = new ListTarget(UnfilteredTargets);
      }
    }

    public bool BatchDirty
    {
      get { return _batchDirtyRunning; }
      set { if (_batchDirtyRunning == false) StartCoroutine(RunBatchDirty()); }
    }

    private bool _batchDirtyRunning;

    protected IEnumerator RunBatchDirty()
    {
      _batchDirtyRunning = true;
      yield return new WaitForEndOfFrame();
      _batchDirtyRunning = false;
      Dirty = true;
    }

    public virtual ListTarget ListTargets
    {
      get { return _targets; }
      set
      {
        UnfilteredTargets.Clear();
        _targets.Clear();
        if (NumberOfTargets == 0 || value.Count == 0)
        {
          if (OnTargetsUpdates != null) OnTargetsUpdates(this);
            
          return;
        }

        UnfilteredTargets.AddRange(value);
        _targets.AddRange(UnfilteredTargets);

        if (SortingStyle != SortingStyles.None)
        {
          if (UnfilteredTargets.Count > 1)
          {
            var comparer = new TargetComparer
            (
              SortingStyle,
              transform
            );

            _targets.Sort(comparer);
          }

          if (!_isUpdateTargetsUpdateRunning && SortingStyle != SortingStyles.None) StartCoroutine(UpdateTargets());
        }

        if (OnPostSort != null)
          OnPostSort(this, _targets);

        if (NumberOfTargets > -1)
        {
          int count = _targets.Count;
          int num = Mathf.Clamp(NumberOfTargets, 0, count);
          _targets.RemoveRange(num, count - num);
        }
        if (OnTargetsUpdates != null)  OnTargetsUpdates(this);
      }
    }

    protected ListTarget _targets = new ListTarget();

    private bool _isUpdateTargetsUpdateRunning;

    protected ListTarget UnfilteredTargets = new ListTarget();

    protected IEnumerator UpdateTargets()
    {
      _isUpdateTargetsUpdateRunning = true;

      while (UnfilteredTargets.Count > 0)
      {
        if (Math.Abs(_sortingCycle) < 0.001f)
          yield return null;
        else
          yield return new WaitForSeconds(_sortingCycle);

        Dirty = true;
      }
      _isUpdateTargetsUpdateRunning = false;
    }

    public virtual Area Area
    {
      get { return null; }
      protected set { }
    }


    protected virtual void OnEnable()
    {
      Dirty = true;
    }
    
    internal bool IsAlreadyDetectedDelegate(Target target)
    {
      if (OnAddDetected == null) return false;

      var dels = OnAddDetected.GetInvocationList();
      foreach (var @delegate in dels)
      {
        var del = (OnAddDetected) @delegate;
        if (!del(this, target)) return true;
      }

      return false;
    }

    #region [[ CALLBACKS ]]
    protected internal OnPostSort OnPostSort;

    public void AddOnPostSort(OnPostSort del)
    {
      OnPostSort -= del;
      OnPostSort += del;
    }

    public void SetOnPostSort(OnPostSort del)
    {
      OnPostSort = del;
    }

    public void RemoveOnPostSort(OnPostSort del)
    {
      OnPostSort -= del;
    }
    
    protected internal OnTargetsUpdate OnTargetsUpdates;

    public void AddOnTargetsChanged(OnTargetsUpdate del)
    {
      OnTargetsUpdates -= del;
      OnTargetsUpdates += del;
    }

    public void SetOnTargetsChanged(OnTargetsUpdate del)
    {
      OnTargetsUpdates = del;
    }

    public void RemoveOnTargetsChanged(OnTargetsUpdate del)
    {
      OnTargetsUpdates -= del;
    }

    protected internal OnAddDetected OnAddDetected; 

    public void AddOnNewDetected(OnAddDetected del)
    {
      OnAddDetected -= del;
      OnAddDetected += del;
    }

    public void SetOnNewDetected(OnAddDetected del)
    {
      OnAddDetected = del;
    }

    public void RemoveOnNewDetected(OnAddDetected del)
    {
      OnAddDetected -= del;
    }
    
    protected internal OnRemoveDetected OnRemoveDetected;

    public void AddOnNotDetected(OnRemoveDetected del)
    {
      OnRemoveDetected -= del;
      OnRemoveDetected += del;
    }

    public void SetOnNotDetected(OnRemoveDetected del)
    {
      OnRemoveDetected = del;
    }

    public void RemoveOnNotDetected(OnRemoveDetected del)
    {
      OnRemoveDetected -= del;
    }

    #endregion


    #region [[ SORTING ]]

    public enum SortingStyles
    {
      None = 0,
      Nearest = 1,
      Farthest = 2,
      NearestToDestination = 3,
      FarthestFromDestination = 4,
      MostPowerful = 5,
      LeastPowerful = 6
    }
    
    public interface ITargetComparer : IComparer<Target>
    {
      new int Compare(Target targetA, Target targetB);
    }

    public class TargetComparer : ITargetComparer
    {
      protected Transform Transform;
      protected SortingStyles SortStyle;

      public TargetComparer(SortingStyles sortStyle, Transform other)
      {
        Transform = other;
        SortStyle = sortStyle;
      }

      public int Compare(Target targetA, Target targetB)
      {
        switch (SortStyle)
        {
          case SortingStyles.Farthest:
            float na = targetA.Trackable.GetSqrDistToPos(Transform.position);
            float nb = targetB.Trackable.GetSqrDistToPos(Transform.position);
            return nb.CompareTo(na);


          case SortingStyles.Nearest:
            float fa = targetA.Trackable.GetSqrDistToPos(Transform.position);
            float fb = targetB.Trackable.GetSqrDistToPos(Transform.position);
            return fa.CompareTo(fb);


          case SortingStyles.FarthestFromDestination:
            return targetB.Trackable.PathLength.CompareTo(
              targetA.Trackable.PathLength);

          case SortingStyles.NearestToDestination:
            return targetA.Trackable.PathLength.CompareTo(
              targetB.Trackable.PathLength);

          case SortingStyles.LeastPowerful:
            return targetA.Trackable.Strength.CompareTo(
              targetB.Trackable.Strength);

          case SortingStyles.MostPowerful:
            return targetB.Trackable.Strength.CompareTo(
              targetA.Trackable.Strength);
        }
        return 0;
      }
    }
    #endregion
  }
  
  interface ITracker
  {
    ListTarget ListTargets { get; set; }
    bool Dirty { get; set; }
    Area Area { get; }
    void AddOnPostSort(OnPostSort del);
    void SetOnPostSort(OnPostSort del);
    void RemoveOnPostSort(OnPostSort del);
    void AddOnNewDetected(OnAddDetected del);
    void SetOnNewDetected(OnAddDetected del);
    void RemoveOnNewDetected(OnAddDetected del);
  }
}