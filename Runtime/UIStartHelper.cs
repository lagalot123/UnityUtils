using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils.Runtime {
    public class UIStartHelper : MonoBehaviour{
        public bool resetPosition = true;
        public bool disable = true;

        void Awake() {
            if (resetPosition) {
                RectTransform rT = GetComponent<RectTransform>();
                rT.offsetMax = rT.offsetMin = new Vector2(0, 0);
            }

            gameObject.SetActive(!disable);
        }

    }
}
