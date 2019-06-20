using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceCreatorLayer : MonoBehaviour
	{
		public GameObject UIObjectObj_ { get; set; }

		// Objets enfants
		private GameObject canvasObj_;
		private GameObject bodyLandscapeObj_;
		private GameObject layerNameObj_;
		private GameObject buttonRemoveLayerObj_;
		public GameObject buttonHideLayerObj_;
		private GameObject buttonMoveForwardObj_;
		private GameObject buttonMoveBackwardObj_;

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		private Vector2 scaledTouchSurfaceSize_;

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		private float scaledMarginSize_;

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		private int scaledBodyFontSize_;

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

		private Vector2 tempVector2_;
		private Quaternion tempQuaternion_;

		public void SetupTranslationLayer(GameObject uiLayerObj_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			tempVector2_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().GetPositionXY(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_]);
		}

		public void TranslateLayer(GameObject uiLayerObj_, Vector2 newPos_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			newPos_.x *= -1;
			newPos_ += tempVector2_;

			uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().SetPositionXY(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_], newPos_);
		}

		public void SetupRotationLayer(GameObject uiLayerObj_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			tempQuaternion_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().GetRotation(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_]);
		}

		public void RotateLayer(GameObject uiLayerObj_, float newAng_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			newAng_ *= -1;

			uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().SetRotation(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_], tempQuaternion_ * Quaternion.AngleAxis(newAng_, Vector3.forward));
		}

		public void SetupScalingLayer(GameObject uiLayerObj_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			tempVector2_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().GetComponent<AugmentedFaceCreatorLayerWorker>().GetScale(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_]);
		}

		public void ScaleLayer(GameObject uiLayerObj_, Vector2 newScale_)
		{
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().GetComponent<AugmentedFaceCreatorLayerWorker>().SetScale(uiObjectObjUIAFCO_.LayersObjs_[uiLayerObj_], newScale_ * tempVector2_);
		}

		/// <summary>
		/// Configure la gestion des boutons.
		/// </summary>
		private void SetupButton()
		{
			var bodyLandscapeObjAFCBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var uiObjectObjUIAFCO_ = UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var uiObjectObjSR_ = uiObjectObjUIAFCO_.UILayersObj_.GetComponent<ScrollRect>(); ;
			var uit_ = GetComponent<UITouch>();
			var buttonRemoveLayerObjB_ = buttonRemoveLayerObj_.GetComponent<Button>();
			var buttonHideLayerObjB_ = buttonHideLayerObj_.GetComponent<Button>();
			var buttonMoveForwardObjB_ = buttonMoveForwardObj_.GetComponent<Button>();
			var buttonMoveBackwardObjB_ = buttonMoveBackwardObj_.GetComponent<Button>();

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
				uiObjectObjUIAFCO_.RemoveLayer(gameObject);
			});

			buttonHideLayerObjB_.onClick.AddListener(() =>
			{
				uiObjectObjUIAFCO_.ShowHideLayer(gameObject);
			});

			buttonMoveForwardObjB_.onClick.AddListener(() =>
			{
				uiObjectObjUIAFCO_.MoveForwardLayer(gameObject);
			});

			buttonMoveBackwardObjB_.onClick.AddListener(() =>
			{
				uiObjectObjUIAFCO_.MoveBackwardLayer(gameObject);
			});
		}

		/// <summary>
		/// Intialise une nouvelle interface de calque.
		/// </summary>
		public void Initialize(string uiLayerName_, GameObject uiObjectObj_)
		{
			this.UIObjectObj_ = uiObjectObj_;

			// Objets enfants
			canvasObj_ = GameObject.Find("Canvas");
			bodyLandscapeObj_ = GameObject.Find("Body-Landscape");
			layerNameObj_ = transform.Find("Text-Name").gameObject;
			buttonRemoveLayerObj_ = transform.Find("Button-Remove").gameObject;
			buttonHideLayerObj_ = transform.Find("Button-Hide").gameObject;
			buttonMoveForwardObj_ = transform.Find("Button-MoveForward").gameObject;
			buttonMoveBackwardObj_ = transform.Find("Button-MoveBackward").gameObject;

			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.Draw += DrawUI;
			bodyLandscapeObjUIBL_.ForceDraw();

			var layerNameObjT_ = layerNameObj_.GetComponentInChildren<Text>();
			layerNameObjT_.text = uiLayerName_;

			SetupButton();
		}

		/// <summary>
		/// Dessine l'interface utilisateur.
		/// </summary>
		private void DrawUI(float scale_)
		{
			// Composants
			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var rt_ = GetComponent<RectTransform>();
			var nameObjRT_ = layerNameObj_.GetComponent<RectTransform>();
			var nameObjT_ = layerNameObj_.GetComponentInChildren<Text>();
			var buttonRemoveObjRT_ = buttonRemoveLayerObj_.GetComponent<RectTransform>();
			var buttonHideObjRT_ = buttonHideLayerObj_.GetComponent<RectTransform>();
			var buttonMoveForwardObjRT_ = buttonMoveForwardObj_.GetComponent<RectTransform>();
			var buttonMoveBackwardObjRT_ = buttonMoveBackwardObj_.GetComponent<RectTransform>();

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

		private void OnDestroy()
		{
			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.Draw -= DrawUI;
		}
	}
}
