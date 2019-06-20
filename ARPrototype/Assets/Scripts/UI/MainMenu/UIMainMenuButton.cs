using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Button))]
	public class UIMainMenuButton : MonoBehaviour
	{
		private GameObject appManagerObj_;
		private Navigation navigation_;
		private Button button_;

		private void OnEnable()
		{
			appManagerObj_ = GameObject.Find("AppManager");
			navigation_ = appManagerObj_.GetComponent<Navigation>();

			button_ = GetComponent<Button>();
			button_.onClick = new Button.ButtonClickedEvent();
			button_.onClick.AddListener(() => { navigation_.LoadMainMenu(); });
		}
	}
}