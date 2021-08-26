using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_PlatformManager : MonoBehaviour
{
    public GameObject iOSExampleDownloads;
    public GameObject androidExampleDownloads;

    void Start()
    {

#if UNITY_IOS

        iOSExampleDownloads.SetActive(true);
        androidExampleDownloads.SetActive(false);

#elif UNITY_ANDROID

        iOSExampleDownloads.SetActive(false);
        androidExampleDownloads.SetActive(true);

#else

        Debug.Log("Any Other Platform");

#endif

    }
}
