using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Image))]
	public class UISelection : MonoBehaviour
	{
		public static event Action<GameObject, GameObject> SelectionChanged;

		/// <summary>
		/// Désactive la sélection.
		/// </summary>
		static public bool NoSelect_ { get; set; }

		/// <summary>
		/// Objet actuellement sélectionné.
		/// </summary>
		static public GameObject CurrentSelectionObj_ { get; private set; }

		/// <summary>
		/// Coulour de l'objet sélectionné.
		/// </summary>
		public static readonly Color selected_ = Color.HSVToRGB(200f / 360f, 0.25f, 0.75f);

		/// <summary>
		/// Coulour de l'objet désélectionné.
		/// </summary>
		public static readonly Color unselected1_ = Color.HSVToRGB(0f, 0f, 0.65f);

		/// <summary>
		/// Coulour de l'objet désélectionné.
		/// </summary>
		public static readonly Color unselected2_ = Color.HSVToRGB(0f, 0f, 1f);

		/// <summary>
		/// Selectionne l'objet et désélectionne l'objet déjà sélectionné.
		/// </summary>
		/// <param name="uiObj_"></param>
		static public void Select(GameObject uiObj_)
		{
			if (NoSelect_)
			{
				return;
			}

			if (CurrentSelectionObj_ != null && CurrentSelectionObj_ == uiObj_)
			{
				return;
			}

			var uiObjUIS_ = uiObj_.GetComponent<UISelection>();

			if (uiObjUIS_ == null)
			{
				return;
			}

			if (CurrentSelectionObj_ != null && CurrentSelectionObj_ != uiObj_)
			{
				CurrentSelectionObj_.GetComponent<Image>().color = unselected1_;
			}

			var old_ = CurrentSelectionObj_;

			CurrentSelectionObj_ = uiObj_;
			CurrentSelectionObj_.GetComponent<Image>().color = selected_;

			SelectionChanged?.Invoke(old_, CurrentSelectionObj_);
		}
	}
}
