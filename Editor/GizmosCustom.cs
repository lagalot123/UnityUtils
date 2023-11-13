using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor {

    public static class GizmosCustom {

        public static void DrawRectXZ(Vector2 c, Vector2 size) {
            Vector3 v = new Vector3();
            v.x = c.x - size.x * 0.5f;
            v.z = c.y - size.y * 0.5f;

            Gizmos.DrawLine(v, v + new Vector3(size.x, 0, 0));
            v.x += size.x;
            Gizmos.DrawLine(v, v + new Vector3(0, 0, size.y));
            v.z += size.y;
            Gizmos.DrawLine(v, v + new Vector3(-size.x, 0, 0));
            v.x -= size.x;
            Gizmos.DrawLine(v, v + new Vector3(0, 0, -size.y));
        }

        public static void DrawArrow(Vector3 start, Vector3 end, float size, float thickness) {
            Handles.DrawLine(start, end, thickness);

            DrawArrowHead(start, end - start, size, thickness);
        }

        private static void DrawArrowHead(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float thickness = 1, float arrowHeadAngle = 20.0f) {
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

            Vector3 p1 = pos + direction;

            Handles.DrawLine(p1, p1 + right * arrowHeadLength, thickness);
            Handles.DrawLine(p1, p1 + left * arrowHeadLength, thickness);
            Handles.DrawLine(p1, p1 + up * arrowHeadLength, thickness);
            Handles.DrawLine(p1, p1 + down * arrowHeadLength, thickness);

            Handles.DrawLine(pos + direction + right * arrowHeadLength, pos + direction + left * arrowHeadLength, thickness);
            Handles.DrawLine(pos + direction + up * arrowHeadLength, pos + direction + down * arrowHeadLength, thickness);
        }
    }

}