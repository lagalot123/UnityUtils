using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils.Runtime {
    public class UITabs : MonoBehaviour {

        public int currentTab;

        public GameObject[] tabs;
        public GameObject[] tabSelectorOutlines;

        virtual public void Start() {
            for (int i = 0; i < tabs.Length; i++) {
                tabs[i].SetActive(i == currentTab);

                if (tabSelectorOutlines != null && tabSelectorOutlines.Length > 0) {
#if UNITY_EDITOR
                    UnityEngine.Assertions.Assert.IsTrue(tabSelectorOutlines.Length == tabs.Length);
#endif
                    tabSelectorOutlines[i].SetActive(i == currentTab);
                }
            }
        }

        public void SetOrToggleTab(int t) {
            if (t == currentTab)
                tabs[currentTab].SetActive(!tabs[currentTab].activeSelf);
            else {
                tabs[currentTab].SetActive(false);
                currentTab = t;
                tabs[currentTab].SetActive(true);
            }
        }

        public void SetTab(int t) {
            if (tabSelectorOutlines != null && tabSelectorOutlines.Length > 0) {
                tabSelectorOutlines[currentTab].SetActive(false);
                tabSelectorOutlines[t].SetActive(true);
            }

            tabs[currentTab].SetActive(false);
            currentTab = t;
            tabs[currentTab].SetActive(true);
        }

    }
}
