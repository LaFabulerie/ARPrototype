using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIMainMenuBodyPortrait : MonoBehaviour
	{
		// AppManager
		private AppManager appManager_;

		// Navigation
		private Navigation navigation_;

		// Logo
		private GameObject logoObj_;

		// Buttons
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
			var position_ = new Vector2(2f * margin_ + buttonSize_.x, 0f);

			// Logo
			var logoI_ = logoObj_.GetComponent<UnityEngine.UI.Image>();
			var logoARF_ = logoObj_.GetComponent<AspectRatioFitter>();
			var LogoRT_ = logoObj_.GetComponent<RectTransform>();

			logoARF_.aspectRatio = logoI_.sprite.rect.width / logoI_.sprite.rect.height;

			LogoRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize_.x);

			position_.y += margin_ + buttonSize_.y;

			// Buttons
			var buttonAugmentedImageRT_ = buttonAugmentedImageObj_.GetComponent<RectTransform>();
			var buttonAugmentedImageB_ = buttonAugmentedImageObj_.GetComponent<Button>();
			var buttonAugmentedFaceRT_ = buttonAugmentedFaceObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceB_ = buttonAugmentedFaceObj_.GetComponent<Button>();
			var buttonAugmentedFaceCreatorRT_ = buttonAugmentedFaceCreatorObj_.GetComponent<RectTransform>();
			var buttonAugmentedFaceCreatorB_ = buttonAugmentedFaceCreatorObj_.GetComponent<Button>();

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

			position_.y += 4f * margin_ + 3f * buttonSize_.y;

			// Body-Portrait
			GetComponent<RectTransform>().sizeDelta = position_;
		}

		private void OnEnable()
		{
			// AppManager
			appManager_ = AppManager.Current_;

			// Navigation
			navigation_ = Navigation.Current_;

			// Logo
			logoObj_ = transform.Find("Logo").gameObject;

			// Buttons
			buttonAugmentedImageObj_ = transform.Find("Button-AugmentedImage").gameObject;
			buttonAugmentedFaceObj_ = transform.Find("Button-AugmentedFace").gameObject;
			buttonAugmentedFaceCreatorObj_ = transform.Find("Button-AugmentedFaceCreator").gameObject;
		}

	private void Update()
	{
		// Logic
		Think();
	}
}
}