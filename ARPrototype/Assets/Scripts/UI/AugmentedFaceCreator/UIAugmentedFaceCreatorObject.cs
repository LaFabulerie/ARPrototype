using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceCreatorObject : MonoBehaviour
	{
		public static GameObject ReferenceObj_ { get; private set; }

		// Objets parents
		private GameObject workerObj_;
		private GameObject bodyLandscapeObj_;

		// Objets enfants
		private GameObject objectNameObj_;
		private GameObject buttonRemoveObjectObj_;
		private GameObject buttonHideObjectObj_;
		private GameObject buttonAddLayerObj_;
		private GameObject dropdownAnchorObj_;
		private GameObject uiLayersObj_;
		private GameObject uiLayersContentObj_;

		public Dictionary<GameObject, GameObject> LayersObjs_ { get; private set; }

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

		/// <summary>
		/// Préfabriqué du l'interface de calque.
		/// </summary>
		[SerializeField]
		private GameObject uiLayerPrefab_;

		/// <summary>
		/// Préfabriqué du support de calque.
		/// </summary>
		[SerializeField]
		private GameObject layerSupportPrefab_;

		/// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector2 tempVector2_;

		/// <summary>
		/// Quaternion temporaire.
		/// </summary>
		private Quaternion temQuaternion_;

		/// <summary>
		/// L'index de l'objet au seins du système.
		/// </summary>
		public int Index_ { get; private set; }

		/// <summary>
		/// Le travailleur du maillage.
		/// </summary>
		public GameObject MeshWorkerObj_ { get; private set; }

		/// <summary>
		/// Le travailleur de calques du maillage.
		/// </summary>
		public GameObject LayerWorkerObj_ { get; private set; }

		/// <summary>
		/// Indique si l'objet est une référence.
		/// </summary>
		public bool IsReference_ { get; private set; }

		/// <summary>
		/// Indique si le maillage est suprimable ou pas.
		/// </summary>
		public bool IsPermanent_ { get; private set; }

		/// <summary>
		/// Indique si le maillage est déplacable.
		/// </summary>
		public bool IsMoveable_ { get; private set; }

		/// <summary>
		/// Indique si l'objet est initialisé.
		/// </summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
		/// Liste contenant les calques.
		/// </summary>
		public GameObject UILayersObj_ { get => uiLayersObj_; }

		public int AnchorIndex_ { get; private set; }

		public string AssetPath_ { get; private set; }

		public void SetupTranslationObject()
		{
			tempVector2_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetPositionXY();
		}

		public void TranslateObject(Vector2 newPos_)
		{
			newPos_ += tempVector2_;

			MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().SetPositionXY(newPos_.x, newPos_.y);
		}

		public float SetupTranslateOnZ()
		{
			return MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().PosRatio_;
		}

		public void TranslateOnZ(float zRatio_)
		{
			MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().PosRatio_ = zRatio_;
		}

		public void SetupRotationObject()
		{
			temQuaternion_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetRotation();
		}

		public void RotateObject(float newAng_)
		{
			MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().SetRotation(temQuaternion_ * Quaternion.AngleAxis(newAng_, Vector3.forward));
		}

		public void SetupScalingObject()
		{
			tempVector2_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetScale();
		}

		public void ScaleObject(Vector2 newScale_)
		{
			MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().SetScale(newScale_ * tempVector2_);
		}

		/// <summary>
		/// Ajoute une interface de calque à partir d'une image.
		/// </summary>
		/// <param name="texture_">Texture du calque.</param>
		public void AddLayer(Texture2D texture_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			var (index_, layerObj_) = workerObjAFCW_.AddLayer(MeshWorkerObj_, LayerWorkerObj_, layerSupportPrefab_, texture_);

			var uiLayerObj_ = Instantiate(uiLayerPrefab_, Vector3.zero, Quaternion.identity, uiLayersContentObj_.transform);
			uiLayerObj_.name = $"UILayer-{index_}";

			var uiLayerObjUIAFCL_ = uiLayerObj_.GetComponent<UIAugmentedFaceCreatorLayer>();
			uiLayerObjUIAFCL_.Initialize(uiLayerObj_.name, gameObject);

			LayersObjs_.Add(uiLayerObj_, layerObj_);
		}

		/// <summary>
		/// Ajoute une interface de calque en demandant de choisir une nouvelle texture.
		/// </summary>
		public void AddLayer(Action<Texture2D> callback_)
		{
#if !UNITY_EDITOR
			if (NativeGallery.CheckPermission() != NativeGallery.Permission.Granted)
			{
				if (NativeGallery.RequestPermission() != NativeGallery.Permission.Granted &&
					NativeGallery.CanOpenSettings())
				{
					NativeGallery.OpenSettings();
				}
				else
				{
					Application.Quit();
				}
			}

			if (!NativeGallery.IsMediaPickerBusy())
			{
				var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

				var maxSize_ = Mathf.Max(workerObjAFCW_.TextureWidth_, workerObjAFCW_.TextureHeight_);

				NativeGallery.GetImageFromGallery((path_) =>
				{
					var texture_ = NativeGallery.LoadImageAtPath(path_, maxSize: maxSize_, markTextureNonReadable: false, generateMipmaps: false);

					AddLayer(texture_);

					callback_?.Invoke(texture_);
				}, title: "Select a texture");
			}
#else
			var workerAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			var texture_ = Texture2D.whiteTexture;
			texture_.Resize(1024, 512);
			texture_.Apply();

			AddLayer(texture_);

			callback_?.Invoke(texture_);
#endif
		}

		/// <summary>
		/// Supprime l'interface de calque.
		/// </summary>
		/// /// <param name="uiLayerObj_">Interface du calque.</param>
		public void RemoveLayer(GameObject uiLayerObj_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			workerObjAFCW_.RemoveLayer(MeshWorkerObj_, LayerWorkerObj_, LayersObjs_[uiLayerObj_]);

			LayersObjs_.Remove(uiLayerObj_);

#if UNITY_EDITOR
			DestroyImmediate(uiLayerObj_);
#else
			Destroy(uiLayerObj_);
#endif

			workerObjAFCW_.SetLayerCameraPos(MeshWorkerObj_, LayerWorkerObj_);
		}

		/// <summary>
		/// Montre ou masque le calque.
		/// </summary>
		/// <param name="uiLayerObj_">Interface du calque.</param>
		public void ShowHideLayer(GameObject uiLayerObj_)
		{
			if (LayersObjs_[uiLayerObj_].activeSelf)
			{
				uiLayerObj_.GetComponent<UIAugmentedFaceCreatorLayer>().buttonHideLayerObj_.GetComponent<Image>().color = UISelection.selected_;

				LayersObjs_[uiLayerObj_].SetActive(false);
			}
			else
			{
				uiLayerObj_.GetComponent<UIAugmentedFaceCreatorLayer>().buttonHideLayerObj_.GetComponent<Image>().color = UISelection.unselected2_;

				LayersObjs_[uiLayerObj_].SetActive(true);
			}
		}

		/// <summary>
		/// Déplace le calque vers l'avant.
		/// </summary>
		public void MoveForwardLayer(GameObject uiLayerObj_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			workerObjAFCW_.MoveForwardLayer(MeshWorkerObj_, LayerWorkerObj_, LayersObjs_[uiLayerObj_]);

			var index_ = uiLayerObj_.transform.GetSiblingIndex();

			if (index_ > 0f)
			{
				uiLayerObj_.transform.SetSiblingIndex(index_ - 1);
			}
		}

		/// <summary>
		/// Déplace le calque vers l'arrière.
		/// </summary>
		public void MoveBackwardLayer(GameObject uiLayerObj_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			workerObjAFCW_.MoveBackwardLayer(MeshWorkerObj_, LayerWorkerObj_, LayersObjs_[uiLayerObj_]);

			var index_ = uiLayerObj_.transform.GetSiblingIndex();

			if (index_ < uiLayersObj_.transform.childCount - 1)
			{
				uiLayerObj_.transform.SetSiblingIndex(index_ + 1);
			}
		}

		/// <summary>
		/// Configure la gestion des boutons.
		/// </summary>
		private void SetupButton()
		{
			var bodyLandscapeUIAFCBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var bodyLandscapeSR_ = bodyLandscapeUIAFCBL_.ScrollViewObjectsObj_.GetComponent<ScrollRect>();
			var uit_ = GetComponent<UITouch>();
			var i_ = GetComponent<Image>();
			var buttonRemoveObjectObjB_ = buttonRemoveObjectObj_.GetComponent<Button>();
			var buttonHideObjectObjB_ = buttonHideObjectObj_.GetComponent<Button>();
			var buttonAddLayerObjB_ = buttonAddLayerObj_.GetComponent<Button>();

			uit_.BeginDrag += (e_, obj_) =>
			{
				UISelection.NoSelect_ = true;
				bodyLandscapeSR_.OnBeginDrag(e_);
			};

			uit_.Drag += (e_, obj_) =>
			{
				bodyLandscapeSR_.OnDrag(e_);
			};

			uit_.EndDrag += (e_, obj_) =>
			{
				UISelection.NoSelect_ = false;
				bodyLandscapeSR_.OnEndDrag(e_);
			};

			uit_.Click += (obj_, clickCount, touchCount) =>
			{
				UISelection.Select(obj_);
			};

			buttonRemoveObjectObjB_.onClick.AddListener(() =>
			{
				if (!IsPermanent_)
				{
					bodyLandscapeUIAFCBL_.DeleteUIObject(name, MeshWorkerObj_.name, LayerWorkerObj_.name);
				}
			});

			buttonHideObjectObjB_.onClick.AddListener(() =>
			{
				var MeshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

				if (MeshWorkerObjAFCMW_.IsHide)
				{
					MeshWorkerObjAFCMW_.Show();

					buttonHideObjectObj_.GetComponent<Image>().color = UISelection.unselected1_;
				}
				else
				{
					buttonHideObjectObj_.GetComponent<Image>().color = UISelection.selected_;

					MeshWorkerObjAFCMW_.Hide();
				}
			});

			buttonAddLayerObjB_.onClick.AddListener(() =>
			{
				AddLayer(texture_ => { });
			});
		}

		/// <summary>
		/// Intialise un nouvel objet d'interface de maillage.
		/// </summary>
		/// <param name="index_">L'index au seins du système.</param>
		/// <param name="meshWorkerObj_">Le travailleur du maillage.</param>
		/// <param name="layerWorkerObj_">Le travailleur de calques du maillage.</param>
		/// <param name="isReference_">Indique si l'objet est une référence.</param>
		public string Initialize(int index_, GameObject meshWorkerObj_, GameObject layerWorkerObj_, bool isReference_, bool isPermanent_, bool isMoveable_, string assetPath_)
		{
			if (IsInitialized)
			{
				Debug.LogError("L'objet est initialisé deux fois.");
				return string.Empty;
			}

			workerObj_ = GameObject.Find("Worker");
			bodyLandscapeObj_ = GameObject.Find("Body-Landscape");

			// Objets enfants
			objectNameObj_ = transform.Find("Text-ObjectName").gameObject;
			buttonRemoveObjectObj_ = transform.Find("Button-RemoveObject").gameObject;
			buttonHideObjectObj_ = transform.Find("Button-HideObject").gameObject;
			buttonAddLayerObj_ = transform.Find("Button-AddLayer").gameObject;
			dropdownAnchorObj_ = transform.Find("Dropdown-Anchor").gameObject;
			uiLayersObj_ = transform.Find("ScrollView-Layers").gameObject;
			uiLayersContentObj_ = uiLayersObj_.transform.Find("Viewport").Find("Content").gameObject;

			LayersObjs_ = new Dictionary<GameObject, GameObject>();

			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.Draw += DrawUI;

			Index_ = index_;
			MeshWorkerObj_ = meshWorkerObj_;
			LayerWorkerObj_ = layerWorkerObj_;
			IsReference_ = isReference_;
			IsPermanent_ = isPermanent_;
			IsMoveable_ = isMoveable_;
			IsInitialized = true;
			AssetPath_ = assetPath_;

			name = $"UIObject-{index_:D3}";
			objectNameObj_.GetComponent<Text>().text = name;

			SetupButton();

			bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>().ForceDraw();

			if (IsReference_)
			{
				ReferenceObj_ = gameObject;
			}

			return name;
		}

		/// <summary>
		/// Dessine l'interface utilisateur.
		/// </summary>
		private void DrawUI(float scale_)
		{
			// Composants
			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var rt_ = GetComponent<RectTransform>();
			var objectNameObjRT_ = objectNameObj_.GetComponent<RectTransform>();
			var objectNameObjT_ = objectNameObj_.GetComponentInChildren<Text>();
			var removeObjectObjRT_ = buttonRemoveObjectObj_.GetComponent<RectTransform>();
			var hideObjectObjRT_ = buttonHideObjectObj_.GetComponent<RectTransform>();
			var addLayerObjRT_ = buttonAddLayerObj_.GetComponent<RectTransform>();
			var dropdownAnchorObjRT_ = dropdownAnchorObj_.GetComponent<RectTransform>();
			var dropdownAnchorObjD_ = dropdownAnchorObj_.GetComponent<Dropdown>();
			var dropdownAnchorLabelObjT_ = dropdownAnchorObj_.transform.Find("Label").GetComponent<Text>();
			var dropdownAnchorArrowObjRT_ = dropdownAnchorObj_.transform.Find("Arrow").GetComponent<RectTransform>();
			var dropdownAnchorTemplateItemObjRT_ = dropdownAnchorObj_.transform.Find("Template").Find("Viewport").Find("Content").Find("Item").GetComponent<RectTransform>();
			var dropdownAnchorTemplateLabelObjT_ = dropdownAnchorObj_.transform.Find("Template").Find("Viewport").Find("Content").Find("Item").Find("Item Label").GetComponent<Text>();
			var layersObjRT_ = uiLayersObj_.GetComponent<RectTransform>();
			var layersContentObjVLG_ = uiLayersContentObj_.GetComponent<VerticalLayoutGroup>();

			// Constantes
			if (scale_ > 0)
			{
				scaledTouchSurfaceSize_ = touchSurfaceSize_ * scale_;
				scaledMarginSize_ = marginSize_ * scale_;
				scaledBodyFontSize_ = Mathf.RoundToInt(bodyfontSize_ * scale_);
			}

			// Logiques
			var height_ = 0f;

			//// Nom de l'objet
			var objectNameObjWidth_ = bodyLandscapeObjUIBL_.ScaledRightPanelWidth_ - scaledTouchSurfaceSize_.x * 2f - scaledMarginSize_ * 3f - bodyLandscapeObjUIBL_.ScaledMarginSize_ * 2f;

			objectNameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, objectNameObjWidth_);
			objectNameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			objectNameObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, scaledMarginSize_ * -1f);

			objectNameObjT_.fontSize = scaledBodyFontSize_;

			//// Bouton de suppresion
			removeObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			removeObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			removeObjectObjRT_.anchoredPosition = new Vector2(scaledMarginSize_ * -1, scaledMarginSize_ * -1f);

			//// Bouton de masquage
			hideObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			hideObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			hideObjectObjRT_.anchoredPosition = new Vector2((scaledTouchSurfaceSize_.x + scaledMarginSize_) * -1, scaledMarginSize_ * -1f);

			//// Première ligne
			height_ += Mathf.Abs(scaledMarginSize_ + scaledTouchSurfaceSize_.y);

			//// Bouton d'ajout de calques
			var secondLineOffset_ = (scaledTouchSurfaceSize_.y + scaledMarginSize_ * 2f) * -1f;
			addLayerObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			addLayerObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			addLayerObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, secondLineOffset_);

			//// Dropdown des ancres

			if (!IsReference_ && ReferenceObj_ != null)
			{
				dropdownAnchorObjRT_.offsetMin = new Vector2(scaledTouchSurfaceSize_.x + scaledMarginSize_ * 2f, 0f);
				dropdownAnchorObjRT_.offsetMax = new Vector2(scaledMarginSize_ * -1f, secondLineOffset_);
				dropdownAnchorObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);

				dropdownAnchorLabelObjT_.fontSize = scaledBodyFontSize_;

				dropdownAnchorArrowObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
				dropdownAnchorArrowObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
				dropdownAnchorArrowObjRT_.anchoredPosition = Vector2.zero;

				dropdownAnchorTemplateItemObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);

				dropdownAnchorTemplateLabelObjT_.fontSize = scaledBodyFontSize_;

				var anchorsNames_ = ReferenceObj_?.GetComponent<UIAugmentedFaceCreatorObject>()?.MeshWorkerObj_?.GetComponent<AugmentedFaceCreatorMeshWorker>()?.GetAnchorNames_();

				if (anchorsNames_ != null)
				{
					dropdownAnchorObjD_.ClearOptions();
					dropdownAnchorObjD_.AddOptions(anchorsNames_);

				}

				dropdownAnchorObjD_.onValueChanged.AddListener(value_ =>
				{
					AnchorIndex_ = value_;
				});
			}

			dropdownAnchorObj_.SetActive(!IsReference_);

			//// Deuxième ligne
			height_ += Mathf.Abs(scaledMarginSize_ + scaledTouchSurfaceSize_.y);

			//// Liste des calques
			var layersObjOffset_ = (scaledTouchSurfaceSize_.y * 2f + scaledMarginSize_ * 3f) * -1f;
			layersObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bodyLandscapeObjUIBL_.ScaledRightPanelWidth_ - bodyLandscapeObjUIBL_.ScaledMarginSize_ * 2f - scaledMarginSize_ * 2f);
			layersObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y * 2f + scaledMarginSize_ * 1f);
			layersObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, layersObjOffset_);
			layersContentObjVLG_.spacing = scaledMarginSize_;

			//// Troisième ligne
			height_ += Mathf.Abs(scaledMarginSize_ + scaledTouchSurfaceSize_.y * 2f + scaledMarginSize_ * 1f);

			//// Quatrième ligne (marge)
			height_ += Mathf.Abs(scaledMarginSize_);

			//// Ce panneau
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bodyLandscapeObjUIBL_.ScaledRightPanelWidth_ - bodyLandscapeObjUIBL_.ScaledMarginSize_ * 2f);
			rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height_);
		}

		private void OnDestroy()
		{
			var bodyLandscapeObjUIBL_ = bodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.Draw -= DrawUI;
		}
	}
}