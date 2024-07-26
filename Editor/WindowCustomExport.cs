using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtils.Runtime;
#if FUSION_WEAVER
using Fusion.Photon.Realtime;
#endif

namespace UnityUtils.Editor {
    public class WindowCustomExport : EditorWindow {

        [MenuItem("Export/Export Window")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(WindowCustomExport));
        }

        GUIStyle centerLabelStyle;

        const string resourcesUtilsPrefabPath = "Utility";
        const string resourcesPhotonFusionRealtimeSettings = "PhotonAppSettings";

        const string KEY_ENABLEDEEPPROFILING = "Export_EnableDeepProfilingSUpport";

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
            //PlayerSettings.applicationIdentifier = EditorGUILayout.TextField("Application Identifier", PlayerSettings.applicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, EditorGUILayout.TextField("Application Identifier", PlayerSettings.applicationIdentifier));

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
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
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


            if (sCurrentBVC != s.AppSettings.AppVersion) {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }   
#endif

#if UNITY_ANDROID

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();


            androidKeystorePassword = EditorGUILayout.TextField("Keystore Password", ProjectPrefs.GetString("UnityUtils.CustomExport.KeystorePass", ""));
            ProjectPrefs.SetString("UnityUtils.CustomExport.KeystorePass", androidKeystorePassword);

            GUILayout.EndHorizontal();
            GUILayout.Space(20);

           
            GUILayout.BeginHorizontal();
            PlayerSettings.Android.useAPKExpansionFiles = GUILayout.Toggle(PlayerSettings.Android.useAPKExpansionFiles, "Split Application Binary");

            EditorPrefs.SetBool(KEY_ENABLEDEEPPROFILING, GUILayout.Toggle(EditorPrefs.GetBool(KEY_ENABLEDEEPPROFILING, false), KEY_ENABLEDEEPPROFILING));

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            EditorUserBuildSettings.androidCreateSymbols = (AndroidCreateSymbols)EditorGUILayout.EnumPopup("Symbols: ", EditorUserBuildSettings.androidCreateSymbols);

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            //GUILayout.Label("Play Store APK");
            if (GUILayout.Button("PlayStore APK")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.GooglePlay);
                }
            }

            //GUILayout.Label("Play Store AAB");
            if (GUILayout.Button("PlayStore AAB")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, true, AndroidStore.GooglePlay);
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Amazon APK")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.Amazon);
                }
            }

            if (GUILayout.Button("Amazon AAB")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, true, AndroidStore.Amazon);
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Test APK")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.GooglePlay, BuildType.Tester);
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Debug APK")) {
                if (PreBuildSetup()) {
                    Build(AndroidArchitecture.All, false, AndroidStore.GooglePlay, BuildType.Debug);
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Open Export Folder")) {
                EditorUtility.RevealInFinder(Application.dataPath);
            }



#endif

#if UNITY_IOS
            GUILayout.Space(20);
            GUILayout.Label("XCode Project");
            GUILayout.BeginHorizontal();


            Path = EditorGUILayout.TextField("Location", Path);

            if (GUILayout.Button("...", GUILayout.Width(50))) {
                Path = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Folder = EditorGUILayout.TextField("Folder Name", Folder);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Export XCode Project")) {
                if (PreBuildSetup()) {
                    Build();
                }
            }            

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (GUILayout.Button("Open Export Folder")) {
                EditorUtility.RevealInFinder(Path);
            }
#endif


            GUILayout.EndVertical();

        }

        static string _path;
        static string _folder;


        static string Path {
            get {
                if (string.IsNullOrEmpty(_path)) {
                    _path = ProjectPrefs.GetString("UnityUtils.Editor.WindowCustomExport.Path", "");
                }
                if (string.IsNullOrEmpty(_path)) {
                    _path = Application.dataPath;
                }
                return _path;
            }
            set {
                _path = value;
                ProjectPrefs.SetString("UnityUtils.Editor.WindowCustomExport.Path", _path);
            }
        }

        static string Folder {
            get {
                if (string.IsNullOrEmpty(_folder)) {
                    _folder = ProjectPrefs.GetString("UnityUtils.Editor.WindowCustomExport.Folder", "");
                }
                if (string.IsNullOrEmpty(_folder)) {
                    _folder = "XC_" + PlayerSettings.productName;
                }
                return _folder;
            }
            set {
                _folder = value;
                ProjectPrefs.SetString("UnityUtils.Editor.WindowCustomExport.Folder", _folder);
            }
        }

        static bool PreBuildSetup() {
            if (PlayerSettings.Android.bundleVersionCode > 99999) {
                Debug.LogError("Bundle version code too high!");
                return false;
            }

            PlayerSettings.SplashScreen.showUnityLogo = false;

            return true;
        }


        public delegate void PreBuildEvent(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType, AndroidCreateSymbols createSymbolsZip);
        public static event PreBuildEvent PreBuild;

        public enum BuildType {
            Release = 0,
            Tester = 1,
            Debug = 2
        }

        public void Build(BuildType buildType = BuildType.Release) {
            Debug.Log("Starting build");
            SetDebugBuildStatus(buildType);

            string filename = Path;

            string filenameNoExtension = filename;
            //filename += (aabExport ? ".aab" : ".apk");


            string[] levels = new string[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < levels.Length; i++) {
                levels[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            }

            BuildPlayerOptions bo = new BuildPlayerOptions {
                scenes = levels,
                locationPathName = Path + "/" + Folder,
                target = BuildTarget.iOS,
                options = GetBuildOptions(buildType),
            };

            BuildPipeline.BuildPlayer(bo);
            Debug.Log("Exported Xcode Project");


            //string burstDebugInformationDirectoryPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/" + filenameNoExtension + "_BurstDebugInformation_DoNotShip";
            string burstDebugInformationDirectoryPath = Path + "/" + Folder + "_BurstDebugInformation_DoNotShip";
            if (Directory.Exists(burstDebugInformationDirectoryPath)) {
                Debug.Log($" > Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");

                Directory.Delete(burstDebugInformationDirectoryPath, true);
            }

            SetDebugBuildStatus(buildType);
        }


        static BuildOptions GetBuildOptions(BuildType type) {
            BuildOptions tmp = BuildOptions.None | ((type != BuildType.Release) ? BuildOptions.CompressWithLz4 : BuildOptions.CompressWithLz4HC);

            if (type == BuildType.Debug) {
                tmp = tmp | BuildOptions.Development | BuildOptions.ConnectWithProfiler;
            }

            if (type == BuildType.Release)
                tmp |= BuildOptions.CleanBuildCache;

            return tmp;
        }


        public static void Build(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType = BuildType.Release) {
            Debug.Log("Starting build");
            EditorUserBuildSettings.androidCreateSymbols = EditorPrefs.GetBool("", false) ? AndroidCreateSymbols.Debugging : AndroidCreateSymbols.Disabled;

            SetDebugBuildStatus(buildType);

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


            PreBuild?.Invoke(arch, aabExport, androidStore, buildType, EditorUserBuildSettings.androidCreateSymbols);

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


            if (buildType != BuildType.Release) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Test_00" + PlayerSettings.Android.bundleVersionCode;
            } else if (androidStore == AndroidStore.GooglePlay) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Play_0" + PlayerSettings.Android.bundleVersionCode + "_" + arch;
            } else if (androidStore == AndroidStore.Amazon) {
                filename = PlayerSettings.productName + "_" + Application.version + "_Amazon_00" + PlayerSettings.Android.bundleVersionCode + "_" + arch;
            }

            string filenameNoExtension = filename;
            filename += (aabExport ? ".aab" : ".apk");


            string[] levels = new string[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < levels.Length; i++) {
                levels[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            }

            BuildPlayerOptions bo = new BuildPlayerOptions {
                scenes = levels,
                locationPathName = filename,
                target = BuildTarget.Android,
                options = GetBuildOptions(buildType),
            };

            BuildPipeline.BuildPlayer(bo);
            Debug.Log("Exported APK " + androidStore + "/" + arch);

            //old burst debug information folder export path
            string burstDebugInformationDirectoryPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/" + filenameNoExtension + "_BurstDebugInformation_DoNotShip";
            if (Directory.Exists(burstDebugInformationDirectoryPath)) {
                Debug.Log($" > Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");

                Directory.Delete(burstDebugInformationDirectoryPath, true);
            }

            //new burst debug information folder export path
            burstDebugInformationDirectoryPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/" + Application.productName + "_BurstDebugInformation_DoNotShip";
            if (Directory.Exists(burstDebugInformationDirectoryPath)) {
                Debug.Log($" > Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");

                Directory.Delete(burstDebugInformationDirectoryPath, true);
            }

            PlayerSettings.Android.bundleVersionCode = originalBundleCode;
            SetDebugBuildStatus(BuildType.Release);
        }



        static void SetDebugBuildStatus(BuildType type) {
            bool debug = type != BuildType.Release;

            if (Resources.Load(resourcesUtilsPrefabPath) != null) {
                GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>((AssetDatabase.GetAssetPath((Resources.Load(resourcesUtilsPrefabPath) as GameObject).GetInstanceID())));


                if (g.GetComponent<Utility>().testBuild != debug) {
                    g.GetComponent<Utility>().testBuild = debug;
                    EditorUtility.SetDirty(g);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }

            EditorUserBuildSettings.development = debug;

            string name = PlayerSettings.productName;

            if (debug && (name.Length < 5 || (name.Length >= 5 && name.Substring(name.Length - 5, 5) != " Test")))
                PlayerSettings.productName = PlayerSettings.productName + " Test";
            else if (!debug && name.Length >= 5 && name.Substring(name.Length - 5, 5) == " Test")
                PlayerSettings.productName = name[0..^5];

            string ident = PlayerSettings.applicationIdentifier;

            if (debug && (ident.Length < 4 || (ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) != "test")))
                PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, ident + "test");
            else if (!debug && ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) == "test")
                PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, ident[0..^4]);
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
