using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceCreatorLayer : MonoBehaviour
	{
		public static GameObject ReferenceLayerObj_ { get; private set; }

		/// <summary>
		/// Canvas.
		/// </summary>
		public GameObject CanvasObj_ => GameObject.Find("Canvas");

		/// <summary>
		/// Vue en mode paysage.
		/// </summary>
		public GameObject BodyLandscapeObj_ => CanvasObj_.transform.Find("Body").Find("Body-Landscape").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ObjectObj_ => transform.parent.parent.parent.parent.gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject LayerNameObj_ => transform.Find("Text-Name").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonRemoveLayerObj_ => transform.Find("Button-Remove").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonHideLayerObj_ => transform.Find("Button-Hide").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonMoveForwardObj_ => transform.Find("Button-MoveForward").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonMoveBackwardObj_ => transform.Find("Button-MoveBackward").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject LayerObj_ { get; private set; }

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		public Vector2 scaledTouchSurfaceSize_ { get; private set; }

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		public float scaledMarginSize_ { get; private set; }

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		public int scaledBodyFontSize_ { get; private set; }

		/// <summary>
		/// Taille des surfaces à toucher.
		/// </summary>
		[SerializeField]
		private Vector2 touchSurfaceSize_;

		/// <summary>
		/// Taille des marges.
		/// </summary>
		[SerializeField]
		private float marginSize_;

		/// <summary>
		/// Taille de la police de corps.
		/// </summary>
		[SerializeField]
		private int bodyfontSize_;

		/// <summary>
		/// Indique si le calque est une référence.
		/// </summary>
		public bool IsReference_ { get; private set; }

		/// <summary>
		/// Indique si le calquele calque est suprimable ou pas.
		/// </summary>
		public bool IsPermanent_ { get; private set; }

		/// <summary>
		/// Indique si le calque est déplacable.
		/// </summary>
		public bool IsMoveable_ { get; private set; }

		/// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector3 initialPosition_;

		/// <summary>
		/// Quaternion temporaire.
		/// </summary>
		private Quaternion initialRotation_;

		// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector3 initialScale_;

		public void SetupTranslation()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			initialPosition_ = LayerObj_.transform.localPosition;
		}

		public void Translate(Vector2 pos_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			pos_.x *= -1;

			LayerObj_.transform.localPosition = new Vector3(pos_.x + initialPosition_.x, pos_.y + initialPosition_.y, initialPosition_.z);
		}

		public void TranslateOnZ(GameObject layerObj_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var layerCameraAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var LayerWorkerObjAFLW_ = ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>().LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			layerObj_.transform.localPosition = layerCameraAFCC_.Forward_ * ((layerObj_.transform.GetSiblingIndex() + 1) * LayerWorkerObjAFLW_.LayerOffset_ + layerCameraAFCC_.ClippingPlane_.near_);
		}

		public void SetupRotation()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			initialRotation_ = LayerObj_.transform.localRotation;
		}

		public void Rotate(float ang_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			ang_ *= -100f;

			LayerObj_.transform.localRotation = Quaternion.AngleAxis(ang_, Vector3.forward) * initialRotation_;
		}

		public void SetupScaling()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			initialScale_ = LayerObj_.transform.localScale;
		}

		public void Scale(Vector2 scale_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			scale_ *= 0.001f;
			scale_.x++;
			scale_.y++;

			LayerObj_.transform.localScale = new Vector3(scale_.x * initialScale_.x, scale_.y * initialScale_.y, initialScale_.z);
		}

		/// <summary>
		/// Avant le calque vers l'avant d'une unité.
		/// </summary>
		public void MoveForwardLayer()
		{
			var LayerWorkerObjAFCLW_ = ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>().LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			var origin_ = LayerObj_.transform.GetSiblingIndex();
			var target_ = Mathf.Clamp(origin_ - 1, 0, LayerWorkerObjAFCLW_.LayersObj_.transform.childCount - 1);

			LayerObj_.transform.parent.GetChild(target_).SetSiblingIndex(origin_);
			LayerObj_.transform.SetSiblingIndex(target_);
			TranslateOnZ(LayerObj_.transform.parent.GetChild(origin_).gameObject);
			TranslateOnZ(LayerObj_);

			origin_ = transform.GetSiblingIndex();
			target_ = Mathf.Clamp(origin_ - 1, 0, LayerWorkerObjAFCLW_.LayersObj_.transform.childCount - 1);
			transform.parent.GetChild(target_).SetSiblingIndex(origin_);
			transform.SetSiblingIndex(target_);
		}

		/// <summary>
		/// Recule le calque vers l'arrière d'une unité.
		/// </summary>
		public void MoveBackwardLayer()
		{
			var LayerWorkerObjAFCLW_ = ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>().LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			var origin_ = LayerObj_.transform.GetSiblingIndex();
			var target_ = Mathf.Clamp(origin_ + 1, 0, LayerWorkerObjAFCLW_.LayersObj_.transform.childCount - 1);

			LayerObj_.transform.parent.GetChild(target_).SetSiblingIndex(origin_);
			LayerObj_.transform.SetSiblingIndex(target_);
			TranslateOnZ(LayerObj_.transform.parent.GetChild(origin_).gameObject);
			TranslateOnZ(LayerObj_);

			origin_ = transform.GetSiblingIndex();
			target_ = Mathf.Clamp(origin_ + 1, 0, LayerWorkerObjAFCLW_.LayersObj_.transform.childCount - 1);
			transform.parent.GetChild(target_).SetSiblingIndex(origin_);
			transform.SetSiblingIndex(target_);
		}

		/// <summary>
		/// 
		/// </summary>
		private void ShowHide()
		{
			LayerObj_.SetActive(!LayerObj_.activeSelf);

			var buttonHideLayerObjC_ = ButtonHideLayerObj_.GetComponent<Image>();

			if (LayerObj_.activeSelf)
			{
				buttonHideLayerObjC_.color = UISelection.unselected2_;
			}
			else
			{
				buttonHideLayerObjC_.color = UISelection.selected_;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Remove(GameObject uilayerObj_)
		{
			var uilayerObjAFCL_ = uilayerObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

#if UNITY_EDITOR
			DestroyImmediate(uilayerObjAFCL_.LayerObj_);
			DestroyImmediate(uilayerObj_);
#else
			Destroy(uilayerObjAFCL_.LayerObj_);
			Destroy(uilayerObj_);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public void Remove()
		{
			if (IsReference_ || IsPermanent_)
			{
				return;
			}

			Remove(gameObject);
		}

		/// <summary>
		/// Configure la gestion des boutons.
		/// </summary>
		private void SetupButton()
		{
			var bodyLandscapeObjAFCBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var uiObjectObjUIAFCO_ = ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var uiObjectObjSR_ = uiObjectObjUIAFCO_.UILayersObj_.GetComponent<ScrollRect>(); ;
			var uit_ = GetComponent<UITouch>();
			var buttonRemoveLayerObjB_ = ButtonRemoveLayerObj_.GetComponent<Button>();
			var buttonHideLayerObjB_ = ButtonHideLayerObj_.GetComponent<Button>();
			var buttonMoveForwardObjB_ = ButtonMoveForwardObj_.GetComponent<Button>();
			var buttonMoveBackwardObjB_ = ButtonMoveBackwardObj_.GetComponent<Button>();

			uit_.BeginDrag += (e_, obj_) =>
			{
				UISelection.NoSelect_ = true;
				uiObjectObjSR_.OnBeginDrag(e_);
			};

			uit_.Drag += (e_, obj_) =>
			{
				uiObjectObjSR_.OnDrag(e_);
			};

			uit_.EndDrag += (e_, obj_) =>
			{
				UISelection.NoSelect_ = false;
				uiObjectObjSR_.OnEndDrag(e_);
			};

			uit_.Click += (obj_, clickCount, touchCount) =>
			{
				UISelection.Select(obj_);
			};

			buttonRemoveLayerObjB_.onClick.AddListener(() =>
			{
				Remove();
			});

			buttonHideLayerObjB_.onClick.AddListener(() =>
			{
				ShowHide();
			});

			buttonMoveForwardObjB_.onClick.AddListener(() =>
			{
				MoveForwardLayer();
			});

			buttonMoveBackwardObjB_.onClick.AddListener(() =>
			{
				MoveBackwardLayer();
			});
		}

		/// <summary>
		/// Intialise une nouvelle interface de calque.
		/// </summary>
		public void Initialize(GameObject layerObj_, bool isReference_, bool isPermanent_, bool isMoveable_)
		{
			LayerObj_ = layerObj_;

			IsReference_ = isReference_;
			IsPermanent_ = isPermanent_;
			IsMoveable_ = isMoveable_;

			// Objets enfants
			var bodyLandscapeObjUIBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.DrawUI += Draw;
			bodyLandscapeObjUIBL_.ForceDraw();

			var layerNameObjT_ = LayerNameObj_.GetComponentInChildren<Text>();
			layerNameObjT_.text = $"Layer-{layerObj_.transform.GetSiblingIndex():D3}";

			SetupButton();

			if (isReference_)
			{
				if (ReferenceLayerObj_ != null)
				{
					Remove(ReferenceLayerObj_);
				}

				ReferenceLayerObj_ = gameObject;
			}
		}

		/// <summary>
		/// Dessine l'interface utilisateur.
		/// </summary>
		private void Draw(float scale_)
		{
			// Composants
			var bodyLandscapeObjUIBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var rt_ = GetComponent<RectTransform>();
			var nameObjRT_ = LayerNameObj_.GetComponent<RectTransform>();
			var nameObjT_ = LayerNameObj_.GetComponentInChildren<Text>();
			var buttonRemoveObjRT_ = ButtonRemoveLayerObj_.GetComponent<RectTransform>();
			var buttonHideObjRT_ = ButtonHideLayerObj_.GetComponent<RectTransform>();
			var buttonMoveForwardObjRT_ = ButtonMoveForwardObj_.GetComponent<RectTransform>();
			var buttonMoveBackwardObjRT_ = ButtonMoveBackwardObj_.GetComponent<RectTransform>();

			// Constantes
			if (scale_ > 0)
			{
				scaledTouchSurfaceSize_ = touchSurfaceSize_ * scale_;
				scaledMarginSize_ = marginSize_ * scale_;
				scaledBodyFontSize_ = Mathf.RoundToInt(bodyfontSize_ * scale_);
			}

			// Logiques
			var height_ = 0f;

			//// Nom du calque.
			var layerNameWidth_ = bodyLandscapeObjUIBL_.ScaledRightPanelWidth_ - scaledTouchSurfaceSize_.x * 2f - scaledMarginSize_ * 4f - bodyLandscapeObjUIBL_.ScaledMarginSize_ * 2f;

			nameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layerNameWidth_);
			nameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			nameObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, 0f);

			nameObjT_.fontSize = scaledBodyFontSize_;

			//// Bouton de suppresion
			buttonRemoveObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonRemoveObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonRemoveObjRT_.anchoredPosition = new Vector2(0f, 0f);

			//// Bouton de masquage
			buttonHideObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonHideObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonHideObjRT_.anchoredPosition = new Vector2(scaledTouchSurfaceSize_.x * -1f, 0f);

			//// Première ligne
			height_ += scaledTouchSurfaceSize_.y;

			//// Bouton de déplacement en avant
			buttonMoveForwardObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonMoveForwardObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonMoveForwardObjRT_.anchoredPosition = new Vector2(0f, (scaledTouchSurfaceSize_.y + scaledMarginSize_) * -1f);

			//// Bouton de déplacement en arrière
			buttonMoveBackwardObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonMoveBackwardObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonMoveBackwardObjRT_.anchoredPosition = new Vector2(scaledTouchSurfaceSize_.x + scaledMarginSize_, (scaledTouchSurfaceSize_.y + scaledMarginSize_) * -1f);

			//// Deuxième ligne
			height_ += scaledMarginSize_ + scaledTouchSurfaceSize_.y;

			//// Ce panneau
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bodyLandscapeObjUIBL_.ScaledRightPanelWidth_ - bodyLandscapeObjUIBL_.ScaledMarginSize_ * 2f - scaledMarginSize_ * 2f);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height_);
		}

		public void OnDestroy()
		{
			UIAugmentedFaceCreatorBodyLandscape.Instance_.DrawUI -= Draw;
		}
	}
}
