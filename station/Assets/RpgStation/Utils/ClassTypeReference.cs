using System;
using UnityEngine;

namespace Station
{
  [Serializable]
  public sealed class ClassTypeReference : ISerializationCallbackReceiver
  {
    public static string GetClassRef(Type type)
    {
      return type != null
        ? type.FullName + ", " + type.Assembly.GetName().Name
        : "";
    }

    public ClassTypeReference()
    {
    }

    public ClassTypeReference(string assemblyQualifiedClassName)
    {
      Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
        ? Type.GetType(assemblyQualifiedClassName)
        : null;
    }

    public ClassTypeReference(Type type)
    {
      Type = type;
    }

    [SerializeField] private string _classRef;

    #region ISerializationCallbackReceiver Members

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (!string.IsNullOrEmpty(_classRef))
      {
        _type = Type.GetType(_classRef);

        if (_type == null)
          Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", _classRef));
      }
      else
      {
        _type = null;
      }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    #endregion

    private Type _type;


    public Type Type
    {
      get { return _type; }
      set
      {
        if (value != null && !value.IsClass)
          throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");

        _type = value;
        _classRef = GetClassRef(value);
      }
    }

    public static implicit operator string(ClassTypeReference typeReference)
    {
      return typeReference._classRef;
    }

    public static implicit operator Type(ClassTypeReference typeReference)
    {
      return typeReference.Type;
    }

    public static implicit operator ClassTypeReference(Type type)
    {
      return new ClassTypeReference(type);
    }

    public override string ToString()
    {
      return Type != null ? Type.FullName : "(None)";
    }
  }
}