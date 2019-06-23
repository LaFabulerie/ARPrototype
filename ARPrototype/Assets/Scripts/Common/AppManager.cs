using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public enum Orientation
	{
		Portrait,
		ReversePortrait,
		LandscapeRight,
		LandscapeLeft,
		Horizontal
	}

	[ExecuteAlways()]
	public class AppManager : MonoBehaviour
	{
		private static bool isCreated_ = false;

		public static AppManager Current_ { get => GameObject.Find("AppManager").GetComponent<AppManager>(); }

		public static Orientation Orientation_ { get; private set; }

		private static Dictionary<string, GameObject> meshObjs_;

		public static Dictionary<string, GameObject> MeshObjs_
		{
			get
			{
				if (meshObjs_ == null)
				{
					meshObjs_ = new Dictionary<string, GameObject>();
				}

				return meshObjs_;
			}
		}

		private Orientation prevOrientation_;

		[SerializeField]
		private bool enableLandscape_;

		public bool EnableLandscape_
		{
			get => enableLandscape_;
			set
			{
				enableLandscape_ = value;
			}
		}

		public void ToggleDeviceCamera()
		{
			var arCoreSession_ = GameObject.Find("ARCore Device")?.GetComponent<ARCoreSession>();

			if (arCoreSession_ != null)
			{
				if (arCoreSession_.DeviceCameraDirection == DeviceCameraDirection.FrontFacing)
				{
					arCoreSession_.DeviceCameraDirection = DeviceCameraDirection.BackFacing;
				}
				else
				{
					arCoreSession_.DeviceCameraDirection = DeviceCameraDirection.FrontFacing;
				}
			}
		}

		public event Action<Orientation> OrientationChanged;

		private IEnumerator RotateScreenManager()
		{
			Orientation_ = Orientation.Portrait;
			UnityEngine.Screen.orientation = ScreenOrientation.Portrait;

			while (true)
			{
#if UNITY_EDITOR
				if (UnityEngine.Screen.width > UnityEngine.Screen.height  &&
					enableLandscape_)
				{
					Orientation_ = Orientation.LandscapeRight;
					UnityEngine.Screen.orientation = ScreenOrientation.LandscapeRight;
				}
				else
				{
					Orientation_ = Orientation.Portrait;
					UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
				}
#else
				switch (Input.deviceOrientation)
				{
					case DeviceOrientation.Unknown:
					case DeviceOrientation.Portrait:
						Orientation_ = Orientation.Portrait;
						UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
						break;
					case DeviceOrientation.PortraitUpsideDown:
						Orientation_ = Orientation.ReversePortrait;
						UnityEngine.Screen.orientation = ScreenOrientation.PortraitUpsideDown;
						break;
					case DeviceOrientation.LandscapeLeft:
						if (enableLandscape_)
						{
							Orientation_ = Orientation.LandscapeLeft;
							UnityEngine.Screen.orientation = ScreenOrientation.LandscapeLeft;
						}
						else
						{
							if (!(Orientation_ == Orientation.Portrait ||
								Orientation_ == Orientation.ReversePortrait))
							{
								Orientation_ = Orientation.Portrait;
								UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
							}
						}
						break;
					case DeviceOrientation.LandscapeRight:
						if (enableLandscape_)
						{
							Orientation_ = Orientation.LandscapeRight;
							UnityEngine.Screen.orientation = ScreenOrientation.LandscapeRight;
						}
						else
						{
							if (!(Orientation_ == Orientation.Portrait ||
								Orientation_ == Orientation.ReversePortrait))
							{
								Orientation_ = Orientation.Portrait;
								UnityEngine.Screen.orientation = ScreenOrientation.Portrait;
							}
						}
						break;
					case DeviceOrientation.FaceUp:
						Orientation_ = Orientation.Horizontal;
						break;
					case DeviceOrientation.FaceDown:
						Orientation_ = Orientation.Horizontal;
						break;
				}
#endif

				if (Orientation_ != prevOrientation_)
				{
					yield return new WaitForSeconds(0.5f);

					OrientationChanged?.Invoke(Orientation_);

					var canvasRT_ = GameObject.Find("Canvas")?.GetComponent<RectTransform>();

					if (canvasRT_ != null)
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRT_);
					}
				}

				prevOrientation_ = Orientation_;

				yield return null; 
			}
		}

		private void Awake()
		{
			if (Application.isPlaying)
			{
				if (!isCreated_)
				{
					DontDestroyOnLoad(gameObject);
					isCreated_ = true;
					StartCoroutine(RotateScreenManager());
					return;
				}

				Destroy(gameObject, 0);
			}
		}

		private void OnDestroy()
		{
			StopAllCoroutines();

			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}
}