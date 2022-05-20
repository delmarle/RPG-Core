using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class testingpanel : MonoBehaviour
{
    public UiPanel panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            panel.Hide(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            panel.Show(true);
        }
    }
}
