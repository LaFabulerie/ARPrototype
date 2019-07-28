using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class UINavigationLoaderManager : MonoBehaviour
	{
		private GameObject CanvasObj_ { get => GameObject.Find("Canvas"); }

		private GameObject BackgroundObj_ { get => transform.Find("Background")?.gameObject; }

		private GameObject HourglassObj_ { get => BackgroundObj_.transform.Find("Hourglass")?.gameObject; }


		private Navigation navigation_;

		private Coroutine rotateCoroutine_;

		[SerializeField]
		private Vector2 iconSize_;

		[SerializeField]
		private float marginSize_;

		private IEnumerator Rotate(AsyncOperation op_)
		{
			var hourglassRT_ = HourglassObj_.GetComponent<RectTransform>();

			var angle_ = 360f / 25f;
			var delay_ = 1f / 25f;

			BackgroundObj_.gameObject.SetActive(true);

			while (true)
			{
				hourglassRT_.Rotate(Vector3.back, angle_);
				yield return new WaitForSeconds(delay_);
			}
		}

		private void Run(AsyncOperation op_)
		{
			if (rotateCoroutine_ == null)
			{
				rotateCoroutine_ = StartCoroutine(Rotate(op_));
			}
		}

		private void DrawUI(float scale_)
		{
			var scaledIconSize_ = iconSize_ * scale_;
			var scaledMargin_ = marginSize_ * scale_;

			var rt_ = GetComponent<RectTransform>();
			rt_.anchoredPosition = new Vector2(scaledMargin_, scaledMargin_ * -1f);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledIconSize_.x);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledIconSize_.y);
		}

		private void Start()
		{
			navigation_ = Navigation.Current_;

			navigation_.Change += Run;

			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			if (canvasObjUICS_ != null)
			{
				canvasObjUICS_.ChangedScale += DrawUI;
				canvasObjUICS_.InvokeChangedScaleEvent();
			}
		}

		private void OnDestroy()
		{
			StopAllCoroutines();

			rotateCoroutine_ = null;

			navigation_.Change -= Run;
		}
	}
}