using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIMainMenuBodyLandscape : MonoBehaviour
	{
		public GameObject CanvasObj_ { get => GameObject.Find("Canvas"); }
		public GameObject LogoObj_ { get => transform.Find("Logo").gameObject; }
		public GameObject ButtonsObj_ { get => transform.Find("Buttons").gameObject; }
		public GameObject ButtonAugmentedImageObj_ { get => ButtonsObj_.transform.Find("Button-AugmentedImage").gameObject; }
		public GameObject ButtonAugmentedFaceObj_ { get => ButtonsObj_.transform.Find("Button-AugmentedFace").gameObject; }
		public GameObject ButtonAugmentedFaceCreatorObj_ { get => ButtonsObj_.transform.Find("Button-AugmentedFaceCreator").gameObject; }

		// Navigation
		private Navigation navigation_;

		// Arguments
		[SerializeField]
		private float margin_;

		[SerializeField]
		private Vector2 buttonSize_;

		[SerializeField]
		private float fontSize_;

		private void DrawUI(float scale_)
		{
			var scaledMargin_ = margin_ * scale_;
			var scaledButtonSize_ = buttonSize_ * scale_;
			var scaledFontSize_ = Mathf.RoundToInt(fontSize_ * scale_);

			// Common
			var position_ = Vector2.zero;

			// Logo
			var logoI_ = LogoObj_.GetComponent<Image>();
			var logoARF_ = LogoObj_.GetComponent<AspectRatioFitter>();
			var LogoRT_ = LogoObj_.GetComponent<RectTransform>();

			logoARF_.aspectRatio = logoI_.sprite.rect.width / logoI_.sprite.rect.height;

			LogoRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledButtonSize_.x);

			position_.y += scaledMargin_ + scaledButtonSize_.y;

			// Buttons
			var buttonsWidth_ = 2f * scaledMargin_ + 3f * scaledButtonSize_.x;

			var buttonsRT_ = ButtonsObj_.GetComponent<RectTransform>();
			var buttonAugmentedImageRT_ = ButtonAugmentedImageObj_.GetComponent<RectTransform>();
			var buttonAugmentedImageB_ = ButtonAugmentedImageObj_.GetComponent<Button>();
			var buttonAugmentedFaceRT_ = ButtonAugmentedFaceObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceB_ = ButtonAugmentedFaceObj_.GetComponent<Button>();
			var buttonAugmentedFaceCreatorRT_ = ButtonAugmentedFaceCreatorObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceCreatorB_ = ButtonAugmentedFaceCreatorObj_.GetComponent<Button>();

			buttonsRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonsWidth_);
			buttonsRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledButtonSize_.y);

			buttonAugmentedImageRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledButtonSize_.x);
			buttonAugmentedImageRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledButtonSize_.y);
			buttonAugmentedImageB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedImageScene();
			});
			ButtonAugmentedImageObj_.GetComponentInChildren<Text>().fontSize = scaledFontSize_;

			buttonAugmentedFaceRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledButtonSize_.x);
			buttonAugmentedFaceRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledButtonSize_.y);
			buttonAugmentedFaceB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedFaceScene();
			});
			ButtonAugmentedFaceObj_.GetComponentInChildren<Text>().fontSize = scaledFontSize_;

			buttonAugmentedFaceCreatorRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledButtonSize_.x);
			buttonAugmentedFaceCreatorRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledButtonSize_.y);
			buttonAugmentedFaceCreatorB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedFaceCreatorScene();
			});
			ButtonAugmentedFaceCreatorObj_.GetComponentInChildren<Text>().fontSize = scaledFontSize_;

			position_.x += 2f * scaledMargin_ + buttonsWidth_;
			position_.y += 2f * scaledMargin_ + scaledButtonSize_.y;

			// Body-Portrait
			GetComponent<RectTransform>().sizeDelta = position_;
		}

		private void OnEnable()
		{
			// Navigation
			navigation_ = Navigation.Current_;

			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();
		}
	}
}