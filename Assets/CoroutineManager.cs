#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    public string accessEmail, accessToken;

    public Texture2D testTexture;

    public void Awake()
    {
        Instance = this;
    }
    public void Start(){
        StartCoroutine(DownloadImage());
    }

    public void ImAlive(){
        Debug.Log("nice");
    }

    private IEnumerator DownloadImage(){
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://sqill.s3.eu-west-3.amazonaws.com/video_editor_assets/asset/215/9dddf40052b9d42d541f1845cea95ee8.png?X-Amz-Algorithm=AWS4-HMAC-SHA256\u0026X-Amz-Credential=AKIAWIMIQWVUDWT2O73D%2F20221025%2Feu-west-3%2Fs3%2Faws4_request\u0026X-Amz-Date=20221025T112957Z\u0026X-Amz-Expires=86400\u0026X-Amz-SignedHeaders=host\u0026X-Amz-Signature=a1e3c85b86eabe776895d5e02c55d86d4f2894f6545ab75958dcd040e17cb1ea");
        
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("Failed Request" + request.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log("Downloaded " + request.downloadHandler.data.Length); 
            testTexture = myTexture;

            // byte[] bytes = myTexture.EncodeToPNG();
            var dirPath = Application.dataPath + "/Raw/Atlas/";
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            var atlasdirPath = Application.dataPath + "/Raw/Prefab/";
            if(!Directory.Exists(atlasdirPath)) {
                Directory.CreateDirectory(atlasdirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".png", request.downloadHandler.data);
            File.WriteAllBytes(atlasdirPath + "TinyImage" + ".png", request.downloadHandler.data);
            
            BuildPipeline.BuildAssetBundles("Assets/Bundles/AndroidBundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
            BuildPipeline.BuildAssetBundles("Assets/Bundles/IOSBundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

            AssetDatabase.Refresh();
            // EditorApplication.Quit();


            // AssetDatabase.CreateAsset(bytes, "Assets/Textures/" + "RandomName.png");

            // AssetDatabase.CreateAsset(myTexture, "Assets/Textures/" + "RandomName.asset");
            // AssetDatabase.SaveAssets();

            // TextureImporter import = (TextureImporter) TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(myTexture));

            // TextureImporterFormat formatIOS = import.GetAutomaticFormat("IOS");
            // TextureImporterFormat formatAndroid = import.GetAutomaticFormat("Android");
            // import.SetPlatformTextureSettings("IOS", 6000, formatIOS);

            // import.SetTextureSettings(formatIOS);
        } 
    }

}

#endif


