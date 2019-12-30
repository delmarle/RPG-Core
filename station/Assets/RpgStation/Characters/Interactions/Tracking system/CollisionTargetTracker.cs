using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
  public class CollisionTracker : Tracker
  {
    protected ListTarget Targets = new ListTarget();

    public override ListTarget ListTargets
    {
      get
      {
        _targets.Clear();
        if (NumberOfTargets == 0) return _targets;

        var validTargets = new List<Target>(Targets);
        foreach (Target target in Targets)
          if (target.GameObject == null || !target.GameObject.activeInHierarchy)
            validTargets.Remove(target);

        if (validTargets.Count == 0) return _targets;

        if (NumberOfTargets == -1)
        {
          _targets.AddRange(validTargets);
        }
        else
        {
          int num = Mathf.Clamp(NumberOfTargets, 0, validTargets.Count);
          for (int i = 0; i < num; i++)
            _targets.Add(validTargets[i]);
        }

        if (OnPostSort != null) OnPostSort(this, _targets);

        return _targets;
      }
    }

    protected bool CompareLayer(GameObject obj)
    {
      LayerMask objMask = 1 << obj.layer;
      LayerMask targetMask = TargetLayers;
      return (targetMask.value & objMask.value) != 0;
    }

    protected void OnCollisionEnter(Collision collisionInfo)
    {
      OnCollisionEnterAny(collisionInfo.gameObject);
    }

    protected void OnCollisionExit(Collision collisionInfo)
    {
      OnCollisionExitAny(collisionInfo.gameObject);
    }

    protected void OnCollisionEnter2D(Collision2D collisionInfo)
    {
      OnCollisionEnterAny(collisionInfo.gameObject);
    }

    protected void OnCollisionExit2D(Collision2D collisionInfo)
    {
      OnCollisionExitAny(collisionInfo.gameObject);
    }

    protected void OnCollisionEnterAny(GameObject otherGo)
    {
      if (!CompareLayer(otherGo)) return;
      var target = new Target(otherGo.transform, this);
      if (target == Target.Null) return;
      if (!target.Trackable.IsTrackable) return;
      if (IsAlreadyDetectedDelegate(target)) return;
      if (!Targets.Contains(target)) Targets.Add(target);
    }

    protected void OnCollisionExitAny(GameObject otherGo)
    {
      var target = new Target();
      foreach (Target currentTarget in Targets)
      {
        if (currentTarget.GameObject == otherGo) target = currentTarget;  
      }

      if (target != Target.Null)  StartCoroutine(DelayRemove(target));
    }	

    protected IEnumerator DelayRemove(Target target)
    {
      yield return null;
      if (Targets.Contains(target)) Targets.Remove(target);
    }
  }
}