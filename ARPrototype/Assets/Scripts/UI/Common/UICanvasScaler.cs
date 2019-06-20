using System;
using UnityEngine;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UICanvasScaler : MonoBehaviour
	{
		private AppManager appManager_;

		[SerializeField]
		private int refWidth_ = 640;

		[SerializeField]
		private int refHeight_ = 960;

		public event Action<float> ChangedScale;

		private bool RefLandscape_ => refWidth_ > refHeight_;

		public float Scale_ { get; private set; }

		private void ComputeScale(int refWidth_, int refHeight_)
		{
			var wr_ = (float)Screen.width / refWidth_;
			var hr_ = (float)Screen.height / refHeight_;
			Scale_ = Mathf.Lerp(wr_, hr_, 0.5f);

			ChangedScale?.Invoke(Scale_);
		}

		private void Rotate(Orientation orientation_)
		{
			switch (orientation_)
			{
				case Orientation.Portrait:
				case Orientation.ReversePortrait:
					if (RefLandscape_)
					{
						ComputeScale(refHeight_, refWidth_);
					}
					else
					{
						ComputeScale(refWidth_, refHeight_);
					}
					break;
				case Orientation.LandscapeRight:
				case Orientation.LandscapeLeft:
					if (RefLandscape_)
					{
						ComputeScale(refWidth_, refHeight_);
					}
					else
					{
						ComputeScale(refHeight_, refWidth_);
					}
					break;
			}
		}

		public bool InvokeChangedScaleEvent()
		{
			if (Scale_ > 0)
			{
				ChangedScale?.Invoke(Scale_);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void OnEnable()
		{
			appManager_ = AppManager.Current_;

			appManager_.OrientationChanged += Rotate;

			ComputeScale(refWidth_, refHeight_);
		}

		private void OnDisable()
		{
			appManager_.OrientationChanged -= Rotate;
		}
	}
}