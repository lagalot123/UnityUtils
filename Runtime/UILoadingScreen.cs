using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UILoadingScreen : MonoBehaviour {
        public Image imgLoadingBar;

        public void SetProgress(float p) {
            imgLoadingBar.fillAmount = Mathf.MoveTowards(imgLoadingBar.fillAmount, p + 0.1f, Time.deltaTime * 5);
        }

        public void Toggle(bool b) {
            gameObject.SetActive(b);
            imgLoadingBar.fillAmount = 0;
        }

    }
}
