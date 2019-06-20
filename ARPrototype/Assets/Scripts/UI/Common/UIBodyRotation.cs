﻿using UnityEngine;

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
					if (appManager_.EnableLandscape_)
					{
						bodyPortrait_.SetActive(true);
						bodyLandscape_.SetActive(false);
					}
					break;
				case Orientation.LandscapeRight:
				case Orientation.LandscapeLeft:
					if (appManager_.EnableLandscape_)
					{
						bodyPortrait_.SetActive(false);
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
			bodyPortrait_ = transform.Find("Body-Portrait").gameObject;
			bodyLandscape_ = transform.Find("Body-Landscape")?.gameObject;

			// Setup
			appManager_.EnableLandscape_ = bodyLandscape_ != null;

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