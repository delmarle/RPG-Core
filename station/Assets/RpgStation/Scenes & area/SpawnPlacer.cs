using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Station
{
  public class SpawnPlacer : MonoBehaviour
  {
    [SerializeField] private List<Transform> _points = new List<Transform>();

    private  BaseDb GetDb(Type currentType)
    {
        BaseDb dbFound = null;
#if UNITY_EDITOR


      string dbPath = "Assets/Content/Databases/" + currentType.Name + @".asset";
      BaseDb found = AssetDatabase.LoadAssetAtPath<BaseDb>(dbPath);
      if (found == null)
      {
        Debug.LogError("cant find db at: "+dbPath);
      }

      dbFound =  found;
      
  
#endif
        return dbFound;
    }
    
    public void Initialize()
    {
      var _settingsDb = (GameSettingsDb)GetDb(typeof(GameSettingsDb));
      var max = _settingsDb.Get().MaxTeamSize;
      while (_points.Count > 2)//max)
      {
        var obj = _points[_points.Count-1];
        DestroyImmediate(obj.gameObject);
        _points.RemoveAt(_points.Count-1);
      }
    }

    public void Clean()
    {
      DestroyImmediate(gameObject);
    }

    public Vector3[] Positions()
    {
      List<Vector3> record = new List<Vector3>();
      foreach (var p in _points)
      {
        record.Add(p.position);
      }
      return record.ToArray();
    }

    public int GetPointsCount()
    {
      return _points.Count;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      Gizmos.color = Color.green;
      Gizmos.DrawSphere(transform.position,0.5f);
      Handles.Label(transform.position+Vector3.up,"[SPAWN]");
      Gizmos.color = Color.yellow;
      foreach (var p in _points)
      {
        Gizmos.DrawSphere(p.position,0.15f);
        Handles.Label(p.position+(Vector3.up/2),p.name);
      }

      ForGizmo(transform.position, transform.forward);
    }
    
      public static void ForGizmo (Vector3 pos, Vector3 direction, float arrowHeadLength = 2.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay (pos, direction);
        DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForGizmo (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay (pos, direction);
        DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForDebug (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay (pos, direction);
        DrawArrowEnd(false, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForDebug (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay (pos, direction, color);
        DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }
   
    private static void DrawArrowEnd (bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Vector3 right = Quaternion.LookRotation (direction) * Quaternion.Euler (arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation (direction) * Quaternion.Euler (-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation (direction) * Quaternion.Euler (0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation (direction) * Quaternion.Euler (0, -arrowHeadAngle, 0) * Vector3.back;
        if (gizmos) {
            Gizmos.color = color;
            Gizmos.DrawRay (pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay (pos + direction, left * arrowHeadLength);
            Gizmos.DrawRay (pos + direction, up * arrowHeadLength);
            Gizmos.DrawRay (pos + direction, down * arrowHeadLength);
        } else {
            Debug.DrawRay (pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay (pos + direction, left * arrowHeadLength, color);
            Debug.DrawRay (pos + direction, up * arrowHeadLength, color);
            Debug.DrawRay (pos + direction, down * arrowHeadLength, color);
        }
    }
    #endif
  }
}

