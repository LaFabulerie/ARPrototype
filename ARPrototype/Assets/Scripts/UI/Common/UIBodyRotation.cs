using UnityEngine;

namespace Assets.Scripts
{
	public class UIBodyRotation : MonoBehaviour
	{
		// AppManager
		private AppManager appManager_;

		// UIs
		private GameObject bodyPortrait_;
		private GameObject bodyLandscape_;

		private void Rotate(Orientation orientation_)
		{
			switch (orientation_)
			{
				case Orientation.Portrait:
				case Orientation.ReversePortrait:
					if (bodyPortrait_ != null)
					{
						bodyPortrait_.SetActive(true);
					}
					if (bodyLandscape_ != null)
					{
						bodyLandscape_.SetActive(false);
					}
					break;
				case Orientation.LandscapeRight:
				case Orientation.LandscapeLeft:
					if (bodyPortrait_ != null)
					{
						bodyPortrait_.SetActive(false);
					}
					if (bodyLandscape_ != null)
					{
						bodyLandscape_.SetActive(true);
					}
					break;
			}
		}

		private void OnEnable()
		{
			// AppManager
			appManager_ = AppManager.Current_;

			// UIs
			bodyPortrait_ = transform.Find("Body-Portrait")?.gameObject;
			bodyLandscape_ = transform.Find("Body-Landscape")?.gameObject;

			// Setup
			if (bodyPortrait_ != null && bodyLandscape_ == null)
			{
				appManager_.FreedomOfOrientation = FreedomOfOrientation.Portrait;
			}
			else if (bodyPortrait_ == null && bodyLandscape_ != null)
			{
				appManager_.FreedomOfOrientation = FreedomOfOrientation.Landscape;
			}
			else
			{
				appManager_.FreedomOfOrientation = FreedomOfOrientation.All;
			}

			// Events
			appManager_.OrientationChanged += Rotate;

			// Logic
			Rotate(AppManager.Orientation_);
		}

		private void OnDisable()
		{
			// Events
			appManager_.OrientationChanged -= Rotate;
		}
	}
}