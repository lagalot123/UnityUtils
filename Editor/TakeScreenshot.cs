using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace UnityUtils.Editor {
    public class TakeScreenshot {

        [MenuItem("Tools/Sceenshots/TakeScreenshot %t")]
        private static void Save() {
            Screenshot(1);
        }
        
        [MenuItem("Tools/Sceenshots/TakeScreenshot 2x")]
        private static void Save2() {
            Screenshot(2);
        }
        [MenuItem("Tools/Sceenshots/TakeScreenshot 3x")]
        private static void Save3() {
            Screenshot(3);
        }
        [MenuItem("Tools/Sceenshots/TakeScreenshot 4x")]
        private static void Save4() {
            Screenshot(4);
        }

		static void Screenshot(int res) {

            if (!Directory.Exists(Folder())) {
                Directory.CreateDirectory(Folder());
            }

            string filename = ScreenShotName(res);
			ScreenCapture.CaptureScreenshot(filename, res);
		}

        public static string Folder() {
            return string.Format("{0}/Screenshots",
                                 Application.dataPath);
        }

		public static string ScreenShotName(int res) {
            return string.Format("{0}/screen_{1}x{2}_{3}.png",
                                 Folder(),
                                 Screen.width * res,
                                 Screen.height * res,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }
    }

}
