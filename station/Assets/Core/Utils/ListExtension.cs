
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Station
{
  public static class ListExtension
  {
    /// <summary>
    /// Shuffle the list in place using the Fisher-Yates method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
      Random rng = new Random();
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    /// <summary>
    /// Return a random item from the list.
    /// Sampling with replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomItem<T>(this IList<T> list)
    {
      if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
      return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Removes a random item from the list, returning that item.
    /// Sampling without replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RemoveRandom<T>(this IList<T> list)
    {
      if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
      int index = UnityEngine.Random.Range(0, list.Count);
      T item = list[index];
      list.RemoveAt(index);
      return item;
    }

    public static void ForceResize(this List<int> list, int size)
    {
      if (size == 0) list.Clear();
      while (list.Count > size) list.RemoveAt(list.Count - 1);
      while (list.Count < size) list.Add(list.Count);
    }

    public static void ForceResize(this List<string> list, int size)
    {
      if (size == 0) list.Clear();
      while (list.Count > size) list.RemoveAt(list.Count - 1);
      while (list.Count < size) list.Add(String.Empty);
    }


    public static void ForceResize(this List<GameObject> list, int size)
    {
      if (size == 0) list.Clear();
      while (list.Count > size) list.RemoveAt(list.Count - 1);
      while (list.Count < size) list.Add(null);
    }

    public static bool IsEmptyOrNull<T>(this IList<T> list)
    {
      if (list == null)
      {
        return true;
      }

      if (list.Count == 0)
      {
        return true;
      }

      return false;
    }

    public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
    {
      var foundItems = list.Select((value, index) => new {value, index}).Where(x => predicate(x.value)).Select(x => x.index);
      var idx = foundItems.Any() ? foundItems.FirstOrDefault() : -1;
      return idx;
    }

    public static T GetLast<T>(this IList<T> list)
    {
      if (list == null || list.Any() == false)
      {
        return default;
      }
      
      return list.LastOrDefault();
    }
    
    public static T GetFirst<T>(this IList<T> list)
    {
      if (list == null || list.Any() == false)
      {
        return default;
      }
      
      return list.FirstOrDefault();
    }
  }
}

