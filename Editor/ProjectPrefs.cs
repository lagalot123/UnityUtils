using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor {
    public static class ProjectPrefs {

        public static string GetProjectKey(string key) {
            return Application.identifier + key;
        }

        public static void SetString(string key, string value) {
            EditorPrefs.SetString(GetProjectKey(key), value);
        }

        public static string GetString(string key, string defaultValue = "") {
            return EditorPrefs.GetString(GetProjectKey(key), defaultValue);
        }

        public static void SetBool(string key, bool value) {
            EditorPrefs.SetBool(GetProjectKey(key), value);
        }

        public static bool GetBool(string key, bool defaultValue = false) {
            return EditorPrefs.GetBool(GetProjectKey(key), defaultValue);
        }

        public static void SetInt(string key, int value) {
            EditorPrefs.SetInt(GetProjectKey(key), value);
        }

        public static int GetInt(string key, int defaultValue = 0) {
            return EditorPrefs.GetInt(GetProjectKey(key), defaultValue);
        }
        public static void SetFloat(string key, float value) {
            EditorPrefs.SetFloat(GetProjectKey(key), value);
        }

        public static float GetFloat(string key, float defaultValue = 0) {
            return EditorPrefs.GetFloat(GetProjectKey(key), defaultValue);
        }

        public static bool HasKey(string key) {
            return EditorPrefs.HasKey(GetProjectKey(key));
        }
        public static void DeleteKey(string key) {
            EditorPrefs.DeleteKey(GetProjectKey(key));
        }

    }
}
