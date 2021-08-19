using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class XRF_DownloadAssetBundle : MonoBehaviour
{
    private string myURLBASE = "https://drive.google.com/uc?export=download&id=";
    private string inputURL;

    public string testURL;//later this will come from an input field
    public InputField inputFieldURL;
    public GameObject cancelInterface;
    public GameObject loadButtonsInterface;
    public GameObject loadingPanel;


    public List<Sprite> SavedMy360Textures = new List<Sprite>();
    public List<Sprite> SavedMyImageTextures = new List<Sprite>();
    public List<Texture> SavedMy360Cubemaps = new List<Texture>();

    void Start()
    {
        cancelInterface.SetActive(false);
        loadButtonsInterface.SetActive(true);

        inputURL = "";
        if (PlayerPrefs.HasKey("myURL"))
        {
            inputURL = PlayerPrefs.GetString("myURL");
            Debug.Log("I have a saved URL and it is: " + inputURL);
        }
        inputFieldURL.text = inputURL;
    }

    public void DownloadAssetBundle()
    {
        //original string to split: https://drive.google.com/file/d/1noS7SYFslxawUa0arX6D2MMuBu3p2VaL/view?usp=sharing

        string[] URLSplit = splitString("/d/", testURL);//this is a helper function to split a string by a string
        string[] theCodeSplit = splitString("/", URLSplit[1]);//split the string to get just the id code from google drive
        string theCode = theCodeSplit[0];//split the id code from google drive
        string finalURL = myURLBASE + theCode;//recreate the google drive url with download

        StartDownload(finalURL);
    }

    public void InputFieldURL()
    {
        inputURL = inputFieldURL.text;
        PlayerPrefs.SetString("myURL", inputURL);
    }


    public void StartDownload(string theURL)
    {
        SavedMy360Textures = new List<Sprite>();
        SavedMyImageTextures = new List<Sprite>();
        SavedMy360Cubemaps = new List<Texture>();
        Caching.ClearCache();
        StartCoroutine(DownloadModel(theURL));
    }

    IEnumerator DownloadModel(string url)
    {
        cancelInterface.SetActive(true);
        loadButtonsInterface.SetActive(false);

        WWW www = WWW.LoadFromCacheOrDownload(url, 1);
        yield return www;

        if (www.isDone)
        {
            AssetBundle bundle = www.assetBundle;
            yield return bundle;

            if (bundle != null)
            {
                string[] assets = bundle.GetAllAssetNames();
                foreach (string asset in assets)
                {
                    Debug.Log("my asset names: " + asset);
                }

                AssetBundleRequest requestTextures = bundle.LoadAllAssetsAsync(typeof(Texture));
                yield return requestTextures;

                Object[] MyTex = requestTextures.allAssets;
                foreach (Object obj in MyTex)
                {
                    Texture t = (Texture)obj;
                    if (obj.name.Contains("360"))
                    {
                        if (obj.name.Contains("sprite"))
                        {
                            SavedMy360Textures.Add(Sprite.Create(t as Texture2D, new Rect(0, 0, t.width, t.height), new Vector2(0.0f, 0.0f)));
                        }
                        else
                        {
                            SavedMy360Cubemaps.Add(t);
                        }
                    }
                    else
                    {
                        SavedMyImageTextures.Add(Sprite.Create(t as Texture2D, new Rect(0, 0, t.width, t.height), new Vector2(0.0f, 0.0f)));
                    }
                }

                yield return new WaitForSeconds(1);

                Caching.ClearAllCachedVersions(bundle.name);
                bundle.Unload(false);
                www.Dispose();

                //done!
                //cancelInterface.SetActive(false);
                //loadButtonsInterface.SetActive(false);

                loadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("no asset bundle found");
                cancelInterface.SetActive(false);
                loadButtonsInterface.SetActive(true);
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
