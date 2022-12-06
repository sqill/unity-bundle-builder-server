
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
    public static void LoadScene()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        string links = "", path = "";
 
        for (int i = 0; i < args.Length; i++) 
        {
            // Debug.Log ("ARG " + i + ": " + args [i]);

            switch(args[i])
            {
                case "-LoadScene":
                    EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");

                    for (int x = 0; x < args.Length; x++) 
                    {
                        if(args[x].Contains("-link")){
                            string contents = File.ReadAllText(args[x + 1]);
                            Debug.Log("JSON contents " + contents );
                            links = contents;

                        }
                        if(args[x].Contains("-output")){
                            path = args[x + 1];
                            
                        }

                    }
                    if(links != "" && path != "")
                        CoroutineManager.Instance.Parse(links, path);  
                    break;
            }
        }
    }
}

#endif



