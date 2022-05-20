using System;
using UnityEngine;
using System.Collections;

namespace Station
{
  public class AreaTracker : Tracker
  {
    #region [[ FIELDS ]]
    public Vector3 Range
    {
      get { return _range; }
      set
      {
        _range = value;
        if (Area != null) UpdateAreaRange();
      }
    }

    [SerializeField] private Vector3 _range = Vector3.one;

    [Flags] public enum AreaShapes
    {
      Capsule,
      Box,
      Sphere,
      Box2D,
      Circle2D
    }

    public bool Is2D
    {
      get
      {
        int shapes = (int) (AreaShapes.Box2D | AreaShapes.Circle2D);
        return Enum.IsDefined(typeof(AreaShapes), shapes);
      }
    }

    public AreaShapes AreaShape
    {
      get { return _areaShape; }
      set
      {
        _areaShape = value;
        if (Area == null) return;

        StartCoroutine(UpdateAreaShape(Area.gameObject));
      }
    }

    [SerializeField] private AreaShapes _areaShape = AreaShapes.Sphere;

    public Vector3 PositionOffset
    {
      get { return _positionOffset; }
      set
      {
        _positionOffset = value;
        if (Area == null) return;
         
        Area.transform.localPosition = value;
      }
    }

    [SerializeField] private Vector3 _positionOffset;

    public Vector3 RotationOffset
    {
      get { return _rotationOffset; }
      set
      {
        _rotationOffset = value;
        if (Area == null) return;
          
        Area.transform.localRotation = Quaternion.Euler(value);
      }
    }

    [SerializeField] private Vector3 _rotationOffset = Vector3.zero;

    public int AreaLayer
    {
      get { return _areaLayer; }
      set
      {
        _areaLayer = value;

        if (Area == null) return;
        Area.gameObject.layer = value;
      }
    }

    [SerializeField] private int _areaLayer = 2;

    public override Area Area { get; protected set; }

    protected bool AreaColliderEnabledAtStart = true;
    #endregion 

    #region [[ MONOBEHAVIORS ]]
    protected void Awake()
    {
      BuildArea();
      Area.Tracker = this;
      UpdateAreaRange(); 
    }
    
    protected override void OnEnable()
    {
      base.OnEnable();

      if (Area != null)
        Area.gameObject.SetActive(true);
    }

    protected virtual void OnDisable()
    {
      if (Area == null) return;

      Area.Clear();
      Area.gameObject.SetActive(false);
    }
    #endregion
    private void BuildArea()
    {
      var areaGo = new GameObject(name + "AreaCollider");
      areaGo.transform.parent = transform;
      areaGo.SetActive(false);
      areaGo.SetActive(true);
      areaGo.transform.localPosition = PositionOffset;
      areaGo.transform.localRotation = Quaternion.Euler(RotationOffset);
      areaGo.layer = AreaLayer;
      StartCoroutine(UpdateAreaShape(areaGo));
      SetAreaState(areaGo, AreaColliderEnabledAtStart);
      Area = areaGo.AddComponent<Area>();
    }

    private IEnumerator UpdateAreaShape(GameObject areaGo)
    {
      var currentCollider = areaGo.GetComponent<Collider>();
      var currentCollider2D = areaGo.GetComponent<Collider2D>();
      switch (AreaShape)
      {
        case AreaShapes.Sphere:
          if (currentCollider is SphereCollider) yield break;
          break;
        case AreaShapes.Box:
          if (currentCollider is BoxCollider) yield break;
          break;
        case AreaShapes.Capsule:
          if (currentCollider is CapsuleCollider) yield break;
          break;
        case AreaShapes.Box2D:
          if (currentCollider2D is BoxCollider2D) yield break;
          break;
        case AreaShapes.Circle2D:
          if (currentCollider2D is CircleCollider2D) yield break;
          break;
      }

      if (currentCollider != null) Destroy(currentCollider);
      if (currentCollider2D != null) Destroy(currentCollider2D);

      switch (AreaShape)
      {
        case AreaShapes.Capsule:
          if (areaGo.GetComponent<Rigidbody2D>() != null)
          {
            Destroy(areaGo.GetComponent<Rigidbody2D>());
            yield return null;
          }

          if (areaGo.GetComponent<Rigidbody>() == null)
          {
            var rbd = areaGo.AddComponent<Rigidbody>();
            rbd.isKinematic = true;
          }

          break;
        case AreaShapes.Circle2D:
          if (areaGo.GetComponent<Rigidbody>() != null)
          {
            Destroy(areaGo.GetComponent<Rigidbody>());
            yield return null;
          }

          if (areaGo.GetComponent<Rigidbody2D>() == null)
          {
            var rbd2D = areaGo.AddComponent<Rigidbody2D>();
            rbd2D.isKinematic = true; 
          }
          break;
      }

      Collider coll = null;
      Collider2D coll2D = null;
      switch (AreaShape)
      {
        case AreaShapes.Sphere:
          coll = areaGo.AddComponent<SphereCollider>();
          coll.isTrigger = true;
          break;

        case AreaShapes.Box:
          coll = areaGo.AddComponent<BoxCollider>();
          coll.isTrigger = true;
          break;

        case AreaShapes.Capsule:
          coll = areaGo.AddComponent<CapsuleCollider>();
          coll.isTrigger = true;
          break;
        case AreaShapes.Box2D:
          coll2D = areaGo.AddComponent<BoxCollider2D>();
          coll2D.isTrigger = true;
          break;

        case AreaShapes.Circle2D:
          coll2D = areaGo.AddComponent<CircleCollider2D>();
          coll2D.isTrigger = true;
          break;
      }

      if (coll)
      {
        coll.isTrigger = true;
        if (Area != null) Area.Coll = coll;
      }
      else if (coll2D)
      {
        coll2D.isTrigger = true;
        if (Area != null) Area.Coll2D = coll2D; 
      }

      UpdateAreaRange(coll, coll2D);
    }

    protected void UpdateAreaRange()
    {
      Collider col = Area.Coll;
      Collider2D col2D = Area.Coll2D;
      UpdateAreaRange(col, col2D);
    }

    protected void UpdateAreaRange(Collider col, Collider2D col2D)
    {
      Vector3 normRange = GetBoundaries();

      if (col is SphereCollider)
      {
        var coll = (SphereCollider) col;
        coll.radius = normRange.x;
      }
      else if (col is CapsuleCollider)
      {
        var coll = (CapsuleCollider) col;
        coll.radius = normRange.x;
        coll.height = normRange.y;
      }
      else if (col is BoxCollider)
      {
        var coll = (BoxCollider) col;
        coll.size = normRange;
      }
      else if (col2D is CircleCollider2D)
      {
        var coll = (CircleCollider2D) col2D;
        coll.radius = normRange.x;
      }
      else if (col2D is BoxCollider2D)
      {
        var coll = (BoxCollider2D) col2D;
        coll.size = new Vector2(normRange.x, normRange.y);
      }
    }

    public Vector3 GetBoundaries()
    {
      Vector3 normRange = Vector3.zero;
      switch (AreaShape)
      {
        case AreaShapes.Circle2D:
          normRange = new Vector3
          (
            _range.x,
            _range.x,
            0
          );
          break;
        case AreaShapes.Sphere:
          normRange = new Vector3
          (
            _range.x,
            _range.x,
            _range.x
          );
          break;
        case AreaShapes.Box2D:
          normRange = new Vector3
          (
            _range.x * 2,
            _range.y * 2,
            0
          );
          break;
        case AreaShapes.Box:
          normRange = new Vector3
          (
            _range.x * 2,
            _range.y,
            _range.z * 2
          );
          break;

        case AreaShapes.Capsule:
          normRange = new Vector3
          (
            _range.x,
            _range.y * 2,
            _range.x
          );
          break;
      }

      return normRange;
    }

    public bool AreaColliderEnabled
    {
      get
      {
        if (!Area) return false;
        if (Area.Coll)
        {
          return Area.Coll.enabled;
        }
        return Area.Coll2D && Area.Coll2D.enabled;
      }
    }

    public void SetAreaState(bool enable)
    {
      SetAreaState(Area.gameObject, enable);
    }

    protected void SetAreaState(GameObject areaGo, bool enable)
    {
      var coll = Area ? Area.Coll : areaGo.GetComponent<Collider>();

      if (coll)
      {
        coll.enabled = enable;
      }
      else
      {
        var coll2D = Area ? Area.Coll2D : areaGo.GetComponent<Collider2D>();
        if (coll2D) coll2D.enabled = enable;
      }
    }
  }
}