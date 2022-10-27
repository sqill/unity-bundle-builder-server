#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;

[System.Serializable]
public class ImageInfo
{
    public string url;
    public int fps;
    public int frameCount;
}

[System.Serializable]
public class EffectInfo
{
    public string url;
    public int fps;
    public int frameCount;
}


// [System.Serializable]
// public class LinksJson
// {
//     public string[] urls;
//     public EffectInfo[] effectsUrl;
// }

[System.Serializable]
public class LinksJson
{
    public ImageInfo[] links;
}


[ExecuteInEditMode]
public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    public string accessEmail, accessToken;

    public Texture2D testTexture;

    public TextAsset jsonFile;

    private LinksJson links;

    private int imageIndex;

    private bool canBuild, canDelete, canQuit;

    private string dirPath, atlasdirPath;

    private string outputPath;

    public void Awake()
    {
        Instance = this;
    }
    public void Start(){
        canBuild = false;
        canDelete = false;
        canQuit = false;
        dirPath = Application.dataPath + "/Raw/Prefab/";
        atlasdirPath = Application.dataPath + "/Raw/Atlas/";
        // StartCoroutine(DownloadImage());

    }

    public void ParseLinkJson(string jsonString)
    {
        canBuild = false;
        imageIndex = 0;
        string finalString = "{\"links\":" + jsonString + "}";
        links = JsonUtility.FromJson<LinksJson>(finalString);
        StartCoroutine(DownloadImages(links));
    }
    public void ParseOutput(string path)
    {
        canDelete = false;
        StartCoroutine(BuildBundles(path));
    }

    public void WaitForResult()
    {
        canQuit = false;
        StartCoroutine(FinishBuilding());
    }
    public void WaitForQuit()
    {
        StartCoroutine(FinishRunning());
    }

    private IEnumerator BuildBundles(string path)
    {
        yield return new WaitUntil(() => canBuild);

        Debug.Log("build Bundles");
        outputPath = path;
        BuildPipeline.BuildAssetBundles(path + "/android", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        BuildPipeline.BuildAssetBundles(path + "/ios", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
        AssetDatabase.Refresh();

        canDelete = true;
    }

    //TestOnly Function
    private void ClearBundles(){
        if (Directory.Exists(outputPath + "/android")) 
        { 
            Directory.Delete(outputPath + "/android", true); 
        }
        if (Directory.Exists(outputPath + "/ios")) 
        { 
            Directory.Delete(outputPath + "/ios", true); 
        }
        Directory.CreateDirectory(outputPath + "/android");
        Directory.CreateDirectory(outputPath + "/ios");
    }

    private void ClearImages(){
        if (Directory.Exists(dirPath)) 
        { 
            Directory.Delete(dirPath, true); 
        }
        if (Directory.Exists(atlasdirPath)) 
        { 
            Directory.Delete(atlasdirPath, true); 
        }
        Directory.CreateDirectory(dirPath);
        Directory.CreateDirectory(atlasdirPath);
    }

    private IEnumerator FinishBuilding()
    {
        Debug.Log("here");
        yield return new WaitUntil(() => canDelete);
        ClearImages();
        canQuit = true;
        Debug.Log("Finish Bundles");

    }

    private IEnumerator FinishRunning()
    {
        yield return new WaitUntil(() => canQuit);
        EditorApplication.Exit(0);
    }

    private IEnumerator DownloadImages(LinksJson links)
    {
        foreach(ImageInfo info in links.links)
        {
            yield return DownloadImage(info.url);
            //For Effects
            if(info.fps != 0)
                Debug.Log(info.fps);
        }
        AssetDatabase.Refresh();
        canBuild = true;  
    }

    private IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("Failed Request" + request.error);
        }
        else
        {
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            if(!Directory.Exists(atlasdirPath)) {
                Directory.CreateDirectory(atlasdirPath);
            }
            File.WriteAllBytes(dirPath + "PrefabImage" + imageIndex + ".png", request.downloadHandler.data);
            File.WriteAllBytes(atlasdirPath + "AtlasImage" + imageIndex + ".png", request.downloadHandler.data);
            imageIndex++;
        }
    }
}

#endif


