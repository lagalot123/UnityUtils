using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component {

    protected static T _I;
    public static T I {
        get {
            if (_I == null) {
                _I = FindObjectOfType(typeof(T)) as T;
            }
            return _I;
        }
    }

    virtual public void Awake() {
        _I = this as T;
    }
}
