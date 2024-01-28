using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_CanvasController : MonoBehaviour
{
    public GameObject ARInterface;
    public GameObject LoadAnAssetInterface;
    public GameObject InfoPanel;
    public GameObject LoadingScreen;

    void Start()
    {
        ARInterface.SetActive(true);
        LoadAnAssetInterface.SetActive(false);
        InfoPanel.SetActive(false);
        LoadingScreen.SetActive(false);
    }


    public void DownloadInterfaceOn()
    {
        LoadAnAssetInterface.SetActive(true);
        InfoPanel.SetActive(false);
    }

    public void DownloadInterfaceOff()
    {
        LoadAnAssetInterface.SetActive(false);
        InfoPanel.SetActive(false);
    }

    public void InfoPanelOn()
    {
        LoadAnAssetInterface.SetActive(false);
        InfoPanel.SetActive(true);
    }
    public void InfoPanelOff()
    {
        LoadAnAssetInterface.SetActive(false);
        InfoPanel.SetActive(false);
    }

    public void LoadingPanelOff()
    {
        LoadingScreen.SetActive(false);
    }
}
