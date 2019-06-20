using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class UISwitchDeviceCameraButtonManager : MonoBehaviour
	{
		private AppManager appManager_;

		private Button b_;

		private void OnEnable()
		{
			appManager_ = AppManager.Current_;
			b_ = GetComponent<Button>();

			b_.onClick.AddListener(() =>
			{
				appManager_.ToggleDeviceCamera();
			});
		}

		private void OnDisable()
		{
			b_.onClick.RemoveAllListeners();
		}
	}
}