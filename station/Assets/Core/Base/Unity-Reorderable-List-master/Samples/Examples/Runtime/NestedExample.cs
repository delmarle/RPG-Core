using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

public class NestedExample : MonoBehaviour {

	[Reorderable]
	public ExampleChildList list;

	[System.Serializable]
	public class ExampleChild {

		[Reorderable(singleLine = true)]
		public NestedChildList nested;
	}

	[System.Serializable]
	public class NestedChild {

		public float myValue;
	}

	[System.Serializable]
	public class NestedChildCustomDrawer {

		public bool myBool;
		public GameObject myGameObject;
	}

	[System.Serializable]
	public class ExampleChildList : ReorderableArray<ExampleChild> {
	}

	[System.Serializable]
	public class NestedChildList : ReorderableArray<NestedChildCustomDrawer> {
	}
}
