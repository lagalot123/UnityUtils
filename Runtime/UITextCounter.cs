using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITextCounter : MonoBehaviour {

	private Text _txt;
	private Text txt {
		get {
			if (_txt == null) {
				_txt = GetComponent<Text>();
				sizeDefault = transform.localScale;
			}

			return _txt;
		}
	}

	public float countSpeed;
	private float val = -1;
	private int valTarget;

	public AnimationCurve sizeCurve;
	private float sizeCurveTime;

	public Vector3 sizeChange;
	public Vector3 sizeDefault;
	public float sizeRevertSpeed;

	public float countTimeMin;
	public float countTimeMax;

	public string txtPre;

	// Use this for initialization
	void Awake() {
		_txt = GetComponent<Text>();

		sizeDefault = transform.localScale;
	}

	// Update is called once per frame
	void Update() {

		float oldVal = (int)val;

		if (val != valTarget)
			val = Mathf.MoveTowards(val, valTarget, Time.unscaledDeltaTime * countSpeed);

		sizeCurveTime += Time.unscaledDeltaTime * sizeRevertSpeed;

		if (Vector3.Lerp(sizeDefault, sizeChange, sizeCurve.Evaluate(sizeCurveTime)) != transform.localScale)
			transform.localScale = Vector3.Lerp(sizeDefault, sizeChange, sizeCurve.Evaluate(sizeCurveTime));

		if ((int)val != oldVal)
			txt.text = txtPre + (int)val;
	}

	public void SetTarget(int target, float time, float sizeChangeTime = 0) {
		valTarget = target;
		countSpeed = Mathf.Clamp((target - val) / time, Mathf.Abs(target - val) / countTimeMin, Mathf.Abs(target - val) / countTimeMax);

		sizeCurveTime = sizeChangeTime;
	}
	public void SetTarget(int startFrom, int target, float time, float sizeChangeTime = 0) {
        val = startFrom;
        SetTarget(target, time, sizeChangeTime);
	}
}
