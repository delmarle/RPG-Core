using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

public class SurrogateTest : MonoBehaviour {

	[SerializeField]
	private MyClass[] objects;

	[SerializeField, Reorderable(surrogateType = typeof(GameObject), surrogateProperty = "gameObject")]
	private MyClassArray myClassArray;

	[System.Serializable]
	public class MyClass {

		public string name;
		public GameObject gameObject;
	}

	[System.Serializable]
	public class MyClassArray : ReorderableArray<MyClass> {
	}
}
