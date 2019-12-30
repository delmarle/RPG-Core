using UnityEngine;
using System.Collections.Generic;

namespace Station
{
  public struct Target : System.IComparable<Target>
  {
    public GameObject GameObject;
    public Transform Transform;
    public Trackable Trackable;
    public Tracker Tracker;
    public Collider Collider;
    public Collider2D Collider2D;

    public static Target Null { get { return _null; } }
    private static Target _null = new Target();

    public Target(Transform transform, Tracker tracker)
    {
      GameObject = transform.gameObject;
      Transform = transform;
      Trackable = transform.GetComponent<Trackable>();
      Tracker = tracker;
      Collider = null;
      Collider2D = null;
    }

    public Target(Trackable trackable, Tracker tracker)
    {
      GameObject = trackable.gameObject;
      Transform = trackable.transform;
      Trackable = trackable;
      Tracker = tracker;
      Collider = null;
      Collider2D = null;
    }

    public Target(Target otherTarget)
    {
      GameObject = otherTarget.GameObject;
      Transform = otherTarget.Transform;
      Trackable = otherTarget.Trackable;
      Tracker = otherTarget.Tracker;
      Collider = otherTarget.Collider;
      Collider2D = otherTarget.Collider2D;
    }

    public static bool operator ==(Target tA, Target tB)
    {
      return tA.GameObject == tB.GameObject;
    }

    public static bool operator !=(Target tA, Target tB)
    {
      return tA.GameObject != tB.GameObject;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override bool Equals(System.Object other)
    {
      if (other == null) return false;
      return this == (Target) other;
    }

    public int CompareTo(Target obj)
    {
      return GameObject == obj.GameObject ? 1 : 0;
    }
  }

  public class ListTarget : List<Target>
  {
    public ListTarget()
    {
    }

    public ListTarget(ListTarget listTarget) : base(listTarget)
    {
    }

    public ListTarget(Area area) : base(area)
    {
    }
  }
}