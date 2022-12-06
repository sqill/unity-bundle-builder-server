
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
// Automatically convert any texture file with "_bumpmap"
// in its file name into a normal map.

class PostprocessTexture : AssetPostprocessor
{

    void OnPreprocessTexture()
    {
        if (assetPath.Contains("Effect"))
        {
            if(assetPath.Contains("Prefab"))
            {
                TextureImporter import = (TextureImporter) assetImporter;
                Debug.Log("Process Effects");
                ImportEffect(import, assetPath);
            }
            if (assetPath.Contains("Atlas"))
            {
                TextureImporter import = (TextureImporter) assetImporter;
                ImportTexture(import, 512, "TestBundle1");
            }
       
        }
        else
        {
            if (assetPath.Contains("Atlas"))
            {
                TextureImporter import = (TextureImporter) assetImporter;
                ImportTexture(import, 256, "TestBundle1");

            }
            if(assetPath.Contains("Prefab"))
            {
                TextureImporter import = (TextureImporter) assetImporter;
                ImportTexture(import, 2048, "TestBundle2");
            }
        }
    }
    void OnPostprocessTexture(Texture2D texture)
    {
        // string path = AssetDatabase.GetAssetPath(texture);
        if(assetPath.Contains("Effect"))
        { 
            if(assetPath.Contains("Prefab"))
            {
                TextureImporter import = (TextureImporter) assetImporter;

                //Replace value with rows and colums from json
                string name = Path.GetFileName(assetPath).Split('.')[0];
                string finalName = Path.GetFileName(assetPath).Split(',')[1];

                string[] splitString = name.Split('_');
                if(import.spritesheet.Length == 0)
                {
                    // GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Raw/Prefab/" + name + ".prefab", typeof(GameObject));

                    // Debug.Log("Prefab name" + prefab.name);
                    import.spritesheet = DivideIntoSprites(texture, int.Parse(splitString[1]), int.Parse(splitString[2]), int.Parse(splitString[3]), name );
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // this takes time, approx. 3s per Asset

                    // AssetDatabase.RenameAsset(assetPath, finalName);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    // var sprites = objects.Where(q => q is Sprite).Cast<Sprite>();
                    
                            // GenerateEffectObject(name + imageIndex, fps, loop);
                    CoroutineManager.Instance.GenerateEffectObject(name, int.Parse(splitString[4]), System.Convert.ToBoolean(splitString[5]));

                }
                // else
                // {
                //     var objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                //     Sprite[] sprites = LoadSprites(objects);
                //     CoroutineManager.Instance.GenerateEffectObject(name,  int.Parse(splitString[4]), System.Convert.ToBoolean(splitString[5]), sprites);
                // }
                // AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // this takes time, approx. 3s per Asset
                Debug.Log("Finish Effect Import " + Path.GetFileName(assetPath));
            }
        }
    }

    private Sprite[] LoadSprites(Object[] objects)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach(Object o in objects){
            Sprite s = o as Sprite;
            if(s != null){
                sprites.Add(s);
            }
        }
        return sprites.ToArray();
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

    private void ImportEffect(TextureImporter import, string path)
    {
        import.isReadable = true;
        import.textureType = TextureImporterType.Sprite;
        import.spriteImportMode = SpriteImportMode.Multiple;
        import.textureCompression = TextureImporterCompression.Uncompressed;
    }
    
    private SpriteMetaData[] DivideIntoSprites(Texture2D texture, int columns, int rows, int count, string name)
    {
        // BaseEffect BaseEffect = prefab.GetComponent<BaseEffect>();
        texture.name = name;
        int spriteWidth = texture.width / columns;
        int spriteHeigth = texture.height / rows;
        int spriteIndex = 0;
        
        SpriteMetaData[] newData = new SpriteMetaData[count];

        for(int j = 0; j < columns; j++)
        {
            for(int i = rows - 1; i >= 0; i--)
            {
                if(count <= spriteIndex)
                    break;

                
                SpriteMetaData smd = new SpriteMetaData();
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 0;
                int newY = Mathf.Abs(i - (rows - 1));
                smd.name = texture.name+"_"+newY+"x"+j; // "name_1x7" for 2nd row & 8th column
                smd.rect = new Rect(j*spriteWidth, i*spriteHeigth, spriteWidth, spriteHeigth);
                newData[spriteIndex] = smd;
                spriteIndex++;
            }
        }
        int middle = (int) (count * 0.5f);

        GenerateThumbnail(texture, newData[middle].rect, name);

        return newData;
    }

    private void GenerateThumbnail(Texture2D t, Rect newRect, string name)
    {
        string atlasdirPath = Application.dataPath + "/Raw/Atlas/";
        string atlasPath = atlasdirPath + name + ".png";
        if(!File.Exists(atlasPath))
        {
            Sprite newSprite = Sprite.Create(t, newRect,new Vector2(0.5f, 0.5f));
            var croppedTexture = new Texture2D( (int)newSprite.rect.width, (int)newSprite.rect.height );
            var pixels = newSprite.texture.GetPixels(  (int)newSprite.textureRect.x, 
                                         (int)newSprite.textureRect.y, 
                                         (int)newSprite.textureRect.width, 
                                         (int)newSprite.textureRect.height );
            croppedTexture.SetPixels( pixels );
            croppedTexture.Apply();

            byte[] bytes = croppedTexture.EncodeToPNG();
            File.WriteAllBytes(atlasPath, bytes);
            AssetDatabase.ImportAsset("Assets/Raw/Atlas/" + name + ".png", ImportAssetOptions.ForceUpdate); // this takes time, approx. 3s per Asset
        }
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