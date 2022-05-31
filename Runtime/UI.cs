using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public static class UI {

        public static GameObject InstantiateUIPrefab(GameObject prefab, RectTransform parent, bool resetPosition = false) {
            GameObject tmp = InstantiateUIPrefab(prefab);

            tmp.transform.SetParent(parent);
            tmp.transform.localScale = prefab.transform.localScale;

            if (resetPosition) {
                RectTransform rT = tmp.GetComponent<RectTransform>();
                rT.offsetMax = rT.offsetMin = new Vector2(0, 0);
            }

            return tmp;
        }

        public static GameObject InstantiateUIPrefab(GameObject prefab) {
            return Object.Instantiate(prefab, prefab.transform.parent);
        }

    }
}
