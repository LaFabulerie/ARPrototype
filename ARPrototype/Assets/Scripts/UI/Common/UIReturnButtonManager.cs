using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class UIReturnButtonManager : MonoBehaviour
	{
		private float scaledMargin_;
		private Vector2 scaledButtonSize_;

		[SerializeField]
		private float marginSize_;

		[SerializeField]
		private Vector2 buttonSize_;

		private void DrawUI(float scale_)
		{
			scaledButtonSize_ = buttonSize_ * scale_;
			scaledMargin_ = marginSize_ * scale_;

			var rt_ = GetComponent<RectTransform>();

			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledButtonSize_.x);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledButtonSize_.y);
			rt_.anchoredPosition = new Vector2(scaledMargin_, scaledMargin_ * -1f);
		}

		private void Start()
		{
			var canvasObjUICS_ = GameObject.Find("Canvas").GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();

			GetComponent<Button>().onClick.AddListener(() =>
			{
				Navigation.Current_.LoadMainMenu();
			});
		}
	}
}