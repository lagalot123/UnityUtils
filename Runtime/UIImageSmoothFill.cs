using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UIImageSmoothFill : MonoBehaviour {

        public float speed = 5.0f;
        private float target;
        public Image img;

        void Update() {
            if (target != img.fillAmount)
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, target, Time.deltaTime * speed);
        }

        public void SetTarget(float t) {
            target = Mathf.Clamp(t, 0, 1);
        }

        public float GetTarget() {
            return target;
        }
    }
}
