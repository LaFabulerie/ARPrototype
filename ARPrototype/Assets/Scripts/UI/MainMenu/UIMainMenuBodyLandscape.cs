using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIMainMenuBodyLandscape : MonoBehaviour
	{
		// Navigation
		private Navigation navigation_;

		// Logo
		private GameObject logoObj_;

		// Buttons
		private GameObject buttonsObj_;
		private GameObject buttonAugmentedImageObj_;
		private GameObject buttonAugmentedFaceObj_;
		private GameObject buttonAugmentedFaceCreatorObj_;

		// Arguments
		[SerializeField]
		private float margin_;
		[SerializeField]
		private Vector2 buttonSize_;

		private void Think()
		{
			// Common
			var position_ = Vector2.zero;

			// Logo
			var logoI_ = logoObj_.GetComponent<UnityEngine.UI.Image>();
			var logoARF_ = logoObj_.GetComponent<AspectRatioFitter>();
			var LogoRT_ = logoObj_.GetComponent<RectTransform>();

			logoARF_.aspectRatio = logoI_.sprite.rect.width / logoI_.sprite.rect.height;

			LogoRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize_.x);

			position_.y += margin_ + buttonSize_.y;

			// Buttons
			var buttonsWidth_ = 2f * margin_ + 3f * buttonSize_.x;

			var buttonsRT_ = buttonsObj_.GetComponent<RectTransform>();
			var buttonAugmentedImageRT_ = buttonAugmentedImageObj_.GetComponent<RectTransform>();
			var buttonAugmentedImageB_ = buttonAugmentedImageObj_.GetComponent<Button>();
			var buttonAugmentedFaceRT_ = buttonAugmentedFaceObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceB_ = buttonAugmentedFaceObj_.GetComponent<Button>();
			var buttonAugmentedFaceCreatorRT_ = buttonAugmentedFaceCreatorObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceCreatorB_ = buttonAugmentedFaceCreatorObj_.GetComponent<Button>();

			buttonsRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonsWidth_);
			buttonsRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize_.y);

			buttonAugmentedImageRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize_.x);
			buttonAugmentedImageRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize_.y);
			buttonAugmentedImageB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedImageScene();
			});

			buttonAugmentedFaceRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize_.x);
			buttonAugmentedFaceRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize_.y);
			buttonAugmentedFaceB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedFaceScene();
			});

			buttonAugmentedFaceCreatorRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize_.x);
			buttonAugmentedFaceCreatorRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize_.y);
			buttonAugmentedFaceCreatorB_.onClick.AddListener(() =>
			{
				navigation_.LoadAugmentedFaceCreatorScene();
			});

			position_.x += 2f * margin_ + buttonsWidth_;
			position_.y += 2f * margin_ + buttonSize_.y;

			// Body-Portrait
			GetComponent<RectTransform>().sizeDelta = position_;
		}

		private void OnEnable()
		{
			// Navigation
			navigation_ = Navigation.Current_;

			// Logo
			logoObj_ = transform.Find("Logo").gameObject;

			// Buttons
			buttonsObj_ = transform.Find("Buttons").gameObject;
			buttonAugmentedImageObj_ = buttonsObj_.transform.Find("Button-AugmentedImage").gameObject;
			buttonAugmentedFaceObj_ = buttonsObj_.transform.Find("Button-AugmentedFace").gameObject;
			buttonAugmentedFaceCreatorObj_ = buttonsObj_.transform.Find("Button-AugmentedFaceCreator").gameObject;
		}

		private void Update()
		{
			// Logic
			Think();
		}
	}
}