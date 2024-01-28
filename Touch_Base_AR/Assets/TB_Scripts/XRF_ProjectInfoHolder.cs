using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XRF_ProjectInfoHolder : MonoBehaviour
{
    [Header("Reference to AR Origin")]
    public Transform ImportHolder;

    [Header("Reference to Canvas Items")]
    public Image HowToImage;
    public GameObject LoadingScreen;
    public GameObject DownloadScreen;


    [Header("Project Information")]
    public string studentName;
    public string projectName;
    public string projectURL;
    public string diagramURL;

    private bool imageDone = false;
    private bool projectDone = false;

    private void Start()
    {
        LoadingScreen.SetActive(false);
    }
    public void DownloadThisProject()
    {
        LoadingScreen.SetActive(true);
        imageDone = false;
        projectDone = false;

        //original string to split: https://drive.google.com/file/d/1noS7SYFslxawUa0arX6D2MMuBu3p2VaL/view?usp=sharing
        string[] splitURLAssetBundle = projectURL.Split("/");
        string k_assetBundleDocID = splitURLAssetBundle[5];
        StartCoroutine(AssetBundleDownloader.DownloadData(k_assetBundleDocID, AfterDownload));


        string[] splitURLDiagram = diagramURL.Split("/");
        string k_diagramDocID = splitURLDiagram[5];
        StartCoroutine(TextureDownloader.DownloadData(k_diagramDocID, AfterDownloadImage));

    }
    public void AfterDownload(GameObject theDownload)
    {
        //clear out old instances...
        //instantiate at 0 local position

        if (ImportHolder.childCount > 0)
        {
            GameObject child = ImportHolder.GetChild(0).gameObject;
            if (child)
            {
                Destroy(child);
            }
        }

        GameObject currentInstance = Instantiate(theDownload);

        Vector3 instancePos = currentInstance.transform.localPosition;
        Quaternion instanceRot = currentInstance.transform.localRotation;

        //make a child of parent geometry
        currentInstance.transform.parent = ImportHolder.transform;
        
        currentInstance.transform.localPosition = instancePos;
        currentInstance.transform.localRotation = instanceRot;

        projectDone = true;
        TryTurnOffInterface();
    }

    public void AfterDownloadImage(Texture theDownload)
    {
        Debug.Log("omg i have the texture! width is: " + theDownload.width);

        //apply this to a ? interface that shows you where to click

        Texture2D tex2D = (theDownload as Texture2D); // where tex is of type 'Texture'

        Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(tex2D.width / 2, tex2D.height / 2));
        HowToImage.sprite = sprite;

        imageDone = true;
        TryTurnOffInterface();
    }

    public void TryTurnOffInterface()
    {
        if(imageDone && projectDone)
        {
            LoadingScreen.SetActive(false);
            DownloadScreen.SetActive(false);
        }
    }
}
