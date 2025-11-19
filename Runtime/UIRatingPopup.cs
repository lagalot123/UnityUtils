using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UIRatingPopup : MonoBehaviourSingleton<UIRatingPopup> {

        public GameObject go;
        public Text txtTitle;

        public int minTriggers = 2;
        private int triggers = 0;
        public bool onlyShowOnce = false;

        public void TriggerCheck() {
#if !UNITY_WEBGL
            triggers++;

            if (triggers >= minTriggers) {
                Toggle();
                triggers = 0;
            }
#endif
        }

        const string playerPrefKey = "UnityUtils.RatingPopup.Seen";


        private void Start() {
            go.SetActive(false);
        }

        public void Toggle() {
            if (PlayerPrefs.GetInt(playerPrefKey, 0) != 0) {
                go.SetActive(false);
                return;
            }
            txtTitle.text = "Enjoying " + Application.productName + "?";
            go.SetActive(!go.activeSelf);
        }

        public void NoThanks() { 
            if(onlyShowOnce)
                PlayerPrefs.SetInt(playerPrefKey, 1);

            Toggle();
        }

        public void Rate() {
            Utility.OpenStorePage();

            PlayerPrefs.SetInt(playerPrefKey, 1);
            go.SetActive(false);
        }


    }
}
