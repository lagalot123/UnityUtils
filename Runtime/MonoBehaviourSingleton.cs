using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif


namespace UnityUtils.Runtime {
    public abstract
#if UNITY_6000_5_OR_NEWER
        partial
#endif
        class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component {

#if UNITY_6000_5_OR_NEWER
        [AutoStaticsCleanup]
#endif
        protected static T _I;
        public static T I {
            get {
                if (_I == null) {
                    _I = FindAnyObjectByType(typeof(T)) as T;
                }
                return _I;
            }
        }

        virtual public void Awake() {
            _I = this as T;
        }
    }
}
