using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    _I = FindObjectOfType<Utility>();
                    if (_I == null && Application.isPlaying) {
                        GameObject tmp = (Instantiate(Resources.Load("Utility", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject);
                        tmp.name = "Utility";
                        _I = tmp.GetComponent<Utility>();
                    }
                    DontDestroyOnLoad(_I.gameObject);
                }
                return _I;
            }
        }


        public int versionCode;
        public bool testBuild;

        public AndroidStore store;

        public UILoadingScreen loadingScreen;


        public UnityEngine.Events.UnityEvent onLevelLoaded;

        public void ReloadCurrentLevel() {
            LoadLevel(SceneManager.GetActiveScene().name);
        }

        public void LoadLevel(string scene) {
            StartCoroutine(SceneLoad(scene));
        }

        internal virtual IEnumerator SceneLoad(string scene) {
            Time.timeScale = 1;
            loadingScreen.Toggle(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            operation.allowSceneActivation = false;
            while (!operation.isDone) {
                loadingScreen.SetProgress(operation.progress);

                if (operation.progress >= 0.9f)
                    operation.allowSceneActivation = true;

                yield return null;
            }
        }

        void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        internal virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex == 0 || scene.buildIndex == 1)
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            else
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

            StartCoroutine(WaitForAwakeStart());

            if (onLevelLoaded != null)
                onLevelLoaded.Invoke();
        }

        IEnumerator WaitForAwakeStart() {
            yield return new WaitForSeconds(0.8f);

            loadingScreen.SetProgress(1);

            yield return new WaitForSeconds(0.2f);

            loadingScreen.Toggle(false);
        }


        public static void OpenStorePage() {
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
    }

}
