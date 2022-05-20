using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
  public class DictGenericDatabase<T> : BaseDb, IEnumerable<T> where T : class
  {
    public virtual IDictionary<string, T> Db
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (var i = 0; i < Count(); i++)
      {
        yield return Db.ElementAt(i).Value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Add(T entry)
    {
      string key = Guid.NewGuid().ToString();
      while (Db.ContainsKey(key))
      {
        key = Guid.NewGuid().ToString();
      }
      Db.Add(key ,entry);
      ForceRefresh();
    }

    public bool HasKey(string key)
    {
      if (string.IsNullOrEmpty(key)) return false;
      return Db.ContainsKey(key);
    }

    public T GetEntry(string key)
    {
      return Db.ContainsKey(key)? Db[key] : null;
    }
    
    public T GetEntry(int index)
    {
      return index>Db.Count-1 ? null : Db.ElementAt(index).Value;
    }

    public int GetIndex(string key)
    {
      int index = 0;

      foreach (var entry in Db.Keys)
      {
        if (entry == key)
          return index;

        index++;
      }
      return -1;
    }
    
    public string GetKey(int index)
    {
      return Db.Keys.ElementAt(index);
    }
    
    public string GetKey(T entryCompared)
    {
      int index = 0;

      foreach (var entry in Db.Values)
      {
        if (entry == entryCompared)
        {
          return GetKey(index);
        }

        index++;
      }

      return string.Empty;
    }

    public void Duplicate(int index)
    {
      var copy = JsonUtility.FromJson<T>(JsonUtility.ToJson(GetEntry(index)));
      Add(copy);
    }

    public bool MoveEntryUp(int index)
    { 
      if (index == 0) return false;

      var keys = Db.Keys.ToList();
      var values = Db.Values.ToList();

      var entryTarget = values.ElementAt(index);
      var entryReplaced = values.ElementAt(index - 1);
      var keyTarget = keys.ElementAt(index);
      var KeyReplaced = keys.ElementAt(index - 1);

      keys[index] = KeyReplaced;
      keys[index - 1] = keyTarget;
      values[index] = entryReplaced;
      values[index - 1] = entryTarget;
      
      Db.Clear();
      for (int i = 0; i < keys.Count; i++)
      {
        Db.Add(keys.ElementAt(i), values.ElementAt(i));
      }
      return true;
    }
    
    public bool MoveEntryDown(int index)
    {
      if (index+1 == Db.Count)return false;

      var keys = Db.Keys.ToList();
      var values = Db.Values.ToList();

      var entryTarget = values.ElementAt(index);
      var entryReplaced = values.ElementAt(index + 1);
      var keyTarget = keys.ElementAt(index);
      var KeyReplaced = keys.ElementAt(index + 1);

      keys[index] = KeyReplaced;
      keys[index + 1] = keyTarget;
      values[index] = entryReplaced;
      values[index + 1] = entryTarget;
      
      Db.Clear();
      for (int i = 0; i < keys.Count; i++)
      {
        Db.Add(keys.ElementAt(i), values.ElementAt(i));
      }
   
      return true;
    }

    public void Remove(T entry)
    {
      var key = string.Empty;
      foreach (var compared in Db)
      {
        if (entry == compared.Value)
        {
          key = compared.Key;
          break;
        }
      }
      OnBeforeDelete(key);

      Db.Remove(key);
      ForceRefresh();
      GUIUtility.ExitGUI();
    }

    public int Count()
    {
      return Db.Count;
    }
    

    public virtual string[] ListEntryNames()
    {
      return null;
    }

    protected virtual void OnBeforeDelete(string key)
    {
    }
  }
}

