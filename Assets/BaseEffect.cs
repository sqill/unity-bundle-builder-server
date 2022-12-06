#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BaseEffect : MonoBehaviour
{
    [Serializable]
    public struct EffectsAttributes
    {
        public int fps;
        public Sprite[] sprites;
        public bool loop;
    }


    [SerializeField]
    public EffectsAttributes attributes;

    public void Init(string name, int fps, bool loop)
    {
        gameObject.name = name;
        attributes.fps = fps;
        attributes.loop = loop;
        SetSprites("Assets/Raw/Prefab/" + name + ".png");
    }

    public void SetSprites(string path)
    {
        var objects = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = LoadSprites(objects);
        int middle = (int) (sprites.Length * 0.5f);
        Debug.Log("Set sprites" + sprites.Length + "with path " + path);
        attributes.sprites = sprites;
        // Image image = gameObject.AddComponent<Image>();
        // image.sprite = sprites[middle];
        // image.SetNativeSize();
    }
    
    private Sprite[] LoadSprites(UnityEngine.Object[] objects)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach(UnityEngine.Object o in objects)
        {
            Debug.Log("Object _" + o);
            Sprite s = o as Sprite;
            if(s != null){
                sprites.Add(s);
            }
        }
        return sprites.ToArray();
    }
}
#endif
