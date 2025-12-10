using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UpdateNotification : MonoBehaviour {
        public struct UpdateNotes {

            public string versionString;
            public int versionCode;
            public string changelog;

        }

        public Text txtChangelog;
        public Text txtCurrentVersion;


        public string linkAndroidGooglePlayXML = "";
        public string linkAndroidAmazonXML = "";
        public string linkIosXML = "";

        public Button btnUpdateNotification;
        public Text txtUpdateCount;

        static List<UpdateNotes> updateNotes;

        void Awake() {
            btnUpdateNotification.gameObject.SetActive(false);
        }

        void Start() {

            txtCurrentVersion.text = Application.version;

            if (updateNotes == null)
                StartCoroutine(LoadUpdateInfo());
            else
                ReloadUI(updateNotes);
        }

        IEnumerator LoadUpdateInfo() {
            updateNotes = new();

#if UNITY_WEBGL

            yield return null;

#else

            string link = linkAndroidGooglePlayXML;
#if UNITY_ANDROID
            if (Utility.I.store == AndroidStore.Amazon) {
                link = linkAndroidAmazonXML;
            } else {
                link = linkAndroidGooglePlayXML;
            }
#elif UNITY_IOS
            link = linkIosXML;
#else
        link = linkAndroidGooglePlayXML;
#endif
            if (!string.IsNullOrEmpty(link)) {

                UnityWebRequest w = UnityWebRequest.Get(link);

                yield return w.SendWebRequest();

                if (w.error != null) {
                    Debug.Log("Update Notification, Link: " + link + ",  Error: " + w.error);
                } else {
                    try {
                        XmlDocument xmlDoc = new();
                        xmlDoc.LoadXml(w.downloadHandler.text);

                        XmlNodeList nodes = xmlDoc.GetElementsByTagName("U");

                        foreach (XmlNode node in nodes) {
                            UpdateNotes u = new UpdateNotes {
                                versionString = node.SelectSingleNode("S").InnerText,
                                versionCode = System.Int32.Parse(node.SelectSingleNode("V").InnerText),
                                changelog = node.SelectSingleNode("N").InnerText
                            };

                            updateNotes.Add(u);
                        }
                    } catch { }

                    ReloadUI(updateNotes);
                }
            }
#endif
        }


        void ReloadUI(List<UpdateNotes> n) {
            string cl = "";
            int updateCount = 0;

            for (int i = n.Count - 1; i >= 0; i--) {
                if (n[i].versionCode > Utility.I.versionCode) {
                    cl += "*" + n[i].versionString;
                    cl += "\n" + n[i].changelog + "\n";
                    updateCount++;
                }
            }

            txtChangelog.text = cl;

            btnUpdateNotification.gameObject.SetActive(cl.Length > 1);

            if (txtUpdateCount != null) txtUpdateCount.text = updateCount + "";
        }

        public void LinkUpdate() {
            Utility.OpenStorePage();
        }
    }
}
