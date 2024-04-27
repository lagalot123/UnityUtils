using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityUtils.Editor {
    public class WindowSceneSwitcher : EditorWindow {
        /// <summary>
        /// Tracks scroll position.
        /// </summary>
        private Vector2 scrollPos;
        static string[] scenesGUIDs;
        static string[] scenesPaths;
        static bool[] isInBuildSettings;
        static string[] buttonLabels;

        static bool filterByBuildScenes;
        /// <summary>
        /// Initialize window state.
        /// </summary>
        [MenuItem("Tools/Scene Switcher")]
        internal static void Init() {
            // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
            // instance if it can't find one. The second parameter is a flag for creating the window as a
            // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
            var window = (WindowSceneSwitcher)GetWindow(typeof(WindowSceneSwitcher), false, "Scene Switcher");
            window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);

            filterByBuildScenes = ProjectPrefs.GetBool("UnityUtils.SceneSwitcher.FilterByBuild", false);
        }

        void GetScenes() {
            scenesGUIDs = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/" });
            scenesPaths = scenesGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            isInBuildSettings = new bool[scenesPaths.Length];
            buttonLabels = new string[scenesPaths.Length];


            for (int i = 0; i < scenesPaths.Length; i++) {

                buttonLabels[i] = scenesPaths[i].Remove(0, 7);
                buttonLabels[i] = buttonLabels[i].Remove(buttonLabels[i].Length - 6, 6);

                for (int j = 0; j < EditorBuildSettings.scenes.Length; j++) {
                    if (scenesPaths[i] == EditorBuildSettings.scenes[j].path) {
                        isInBuildSettings[i] = true;
                    }
                }
            }

            filterByBuildScenes = ProjectPrefs.GetBool("UnityUtils.SceneSwitcher.FilterByBuild", false);
        }

        /// <summary>
        /// Called on GUI events.
        /// </summary>
        internal void OnGUI() {
            if (UnityEngine.Event.current.type == EventType.Repaint && (scenesGUIDs == null || scenesPaths == null)) {
                GetScenes();

                return;
            }

            Color defaultColor = GUI.backgroundColor;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scenes:");

            bool tmp = GUILayout.Toggle(filterByBuildScenes, "Scenes in Buid");

            if (tmp != filterByBuildScenes) {
                ProjectPrefs.SetBool("UnityUtils.SceneSwitcher.FilterByBuild", tmp);
            }
            filterByBuildScenes = tmp;



            if (GUILayout.Button("Refresh Scenes")) {
                GetScenes();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

            //GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
            //for (var i = 0; i < EditorBuildSettings.scenes.Length; i++) {
            //    var scene = EditorBuildSettings.scenes[i];
            //    if (scene.enabled) {
            //        var sceneName = Path.GetFileNameWithoutExtension(scene.path);
            //        var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });
            //        if (pressed) {
            //            //if (EditorApplication.SaveCurrentModifiedScenesIfUserWantsTo()) {
            //            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            //                //EditorApplication.OpenScene(scene.path);
            //                EditorSceneManager.OpenScene(scene.path);
            //            }
            //        }
            //    }
            //}

            //Scene[] tmp = EditorSceneManager.get;

            if (scenesGUIDs != null && scenesPaths != null) {
                for (int i = 0; i < scenesPaths.Length; i++) {
                    Scene scene = EditorSceneManager.GetSceneByPath(scenesPaths[i]);
                    if (scene != null) {
                        if (filterByBuildScenes && !isInBuildSettings[i])
                            continue;

                        if (scenesPaths[i] == EditorSceneManager.GetActiveScene().path) {
                            GUI.backgroundColor = Color.green;
                        } else if (isInBuildSettings[i]) {
                            GUI.backgroundColor = Color.yellow;
                        } else {
                            GUI.backgroundColor = defaultColor;
                        }

                        var pressed = GUILayout.Button(i + ": " + buttonLabels[i], new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });

                        GUI.backgroundColor = defaultColor;
                        if (pressed) {
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                                EditorSceneManager.OpenScene(scenesPaths[i]);
                            }
                        }
                    } else {
                        GUILayout.Label("Couldn't find scene: " + scenesPaths[i]);
                    }
                }
            } else {
                //GetScenes();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}