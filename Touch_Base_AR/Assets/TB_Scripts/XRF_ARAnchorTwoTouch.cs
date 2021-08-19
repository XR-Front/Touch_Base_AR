using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class XRF_ARAnchorTwoTouch : MonoBehaviour
{
    public GameObject ar_2D_Marker;
    public GameObject ar_3D_marker;
    public GameObject ARObject;
    public GameObject setAnchorButton;

    private ARRaycastManager raycastManager;
    private bool setAnchorStart;
    private bool firstAnchorRegistered;
    public GameObject ARSessionOrigin;

    public GameObject SuggestionText;


    void Start()
    {
        ARObject.SetActive(false);
        SuggestionText.SetActive(false);
        setAnchorButton.SetActive(true);

        setAnchorStart = false;
        firstAnchorRegistered = false;

        ar_2D_Marker = Instantiate(ar_2D_Marker, Vector3.zero, Quaternion.identity);
        ar_2D_Marker.SetActive(false);
        ar_3D_marker = Instantiate(ar_3D_marker, Vector3.zero, Quaternion.identity);
        ar_3D_marker.SetActive(false);

        raycastManager = ARSessionOrigin.GetComponent<ARRaycastManager>();

        ARSessionOrigin.GetComponent<ARRaycastManager>().enabled = false;
        ARSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        ARSessionOrigin.GetComponent<ARPointCloudManager>().enabled = false;
    }

    void Update()
    {
        if (!setAnchorStart)
        {
            return;
        }
        else
        {
            // shoot a raycast from the center of the screen
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            //locate the center of the screen
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            //Cast a ray against trackables, in this case planes
            raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

            //if we hit an AR Plane then update the position and rotation of the visual plane to be the same as the hit transforms
            if (hits.Count > 0)
            {

                ar_2D_Marker.transform.position = hits[0].pose.position;
                ar_2D_Marker.transform.rotation = hits[0].pose.rotation;

                //make the visual plane visible if not already
                if (!ar_2D_Marker.activeInHierarchy)
                {
                    ar_2D_Marker.SetActive(true);
                }



                if (!firstAnchorRegistered)
                {
                    SuggestionText.GetComponent<Text>().text = "TAP SCREEN TO SET \"AR ORIGIN\" ON THE GROUND";



                    //TAP WILL PLACE THE ORIGIN
                    if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                    {
                        FirstTouch(hits);
                    }

                }
                else
                {
                    SuggestionText.GetComponent<Text>().text = "TAP SCREEN TO SET \"AR DIRECTION\" ON THE GROUND";

                    //this will make the 3d marker "look at" the current position of the raycast
                    Vector3 targetPostition = new Vector3(hits[0].pose.position.x, ar_3D_marker.transform.position.y, hits[0].pose.position.z);
                    ar_3D_marker.transform.LookAt(targetPostition);


                    //TAP WILL PLACE THE ACTUAL ANCHOR
                    if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                    {
                        SecondTouch();
                    }
                }

            }
            else
            {
                SuggestionText.GetComponent<Text>().text = "SLOWLY LOOK AROUND A FLAT, OPEN AREA WITH YOUR CAMERA";


                if (!ar_2D_Marker.activeInHierarchy)
                {
                    ar_2D_Marker.SetActive(false);
                }
            }
        }
    }

    public void FirstTouch(List<ARRaycastHit> hits)
    {
        ar_3D_marker.transform.position = hits[0].pose.position;
        ar_3D_marker.transform.rotation = hits[0].pose.rotation;
        ar_3D_marker.SetActive(true);

        firstAnchorRegistered = true;
    }

    public void SecondTouch()
    {

        ARObject.transform.position = ar_3D_marker.transform.position;
        ARObject.transform.rotation = ar_3D_marker.transform.rotation;
        ARObject.SetActive(true);
        ar_2D_Marker.SetActive(false);
        ar_3D_marker.SetActive(false);
        setAnchorStart = false;
        firstAnchorRegistered = false;
        SuggestionText.SetActive(false);
        setAnchorButton.SetActive(true);


        ARSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        ARSessionOrigin.GetComponent<ARPointCloudManager>().enabled = false;
        ARSessionOrigin.GetComponent<ARRaycastManager>().enabled = false;

        GameObject[] arPlanes = GameObject.FindGameObjectsWithTag("AR_Plane");
        foreach (GameObject g in arPlanes)
        {
            g.SetActive(false);
        }
    }


    public void StartARAnchor()
    {
        ARObject.SetActive(false);
        SuggestionText.SetActive(true);

        setAnchorStart = true;
        firstAnchorRegistered = false;


        ar_2D_Marker.SetActive(false);
        ar_3D_marker.SetActive(false);


        ARSessionOrigin.GetComponent<ARRaycastManager>().enabled = true;
        ARSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
        ARSessionOrigin.GetComponent<ARPointCloudManager>().enabled = true;

        setAnchorButton.SetActive(false);
    }
}
