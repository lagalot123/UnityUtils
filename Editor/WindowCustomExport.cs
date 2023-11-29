using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtils.Runtime;

namespace UnityUtils.Editor {
    public class WindowCustomExport : EditorWindow {

        [MenuItem("Export/Export Window")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(WindowCustomExport));
        }

        GUIStyle centerLabelStyle;

        private bool keepDebugSymbolsZip = false;

        const string resourcesUtilsPrefabPath = "Utility";
        const string resourcesPhotonFusionRealtimeSettings = "PhotonAppSettings";


        static string androidKeystorePassword = "";
        //string productName = "Demolition Derby 2";
        //string applicationIdentifier = "com.BeerMoneyGames.Demolition2";

        void OnGUI() {

            if (centerLabelStyle == null) {
                centerLabelStyle = new GUIStyle(EditorStyles.label);
                centerLabelStyle.richText = true;
                centerLabelStyle.alignment = TextAnchor.MiddleCenter;
            }
            GUILayout.BeginVertical("Box");
            GUILayout.Label("<b>Custom Export</b>", centerLabelStyle);

            GUILayout.Space(10);

            PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);
            PlayerSettings.applicationIdentifier = EditorGUILayout.TextField("Application Identifier", PlayerSettings.applicationIdentifier);

            GUILayout.Space(20);

            PlayerSettings.bundleVersion = EditorGUILayout.TextField("Bundle Version", PlayerSettings.bundleVersion);
            PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Bundle Version Code", PlayerSettings.Android.bundleVersionCode);
            PlayerSettings.macOS.buildNumber = PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
            //Debug.Log(PlayerSettings.iOS);


            GUILayout.BeginHorizontal();

            if (Resources.Load(resourcesUtilsPrefabPath) != null) {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>((AssetDatabase.GetAssetPath((Resources.Load(resourcesUtilsPrefabPath) as GameObject).GetInstanceID())));

                int uCurrentBVC = g.GetComponent<Utility>().versionCode;

                g.GetComponent<Utility>().versionCode = EditorGUILayout.IntField("Utility BVC", g.GetComponent<Utility>().versionCode);

                if (GUILayout.Button("Copy from BVC")) {
                    g.GetComponent<Utility>().versionCode = PlayerSettings.Android.bundleVersionCode;
                }

                if (uCurrentBVC != g.GetComponent<Utility>().versionCode) {
                    EditorUtility.SetDirty(g);
                    Debug.Log("Changed UVC from " + uCurrentBVC + " to " + g.GetComponent<Utility>().versionCode);
                }
            } else {
                GUILayout.Label("No Utility prefab found at \"Resources/" + resourcesUtilsPrefabPath + "\" to set version code on");
            }
            GUILayout.EndHorizontal();



#if FUSION_WEAVER
            GUILayout.BeginHorizontal();
            PhotonAppSettings s = AssetDatabase.LoadAssetAtPath<PhotonAppSettings>(
                (AssetDatabase.GetAssetPath(
                    ((PhotonAppSettings)Resources.Load("PhotonAppSettings", typeof(PhotonAppSettings))
                    ).GetInstanceID()))
                );
            //s.AppSettings.AppVersion = PlayerSettings.Android.bundleVersionCode + "";
            string sCurrentBVC = s.AppSettings.AppVersion;

            s.AppSettings.AppVersion = EditorGUILayout.TextField("Photon App Id", s.AppSettings.AppVersion);


            if (GUILayout.Button("Copy from BVC")) {
                s.AppSettings.AppVersion = PlayerSettings.Android.bundleVersionCode + "";
            }
            if (sCurrentBVC != s.AppSettings.AppVersion) {
                EditorUtility.SetDirty(s);
                Debug.Log("Changed Photon VC from " + sCurrentBVC + " to " + s.AppSettings.AppVersion);
            }

            GUILayout.EndHorizontal();         


            if (sCurrentBVC != s.AppSettings.AppVersion || uCurrentBVC != g.GetComponent(typeUtils).versionCode) {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }   
#endif

#if UNITY_ANDROID

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();


            androidKeystorePassword = EditorGUILayout.TextField("Keystore Password", EditorPrefs.GetString("UnityUtils.CustomExport.KeystorePass", ""));
            EditorPrefs.SetString("UnityUtils.CustomExport.KeystorePass", androidKeystorePassword);

            keepDebugSymbolsZip = GUILayout.Toggle(keepDebugSymbolsZip, "Generate Debug Symbols");

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Play Store APK");
            if (GUILayout.Button("Android All")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.GooglePlay);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Play Store AAB");
            if (GUILayout.Button("Android All")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, true, AndroidStore.GooglePlay);
                }
            }
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();
            GUILayout.Label("Amazon");
            if (GUILayout.Button("Amazon APK")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.Amazon);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Amazon");
            if (GUILayout.Button("Amazon AAB")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, true, AndroidStore.Amazon);
                }
            }
            GUILayout.EndHorizontal();

#endif
            GUILayout.EndVertical();

        }

        static bool PreBuildSetup() {
            if (PlayerSettings.Android.bundleVersionCode > 99999) {
                Debug.LogError("Bundle version code too high!");
                return false;
            }

            PlayerSettings.SplashScreen.showUnityLogo = false;

            return true;
        }

        public static void Build(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, bool testBuild = false, AndroidCreateSymbols createSymbolsZip = AndroidCreateSymbols.Disabled) {
            Debug.Log("Starting build");
            EditorUserBuildSettings.androidCreateSymbols = createSymbolsZip;

            SetDebugBuildStatus(testBuild);

            if (Resources.Load(resourcesUtilsPrefabPath) != null) {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>((AssetDatabase.GetAssetPath((Resources.Load(resourcesUtilsPrefabPath) as GameObject).GetInstanceID())));
                if (g.GetComponent<Utility>().store != androidStore) {
                    Debug.Log("Changing Store to " + androidStore);

                    g.GetComponent<Utility>().store = androidStore;
                    EditorUtility.SetDirty(g);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }


            string filename = "";
            int originalBundleCode = PlayerSettings.Android.bundleVersionCode;

            //#if UNITY_IOS
            //            UnityPurchasingEditor.TargetAndroidStore(AppStore.AppleAppStore);
            //#elif UNITY_ANDROID
            //            if (androidStore == AndroidStore.Amazon) {
            //                UnityPurchasingEditor.TargetAndroidStore(AppStore.AmazonAppStore);
            //            } else {
            //                UnityPurchasingEditor.TargetAndroidStore(AppStore.GooglePlay);
            //            }
            //#endif
            //Debug.Log("TODO target correct IAP store");

            EditorUserBuildSettings.buildAppBundle = aabExport;

            if (String.IsNullOrEmpty(PlayerSettings.Android.keystorePass) || String.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass)) {
                //return;
                PlayerSettings.Android.keystorePass = androidKeystorePassword;
                PlayerSettings.Android.keyaliasPass = androidKeystorePassword;
            }

            PlayerSettings.Android.targetArchitectures = arch;
            PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);


            PlayerSettings.Android.bundleVersionCode = GetArchitectureCode(originalBundleCode, PlayerSettings.Android.targetArchitectures, aabExport);


            if (testBuild) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Test_00" + PlayerSettings.Android.bundleVersionCode;
            } else if (androidStore == AndroidStore.GooglePlay) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Play_0" + PlayerSettings.Android.bundleVersionCode + "_" + arch;
            } else if (androidStore == AndroidStore.Amazon) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Amazon_00" + PlayerSettings.Android.bundleVersionCode + "_" + arch;
            }

            filename += (aabExport ? ".aab" : ".apk");


            string[] levels = new string[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < levels.Length; i++) {
                levels[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            }

            BuildPlayerOptions bo = new BuildPlayerOptions {
                scenes = levels,
                locationPathName = filename,
                target = BuildTarget.Android,
                options = testBuild ? (BuildOptions.None | BuildOptions.CompressWithLz4) : (BuildOptions.None | BuildOptions.CompressWithLz4HC)
            };

            BuildPipeline.BuildPlayer(bo);
            Debug.Log("Exported APK " + androidStore + "/" + arch);


            string burstDebugInformationDirectoryPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/" + filename + "_BurstDebugInformation_DoNotShip";
            if (Directory.Exists(burstDebugInformationDirectoryPath)) {
                Debug.Log($" > Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");

                Directory.Delete(burstDebugInformationDirectoryPath, true);
            }

            PlayerSettings.Android.bundleVersionCode = originalBundleCode;
            SetDebugBuildStatus(false);
        }



        static void SetDebugBuildStatus(bool debug) {

            if(Resources.Load(resourcesUtilsPrefabPath) != null) {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>((AssetDatabase.GetAssetPath((Resources.Load(resourcesUtilsPrefabPath) as GameObject).GetInstanceID())));

                if (g.GetComponent<Utility>().testBuild != debug) {
                    g.GetComponent<Utility>().testBuild = debug;
                    EditorUtility.SetDirty(g);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }

            EditorUserBuildSettings.development = debug;
            PlayerSettings.productName = debug ? PlayerSettings.productName + " Test" : PlayerSettings.productName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, debug ? PlayerSettings.applicationIdentifier + "test" : PlayerSettings.applicationIdentifier);
        }
        private static int GetArchitectureCode(int originalBundleCode, AndroidArchitecture targetArchitectures, bool aab) {
            if (aab)
                return 600000 + originalBundleCode;

            switch (targetArchitectures) {
                case (AndroidArchitecture.ARMv7): return 300000 + originalBundleCode;
                case (AndroidArchitecture.ARM64): return 400000 + originalBundleCode;
                //case (AndroidArchitecture.X86): return 500000 + originalBundleCode;
                case (AndroidArchitecture.All): return originalBundleCode;
            }

            return originalBundleCode;

            throw new Exception("Can't get bundle code for architecture " + targetArchitectures);
        }
    }
}
