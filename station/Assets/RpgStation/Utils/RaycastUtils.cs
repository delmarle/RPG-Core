using System.Collections.Generic;
using UnityEngine;

namespace Station
{
 
  public static class RaycastUtils 
  {
    public static bool IsVisible(Transform sourceTransform,Vector3 source, Vector3 target, float fov, int obstacleMask)
    {
      Vector3 dirToTarget = (target - source).normalized;
      float dstToTarget = Vector3.Distance(source, target);
      
      Vector3 sightView  = (sourceTransform.forward );
      float dot = Vector3.Dot(sightView, dirToTarget);
 
      //angle difference between looking direction and direction to item (radians)
      float angle = Mathf.Acos(dot);
 
      if(angle > fov)
      {
        return false;
      }
      
      if (Physics.Raycast(source, dirToTarget, dstToTarget, obstacleMask))
      {
        return false;

      }

      return true;
    }

    public static void RaycastTargets<T>(Vector3 origin,int distance, int mask, out List<T> found)
    {
      found = new List<T>();
      Ray ray = Camera.main.ScreenPointToRay(origin);
      RaycastHit[] hits = Physics.RaycastAll(ray,distance,mask);
      foreach(RaycastHit hit in hits )
      {
        T target =  hit.collider.GetComponent<T>();
        if (target != null)
        {
          found.Add(target);
        }
      }
    }
    public static void RaycastTarget<T>(Vector3 origin,int distance, int mask, RaycastHit[] hits, out T found)
    {
      found = default(T);
      if (Camera.main == null) return;
      Ray ray = Camera.main.ScreenPointToRay(origin);
      if (Physics.RaycastNonAlloc(ray, hits, distance, mask)>0)
      {
        foreach(RaycastHit hit in hits )
        {
          T target;
          if (typeof(T) == typeof(GameObject))
          {
            Debug.LogError("Cannot return gameobject;");
          }
          target =  hit.collider.GetComponent<T>();
       
          if (target != null)
          {
            found = target;
            return;
          }
        }
      }
    }
    
    public static void RaycastTarget(Vector3 origin,int distance, int mask, out GameObject found)
    {
      found = null;
      if (Camera.main == null) return;
      Ray ray = Camera.main.ScreenPointToRay(origin);
      RaycastHit[] hits = Physics.RaycastAll(ray,distance,mask);
      foreach(RaycastHit hit in hits )
      {
        GameObject target;
        target =  hit.collider.gameObject;
        found = target;
        return;
      
      }
    }

    public static bool RaycastAllTargets<T>(Vector3 origin, Vector3 direction, int mask, out List<T> targets, out List<RaycastHit> hits)
    {
      targets = new List<T>();
      hits = new List<RaycastHit>();
      int RaycastDistance = 120;
      List<RaycastHit> hitList = new List<RaycastHit>(Physics.RaycastAll(origin, direction, RaycastDistance, mask));
      RaycastHit currentHit;

      for (int i = 0; i < hitList.Count; i++)
      {
        currentHit = hitList[i];

        if (currentHit.collider != null)
        {
          T target = currentHit.collider.GetComponent<T>();

          if (target != null)
          {
            targets.Add(target);
            hits.Add(currentHit);
          }
        }
      }

      bool isOverTargets = targets.Count > 0;
      return isOverTargets;
    }

    public static void SphereCastSearchComponentsAroundSource<T>(T source,Vector3 position, float distance, out List<T>  found)
    {
      found = new List<T>();
      Collider[] scanned = new Collider[0];
      Physics.OverlapSphereNonAlloc(position, distance, scanned);
      if (scanned.Length > 0)
      {
      }

      
    }
  }
}

