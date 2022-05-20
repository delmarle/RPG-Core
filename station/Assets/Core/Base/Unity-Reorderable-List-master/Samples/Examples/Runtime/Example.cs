using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Malee;

public class Example : MonoBehaviour {

	public List<ExampleChild> list1;

	[Reorderable]
	public ExampleChildList list2;

	[Reorderable]
	public ExampleChildList list3;

	[Reorderable]
	public StringList list4;

	[Reorderable]
	public VectorList list5;

	[System.Serializable]
	public class ExampleChild {

		public string name;
		public float value;
		public ExampleEnum myEnum;
		public LayerMask layerMask;
		public long longValue;
		public char charValue;
		public byte byteValue;

		public enum ExampleEnum {
			EnumValue1,
			EnumValue2,
			EnumValue3
		}
	}

	[System.Serializable]
	public class ExampleChildList : ReorderableArray<ExampleChild> {
	}

	[System.Serializable]
	public class StringList : ReorderableArray<string> {
	}

	[System.Serializable]
	public class VectorList : ReorderableArray<Vector4> {
	}
}
