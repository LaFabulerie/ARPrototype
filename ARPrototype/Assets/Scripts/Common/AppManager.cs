using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public enum FreedomOfOrientation
	{
		All,
		Portrait,
		Landscape,
	}

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

		[SerializeField]
		private List<Mesh> meshs_;

		public List<Mesh> Meshs_
		{
			get => meshs_;
		}

		private Orientation prevOrientation_;

		[SerializeField]
		private FreedomOfOrientation freedomOfOrientation_;

		public FreedomOfOrientation FreedomOfOrientation
		{
			get => freedomOfOrientation_;
			set
			{
				freedomOfOrientation_ = value;
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
			Screen.orientation = ScreenOrientation.Portrait;

			while (true)
			{
				switch (FreedomOfOrientation)
				{
					case FreedomOfOrientation.Portrait:
#if UNITY_EDITOR
						Orientation_ = Orientation.Portrait;
						Screen.orientation = ScreenOrientation.Portrait;
#else
						switch (Input.deviceOrientation)
						{
							case DeviceOrientation.Unknown:
							case DeviceOrientation.LandscapeLeft:
							case DeviceOrientation.LandscapeRight:
							case DeviceOrientation.FaceUp:
							case DeviceOrientation.FaceDown:
							case DeviceOrientation.Portrait:
								Orientation_ = Orientation.ReversePortrait;
								Screen.orientation = ScreenOrientation.Portrait;
								break;
							case DeviceOrientation.PortraitUpsideDown:
								Orientation_ = Orientation.ReversePortrait;
								Screen.orientation = ScreenOrientation.PortraitUpsideDown;
								break;
						}
#endif
						break;
					case FreedomOfOrientation.Landscape:
#if UNITY_EDITOR
						Orientation_ = Orientation.LandscapeRight;
						Screen.orientation = ScreenOrientation.LandscapeRight;
#else
						switch (Input.deviceOrientation)
						{
							case DeviceOrientation.Unknown:
							case DeviceOrientation.Portrait:
							case DeviceOrientation.PortraitUpsideDown:
							case DeviceOrientation.FaceUp:
							case DeviceOrientation.FaceDown:
							case DeviceOrientation.LandscapeRight:
								Orientation_ = Orientation.LandscapeRight;
								Screen.orientation = ScreenOrientation.LandscapeRight;
								break;
							case DeviceOrientation.LandscapeLeft:
								Orientation_ = Orientation.LandscapeLeft;
								Screen.orientation = ScreenOrientation.LandscapeLeft;
								break;
						}
#endif
						break;
					case FreedomOfOrientation.All:
#if UNITY_EDITOR
						if (Screen.width > Screen.height)
						{
							Orientation_ = Orientation.LandscapeRight;
							Screen.orientation = ScreenOrientation.LandscapeRight;
						}
						else
						{
							Orientation_ = Orientation.Portrait;
							Screen.orientation = ScreenOrientation.Portrait;
						}
#else
						switch (Input.deviceOrientation)
						{
							case DeviceOrientation.Unknown:
							case DeviceOrientation.Portrait:
								Orientation_ = Orientation.ReversePortrait;
								Screen.orientation = ScreenOrientation.Portrait;
								break;
							case DeviceOrientation.PortraitUpsideDown:
								Orientation_ = Orientation.ReversePortrait;
								Screen.orientation = ScreenOrientation.PortraitUpsideDown;
								break;
							case DeviceOrientation.LandscapeLeft:
								Orientation_ = Orientation.LandscapeLeft;
								Screen.orientation = ScreenOrientation.LandscapeLeft;
								break;
							case DeviceOrientation.LandscapeRight:
								Orientation_ = Orientation.LandscapeRight;
								Screen.orientation = ScreenOrientation.LandscapeRight;
								break;
							case DeviceOrientation.FaceUp:
								Orientation_ = Orientation.Horizontal;
								break;
							case DeviceOrientation.FaceDown:
								Orientation_ = Orientation.Horizontal;
								break;
						}
#endif
						break;
				}

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