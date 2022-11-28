
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

        bool hasLink = false, hasOutput = false;
 
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
                            CoroutineManager.Instance.ParseLinkJson(contents);
                            hasLink = true;

                        }
                        if(args[x].Contains("-output")){
                            CoroutineManager.Instance.ParseOutput(args[x + 1]);
                            hasOutput = true;
                        }
                    }
                    // if(hasLink && hasOutput){
                    //     CoroutineManager.Instance.WaitForResult();
                    //     CoroutineManager.Instance.WaitForQuit();
                    // }
                    break;
            }
        }
    }
}

#endif



