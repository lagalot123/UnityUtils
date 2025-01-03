using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityToolbarExtender;
using UnityEngine.Rendering;

namespace UnityUtils.Editor
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class ToolbarSceneSwitcher
    {
        static ToolbarSceneSwitcher()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }
        static string[] scenePaths;
        static GUIContent[] buttonLabels;

        static void GetScenes()
        {
            bool filterByBuildScenes = ProjectPrefs.GetBool("UnityUtils.SceneSwitcher.FilterByBuild", false);
            string[] scenePathsAll = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/" }).Select(AssetDatabase.GUIDToAssetPath).ToArray();

            List<int> tmp = new();

            for (int i = 0; i < scenePathsAll.Length; i++)
            {
                if (filterByBuildScenes)
                {
                    for (int j = 0; j < EditorBuildSettings.scenes.Length; j++)
                    {
                        if (scenePathsAll[i] == EditorBuildSettings.scenes[j].path)
                        {
                            tmp.Add(i);
                            break;
                        }
                    }
                } else {
                    tmp.Add(i);
                }

            }

            scenePaths = new string[tmp.Count];
            buttonLabels = new GUIContent[tmp.Count];

            for (int i = 0; i < scenePaths.Length; i++)
            {
                scenePaths[i] = scenePathsAll[tmp[i]];
                buttonLabels[i] = new GUIContent(scenePathsAll[tmp[i]][7..^6].Replace('/', '_'));
            }

        }

        public static int index = 0;

        static void OnToolbarGUI()
        {

            if (UnityEngine.Event.current.type == EventType.Repaint && (scenePaths == null))
            {
                GetScenes();
                return;
            }

            if (scenePaths == null)
                return;

            index = -1;

            for (int i = 0; i < scenePaths.Length; i++)
            {
                if (EditorSceneManager.GetActiveScene().path == scenePaths[i])
                {
                    index = i;
                    break;
                }
            }

            index = EditorGUILayout.Popup(index, buttonLabels, GUILayout.Width(200));

            if(index >= 0 && index < scenePaths.Length) { 
                if (EditorSceneManager.GetActiveScene().path != scenePaths[index])
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePaths[index]);
                    }
                }
            }

            bool tmp = GUILayout.Toggle(ProjectPrefs.GetBool("UnityUtils.SceneSwitcher.FilterByBuild", false), "In Buid", GUILayout.Width(70));

            if (tmp != ProjectPrefs.GetBool("UnityUtils.SceneSwitcher.FilterByBuild", false))
            {
                ProjectPrefs.SetBool("UnityUtils.SceneSwitcher.FilterByBuild", tmp);
                GetScenes();
            }

            if (GUILayout.Button("Refresh Scenes", GUILayout.Width(100)))
            {
                GetScenes();
            }            
        }
    }
}