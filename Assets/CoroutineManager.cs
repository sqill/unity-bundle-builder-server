#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;
using UnityEngine.U2D;
using UnityEditor.U2D;

[System.Serializable]
public class ImageInfo
{
    public string name;
    public string url;
    public int fps;
    public int columns;
    public int rows;
    public int count;
    public bool loop;
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

    public SpriteAtlas spriteAtlas;

    public TextAsset jsonFile;

    private LinksJson links;

    private int imageIndex;

    UnityWebRequestAsyncOperation asyncOperation;
    // private bool canBuild, canDelete, canQuit;

    private string dirPath, atlasdirPath;

    private string outputPath;

    private string testString;

    public void Awake()
    {
        Instance = this;
    }

    public void Parse(string jsonString, string path)
    {
        dirPath = Application.dataPath + "/Raw/Prefab/";
        atlasdirPath = Application.dataPath + "/Raw/Atlas/";
        outputPath = path;
        imageIndex = 0;
        string finalString = "{\"links\":" + jsonString + "}";
        links = JsonUtility.FromJson<LinksJson>(finalString);
        StartCoroutine(DownloadImages(links));
    }

    private void BuildBundles(string path)
    {
        // yield return new WaitUntil(() => canBuild);

        Debug.Log("Build Bundles");
        if (!Directory.Exists(outputPath + "/android")) 
        { 
            Directory.CreateDirectory(outputPath + "/android"); 
        }
        if (!Directory.Exists(outputPath + "/ios")) 
        { 
            Directory.CreateDirectory(outputPath + "/ios"); 
        }

        if (!Directory.Exists(outputPath + "/mac")) 
        { 
            Directory.CreateDirectory(outputPath + "/mac"); 
        }

        if (!Directory.Exists(outputPath + "/windows")) 
        { 
            Directory.CreateDirectory(outputPath + "/windows"); 
        }
        
        
        SpriteAtlasUtility.PackAllAtlases(BuildTarget.Android);

        BuildPipeline.BuildAssetBundles(path + "/android", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);

        SpriteAtlasUtility.PackAllAtlases(BuildTarget.iOS);

        BuildPipeline.BuildAssetBundles(path + "/ios", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

        SpriteAtlasUtility.PackAllAtlases(BuildTarget.StandaloneOSX);

        BuildPipeline.BuildAssetBundles(path + "/mac", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);

        SpriteAtlasUtility.PackAllAtlases(BuildTarget.StandaloneWindows);

        BuildPipeline.BuildAssetBundles(path + "/windows", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

        AssetDatabase.Refresh();

        // canDelete = true;
        ClearImages();
        FinishRunning();

    }

    //TestOnly Function
    private void ClearBundles()
    {
        if (Directory.Exists(outputPath + "/android")) 
        { 
            Directory.Delete(outputPath + "/android", true); 
        }
        if (Directory.Exists(outputPath + "/ios")) 
        { 
            Directory.Delete(outputPath + "/ios", true); 
        }
        if (Directory.Exists(outputPath + "/windows")) 
        { 
            Directory.Delete(outputPath + "/windows", true); 
        }
        if (Directory.Exists(outputPath + "/mac")) 
        { 
            Directory.Delete(outputPath + "/mac", true); 
        }
        Directory.CreateDirectory(outputPath + "/android");
        Directory.CreateDirectory(outputPath + "/ios");
        Directory.CreateDirectory(outputPath + "/windows");
        Directory.CreateDirectory(outputPath + "/mac");
    }

    private void ClearImages(){
        if (Directory.Exists(dirPath)) 
        { 
            Directory.Delete(dirPath, true); 
        }
        if (Directory.Exists(atlasdirPath)) 
        { 
            foreach(string file in Directory.GetFiles(atlasdirPath))
                if(!file.Contains("atlas"))
                    File.Delete(file);
        }
        Directory.CreateDirectory(dirPath);
    }

    private void FinishRunning()
    {
        EditorApplication.Exit(0);
    }

    private IEnumerator DownloadImages(LinksJson json)
    {
        Debug.Log("Start Donwloading Images");
        int index = 0;
        foreach(ImageInfo info in json.links)
        {
            Debug.Log("Download Image " + info.url);
            yield return DownloadImage(info.name, info.url, info.fps, info.rows, info.columns, info.count, info.loop);

            // if(info.count > 0)
            //     yield return GenerateEffectObject(info.name,info.fps, info.loop);
            //For Effects
        }
        // UnityEngine.Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Raw/Prefab/" + testString + ".png");

        // Debug.Log(data.Length + " Data Length");

        // foreach (Object o in data)
        // {
        //     Debug.Log("Object" + o);
        // }

        // Debug.Log("Finish Download Images");
        // // StartCoroutine(StartBuildBundles());
        AssetDatabase.Refresh();
        BuildBundles(outputPath);

        // StartCoroutine(StartBuildBundles());

    }

    private IEnumerator DownloadImage(string NewName, string url, int fps, int rows, int columns, int count, bool loop)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("Failed Request" + request.error);
        }
        else
        {
            Debug.Log("Downloaded Image at url " + url);
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            if(!Directory.Exists(atlasdirPath)) {
                Directory.CreateDirectory(atlasdirPath);
            }

            //Replace "Image" with name

            byte[] imageData = request.downloadHandler.data;
            string name = "";
            if(fps > 0)
                name = "Effect_" + rows + "_" + columns + "_" + count + "_" + fps + "_" + loop +"_,";
            else
                File.WriteAllBytes(atlasdirPath + NewName + ".png", imageData);

            name += NewName;
            testString = name;
            File.WriteAllBytes(dirPath + name + ".png", imageData);
            AssetDatabase.Refresh();

            
            // if(fps > 0)
            //     GenerateEffect(name, fps, loop);
            imageIndex++;
        }
    }


    public void GenerateEffectObject(string name, int fps, bool loop)
    {
        // BaseScriptableEffect asset = ScriptableObject.CreateInstance<BaseScriptableEffect>();
        // asset.Init(name, fps, loop);

        // AssetDatabase.CreateAsset(asset, "Assets/Raw/Prefab/" + name + ".asset");
        // AssetDatabase.SaveAssets();
        Debug.Log("Generate Prefab");
        GameObject g = new GameObject();
        g.name = name;
        // AssetDatabase.CreateAsset(g, "Assets/Raw/Prefab/" + name);
        g.AddComponent<BaseEffect>().Init(name, fps, loop);
        // g.Init(name, fps, loop);
        PrefabUtility.SaveAsPrefabAsset(g, "Assets/Raw/Prefab/" + g.name + ".prefab");
        AssetDatabase.SaveAssets();

        // for (int i = 0; i < colorList.Length; ++i)
        // {
        //     Material material = new Material(Shader.Find("Specular"));
        //     var materialName = "material_" + i + ".mat";
        //     AssetDatabase.CreateAsset(material, "Assets/Artifacts/" + materialName);

        //     material.SetColor("_Color", colorList[i]);
        // }

        // AssetDatabase.SaveAssets();
    }
}

#endif


