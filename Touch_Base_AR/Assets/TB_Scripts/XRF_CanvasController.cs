using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_CanvasController : MonoBehaviour
{
    public GameObject downloadInterfaceCanvas;
    public GameObject inputFieldParent;
    public GameObject loadingParent;
    public GameObject exampleDownloadParents;

    void Start()
    {
        downloadInterfaceCanvas.SetActive(false);

        inputFieldParent.SetActive(true);
        loadingParent.SetActive(false);
        exampleDownloadParents.SetActive(true);
    }

    public void DownloadInterfaceOn()
    {
        downloadInterfaceCanvas.SetActive(true);
        inputFieldParent.SetActive(true);
        loadingParent.SetActive(false);
        exampleDownloadParents.SetActive(true);
    }

    public void DownloadInterfaceOff()
    {
        downloadInterfaceCanvas.SetActive(false);

    }

}
