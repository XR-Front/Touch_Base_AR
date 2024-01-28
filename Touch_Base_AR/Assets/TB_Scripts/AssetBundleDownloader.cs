using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleDownloader
{
    internal static IEnumerator DownloadData(string docID, System.Action<GameObject> onCompleted)
    {
        string url = "https://drive.google.com/uc?export=download&id=" + docID;

        Debug.Log("i will download this url: " + url);
        yield return new WaitForEndOfFrame();


        //Debug.Log("Web Request From: " + url);
        using (UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return webRequest.SendWebRequest();


            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);


                if (bundle != null)
                {
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

                    onCompleted(myPrefab);

                    yield return new WaitForSeconds(1);

                    Debug.Log("clearing cached versions");

                    Caching.ClearAllCachedVersions(bundle.name);
                    bundle.Unload(false);
                    webRequest.Dispose();

                }
                else
                {

                }
            }
        }
    }
}