using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_TilingEditor : MonoBehaviour
{
    private float scaleFactor = 5.0f;

    void Update()
    {
        Vector2 tiling = new Vector2(1, 1);

        float xTiling = GetComponent<MeshRenderer>().bounds.size.x;
        float zTiling = GetComponent<MeshRenderer>().bounds.size.z;

        tiling.x = xTiling / scaleFactor;
        tiling.y = zTiling / scaleFactor;

        GetComponent<MeshRenderer>().material.mainTextureScale = tiling;

    }
}
