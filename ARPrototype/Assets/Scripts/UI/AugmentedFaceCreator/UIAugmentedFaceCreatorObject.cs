using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceCreatorObject : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public static GameObject ReferenceObjectObj_ { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public GameObject WorkerObj_ => GameObject.Find("Worker");

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
		public GameObject ObjectNameObj_ => transform.Find("Text-ObjectName").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonRemoveObjectObj_ => transform.Find("Button-RemoveObject").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonHideObjectObj_ => transform.Find("Button-HideObject").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonAddLayerObj_ => transform.Find("Button-AddLayer").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject DropdownAnchorObj_ => transform.Find("Dropdown-Anchor").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject UILayersObj_ => transform.Find("ScrollView-Layers").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject UILayersContentObj_ => UILayersObj_.transform.Find("Viewport").Find("Content").gameObject;

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
		private Vector3 initialPosition_;

		/// <summary>
		/// Quaternion temporaire.
		/// </summary>
		private Quaternion initialRotation_;

		// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector3 initialScale_;

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
		/// 
		/// </summary>
		public int AnchorIndex_ { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public int MeshObjIndex_ { get; set; }

		public void SetupTranslation()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			initialPosition_ = meshWorkerObjAFCMW_.PivotObj_.transform.localPosition;
		}

		public void Translate(Vector2 pos_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			meshWorkerObjAFCMW_.PivotObj_.transform.localPosition = new Vector3(pos_.x + initialPosition_.x, pos_.y + initialPosition_.y, initialPosition_.z);
		}

		public void TranslateOnZ(float zRatio_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var meshCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			meshWorkerObjAFCMW_.PivotObj_.transform.localPosition = new Vector3(meshWorkerObjAFCMW_.PivotObj_.transform.localPosition.x, meshWorkerObjAFCMW_.PivotObj_.transform.localPosition.y, zRatio_ * meshCameraPivotObjAFCC_.Depth_ * 0.99f + meshCameraPivotObjAFCC_.ClippingPlane_.near_ * 1.01f);
		}

		public void SetupRotation()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			initialRotation_ = meshWorkerObjAFCMW_.PivotObj_.transform.localRotation;
		}

		public void Rotate(float ang_)
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			ang_ *= 100f;

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			meshWorkerObjAFCMW_.PivotObj_.transform.localRotation = Quaternion.AngleAxis(ang_, Vector3.forward) * initialRotation_;
		}

		public void SetupScaling()
		{
			if (IsReference_ || !IsMoveable_)
			{
				return;
			}

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			initialScale_ = meshWorkerObjAFCMW_.PivotObj_.transform.localScale;
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

			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			meshWorkerObjAFCMW_.PivotObj_.transform.localScale = new Vector3(scale_.x * initialScale_.x, scale_.y * initialScale_.y, initialScale_.z);
		}

		/// <summary>
		/// Ajoute une interface de calque à partir d'une image.
		/// </summary>
		/// <param name="texture_">Texture du calque.</param>
		public void AddLayer(Texture2D texture_, bool isReference_, bool isPermanent_, bool isMoveable_)
		{
			var LayerWorkerObjAFCLW_ = LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			if (!LayerWorkerObjAFCLW_.CanAddLayer_)
			{
				return;
			}

			var layerCameraAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var bodyLandscapeObjAFCBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjAFCBL_.Focus(MeshWorkerObj_, LayerWorkerObj_);

			var layerObj_ = LayerWorkerObjAFCLW_.AddLayer(layerSupportPrefab_, texture_);
			layerObj_.transform.localPosition = layerCameraAFCC_.Forward_ * ((layerObj_.transform.GetSiblingIndex() + 1) * LayerWorkerObjAFCLW_.LayerOffset_ + layerCameraAFCC_.ClippingPlane_.near_);

			if (isReference_)
			{
				layerObj_.layer = 8;
			}

			var uiLayerObj_ = Instantiate(uiLayerPrefab_, Vector3.zero, Quaternion.identity, UILayersContentObj_.transform);
			uiLayerObj_.name = $"UILayer-{uiLayerObj_.transform.GetSiblingIndex() - 1}";

			var uiLayerObjUIAFCL_ = uiLayerObj_.GetComponent<UIAugmentedFaceCreatorLayer>();
			uiLayerObjUIAFCL_.Initialize(layerObj_, isReference_, isPermanent_, isMoveable_);
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
				var workerObjAFCW_ = AugmentedFaceCreatorWorker.Instance_;

				var maxSize_ = Mathf.Max(workerObjAFCW_.TextureWidth_, workerObjAFCW_.TextureHeight_);

				NativeGallery.GetImageFromGallery((path_) =>
				{
					var texture_ = NativeGallery.LoadImageAtPath(path_, maxSize: maxSize_, markTextureNonReadable: false, generateMipmaps: false);

					AddLayer(texture_, false, false, true);

					callback_?.Invoke(texture_);
				}, title: "Select a texture");
			}
#else
			var workerAFCW_ = WorkerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			var texture_ = Texture2D.whiteTexture;
			texture_.Resize(1024, 512);
			texture_.Apply();

			AddLayer(texture_, false, false, true);

			callback_?.Invoke(texture_);
#endif
		}

		private void Remove(GameObject objectObj_)
		{
			var objectObjUIAFCO_ = objectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

#if UNITY_EDITOR
			DestroyImmediate(objectObj_);
			DestroyImmediate(objectObjUIAFCO_.MeshWorkerObj_);
			DestroyImmediate(objectObjUIAFCO_.LayerWorkerObj_);
#else
			Destroy(objectObj_);
			Destroy(objectObjUIAFCO_.MeshWorkerObj_);
			Destroy(objectObjUIAFCO_.LayerWorkerObj_);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <summary>
		/// Supprime l'objet.
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
			var bodyLandscapeUIAFCBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			var bodyLandscapeSR_ = bodyLandscapeUIAFCBL_.ScrollViewObjectsObj_.GetComponent<ScrollRect>();
			var uit_ = GetComponent<UITouch>();
			var i_ = GetComponent<Image>();
			var buttonRemoveObjectObjB_ = ButtonRemoveObjectObj_.GetComponent<Button>();
			var buttonHideObjectObjB_ = ButtonHideObjectObj_.GetComponent<Button>();
			var buttonAddLayerObjB_ = ButtonAddLayerObj_.GetComponent<Button>();

			uit_.BeginDrag += e_ =>
			{
				UISelection.NoSelect_ = true;
				bodyLandscapeSR_.OnBeginDrag(e_);
			};

			uit_.Drag += e_ =>
			{
				bodyLandscapeSR_.OnDrag(e_);
			};

			uit_.EndDrag += e_ =>
			{
				UISelection.NoSelect_ = false;
				bodyLandscapeSR_.OnEndDrag(e_);
			};

			uit_.Click += (obj_, clickCount, touchCount) =>
			{
				UISelection.Select(obj_);
			};

			buttonRemoveObjectObjB_.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
			{
				if (!IsPermanent_)
				{
					Remove();
				}
			}));

			buttonHideObjectObjB_.onClick.AddListener(() =>
			{
				ShowHide();
			});

			buttonAddLayerObjB_.onClick.AddListener(() =>
			{
				AddLayer(texture_ => { });
			});
		}

		/// <summary>
		/// Intialise un nouvel objet d'interface de maillage.
		/// </summary>
		/// <param name="meshWorkerObj_">Le travailleur du maillage.</param>
		/// <param name="layerWorkerObj_">Le travailleur de calques du maillage.</param>
		/// <param name="isReference_">Indique si l'objet est une référence.</param>
		public void Initialize(GameObject meshWorkerObj_, GameObject layerWorkerObj_, bool isReference_, bool isPermanent_, bool isMoveable_)
		{
			MeshWorkerObj_ = meshWorkerObj_;
			LayerWorkerObj_ = layerWorkerObj_;
			IsReference_ = isReference_;
			IsPermanent_ = isPermanent_;
			IsMoveable_ = isMoveable_;
			IsInitialized = true;

			name = $"UIObject-{transform.GetSiblingIndex():D3}";
			ObjectNameObj_.GetComponent<Text>().text = $"Object-{transform.GetSiblingIndex():D3}";

			SetupButton();

			BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>().ForceDraw();

			if (isReference_)
			{
				if (ReferenceObjectObj_ != null)
				{
					Remove(ReferenceObjectObj_);
				}

				ReferenceObjectObj_ = gameObject;

				var WorkerObjAFCW_ = WorkerObj_.GetComponent<AugmentedFaceCreatorWorker>();
				WorkerObjAFCW_.SetupLights(meshWorkerObj_);
			}

			var bodyLandscapeObjUIBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeObjUIBL_.DrawUI += Draw;
			bodyLandscapeObjUIBL_.ForceDraw();
		}

		/// <summary>
		/// 
		/// </summary>
		public void ShowHide()
		{
			var meshWorkerObjAFCMW_ = MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

			meshWorkerObjAFCMW_.PivotObj_.SetActive(!meshWorkerObjAFCMW_.PivotObj_.activeSelf);

			var buttonHideObjectObjC_ = ButtonHideObjectObj_.GetComponent<Image>();

			if (meshWorkerObjAFCMW_.PivotObj_.activeSelf)
			{
				buttonHideObjectObjC_.color = UISelection.unselected2_;
			}
			else
			{
				buttonHideObjectObjC_.color = UISelection.selected_;
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
			var objectNameObjRT_ = ObjectNameObj_.GetComponent<RectTransform>();
			var objectNameObjT_ = ObjectNameObj_.GetComponentInChildren<Text>();
			var removeObjectObjRT_ = ButtonRemoveObjectObj_.GetComponent<RectTransform>();
			var hideObjectObjRT_ = ButtonHideObjectObj_.GetComponent<RectTransform>();
			var addLayerObjRT_ = ButtonAddLayerObj_.GetComponent<RectTransform>();
			var dropdownAnchorObjRT_ = DropdownAnchorObj_.GetComponent<RectTransform>();
			var dropdownAnchorObjD_ = DropdownAnchorObj_.GetComponent<Dropdown>();
			var dropdownAnchorLabelObjT_ = DropdownAnchorObj_.transform.Find("Label").GetComponent<Text>();
			var dropdownAnchorArrowObjRT_ = DropdownAnchorObj_.transform.Find("Arrow").GetComponent<RectTransform>();
			var dropdownAnchorTemplateItemObjRT_ = DropdownAnchorObj_.transform.Find("Template").Find("Viewport").Find("Content").Find("Item").GetComponent<RectTransform>();
			var dropdownAnchorTemplateLabelObjT_ = DropdownAnchorObj_.transform.Find("Template").Find("Viewport").Find("Content").Find("Item").Find("Item Label").GetComponent<Text>();
			var layersObjRT_ = UILayersObj_.GetComponent<RectTransform>();
			var layersContentObjVLG_ = UILayersContentObj_.GetComponent<VerticalLayoutGroup>();

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

			if (!IsReference_ && ReferenceObjectObj_ != null)
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

				var anchorsNames_ = ReferenceObjectObj_?.GetComponent<UIAugmentedFaceCreatorObject>()?.MeshWorkerObj_?.GetComponent<AugmentedFaceCreatorMeshWorker>()?.GetAnchorNames_();

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

			DropdownAnchorObj_.SetActive(!IsReference_);

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

		public void OnDestroy()
		{
			UIAugmentedFaceCreatorBodyLandscape.Instance_.DrawUI -= Draw;
		}
	}
}