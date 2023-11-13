using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }
}
