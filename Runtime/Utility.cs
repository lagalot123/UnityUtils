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

    public class Utility : MonoBehaviour
    {

        public int versionCode;
        public bool testBuild;

        public AndroidStore store;

        public UILoadingScreen loadingScreen;


        public void LoadLevel(string scene) {
            StartCoroutine(SceneLoad(scene));
        }

        IEnumerator SceneLoad(string scene) {
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

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex == 0 || scene.buildIndex == 1)
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            else
                Screen.sleepTimeout = SleepTimeout.NeverSleep;

            StartCoroutine(WaitForAwakeStart());
        }

        IEnumerator WaitForAwakeStart() {
            yield return new WaitForSeconds(1);

            loadingScreen.Toggle(false);
        }

    }
}
