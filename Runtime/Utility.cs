using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]

namespace UnityUtils.Runtime {

    public enum AndroidStore {
        GooglePlay,
        Amazon,
        Other
    }

    public class Utility : MonoBehaviour {
        private static Utility _I;
        public static Utility I {
            get {
                if (_I == null) {
                    _I = FindAnyObjectByType<Utility>();
                }
                return _I;
            }
        }

#if UNITY_UTILS_AUTO_INIT_UTILITY_PREFAB

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad(){
#if UNITY_EDITOR
            if (Resources.Load("Utility", typeof(GameObject)) == null)
                Debug.LogError("No Utility prefab found at Resources/Utility.prefab");
#endif

            GameObject tmp = (Instantiate(Resources.Load("Utility", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject);
            tmp.name = "Utility";
            _I = tmp.GetComponent<Utility>();
            DontDestroyOnLoad(_I.gameObject);

            _I.OnInit();
        }
#endif

        public int versionCode;
        public bool testBuild;

        public AndroidStore store;

        public UILoadingScreen loadingScreen;


        public UnityEngine.Events.UnityEvent onLevelLoadingStarted;
        public UnityEngine.Events.UnityEvent onLevelLoaded;
        public UnityEngine.Events.UnityEvent onLevelLoadedAfterWaitForAwakeStart;

        virtual public void OnInit() { 
        
        }

        virtual public void ReloadCurrentLevel() {
            LoadLevel(SceneManager.GetActiveScene().name);
        }

        virtual public async void LoadLevel(string scene) {
            await SceneLoad(scene);
        }

        virtual public async Task SceneLoad(string scene) {
            Time.timeScale = 1;
            loadingScreen.Toggle(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            operation.allowSceneActivation = false;

            if (onLevelLoadingStarted != null)
                onLevelLoadingStarted.Invoke();

            while (!operation.isDone) {
                loadingScreen.SetProgress(operation.progress);

                if (operation.progress >= 0.9f) {
                    await Awaitable.NextFrameAsync();
                    await Awaitable.NextFrameAsync();
                    operation.allowSceneActivation = true;
                }

                await Awaitable.NextFrameAsync();
            }
        }

        virtual public void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        virtual public void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        virtual public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex == 0 || scene.buildIndex == 1)
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            else
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (scene.buildIndex != 0)
                WaitForAwakeStart();

            if (onLevelLoaded != null)
                onLevelLoaded.Invoke();
        }

        virtual public async void WaitForAwakeStart() {
            await Awaitable.WaitForSecondsAsync(0.8f);

            loadingScreen.SetProgress(1);

            await Awaitable.WaitForSecondsAsync(0.2f);

            loadingScreen.Toggle(false);


            if (onLevelLoadedAfterWaitForAwakeStart != null)
                onLevelLoadedAfterWaitForAwakeStart.Invoke();
        }

        public static string overrideStoreLink;

        public static void OpenStorePage() {
            if (!string.IsNullOrEmpty(overrideStoreLink)) {
                Application.OpenURL(overrideStoreLink);
                return;
            }

#if UNITY_ANDROID
            if (I.store == AndroidStore.GooglePlay) {
                Application.OpenURL("market://details?id=" + Application.identifier);
            } else if (I.store == AndroidStore.Amazon) {
                Application.OpenURL("http://www.amazon.com/gp/mas/dl/android?p=" + Application.identifier);
            }
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/" + Application.identifier);
#endif
        }

        public static bool IsGenuine {
            get {
                return
                    !Application.genuineCheckAvailable
                    || (Application.genuineCheckAvailable && Application.genuine)
                    || I.testBuild;
            } 
        }
    }

}
