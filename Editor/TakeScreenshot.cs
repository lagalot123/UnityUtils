using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace UnityUtils.Editor {
    public class TakeScreenshot {

        [MenuItem("Tools/TakeScreenshot %t")]
        private static void Save() {
            Screenshot();
        }
		static void Screenshot() {

            if (!Directory.Exists(Folder())) {
                Directory.CreateDirectory(Folder());
            }

            string filename = ScreenShotName();
			ScreenCapture.CaptureScreenshot(filename, 1);
		}

        public static string Folder() {
            return string.Format("{0}/Screenshots",
                                 Application.dataPath);
        }

		public static string ScreenShotName() {
            return string.Format("{0}/screen_{1}x{2}_{3}.png",
                                 Folder(),
                                 Screen.width,
                                 Screen.height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }
    }

}
