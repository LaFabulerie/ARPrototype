using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
	/// <summary>
	/// Diffrent pôle de la direction cardinale.
	/// </summary>
	public enum UIDragDirection
	{
		None,
		Up,
		Left,
		Down,
		Right,
		Angle
	}

	public class UITouch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Seuil concernant la détection de la direction.
		/// </summary>
		private readonly static float threshold_ = Mathf.Cos(Mathf.PI * 0.125f);


		/// <summary>
		/// Position au début de l'action.
		/// </summary>
		private Vector2 beginPos_;

		/// <summary>
		/// Magnitude au début de l'action.
		/// </summary>
		private float beginMag_;

		/// <summary>
		/// Angle au début de l'action.
		/// </summary>
		private float beginAng_;


		/// <summary>
		/// Coroutine de gestion du multi clique.
		/// </summary>
		private Coroutine handleMultiClickCoroutine_;

		/// <summary>
		/// Nombre de clique au début de l'action.
		/// </summary>
		private int beginTouchCount_;

		/// <summary>
		/// Appeler lors du début du déplacement avec un doigt dans le cas d'un bypass.
		/// </summary>
		public event Action<PointerEventData, GameObject> BeginDrag;

		/// <summary>
		/// Appeler lors du déplacement avec un doigt dans le cas d'un bypass.
		/// </summary>
		public event Action<PointerEventData, GameObject> Drag;

		/// <summary>
		/// Appeler lors de la fin du déplacement avec un doigt dans le cas d'un bypass.
		/// </summary>
		public event Action<PointerEventData, GameObject> EndDrag;

		/// <summary>
		/// Appeler lors du début du déplacement avec un doigt. L'écouteur prend en argument la position.
		/// </summary>
		public event Action<Vector2> OneBeginDrag;

		/// <summary>
		/// Appeler lors du déplacement avec un doigt. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection> OneDrag;

		/// <summary>
		/// Appeler lors de fin du déplacement avec un doigts. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection> OneEndDrag;

		/// <summary>
		/// Appeler lors du début du déplacement avec deux doigts. L'écouteur prend en argument la position, la direction normalisée entre les deux doigts, la magnitude entre les deux doigts, l'angle entre les deux doigts et la direction cardinale entre les deux doigts.
		/// </summary>
		public event Action<Vector2, Vector2, float, float, UIDragDirection> TwoBeginDrag;

		/// <summary>
		/// Appeler lors du déplacement avec deux doigts. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale, la magnitude entre les deux doigts, l'angle entre les deux doigts, la direction cardinale entre les deux doigts.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection, float, float, UIDragDirection> TwoDrag;

		/// <summary>
		/// Appeler lors de la fin du déplacement avec deux doigts. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale, la magnitude entre les deux doigts, l'angle entre les deux doigts, la direction cardinale entre les deux doigts.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection, float, float, UIDragDirection> TwoEndDrag;

		/// <summary>
		/// Appeler lors de la fin du déplacement avec trois doigts. L'écouteur prend en argument la position.
		/// </summary>
		public event Action<Vector2> ThreeBeginDrag;

		/// <summary>
		/// Appeler lors de la fin du déplacement avec trois doigts. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection> ThreeDrag;

		/// <summary>
		/// Appeler lors de la fin du déplacement avec trois doigts. L'écouteur prend en argument la position, la direction normalisée, la magnitude, la direction cardinale.
		/// </summary>
		public event Action<Vector2, Vector2, float, UIDragDirection> ThreeEndDrag;

		/// <summary>
		/// Appeler quand le pointeur entre sur l'objet. 
		/// </summary>
		public event Action<GameObject> Enter;

		/// <summary>
		/// Appeler quand le pointeur sort de l'objet. 
		/// </summary>
		public event Action<GameObject> Exit;

		/// <summary>
		/// Appeler quand le pointeur ... 
		/// </summary>
		public event Action<GameObject> Down;

		/// <summary>
		/// Appeler quand le pointeur ... 
		/// </summary>
		public event Action<GameObject> Up;

		/// <summary>
		/// Appeler à chaque clique. L'écouteur prend en argument le nombre de doigt et le nombre de clique.
		/// </summary>
		public event Action<GameObject, int, int> Click;

		/// <summary>
		/// Retourne la valeur moyenne.
		/// </summary>
		/// <param name="items_">Valeurs.</param>
		/// <returns>Moyenne.</returns>
		private Vector2 Average(params Vector2[] items_)
		{
			var sum_ = Vector2.zero;

			foreach (var item_ in items_)
			{
				sum_ += item_;
			}

			return sum_ / items_.Length;
		}

		/// <summary>
		/// Retourne la direction cardinale.
		/// </summary>
		/// <param name="dir_">Direction.</param>
		/// <returns>Direction cardinale.</returns>
		private UIDragDirection GetDragDir(Vector2 dir_)
		{
			dir_.Normalize();

			var dragDir_ = UIDragDirection.None;

			var upDot_ = Vector3.Dot(dir_, Vector3.up);
			var rightDot_ = Vector3.Dot(dir_, Vector3.right);

			if (upDot_ <= -threshold_)
			{
				dragDir_ = UIDragDirection.Up;
			}
			else if (upDot_ >= threshold_)
			{
				dragDir_ = UIDragDirection.Down;
			}
			else if (rightDot_ >= threshold_)
			{
				dragDir_ = UIDragDirection.Right;
			}
			else if (rightDot_ <= -threshold_)
			{
				dragDir_ = UIDragDirection.Left;
			}
			else
			{
				dragDir_ = UIDragDirection.Angle;
			}

			return dragDir_;
		}

		public void OnBeginDrag(PointerEventData eventData_)
		{
			if (BeginDrag != null)
			{
				BeginDrag(eventData_, gameObject);
				return;
			}

			if (Input.mousePresent)
			{
				var pos_ = Input.mousePosition;
				beginPos_ = pos_;
				OneBeginDrag?.Invoke(pos_);
			}
			else if (Input.touchSupported && Input.multiTouchEnabled)
			{
				switch (Input.touchCount)
				{
					case 1:
						{
							var pos_ = Input.touches[0].position;
							beginPos_ = pos_;
							OneBeginDrag?.Invoke(pos_);
							break;
						}
					case 2:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var average_ = Average(pos1_, pos2_);
							beginPos_ = average_;
							var dir_ = pos2_ - pos1_;
							var dragDir_ = GetDragDir(dir_);
							beginMag_ = dir_.magnitude;
							beginAng_ = Mathf.Atan2(dir_.y, dir_.x);
							TwoBeginDrag?.Invoke(average_, dir_.normalized, beginMag_, beginAng_, dragDir_);
							break;
						}
					case 3:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var pos3_ = Input.touches[2].position;
							var average_ = Average(pos1_, pos2_, pos3_);
							beginPos_ = average_;
							ThreeBeginDrag?.Invoke(average_);
							break;
						}
				}
			}
		}

		public void OnDrag(PointerEventData eventData_)
		{
			if (Drag != null)
			{
				Drag(eventData_, gameObject);
				return;
			}

			if (Input.mousePresent)
			{
				var pos_ = (Vector2)Input.mousePosition;
				var dir_ = beginPos_ - pos_;
				var touchDir_ = GetDragDir(dir_);
				OneDrag?.Invoke(pos_, dir_.normalized, dir_.magnitude, touchDir_);
			}
			else if (Input.touchSupported && Input.multiTouchEnabled)
			{
				switch (Input.touchCount)
				{
					case 1:
						{
							var pos_ = Input.touches[0].position;
							var dir_ = beginPos_ - pos_;
							var touchDir_ = GetDragDir(dir_);
							OneDrag?.Invoke(pos_, dir_.normalized, dir_.magnitude, touchDir_);
							break;
						}
					case 2:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var average_ = Average(pos1_, pos2_);
							var dir_ = beginPos_ - average_;
							var dragDir_ = GetDragDir(dir_);
							var dir2_ = pos2_ - pos1_;
							var mag_ = dir2_.magnitude - beginMag_;
							var ang_ = Mathf.Atan2(dir2_.y, dir2_.x) - beginAng_;
							var dragDir2_ = GetDragDir(dir2_);
							TwoDrag?.Invoke(average_, dir_.normalized, dir_.magnitude, dragDir_, mag_, ang_, dragDir2_);
							break;
						}
					case 3:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var pos3_ = Input.touches[2].position;
							var average_ = Average(pos1_, pos2_, pos3_);
							var dir_ = beginPos_ - average_;
							var dragDir_ = GetDragDir(dir_);
							ThreeDrag?.Invoke(average_, dir_.normalized, dir_.magnitude, dragDir_);
							break;
						}
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData_)
		{
			if (EndDrag != null)
			{
				EndDrag(eventData_, gameObject);
				return;
			}

			if (Input.mousePresent)
			{
				var pos_ = (Vector2)Input.mousePosition;
				var dir_ = beginPos_ - pos_;
				var touchDir_ = GetDragDir(dir_);
				OneEndDrag?.Invoke(pos_, dir_.normalized, dir_.magnitude, touchDir_);
			}
			else if (Input.touchSupported && Input.multiTouchEnabled)
			{
				switch (Input.touchCount)
				{
					case 1:
						{
							var pos_ = Input.touches[0].position;
							var dir_ = beginPos_ - pos_;
							var touchDir_ = GetDragDir(dir_);
							OneEndDrag?.Invoke(pos_, dir_.normalized, dir_.magnitude, touchDir_);
							break;
						}
					case 2:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var average_ = Average(pos1_, pos2_);
							var dir_ = beginPos_ - average_;
							var dragDir_ = GetDragDir(dir_);
							var dir2_ = pos2_ - pos1_;
							var mag_ = dir2_.magnitude - beginMag_;
							var ang_ = Mathf.Atan2(dir2_.y, dir2_.x) - beginAng_;
							var dragDir2_ = GetDragDir(dir2_);
							TwoEndDrag?.Invoke(average_, dir_.normalized, dir_.magnitude, dragDir_, mag_, ang_, dragDir2_);
							break;
						}
					case 3:
						{
							var pos1_ = Input.touches[0].position;
							var pos2_ = Input.touches[1].position;
							var pos3_ = Input.touches[2].position;
							var average_ = Average(pos1_, pos2_, pos3_);
							var dir_ = beginPos_ - average_;
							var dragDir_ = GetDragDir(dir_);
							ThreeEndDrag?.Invoke(average_, dir_.normalized, dir_.magnitude, dragDir_);
							break;
						}
				}
			}
		}

		/// <summary>
		/// Retourne le nombre de doigts.
		/// </summary>
		/// <returns></returns>
		private int GetTouchCount()
		{
			if (Input.mousePresent)
			{
				return 1;
			}
			else if (Input.touchSupported && Input.multiTouchEnabled)
			{
				return Input.touchCount;
			}
			else
			{
				return 0;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Enter?.Invoke(gameObject);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Exit?.Invoke(gameObject);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Down?.Invoke(gameObject);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			Up?.Invoke(gameObject);
		}

		public void OnPointerClick(PointerEventData eventData_)
		{
			var touchCount_ = 1;

			if (Input.touchSupported && Input.multiTouchEnabled)
			{
				touchCount_ = Input.touchCount;
			}

			Click?.Invoke(gameObject, touchCount_, eventData_.clickCount);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			handleMultiClickCoroutine_ = null;
		}
	}
}