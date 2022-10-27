
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Automatically convert any texture file with "_bumpmap"
// in its file name into a normal map.

class PostprocessTexture : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("Atlas"))
        {
            TextureImporter import = (TextureImporter) assetImporter;
            ImportTexture(import, 256, "TestBundle1");
        }
        if(assetPath.Contains("Prefab"))
        {
            TextureImporter import = (TextureImporter) assetImporter;
            ImportTexture(import, 8192, "TestBundle2");
        }
    }

    private void ImportTexture(TextureImporter import, int size, string bundle)
    {
        TextureImporterFormat formatIOS = import.GetAutomaticFormat("IOS");
        TextureImporterFormat formatAndroid = import.GetAutomaticFormat("Android");
        TextureImporterFormat formatAlone = import.GetAutomaticFormat("Standalone");
        import.SetPlatformTextureSettings("Android", size, formatAndroid);
        import.SetPlatformTextureSettings("IOS", size, formatIOS);
        import.SetPlatformTextureSettings("Standalone", size, formatAlone);
    }
}

#endif


// using UnityEditor;
// using UnityEngine;
// using System.Collections;

// // Postprocesses all textures that are placed in a folder
// // "invert color" to have their colors inverted.
// public class PostprocessTexture : AssetPostprocessor
// {
//     void OnPostprocessTexture(Texture2D texture)
//     {
//         // Only post process textures if they are in a folder
//         // "invert color" or a sub folder of it.        
//         var dirPath = AssetDatabase.La(texture);
//         Debug.Log("hereeeeeeeee");
        
//         Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/SaveImages/" + texture.name, typeof(Texture2D));



//         // if (AssetDatabase.GetAssetPath(texture).Contains("SaveTinyImages"))
//         // {
//         //     TextureImporter import = (TextureImporter) TextureImporter.GetAtPath(dirPath);

//         //     TextureImporterFormat formatIOS = import.GetAutomaticFormat("IOS");
//         //     TextureImporterFormat formatAndroid = import.GetAutomaticFormat("Android");
//         //     TextureImporterFormat formatAlone = import.GetAutomaticFormat("Standalone");
//         //     Debug.Log(texture.name);
//         //     import.SetPlatformTextureSettings("Android", 256, formatAndroid);
//         //     import.SetPlatformTextureSettings("IOS", 256, formatIOS);
//         //     import.SetPlatformTextureSettings("Standalone", 256, formatAlone);
//         // }

//         if (!dirPath.Contains("SaveImages"))
//             return;

//         Debug.Log("here");


//         TextureImporter import2 = (TextureImporter) TextureImporter.GetAtPath(dirPath);

//         TextureImporterFormat format2IOS = import2.GetAutomaticFormat("IOS");
//         TextureImporterFormat format2Android = import2.GetAutomaticFormat("Android");
//         TextureImporterFormat format2Alone = import2.GetAutomaticFormat("Standalone");

//         import2.SetPlatformTextureSettings("Android", 6000, format2Android);
//         import2.SetPlatformTextureSettings("IOS", 6000, format2IOS);
//         import2.SetPlatformTextureSettings("Standalone", 6000, format2Alone);



//         // for (int m = 0; m < texture.mipmapCount; m++)
//         // {
//         //     Color[] c = texture.GetPixels(m);

//         //     for (int i = 0; i < c.Length; i++)
//         //     {
//         //         c[i].r = 1 - c[i].r;
//         //         c[i].g = 1 - c[i].g;
//         //         c[i].b = 1 - c[i].b;
//         //     }
//         //     texture.SetPixels(c, m);
//         // }
//         // Instead of setting pixels for each mip map level, you can modify
//         // the pixels in the highest mipmap then use texture.Apply(true);
//         // to generate lower mip levels.
//     }
// }