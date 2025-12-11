using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_6000_0_OR_NEWER
using UnityEditor.Build;
using UnityEditor.Build.Content;
#endif
using UnityEngine;
using UnityUtils.Runtime;
#if FUSION_WEAVER
using Fusion.Photon.Realtime;
#endif

namespace UnityUtils.Editor {
    public class WindowCustomExport : EditorWindow {

#if UNITY_ANDROID

        [MenuItem("Export/Export Window")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(WindowCustomExport));
        }

        GUIStyle centerLabelStyle;

        const string resourcesUtilsPrefabPath = "Utility";
        const string resourcesPhotonFusionRealtimeSettings = "PhotonAppSettings";

        const string KEY_ENABLEDEEPPROFILING = "Export_EnableDeepProfilingSupport";
        const string KEY_DELETEIL2CPPOUTPUT = "Export_DeleteIl2cppOutput";

        const string KEY_AUTORUNPLAYER = "Export_AutoRunPlayer";

        static string androidKeystorePassword = "";
        //string productName = "Demolition Derby 2";
        //string applicationIdentifier = "com.BeerMoneyGames.Demolition2";

        string CodeToVersionString(int code) {
            string tmp = code.ToString();

            string s = "";

            s = tmp.Length > 4 ? tmp.Substring(0, tmp.Length - 4) + "." : "0.";
            s += tmp.Length > 3 ? tmp.Substring(tmp.Length - 4, 1) + "." : "0.";
            if (tmp.Length == 2)
                s += "0";
            if (tmp.Length == 1)
                s += "00";

            s += tmp.Length >= 1 ? tmp.Substring(Mathf.Clamp(tmp.Length - 3, 0, tmp.Length - 3)) + "" : "0";

            return s;
        }

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

#if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), EditorGUILayout.TextField("Application Identifier", PlayerSettings.applicationIdentifier));
#else
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, EditorGUILayout.TextField("Application Identifier", PlayerSettings.applicationIdentifier));
#endif

            GUILayout.Space(20);

            int oldCode = PlayerSettings.Android.bundleVersionCode;

            EditorGUILayout.BeginHorizontal();
            PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Bundle Version Code", PlayerSettings.Android.bundleVersionCode);

            ProjectPrefs.SetBool("UnityUtils.CustomExport.AutoCopyVersionCodeToVersionString", EditorGUILayout.Toggle("Copy to Utils/Version String", ProjectPrefs.GetBool("UnityUtils.CustomExport.AutoCopyVersionCodeToVersionString")));

            EditorGUILayout.EndHorizontal();

            if (oldCode != PlayerSettings.Android.bundleVersionCode) {
                PlayerSettings.bundleVersion = CodeToVersionString(PlayerSettings.Android.bundleVersionCode);
            }

            PlayerSettings.bundleVersion = EditorGUILayout.TextField("Bundle Version", PlayerSettings.bundleVersion);
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

                if (oldCode != PlayerSettings.Android.bundleVersionCode) {
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
#if UNITY_6000_0_OR_NEWER
            //PlayerSettings.Android.useAPKExpansionFiles = GUILayout.Toggle(PlayerSettings.Android.useAPKExpansionFiles, "Split Application Binary");
            PlayerSettings.Android.splitApplicationBinary = GUILayout.Toggle(PlayerSettings.Android.splitApplicationBinary, "Split Application Binary");
#else
            PlayerSettings.Android.useAPKExpansionFiles = GUILayout.Toggle(PlayerSettings.Android.useAPKExpansionFiles, "Split Application Binary");
#endif

            ProjectPrefs.SetBool(KEY_ENABLEDEEPPROFILING, GUILayout.Toggle(ProjectPrefs.GetBool(KEY_ENABLEDEEPPROFILING, false), KEY_ENABLEDEEPPROFILING));

            ProjectPrefs.SetBool(KEY_DELETEIL2CPPOUTPUT, GUILayout.Toggle(ProjectPrefs.GetBool(KEY_DELETEIL2CPPOUTPUT, false), KEY_DELETEIL2CPPOUTPUT));

            ProjectPrefs.SetBool(KEY_AUTORUNPLAYER, GUILayout.Toggle(ProjectPrefs.GetBool(KEY_AUTORUNPLAYER, false), KEY_AUTORUNPLAYER));

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

#if UNITY_6000_0_OR_NEWER
            //EditorUserBuildSettings.androidCreateSymbols = (AndroidCreateSymbols)EditorGUILayout.EnumPopup("Symbols: ", EditorUserBuildSettings.androidCreateSymbols);
            //EditorUserBuildSettings.androidCreateSymbols = (AndroidCreateSymbols)EditorGUILayout.EnumPopup("Symbols: ", EditorUserBuildSettings.androidCreateSymbols);
            UnityEditor.Android.UserBuildSettings.DebugSymbols.level = (Unity.Android.Types.DebugSymbolLevel)EditorGUILayout.EnumPopup("Symbols: ", UnityEditor.Android.UserBuildSettings.DebugSymbols.level);
#else
            EditorUserBuildSettings.androidCreateSymbols = (AndroidCreateSymbols)EditorGUILayout.EnumPopup("Symbols: ", EditorUserBuildSettings.androidCreateSymbols);
#endif

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


#if UNITY_6000_0_OR_NEWER
        public delegate void PreBuildEvent(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType, Unity.Android.Types.DebugSymbolLevel createSymbolsZip);
        public static event PreBuildEvent PreBuild;

        public delegate void PostBuildEvent(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType, Unity.Android.Types.DebugSymbolLevel createSymbolsZip);
        public static event PostBuildEvent PostBuild;
#else
        public delegate void PreBuildEvent(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType, AndroidCreateSymbols createSymbolsZip);
        public static event PreBuildEvent PreBuild;

        public delegate void PostBuildEvent(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType, AndroidCreateSymbols createSymbolsZip);
        public static event PostBuildEvent PostBuild;
#endif


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

                if (ProjectPrefs.GetBool(KEY_ENABLEDEEPPROFILING, false))
                    tmp |= BuildOptions.EnableDeepProfilingSupport;
            }

            if (type == BuildType.Release)
                tmp |= BuildOptions.CleanBuildCache;

            if(ProjectPrefs.GetBool(KEY_AUTORUNPLAYER, false))
                tmp |= BuildOptions.AutoRunPlayer;

            return tmp;
        }


        public static void Build(AndroidArchitecture arch, bool aabExport, AndroidStore androidStore, BuildType buildType = BuildType.Release) {
            Debug.Log("Starting build");

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


#if UNITY_6000_0_OR_NEWER
            PreBuild?.Invoke(arch, aabExport, androidStore, buildType, UnityEditor.Android.UserBuildSettings.DebugSymbols.level);
#else
            PreBuild?.Invoke(arch, aabExport, androidStore, buildType, EditorUserBuildSettings.androidCreateSymbols);
#endif


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


#if UNITY_6000_0_OR_NEWER
            //PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.Android), ScriptingImplementation.IL2CPP);
#else
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
#endif



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


            //il2cpp
            if(ProjectPrefs.GetBool(KEY_DELETEIL2CPPOUTPUT, true)) {
                string il2cppDebugInformationDirectoryPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/" + filenameNoExtension + "_BackUpThisFolder_ButDontShipItWithYourGame";
                if (Directory.Exists(il2cppDebugInformationDirectoryPath))
                {
                    Debug.Log($" > Deleting il2cppOutput information folder at path '{il2cppDebugInformationDirectoryPath}'...");
                    Directory.Delete(il2cppDebugInformationDirectoryPath, true);
                }
            }
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


#if UNITY_6000_0_OR_NEWER
            PostBuild?.Invoke(arch, aabExport, androidStore, buildType, UnityEditor.Android.UserBuildSettings.DebugSymbols.level);
#else
            PostBuild?.Invoke(arch, aabExport, androidStore, buildType, EditorUserBuildSettings.androidCreateSymbols);
#endif
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

#if UNITY_6000_0_OR_NEWER
            if (debug && (ident.Length < 4 || (ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) != "test")))
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), ident + "test");
            else if (!debug && ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) == "test")
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), ident[0..^4]);
#else
            if (debug && (ident.Length < 4 || (ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) != "test")))
                PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, ident + "test");
            else if (!debug && ident.Length >= 4 && ident.Substring(ident.Length - 4, 4) == "test")
                PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, ident[0..^4]);
#endif
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

#endif
    }
}
