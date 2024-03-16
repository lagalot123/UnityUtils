using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils.Runtime {
    public static class Misc {


        public static int IncrementLoop(int start, int max) {
            return ++start > max ? 0 : start;
        }

        public static int DecrementLoop(int start, int max) {
            return --start % max;
        }

        /// <summary>
        /// Max inclusive
        /// </summary>
        public static int IndexLoop(int current, int change, int max) {
            int v = current + change;
            if (change > 0)
                return v > max ? v % (max + 1) : v;
            else
                return v < 0 ? v + (max + 1) : v;
        }

        public static bool Contains(Vector2 point, Rect rect) {
            return point.x > rect.x && point.x < rect.x + rect.width && point.y > rect.y && point.y < rect.y + rect.height;
        }

        internal static Rect GetScreenRectFromRectTransform(RectTransform rT) {
            Vector3[] corners = new Vector3[4];
            rT.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[1].y - corners[0].y);
        }

    }
}
