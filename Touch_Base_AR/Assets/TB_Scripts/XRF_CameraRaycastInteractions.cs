using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class XRF_CameraRaycastInteractions : MonoBehaviour
{

    public GameObject raycastCamera;
    private bool dontHighlight;
    private Material[] tempMaterialsHigh;
    private Material[] matsHigh;
    private GameObject tempSelectedObject;
    private Ray myRay;


    [System.Serializable]
    public enum ClickType // your custom enumeration
    {
        MouseClick,
        ARCamera,
    };
    public ClickType camType = ClickType.MouseClick;  // this public var should appear as a drop down

    void Update()
    {

        if (camType == ClickType.MouseClick)
        {
            myRay = raycastCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        }
        else if(camType == ClickType.ARCamera)
        {
            myRay = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
        }


        if (!IsPointerOverUIObject())
        {
            RaycastHit myRayHit;
            if (Physics.Raycast(myRay, out myRayHit, 100.0f))
            {
                //i shot out a ray and it hit something
                Debug.Log("I hit something");
                GameObject hitObject = myRayHit.transform.gameObject;
                if (hitObject.GetComponent<XRF_InteractionController>())
                {
                    //i shot out a ray and hit something with an interaction controller
                    Debug.Log("I hit something with an interaction controller on it");
                    RayHit(hitObject);

                    if (Input.GetMouseButtonDown(0))
                    {
                        XRF_InteractionController[] myInteractions = hitObject.GetComponents<XRF_InteractionController>();
                        foreach (XRF_InteractionController t in myInteractions)
                        {
                            t.DoTheThing();
                        }
                    }
                    else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                    {
                        XRF_InteractionController[] myInteractions = hitObject.GetComponents<XRF_InteractionController>();
                        foreach (XRF_InteractionController t in myInteractions)
                        {
                            t.DoTheThing();
                        }

                    }
                }
                else
                {
                    RayMissed();
                }
            }
            else
            {
                RayMissed();
            }
        }
    }
    void RayHit(GameObject touchObject)
    {
        if (tempSelectedObject != touchObject)
        {
            if (tempSelectedObject != null)
            {
                UnHighlightObj(tempSelectedObject);
            }
        }
        tempSelectedObject = touchObject;
        HighlightObj(tempSelectedObject);
    }
    void RayMissed()
    {
        //Debug.Log("ray missed");

        if (tempSelectedObject != null)
        {
            UnHighlightObj(tempSelectedObject);
            tempSelectedObject = null;
        }
    }
    void HighlightObj(GameObject highlightThis)
    {
        MeshRenderer rend = highlightThis.transform.gameObject.GetComponent<MeshRenderer>();
        if (rend != null)
        {
            if (!dontHighlight)
            {
                tempMaterialsHigh = highlightThis.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                matsHigh = new Material[tempMaterialsHigh.Length];

                Material highlightMaterial = highlightThis.GetComponent<XRF_InteractionController>().HighlightMaterial;

                for (int i = 0; i < tempMaterialsHigh.Length; i++)
                {
                    matsHigh[i] = highlightMaterial;
                }
                highlightThis.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterials = matsHigh;
                dontHighlight = true;
            }
        }
    }
    void UnHighlightObj(GameObject unHighlightThis)
    {
        MeshRenderer rend = unHighlightThis.GetComponent<MeshRenderer>();
        if (rend != null)
        {
            unHighlightThis.transform.gameObject.GetComponent<MeshRenderer>().sharedMaterials = tempMaterialsHigh;
            dontHighlight = false;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
