using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils.Runtime {
    public class UIImageSmoothFill : MonoBehaviour {

        public float speed = 5.0f;
        private float target;
        public Image img;
        private Color colBlink;
        private Color colOriginal;
        private float t;

        private Coroutine blinkRoutine;

        private void Start() {
            colOriginal = img.color;
        }

        void Update() {
            if (target != img.fillAmount) { 
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, target, Time.deltaTime * speed);
            }

            if(t < 1)
            img.color = Color.Lerp(colOriginal, colBlink, t);
        }

        public void SetTarget(float t) {
            target = Mathf.Clamp(t, 0, 1);
        }

        public void SetTarget(float t, Color blinkColor) {
            SetTarget(target);

            colBlink = blinkColor;

            if (blinkRoutine != null)
                StopCoroutine(blinkRoutine);

            blinkRoutine = StartCoroutine(BlinkRoutine(colBlink, 0.5f));        
        }

        public float GetTarget() {
            return target;
        }

        private void OnDisable() {
            if (blinkRoutine != null)
                StopCoroutine(blinkRoutine);

            img.color = colOriginal;
        }

        private IEnumerator BlinkRoutine(Color targetColor, float duration) {
            float half = Mathf.Max(duration * 0.5f, 0.0001f);

            for (float t = 0f; t < half; t += Time.deltaTime) {
                img.color = Color.Lerp(colOriginal, targetColor, t / half);
                yield return null;
            }
            img.color = targetColor;

            for (float t = 0f; t < half; t += Time.deltaTime) {
                img.color = Color.Lerp(targetColor, colOriginal, t / half);
                yield return null;
            }

            img.color = colOriginal;
            blinkRoutine = null;
        }
    }
}
