using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRF_DontSleep : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
