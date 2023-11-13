using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils {
//    public class WindowCustomExport : EditorWindow {
//        [MenuItem("Export/Export Window")]
//        public static void ShowWindow() {
//            EditorWindow.GetWindow(typeof(WindowCustomExport));
//        }

//        GUIStyle centerLabelStyle;

//        private bool keepDebugSymbolsZip = false;

//        const string resourcesUtilsPrefabPath = "Utility";
//        const string resourcesPhotonFusionRealtimeSettings = "PhotonAppSettings";
//        string typeUtils;

//        void OnGUI() {

//            if (centerLabelStyle == null) {
//                centerLabelStyle = new GUIStyle(EditorStyles.label);
//                centerLabelStyle.richText = true;
//                centerLabelStyle.alignment = TextAnchor.MiddleCenter;
//            }
//            GUILayout.BeginVertical("Box");
//            GUILayout.Label("<b>Custom Export</b>", centerLabelStyle);

//            GUILayout.Space(10);

//            PlayerSettings.bundleVersion = EditorGUILayout.TextField("Bundle Version", PlayerSettings.bundleVersion);
//            PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Bundle Version Code", PlayerSettings.Android.bundleVersionCode);
//            PlayerSettings.macOS.buildNumber = PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
//            //Debug.Log(PlayerSettings.iOS);


//            GUILayout.BeginHorizontal();

//            GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>((AssetDatabase.GetAssetPath((Resources.Load(resourcesUtilsPrefabPath) as GameObject).GetInstanceID())));
//            int uCurrentBVC = g.GetComponent(typeUtils).versionCode;

//            g.GetComponent(typeUtils).versionCode = EditorGUILayout.IntField("Utility BVC", g.GetComponent(typeUtils).versionCode);

//            if (GUILayout.Button("Copy from BVC")) {
//                g.GetComponent(typeUtils).versionCode = PlayerSettings.Android.bundleVersionCode;
//            }

//            if (uCurrentBVC != g.GetComponent(typeUtils).versionCode) {
//                EditorUtility.SetDirty(g);
//                Debug.Log("Changed UVC from " + uCurrentBVC + " to " + g.GetComponent(typeUtils).versionCode);
//            }

//            GUILayout.EndHorizontal();


//            GUILayout.BeginHorizontal();

//            PhotonAppSettings s = AssetDatabase.LoadAssetAtPath<PhotonAppSettings>(
//                (AssetDatabase.GetAssetPath(
//                    ((PhotonAppSettings)Resources.Load("PhotonAppSettings", typeof(PhotonAppSettings))
//                    ).GetInstanceID()))
//                );
//            //s.AppSettings.AppVersion = PlayerSettings.Android.bundleVersionCode + "";
//            string sCurrentBVC = s.AppSettings.AppVersion;

//            s.AppSettings.AppVersion = EditorGUILayout.TextField("Photon App Id", s.AppSettings.AppVersion);

//            if (GUILayout.Button("Copy from BVC")) {
//                s.AppSettings.AppVersion = PlayerSettings.Android.bundleVersionCode + "";
//            }

//            if (sCurrentBVC != s.AppSettings.AppVersion) {
//                EditorUtility.SetDirty(s);
//                Debug.Log("Changed Photon VC from " + sCurrentBVC + " to " + s.AppSettings.AppVersion);
//            }

//            GUILayout.EndHorizontal();


//            if (sCurrentBVC != s.AppSettings.AppVersion || uCurrentBVC != g.GetComponent(typeUtils).versionCode) {
//                AssetDatabase.Refresh();
//                AssetDatabase.SaveAssets();
//            }

//#if UNITY_ANDROID


//            GUILayout.BeginHorizontal();

//            keepDebugSymbolsZip = GUILayout.Toggle(keepDebugSymbolsZip, "Generate Debug Symbols");

//            GUILayout.EndHorizontal();

//            GUILayout.Space(20);

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Play Store APK");
//            if (GUILayout.Button("Android All")) {
//                if (PreBuildSetup()) {
//                    Build(AndroidArchitecture.All, false, );
//                }
//            }
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Play Store AAB");
//            if (GUILayout.Button("Android All")) {
//                if (PreBuildSetup()) {
//                    Build(AndroidArchitecture.All, true);
//                }
//            }
//            GUILayout.EndHorizontal();

//#endif
//            GUILayout.EndVertical();

//        }

//        static bool PreBuildSetup() {
//            if (PlayerSettings.Android.bundleVersionCode > 99999) {
//                Debug.LogError("Bundle version code too high!");
//                return false;
//            }

//            PlayerSettings.SplashScreen.showUnityLogo = false;

//            return true;
//        }

//        public static void Build(AndroidArchitecture arch, bool aabExport, Utility.Store apkStorey, bool testBuild = false, bool createSymbolsZip = false) {


//        }
//    }
}
