using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UIRatingPopup : MonoBehaviourSingleton<UIRatingPopup> {

        public GameObject go;
        public Text txtTitle;

        public string linkRatingFallback = "https://beermoneygames.com";

        public int minTriggers = 2;
        private int triggers = 0;
        public bool onlyShowOnce = false;

        public void TriggerCheck() {
            triggers++;

            if (triggers >= minTriggers) {
                Toggle();
                triggers = 0;
            }
        }

        const string playerPrefKey = "UnitUtils.RatingPopup.Seen";


        private void Start() {
            txtTitle.text = "Enjoying " + Application.productName + "?";
            go.SetActive(false);
        }

        public void Toggle() {
            if (PlayerPrefs.GetInt(playerPrefKey, 0) != 0) {
                go.SetActive(false);
                return;
            }
            go.SetActive(!go.activeSelf);
        }

        public void NoThanks() { 
            if(onlyShowOnce)
                PlayerPrefs.SetInt(playerPrefKey, 1);

            Toggle();
        }

        public void Rate() {
#if UNITY_ANDROID
            if (Utility.I.store == AndroidStore.GooglePlay) {
                Application.OpenURL("market://details?id=" + Application.identifier);
            } else if (Utility.I.store == AndroidStore.Amazon) {
                Application.OpenURL("http://www.amazon.com/gp/mas/dl/android?p=" + Application.identifier);
            } else {
                Application.OpenURL(linkRatingFallback);
            }
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/" + Application.identifier);
#else
            Application.OpenURL(linkRatingFallback);
#endif


            PlayerPrefs.SetInt(playerPrefKey, 1);
            go.SetActive(false);
        }


    }
}
