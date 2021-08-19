using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_CanvasItemScale : MonoBehaviour {

    private float screenWidth = 1;
    private float scaleFactor = 1125.0f;
    private float scale;

    private void Update()
    {

        if (Screen.width > Screen.height)
        {
            if (screenWidth != Screen.height)
            {
                screenWidth = Screen.height;
                scale = screenWidth / scaleFactor;
                transform.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            if (screenWidth != Screen.width)
            {
                screenWidth = Screen.width;
                scale = screenWidth / scaleFactor;
                transform.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}