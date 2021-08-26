using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class XRF_DownloadAssetBundle : MonoBehaviour
{
    private string myURLBASE = "https://drive.google.com/uc?export=download&id=";
    private string inputURL;

    public string testURL;//later this will come from an input field
    public InputField inputFieldURL;
    public GameObject cancelInterface;
    public GameObject loadButtonsInterface;
    public GameObject loadingPanel;
    public GameObject errorInterface;

    public GameObject geometryBase;
    public GameObject currentInstance;
    public GameObject exampleDownloadParents;

    void Start()
    {
        cancelInterface.SetActive(false);
        loadButtonsInterface.SetActive(true);
        errorInterface.SetActive(false);

        inputURL = "";
        if (PlayerPrefs.HasKey("myURL"))
        {
            inputURL = PlayerPrefs.GetString("myURL");
            Debug.Log("I have a saved URL and it is: " + inputURL);
        }
        inputFieldURL.text = inputURL;
    }

    public void InputFieldURL()
    {
        inputURL = inputFieldURL.text;
        PlayerPrefs.SetString("myURL", inputURL);

        errorInterface.SetActive(false);
    }


    public void InputFieldDownloadButton()
    {
        //checks for validity of url, then try downloads url
        if (!string.IsNullOrEmpty(inputURL))
        {
            Uri result;
            if (Uri.TryCreate(inputURL, UriKind.Absolute, out result))
            {
                string finalURL = FormatURL(inputURL);
                StartDownload(finalURL);
            }
            else
            {
                errorInterface.SetActive(true);
            }
        }
        else
        {
            errorInterface.SetActive(true);
        }
    }
    public void HardCodedDownloadButton(string hardCodedURL)
    {
        //checks for validity of url, then try downloads url
        if (!string.IsNullOrEmpty(hardCodedURL))
        {
            Uri result;
            if (Uri.TryCreate(hardCodedURL, UriKind.Absolute, out result))
            {
                string finalURL = FormatURL(hardCodedURL);
                StartDownload(finalURL);
            }
        }
    }

    public void StartDownload(string theURL)
    {
        Caching.ClearCache();
        StartCoroutine(DownloadModel(theURL));
    }
    public string FormatURL(string originalURL)
    {
        //original string to split: https://drive.google.com/file/d/1noS7SYFslxawUa0arX6D2MMuBu3p2VaL/view?usp=sharing

        string[] URLSplit = splitString("/d/", originalURL);//this is a helper function to split a string by a string
        string[] theCodeSplit = splitString("/", URLSplit[1]);//split the string to get just the id code from google drive
        string theCode = theCodeSplit[0];//split the id code from google drive
        string finalURL = myURLBASE + theCode;//recreate the google drive url with download

        return finalURL;
    }

    IEnumerator DownloadModel(string url)
    {
        cancelInterface.SetActive(true);
        loadButtonsInterface.SetActive(false);
        exampleDownloadParents.SetActive(false);

        WWW www = WWW.LoadFromCacheOrDownload(url, 1);
        yield return www;

        if (www.isDone)
        {
            AssetBundle bundle = www.assetBundle;
            yield return bundle;

            if (bundle != null)
            {
                if(currentInstance)
                {
                    Destroy(currentInstance);
                }

                string[] assets = bundle.GetAllAssetNames();
                foreach (string asset in assets)
                {
                    Debug.Log("my asset names: " + asset);
                }

                AssetBundleRequest objectRequest = bundle.LoadAllAssetsAsync(typeof(GameObject));
                // Wait for completion
                yield return objectRequest;
                // I don't want to instantiate like this right now
                GameObject myPrefab = objectRequest.asset as GameObject;
                currentInstance = Instantiate(myPrefab);

                Vector3 instancePos = currentInstance.transform.localPosition;
                Quaternion instanceRot = currentInstance.transform.localRotation;

                //make a child of parent geometry
                currentInstance.transform.parent = geometryBase.transform;
                currentInstance.transform.localPosition = instancePos;
                currentInstance.transform.localRotation = instanceRot;

                yield return new WaitForSeconds(1);

                Caching.ClearAllCachedVersions(bundle.name);
                bundle.Unload(false);
                www.Dispose();

                loadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("no asset bundle found");
                cancelInterface.SetActive(false);
                loadButtonsInterface.SetActive(true);
                exampleDownloadParents.SetActive(true);
            }
        }
    }


    public string[] splitString(string needle, string haystack)//this is a helper function to split a string by a string
    {
        return haystack.Split(new string[] { needle }, System.StringSplitOptions.None);
    }

    public void displayArray(string[] array)//this is a helper function that debugs all the elements from a string[]
    {
        Debug.Log("Array length: " + array.Length + "\n");
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log("array[" + i + "] = " + array[i] + "\n");
        }
    }
}
