using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class UISwitchDeviceCameraButtonManager : MonoBehaviour
	{
		public GameObject CanvasObj_ { get => GameObject.Find("Canvas"); }

		private AppManager appManager_;

		private Button b_;

		[SerializeField]
		private Vector2 iconSize_;

		[SerializeField]
		private float marginSize_;

		private void DrawUI(float scale_)
		{
			var scaledIconSize_ = iconSize_ * scale_;
			var scaledMargin_ = marginSize_ * scale_;

			var rt_ = GetComponent<RectTransform>();
			rt_.anchoredPosition = new Vector2(scaledMargin_ * -1f, scaledMargin_ * -1f);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledIconSize_.x);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledIconSize_.y);
		}

		private void Start()
		{
			appManager_ = AppManager.Current_;
			b_ = GetComponent<Button>();

			b_.onClick.AddListener(() =>
			{
				appManager_.ToggleDeviceCamera();
			});

			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();
		}

		private void OnDestroy()
		{
			b_.onClick.RemoveAllListeners();
		}
	}
}