
#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity;
using UnityEditor.SceneManagement;


public class BuildAssets
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/ExportedBundles";
        if(!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    static void ImportTexture()
    {
        string assetBundleDirectory = "Assets/ExportedBundles";
        if(!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }
    
    public static void LoadScene()
    {
        string[] args = System.Environment.GetCommandLineArgs();
 
        for (int i = 0; i < args.Length; i++) 
        {
            Debug.Log ("ARG " + i + ": " + args [i]);

            switch(args[i])
            {
                case "-LoadScene":
                    EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");

                    CoroutineManager.Instance.ImAlive();

                    break;
            }
        }
 

    }
 
    private static System.Collections.IEnumerator BuildImpl()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://sqill.s3.eu-west-3.amazonaws.com/video_editor_assets/asset/215/9dddf40052b9d42d541f1845cea95ee8.png?X-Amz-Algorithm=AWS4-HMAC-SHA256\u0026X-Amz-Credential=AKIAWIMIQWVUDWT2O73D%2F20221025%2Feu-west-3%2Fs3%2Faws4_request\u0026X-Amz-Date=20221025T112957Z\u0026X-Amz-Expires=86400\u0026X-Amz-SignedHeaders=host\u0026X-Amz-Signature=a1e3c85b86eabe776895d5e02c55d86d4f2894f6545ab75958dcd040e17cb1ea");
        Debug.Log("StartRequest");

        yield return request.SendWebRequest();
        Debug.Log("here");
        Debug.Log("Downloaded " + request.downloadProgress); 


         // Your build code here...

    }

    public static void ExecuteFromExternal() 
    {
        string[] args = System.Environment.GetCommandLineArgs ();

        // maybe also add a flag to make sure a second thing is only done
        // if the one before was successfull
        bool lastSucceeded = true;
        // just in combination with the flag to know which was the last
        // command executed
        int lastI = 0;

        for (int i = 0; i < args.Length; i++) 
        {
            Debug.Log ("ARG " + i + ": " + args [i]);

            // interrupt if one command failed
            if(!lastSucceeded)
            {
                Debug.LogFormat("Last command \"{0}\" failed! -> cancel", args[lastI]);
                return;
            }

            switch(args[i])
            {
                case "-doSomething":
                    lastI = i;

                    // do something
                    break;

                case "-build":
                    lastI = i;

                    // maybe you want to get some values after "-build"
                    // because they are needed let's say e.g. an int and a path
                    // int value = args[i + 1];
                    // string path = args[i + 2];
                    // maybe a sanity check here

                    // now build using that value and the path

                    // you could increase i since the next two
                    // arguments don't have to be checked
                    i += 2;
                    break;
            }
        }
    }
}

#endif



