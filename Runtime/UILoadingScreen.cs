using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UILoadingScreen : MonoBehaviour {
        public Image imgLoadingBar;

        private float f;

        void Update() {
            if(f > imgLoadingBar.fillAmount)
                imgLoadingBar.fillAmount = Mathf.MoveTowards(imgLoadingBar.fillAmount, f, Time.unscaledDeltaTime * 5);
        }

        public void SetProgress(float p) {
            f = p;
        }

        public void Toggle(bool b) {
            gameObject.SetActive(b);
            imgLoadingBar.fillAmount = 0;
            f = 0;
        }
    }
}
