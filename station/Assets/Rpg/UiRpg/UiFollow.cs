using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollow : MonoBehaviour
{
    public GameObject target;
    public Vector2 offset;



    void LateUpdate()
    {
        if (Camera.main != null)
        {
            ((RectTransform)transform).anchoredPosition = Camera.main.WorldToScreenPoint(target.transform.position) + (Vector3)offset;
        }
    }
}
