using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceCreatorBodyLandscape : MonoBehaviour
	{
		[SerializeField]
		private Text Debug_;

		private GameObject canvasObj_;

		// Objets enfants
		private GameObject backgroundObj_;
		private GameObject leftPanelObj_;
		private GameObject leftPanelScrollViewObj_;
		private GameObject leftPanelScrollViewContentObj_;
		private GameObject buttonResetObj_;
		private GameObject buttonTranslateObj_;
		private GameObject buttonRotateObj_;
		private GameObject buttonScaleObj_;
		private GameObject rightPanelObj_;
		private GameObject rightPanelHandleObj_;
		private GameObject buttonCancelObj_;
		private GameObject inputFieldNameObj_;
		private GameObject buttonOkObj_;
		private GameObject buttonAddObjectObj_;
		private GameObject scrollViewObjectsObj_;
		private GameObject scrollViewObjectsContentObj_;
		private GameObject viewerPanelObj_;
		private GameObject sliderVerticalObj_;

		/// <summary>
		/// Largeur du panneau de droite avant transformation mis à l'échelle.
		/// </summary>
		private float beginRightPanelWidth_;

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		private Vector2 scaledTouchSurfaceSize_;

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		private float scaledRightPanelWidth_;

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		private float scaledMarginSize_;

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		private int scaledBodyFontSize_;

		/// <summary>
		/// Indique si le composant est initialisé.
		/// </summary>
		private bool isInitialized_;

		/// <summary>
		/// Liste des d'objets de maillage d'interface.
		/// </summary>
		private Dictionary<string, GameObject> uiObjectsObjs_;

		/// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector2 tempVector2_;

		/// <summary>
		/// Nombre flotant temporaire.
		/// </summary>
		private float tempFloat_;

		/// <summary>
		/// Direction cardinale temporaire.
		/// </summary>
		private UIDragDirection tempDragDirection_;

		/// <summary>
		/// Taille des surfaces à toucher.
		/// </summary>
		[SerializeField]
		private Vector2 touchSurfaceSize_;

		/// <summary>
		/// Taille des surfaces à toucher.
		/// </summary>
		[SerializeField]
		private float rightPanelWidth_;

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
		/// Objet de travail principal.
		/// </summary>
		[SerializeField]
		private GameObject workerObj_;

		/// <summary>
		/// Préfabriqué d'un objet de maillage d'interface.
		/// </summary>
		[SerializeField]
		private GameObject uiObjectObj_;

		/// <summary>
		/// Préfabriqué du maillage du vissage.
		/// </summary>
		[SerializeField]
		private GameObject faceMeshPrefab_;

		/// <summary>
		/// Préfabriqué du maillage de surface.
		/// </summary>
		[SerializeField]
		private GameObject surfaceMeshPrefab_;

		/// <summary>
		/// Liste des objets de maillage à ajouter.
		/// </summary>
		[SerializeField]
		private List<GameObject> objectsPrefabs_;

		/// <summary>
		/// Largeur de la vue.
		/// </summary>
		private float viewerPanelObjWidth_;

		/// <summary>
		/// Hauteur de la vue.
		/// </summary>
		private float viewerPanelObjHeight_;

		/// <summary>
		/// Position sur l'axe X de la vue.
		/// </summary>
		private float viewerPanelObjXOffset_;

		/// <summary>
		/// Position sur l'axe Y de la vue.
		/// </summary>
		private float viewerPanelObjYOffset_;

		/// <summary>
		/// Objet de référence contenant l'interface du maillage du vissage.
		/// </summary>
		public GameObject UIObject000Obj_ { get; private set; }

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		public Vector2 ScaledTouchSurfaceSize_ { get => scaledTouchSurfaceSize_; }

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		public float ScaledRightPanelWidth_ { get => scaledRightPanelWidth_; }

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		public float ScaledMarginSize_ { get => scaledMarginSize_; }

		/// <summary>
		/// Liste contenant les objets.
		/// </summary>
		public GameObject ScrollViewObjectsObj_ { get => scrollViewObjectsObj_; }

		/// <summary>
		/// Action de l'utilisateur en cours.
		/// </summary>
		public UIUserAction CurrentUserAction_ { get; private set; }

		/// <summary>
		/// Evenement à appeler pour intialiser chaque enfant.
		/// </summary>
		public event Action Initialize;

		/// <summary>
		/// Evenement à appeler pour intialiser chaque enfant.
		/// </summary>
		public event Action<float> Draw;

		/// <summary>
		/// Débute le redimensionnement du panneaux de droite. 
		/// </summary>
		/// <param name="pos_">Position du curseur.</param>
		private void OneBeginDragOnRightPanelHandleObj(Vector2 pos_)
		{
			beginRightPanelWidth_ = Screen.width - pos_.x - rightPanelObj_.GetComponent<RectTransform>().sizeDelta.x;
		}

		/// <summary>
		/// Redimensionnement du panneaux de droite.
		/// </summary>
		/// <param name="pos_">Position du curseur.</param>
		/// <param name="dirNorm_">Direction normalisé du curseur.</param>
		/// <param name="mag_">Magnitude du curseur.</param>
		/// <param name="dir_">Direction cardinale du curseur.</param>
		private void OneDragOnRightPanelHandleObj(Vector2 pos_, Vector2 dirNorm_, float mag_, UIDragDirection dir_)
		{
			var rightPanelHandleObjRT_ = rightPanelHandleObj_.GetComponent<RectTransform>();
			var leftPanelObjRT_ = leftPanelObj_.GetComponent<RectTransform>();

			var min_ = rightPanelWidth_ * canvasObj_.GetComponent<UICanvasScaler>().Scale_;
			var max_ = Screen.width - leftPanelObjRT_.sizeDelta.x - scaledTouchSurfaceSize_.x * 0.5f;

			scaledRightPanelWidth_ = Mathf.Clamp(Screen.width - pos_.x - beginRightPanelWidth_, min_, max_);

			DrawUI(0f);
		}

		private void SetupTranslation(Vector2 pos_)
		{
			if (CurrentUserAction_ != UIUserAction.Translate)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			if (currentSelectedObjectObjUIAFCO_ != null)
			{
				if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
				{
					return;
				}

				tempVector2_ = pos_;

				currentSelectedObjectObjUIAFCO_.SetupTranslationObject();
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				tempVector2_ = pos_;

				currentSelectedObjectObjUIAFCL_.SetupTranslationLayer(UISelection.CurrentSelectionObj_);
			}
		}

		private void Translate(Vector2 pos_, Vector2 dir, float mag, UIDragDirection cardDir_)
		{
			if (CurrentUserAction_ != UIUserAction.Translate)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();
			var meshCameraPivotObjAFCC_ = workerObjAFCW_.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			if (pos_.x >= viewerPanelObjXOffset_ &&
				pos_.x <= viewerPanelObjXOffset_ + viewerPanelObjWidth_ &&
				pos_.y >= viewerPanelObjYOffset_ &&
				pos_.y <= viewerPanelObjYOffset_ + viewerPanelObjHeight_)
			{
				pos_ -= tempVector2_;

				pos_.x /= viewerPanelObjWidth_ + viewerPanelObjXOffset_;
				pos_.y /= viewerPanelObjHeight_ + viewerPanelObjYOffset_;
				pos_ *= meshCameraPivotObjAFCC_.RectBounds_.size;

				if (currentSelectedObjectObjUIAFCO_ != null)
				{
					if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
					{
						return;
					}

					currentSelectedObjectObjUIAFCO_.TranslateObject(pos_);
				}
				else if (currentSelectedObjectObjUIAFCL_ != null)
				{
					currentSelectedObjectObjUIAFCL_.TranslateLayer(UISelection.CurrentSelectionObj_, pos_);
				}
			}
		}

		private void SetupRotation(Vector2 pos_, Vector2 dir_, float mag_, float ang_, UIDragDirection cardDir_)
		{
			if (CurrentUserAction_ != UIUserAction.Rotate)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			if (currentSelectedObjectObjUIAFCO_ != null)
			{
				if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
				{
					return;
				}

				currentSelectedObjectObjUIAFCO_.SetupRotationObject();

				tempFloat_ = ang_;
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				currentSelectedObjectObjUIAFCL_.SetupRotationLayer(UISelection.CurrentSelectionObj_);

				tempFloat_ = ang_;
			}
		}

		private void Rotate(Vector2 pos_, Vector2 dir, float mag_, UIDragDirection cardDir_, float mag2_, float ang_, UIDragDirection cardDir2_)
		{
			if (CurrentUserAction_ != UIUserAction.Rotate)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			if (currentSelectedObjectObjUIAFCO_ != null &&
				!currentSelectedObjectObjUIAFCO_.IsMoveable_)
			{
				return;
			}

			if (currentSelectedObjectObjUIAFCO_ != null)
			{
				if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
				{
					return;
				}

				ang_ *= 100f;
				currentSelectedObjectObjUIAFCO_.RotateObject(ang_);
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				ang_ *= 100f;
				currentSelectedObjectObjUIAFCL_.RotateLayer(UISelection.CurrentSelectionObj_, ang_);
			}
		}

		private void SetupScaling(Vector2 pos_, Vector2 dir_, float mag_, float ang_, UIDragDirection cardDir_)
		{
			if (CurrentUserAction_ != UIUserAction.Scale)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			if (currentSelectedObjectObjUIAFCO_ != null)
			{
				if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
				{
					return;
				}

				currentSelectedObjectObjUIAFCO_.SetupScalingObject();
				tempDragDirection_ = cardDir_;
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				currentSelectedObjectObjUIAFCL_.SetupScalingLayer(UISelection.CurrentSelectionObj_);
				tempDragDirection_ = cardDir_;
			}
		}

		private void Scale(Vector2 pos_, Vector2 dir, float mag_, UIDragDirection cardDir_, float mag2_, float ang_, UIDragDirection cardDir2_)
		{
			if (CurrentUserAction_ != UIUserAction.Scale)
			{
				return;
			}

			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var currentSelectedObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var currentSelectedObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

			if (currentSelectedObjectObjUIAFCO_ != null)
			{
				if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
				{
					return;
				}

				mag2_ *= 0.001f;
				mag2_++;

				if (cardDir2_ == UIDragDirection.Up ||
					cardDir2_ == UIDragDirection.Down)
				{
					currentSelectedObjectObjUIAFCO_.ScaleObject(new Vector2(1f, mag2_));
				}

				if (cardDir2_ == UIDragDirection.Left ||
					cardDir2_ == UIDragDirection.Right)
				{
					currentSelectedObjectObjUIAFCO_.ScaleObject(new Vector2(mag2_, 1f));
				}

				if (cardDir2_ == UIDragDirection.Angle)
				{
					currentSelectedObjectObjUIAFCO_.ScaleObject(new Vector2(mag2_, mag2_));
				}
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				mag2_ *= 0.001f;
				mag2_++;

				if (cardDir2_ == UIDragDirection.Up ||
					cardDir2_ == UIDragDirection.Down)
				{
					currentSelectedObjectObjUIAFCL_.ScaleLayer(UISelection.CurrentSelectionObj_, new Vector2(1f, mag2_));
				}

				if (cardDir2_ == UIDragDirection.Left ||
					cardDir2_ == UIDragDirection.Right)
				{
					currentSelectedObjectObjUIAFCL_.ScaleLayer(UISelection.CurrentSelectionObj_, new Vector2(mag2_, 1f));
				}

				if (cardDir2_ == UIDragDirection.Angle)
				{
					currentSelectedObjectObjUIAFCL_.ScaleLayer(UISelection.CurrentSelectionObj_, new Vector2(mag2_, mag2_));
				}
			}
		}

		/// <summary>
		/// Configure la gestion du toucher (icompris de la souris).
		/// </summary>
		private void SetupTouch()
		{
			// Composants du panneau droit
			var rightPanelHandleObjUIT_ = rightPanelHandleObj_.GetComponent<UITouch>();

			rightPanelHandleObjUIT_.OneBeginDrag += OneBeginDragOnRightPanelHandleObj;
			rightPanelHandleObjUIT_.OneDrag += OneDragOnRightPanelHandleObj;

			// Composants du panneau de vue
			var viewerPanelObjUIT_ = viewerPanelObj_.GetComponent<UITouch>();

			// Avec un doigt : tranlation (objet et texture)
			viewerPanelObjUIT_.OneBeginDrag += SetupTranslation;
			viewerPanelObjUIT_.OneDrag += Translate;

			// Avec deux doigt : rotation et mise à l'échelle (objet et texture)
			viewerPanelObjUIT_.TwoBeginDrag += SetupRotation;
			viewerPanelObjUIT_.TwoDrag += Rotate;

			viewerPanelObjUIT_.TwoBeginDrag += SetupScaling;
			viewerPanelObjUIT_.TwoDrag += Scale;
		}

		/// <summary>
		/// Annule les événements associés à la gestion du toucher.
		/// </summary>
		private void ResetTouchEvents()
		{
			// Composants du panneau droit
			var rightPanelHandleObjUIT_ = rightPanelHandleObj_.GetComponent<UITouch>();

			rightPanelHandleObjUIT_.OneBeginDrag -= OneBeginDragOnRightPanelHandleObj;
			rightPanelHandleObjUIT_.OneDrag -= OneDragOnRightPanelHandleObj;

			// Composants du panneau de vue
			var viewerPanelObjUIT_ = viewerPanelObj_.GetComponent<UITouch>();

			// Avec un doigt : tranlation (objet et texture)
			viewerPanelObjUIT_.OneBeginDrag -= SetupTranslation;
			viewerPanelObjUIT_.OneDrag -= Translate;

			// Avec deux doigt : rotation et mise à l'échelle (objet et texture)
			viewerPanelObjUIT_.TwoBeginDrag -= SetupRotation;
			viewerPanelObjUIT_.TwoDrag -= Rotate;

			viewerPanelObjUIT_.TwoBeginDrag -= SetupScaling;
			viewerPanelObjUIT_.TwoDrag -= Scale;
		}

		/// <summary>
		/// Ajoute un nouvelle objet.
		/// </summary>
		/// <param name="isReference_">Indique si l'objet est une référence pour les autres (généralement le vissage).</param>
		/// <param name="meshPrefab_">L'objet contenant le maillage.</param>
		/// <param name="posRatio_">La position sur l'axe Z de l'objet dans le travailleur.</param>
		/// <param name="resize_">Force l'objet à être redimensionner à la largeur de l'écran.</param>
		/// <param name="hide_">Masque l'objet.</param>
		/// <returns></returns>
		public GameObject AddUIObject(GameObject meshPrefab_, float posRatio_, bool isReference_, bool isPermanent_, bool isMoveable_, bool resize_, bool hide_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			var newUIObjectObj_ = Instantiate(uiObjectObj_, Vector3.zero, Quaternion.identity, scrollViewObjectsContentObj_.transform);
			var newUIObjectObjAFCUIO_ = newUIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			var (index_, meshWorkerObj_, layerWorkerObj_) = workerObjAFCW_.AddSubWorker(meshPrefab_, isReference_ ? 0.5f : posRatio_);

			var name_ = newUIObjectObjAFCUIO_.Initialize(index_, meshWorkerObj_, layerWorkerObj_, isReference_, isPermanent_, isMoveable_, "to detemrine");

			var meshWorkerObjAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

			if (isReference_)
			{
				if (UIObject000Obj_ != null)
				{
					DeleteUIObject(UIObject000Obj_.name, meshWorkerObj_.name, layerWorkerObj_.name);
				}

				UIObject000Obj_ = newUIObjectObj_;
				workerObjAFCW_.SetupLights(meshWorkerObjAFCMW_);
			}

			if (resize_)
			{
				meshWorkerObjAFCMW_.ScaleMeshToCameraBounds();
			}

			if (hide_)
			{
				meshWorkerObjAFCMW_.Hide();
			}

			uiObjectsObjs_.Add(name_, newUIObjectObj_);

			return newUIObjectObj_;
		}

		/// <summary>
		/// Supprime l'objet.
		/// </summary>
		/// <param name="index_">Index dans le système de l'objet.</param>
		public void DeleteUIObject(string uiObjectName_, string meshWorkerName_, string layerWorkerName_)
		{
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			workerObjAFCW_.RemoveSubWorker(meshWorkerName_, layerWorkerName_);

			if (uiObjectsObjs_.ContainsKey(uiObjectName_) && uiObjectsObjs_[uiObjectName_] != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(uiObjectsObjs_[uiObjectName_]);
#else
				Destroy(uiObjectsObjs_[uiObjectName_]);
#endif
			}

			uiObjectsObjs_.Remove(uiObjectName_);
		}

		private void EnableButtonOfLeftPanel(GameObject buttonObj_)
		{
			var buttonObjIs_ = leftPanelScrollViewContentObj_.GetComponentsInChildren<Image>();

			foreach (var buttonObjI_ in buttonObjIs_)
			{
				if (buttonObjI_.gameObject == buttonObj_)
				{
					buttonObjI_.color = Color.HSVToRGB(200f / 360f, 0.25f, 0.75f);

					ToggleVerticalSlider(UISelection.CurrentSelectionObj_);
				}
				else
				{
					buttonObjI_.color = Color.HSVToRGB(0f, 0f, 1f);

					ToggleVerticalSlider(UISelection.CurrentSelectionObj_);
				}
			}
		}

		private void DisableAllButtonsOfLeftPanel()
		{
			var buttonObjIs_ = leftPanelScrollViewContentObj_.GetComponentsInChildren<Image>();

			foreach (var buttonObjI_ in buttonObjIs_)
			{
				buttonObjI_.color = Color.HSVToRGB(0f, 0f, 1f);

				if (buttonObjI_.gameObject == buttonTranslateObj_)
				{
					sliderVerticalObj_.SetActive(false);
				}
			}
		}

		private void Save()
		{
			var rootPath_ = Path.Combine(Application.persistentDataPath, "faces");

			if (!Directory.Exists(rootPath_))
			{
				Directory.CreateDirectory(rootPath_);
			}

			var faceIndex_ = 0;
			var faceName_ = $"face-{faceIndex_:D4}";
			var faceFolderPath_ = Path.Combine(rootPath_, faceName_);

			while (Directory.Exists(faceFolderPath_) && ++faceIndex_ <= 9999)
			{
				faceName_ = $"face-{faceIndex_:D4}";
				faceFolderPath_ = Path.Combine(rootPath_, faceName_);
			}

			Directory.CreateDirectory(faceFolderPath_);

			var faceInfo_ = new FaceInfo();

			faceInfo_.Name_ = inputFieldNameObj_.GetComponentInChildren<Text>().text;
			faceInfo_.Name_ = string.IsNullOrWhiteSpace(faceInfo_.Name_) ? "Unknow face" : faceInfo_.Name_;

			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();
			var workerObjAFCC_ = workerObjAFCW_.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var previewTexture_ = new Texture2D(workerObjAFCC_.RenderTexture_.width, workerObjAFCC_.RenderTexture_.height, TextureFormat.ARGB32, false);

			var currentRenderTexture_ = RenderTexture.active;

			RenderTexture.active = workerObjAFCC_.RenderTexture_;

			previewTexture_.ReadPixels(new Rect(0, 0, workerObjAFCC_.RenderTexture_.width, workerObjAFCC_.RenderTexture_.height), 0, 0);
			previewTexture_.Apply();

			RenderTexture.active = currentRenderTexture_;

			var previewBytes_ = previewTexture_.EncodeToPNG();

			var previewName_ = $"preview-{faceIndex_:D4}.png";
			var previewPath_ = Path.Combine(faceFolderPath_, previewName_);

			File.WriteAllBytes(previewPath_, previewBytes_);

			faceInfo_.PreviewPath_ = previewPath_;

			faceInfo_.FaceParts_ = new List<FacePartInfo>();

			var referenceObj_ = UIAugmentedFaceCreatorObject.ReferenceObj_;

			var referenceObjUIAFCO_ = referenceObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var referenceMeshWorkerObj_ = referenceObjUIAFCO_.MeshWorkerObj_;
			var referenceLayerWorkerObj_ = referenceObjUIAFCO_.LayerWorkerObj_;
			var referenceAnchorObjs_ = referenceMeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetAnchorObjs();

			int facePartIndex_ = 0;
			foreach (var uiObjectObj_ in uiObjectsObjs_.Values)
			{
				var facePart_ = new FacePartInfo();

				var uiObjectObjUIAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var objectObjUIAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

				facePart_.IsReference_ = uiObjectObjUIAFCO_.IsReference_;

				facePart_.Anchor_ = (ReferenceAnchor)uiObjectObjUIAFCO_.AnchorIndex_;

				facePart_.Is2D_ = objectObjUIAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>() != null;

				facePart_.PositionOffset_ = objectObjUIAFCMW_.GetPositionOffset(referenceAnchorObjs_[uiObjectObjUIAFCO_.AnchorIndex_]);

				facePart_.Rotation_ = objectObjUIAFCMW_.GetRotation();

				facePart_.Scale_ = objectObjUIAFCMW_.GetScale();

				facePart_.Dimension_ = objectObjUIAFCMW_.GetDim();

				facePart_.MeshAssetPath_ = uiObjectObjUIAFCO_.AssetPath_;

				var tex_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().Texture_;

				if (tex_ != null)
				{
					var texBytes_ = tex_.EncodeToPNG();

					var texName_ = $"FacePartTexture-{faceIndex_:D4}-{facePartIndex_:D4}.png";
					var texPath_ = Path.Combine(faceFolderPath_, texName_);

					File.WriteAllBytes(texPath_, texBytes_);

					facePart_.TextureWidth_ = tex_.width;

					facePart_.TextureHeight = tex_.height;

					facePart_.TexturePath_ = texPath_;
				}

				faceInfo_.FaceParts_.Add(facePart_);

				facePartIndex_++;
			}

			var memoryStream_ = new MemoryStream();

			DataContractJsonSerializer seserializer_ = new DataContractJsonSerializer(typeof(FaceInfo));
			seserializer_.WriteObject(memoryStream_, faceInfo_);
			var faceJSONBytes_ = memoryStream_.ToArray();
			memoryStream_.Close();
			var faceJSON_ = Encoding.UTF8.GetString(faceJSONBytes_, 0, faceJSONBytes_.Length);

			var facePath_ = Path.Combine(faceFolderPath_, $"{faceName_}.json");

			File.WriteAllText(facePath_, faceJSON_);
		}

		private void ResetObjectAndLayer()
		{
			if (UISelection.CurrentSelectionObj_ == null)
			{
				return;
			}

			var uiObjectObjUIAFCL_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorLayer>();
			var uiObjectObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();

			if (uiObjectObjUIAFCL_ != null)
			{
				uiObjectObjUIAFCO_ = uiObjectObjUIAFCL_.UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var meshWorkerAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var meshWorkerAFCSM_ = meshWorkerAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();


				var layerObj_ = uiObjectObjUIAFCO_.LayersObjs_[UISelection.CurrentSelectionObj_];
				var layerObjAFCSM_ = layerObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();
				var workerAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();
				var layerCameraPivotObjAFCC_ = workerAFCW_.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
				var layerWorkerObjAFCWL_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

				var texture_ = layerObj_.GetComponent<MeshRenderer>().material.mainTexture;

				if (texture_.width > texture_.height)
				{
					var r_ = (float)texture_.width / texture_.height;
					layerCameraPivotObjAFCC_.SetRenderTexture(Mathf.FloorToInt(workerAFCW_.TextureWidth_ * r_), workerAFCW_.TextureHeight_);
					if (meshWorkerAFCSM_ != null)
					{
						meshWorkerAFCSM_.Initialize(workerAFCW_.MeshCameraPivotObj_, 1f, 1f / r_);
					}
				}
				else
				{
					var r_ = (float)texture_.height / texture_.width;
					layerCameraPivotObjAFCC_.SetRenderTexture(workerAFCW_.TextureWidth_, Mathf.FloorToInt(workerAFCW_.TextureHeight_ * r_));
					if (meshWorkerAFCSM_ != null)
					{
						meshWorkerAFCSM_.Initialize(workerAFCW_.MeshCameraPivotObj_, 1f / r_, 1f);
					}
				}

				layerWorkerObjAFCWL_.SetPositionXY(layerObj_, Vector2.zero);
				layerWorkerObjAFCWL_.SetRotation(layerObj_, Quaternion.identity);
				layerWorkerObjAFCWL_.SetScale(layerObj_, Vector2.one);

				if (!uiObjectObjUIAFCO_.IsReference_)
				{
					meshWorkerAFCMW_.SetPositionXY(0f, 0f);
					meshWorkerAFCMW_.SetRotation(Quaternion.identity);
					meshWorkerAFCMW_.SetScale(Vector2.one);
				}

				layerObjAFCSM_.Initialize(workerAFCW_.LayerCameraPivotObj_);
				workerAFCW_.SetLayerCameraPos(uiObjectObjUIAFCO_.MeshWorkerObj_, uiObjectObjUIAFCO_.LayerWorkerObj_);
			}
			else if (uiObjectObjUIAFCO_ != null)
			{
				if (uiObjectObjUIAFCO_.IsReference_)
				{
					return;
				}

				var meshWorkerAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var referenceObjUIAFCO_ = UIAugmentedFaceCreatorObject.ReferenceObj_.GetComponent<UIAugmentedFaceCreatorObject>();

				var offset_ = meshWorkerAFCMW_.GetPositionOffset(referenceObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetAnchorObjs()[uiObjectObjUIAFCO_.AnchorIndex_]);

				meshWorkerAFCMW_.PivotObj_.transform.Translate(offset_);

				meshWorkerAFCMW_.SetPositionXY(0f, 0f);
				meshWorkerAFCMW_.SetRotation(Quaternion.identity);
				meshWorkerAFCMW_.SetScale(Vector2.one);
			}
		}

		private void ToggleVerticalSlider(GameObject uiObjectObj_)
		{

			if (CurrentUserAction_ == UIUserAction.Translate)
			{
				var uiObjectObjAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

				if (uiObjectObjAFCO_ != null)
				{
					sliderVerticalObj_.SetActive(true);

					var sliderVerticalObjS_ = sliderVerticalObj_.GetComponent<Slider>();

					if (uiObjectObjAFCO_.IsReference_)
					{
						var value_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>().GetZRatioOfMeshCamera();

						sliderVerticalObjS_.normalizedValue = value_;
					}
					else
					{
						var value_ = uiObjectObjAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().PosRatio_;

						sliderVerticalObjS_.normalizedValue = value_;
					}
				}
				else
				{
					sliderVerticalObj_.SetActive(false);
				}
			}
			else
			{
				sliderVerticalObj_.SetActive(false);
			}
		}

		/// <summary>
		/// Configure la gestion des boutons.
		/// </summary>
		private void SetupButton()
		{
			// Composants annexes
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();
			var workerObjAFCC_ = workerObjAFCW_.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			// Butons du panneau gauche
			var buttonResetObjB_ = buttonResetObj_.GetComponent<Button>();
			var buttonTranslateObjB_ = buttonTranslateObj_.GetComponent<Button>();
			var sliderVerticalObjS_ = sliderVerticalObj_.GetComponent<Slider>();
			var buttonRotateObjB_ = buttonRotateObj_.GetComponent<Button>();
			var buttonScaleObjB_ = buttonScaleObj_.GetComponent<Button>();

			UISelection.SelectionChanged += (oldObj_, newObj_) =>
			{
				var newObjAFCL_ = newObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

				if (newObjAFCL_ != null)
				{
					var currentSelectionObjAFCO_ = newObjAFCL_.UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
					workerObjAFCW_.SetLayerCameraPos(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
				}

				ToggleVerticalSlider(newObj_);
			};

			//// Bouton de d'annulation
			buttonResetObjB_.onClick.AddListener(() =>
			{
				CurrentUserAction_ = UIUserAction.Reset;
				EnableButtonOfLeftPanel(buttonResetObj_);

				ResetObjectAndLayer();

				CurrentUserAction_ = UIUserAction.None;
				DisableAllButtonsOfLeftPanel();
			});

			//// Bouton de déplacement
			buttonTranslateObjB_.onClick.AddListener(() =>
			{
				if (CurrentUserAction_ == UIUserAction.Translate)
				{
					CurrentUserAction_ = UIUserAction.None;
					DisableAllButtonsOfLeftPanel();
				}
				else
				{
					var currentSelectionObjAFCL_ = UISelection.CurrentSelectionObj_?.GetComponent<UIAugmentedFaceCreatorLayer>();

					if (currentSelectionObjAFCL_ != null)
					{
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						workerObjAFCW_.SetLayerCameraPos(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Translate;
					EnableButtonOfLeftPanel(buttonTranslateObj_);
				}
			});

			//// Slider vetical.
			sliderVerticalObjS_.onValueChanged.AddListener(value_ =>
			{
				if (CurrentUserAction_ == UIUserAction.Translate)
				{
					if (UISelection.CurrentSelectionObj_ == null)
					{
						return;
					}

					var currentSelectionObjUIAFCO_ = UISelection.CurrentSelectionObj_.GetComponent<UIAugmentedFaceCreatorObject>();

					if (currentSelectionObjUIAFCO_ == null)
					{
						return;
					}

					if (currentSelectionObjUIAFCO_.IsReference_)
					{
						workerObjAFCW_.TranslateMeshCameraOnZ(sliderVerticalObjS_.normalizedValue);
					}
					else
					{
						if (!currentSelectionObjUIAFCO_.IsMoveable_)
						{
							var defaultValue_ = currentSelectionObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().PosRatio_;

							sliderVerticalObjS_.normalizedValue = defaultValue_;

							return;
						}

						currentSelectionObjUIAFCO_.TranslateOnZ(sliderVerticalObjS_.normalizedValue);
					}
				}
			});

			//// Bouton de rotation
			buttonRotateObjB_.onClick.AddListener(() =>
			{
				if (CurrentUserAction_ == UIUserAction.Rotate)
				{
					CurrentUserAction_ = UIUserAction.None;
					DisableAllButtonsOfLeftPanel();
				}
				else
				{
					var currentSelectionObjAFCL_ = UISelection.CurrentSelectionObj_?.GetComponent<UIAugmentedFaceCreatorLayer>();

					if (currentSelectionObjAFCL_ != null)
					{
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						workerObjAFCW_.SetLayerCameraPos(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Rotate;
					EnableButtonOfLeftPanel(buttonRotateObj_);
				}
			});

			//// Bouton de mise à l'échelle
			buttonScaleObjB_.onClick.AddListener(() =>
			{
				if (CurrentUserAction_ == UIUserAction.Scale)
				{
					CurrentUserAction_ = UIUserAction.None;
					DisableAllButtonsOfLeftPanel();
				}
				else
				{
					var currentSelectionObjAFCL_ = UISelection.CurrentSelectionObj_?.GetComponent<UIAugmentedFaceCreatorLayer>();

					if (currentSelectionObjAFCL_ != null)
					{
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.UIObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						workerObjAFCW_.SetLayerCameraPos(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Scale;
					EnableButtonOfLeftPanel(buttonScaleObj_);
				}
			});

			//// Butons du panneau droit
			var buttonCancelObjB_ = buttonCancelObj_.GetComponent<Button>();
			var buttonOkObjB_ = buttonOkObj_.GetComponent<Button>();
			var buttonAddObjectObjB_ = buttonAddObjectObj_.GetComponent<Button>();

			//// Bouton d'annulation
			buttonCancelObjB_.onClick.AddListener(() => Navigation.Current_.LoadMainMenu());

			//// Bouton de confirmation
			buttonOkObjB_.onClick.AddListener(() =>
			{
				Save();

				Navigation.Current_.LoadMainMenu();

			});

			//// Bouton d'ajout d'objet (de type surface)
			buttonAddObjectObjB_.onClick.AddListener(() =>
			{
				var uiObjectObj_ = AddUIObject(surfaceMeshPrefab_, 0.25f, false, false, true, false, false);
				var uiObjectObjUIAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var uiObjectObjAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var uiObjectObjAFCSM_ = uiObjectObjAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

				uiObjectObjUIAFCO_.AddLayer(texture_ =>
				{
					var w_ = (float)texture_.width / workerObjAFCC_.RenderTexture_.width;
					var h_ = (float)texture_.height / workerObjAFCC_.RenderTexture_.height;

					var LayerWorkerObjAFCLW_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
					LayerWorkerObjAFCLW_.Width_ = texture_.width;
					LayerWorkerObjAFCLW_.Height_ = texture_.height;

					workerObjAFCW_.SetLayerCameraPos(uiObjectObjUIAFCO_.MeshWorkerObj_, uiObjectObjUIAFCO_.LayerWorkerObj_);
					uiObjectObjAFCSM_.Initialize(workerObjAFCW_.MeshCameraPivotObj_, w_, h_);
				});
			});

			// Ajoute les objets pré-enregistré en les masquants
			foreach (var objectPrefab_ in objectsPrefabs_)
			{
				AddUIObject(objectPrefab_, 0.25f, false, true, true, true, true);
			}
		}

		/// <summary>
		/// Annule les événements associés à la gestion des boutons.
		/// </summary>
		private void ResetButtonEvents()
		{
			// Butons du panneau gauche
			var buttonResetObjB_ = buttonResetObj_.GetComponent<Button>();
			var buttonTranslateObjB_ = buttonTranslateObj_.GetComponent<Button>();
			var buttonRotateObjB_ = buttonRotateObj_.GetComponent<Button>();
			var buttonScaleObjB_ = buttonScaleObj_.GetComponent<Button>();

			//// Butons du panneau droit
			var buttonCancelObjB_ = buttonCancelObj_.GetComponent<Button>();
			var buttonOkObjB_ = buttonOkObj_.GetComponent<Button>();
			var buttonAddObjectObjB_ = buttonAddObjectObj_.GetComponent<Button>();

			buttonResetObjB_.onClick.RemoveAllListeners();
			buttonTranslateObjB_.onClick.RemoveAllListeners();
			buttonRotateObjB_.onClick.RemoveAllListeners();
			buttonScaleObjB_.onClick.RemoveAllListeners();

			buttonCancelObjB_.onClick.RemoveAllListeners();
			buttonOkObjB_.onClick.RemoveAllListeners();
			buttonAddObjectObjB_.onClick.RemoveAllListeners();
		}

		/// <summary>
		/// Dessine l'interface utilisateur.
		/// </summary>
		private void DrawUI(float scale_)
		{
			// Composants du panneau de gauche
			var leftPanelObjRT_ = leftPanelObj_.GetComponent<RectTransform>();
			var leftPanelScrollViewContentObjRT_ = leftPanelScrollViewContentObj_.GetComponent<RectTransform>();
			var leftPanelScrollViewContentObjVLG_ = leftPanelScrollViewContentObj_.GetComponent<VerticalLayoutGroup>();
			var leftPanelScrollViewContentChildObjRTs_ = leftPanelScrollViewContentObj_.GetComponentsInChildren<RectTransform>();

			// Composants du panneau de droite
			var rightPanelObjRT_ = rightPanelObj_.GetComponent<RectTransform>();
			var rightPanelHandleObjRT_ = rightPanelHandleObj_.GetComponent<RectTransform>();
			var buttonCancelObjRT_ = buttonCancelObj_.GetComponent<RectTransform>();
			var inputFieldNameObjRT_ = inputFieldNameObj_.GetComponent<RectTransform>();
			var inputFieldNameObjTs_ = inputFieldNameObj_.GetComponentsInChildren<Text>();
			var buttonOkObjRT_ = buttonOkObj_.GetComponent<RectTransform>();
			var buttonAddObjectObjRT_ = buttonAddObjectObj_.GetComponent<RectTransform>();
			var scrollViewObjectsObjRT_ = scrollViewObjectsObj_.GetComponent<RectTransform>();
			var scrollViewObjectsContentObjRT_ = scrollViewObjectsContentObj_.GetComponent<RectTransform>();
			var scrollViewObjectsContentObjVLG_ = scrollViewObjectsContentObj_.GetComponent<VerticalLayoutGroup>();
			var scrollViewObjectsContentChildObjRTs_ = scrollViewObjectsContentObj_.GetComponentsInChildren<RectTransform>();

			// Compoants du panneau de vue
			var viewerPanelObjRT_ = viewerPanelObj_.GetComponent<RectTransform>();
			var viewerPanelObjRI_ = viewerPanelObj_.GetComponent<RawImage>();
			var workerObjAFCW_ = workerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			// Constantes
			if (scale_ > 0)
			{
				scaledTouchSurfaceSize_ = touchSurfaceSize_ * scale_;
				scaledRightPanelWidth_ = rightPanelWidth_ * scale_;
				scaledMarginSize_ = marginSize_ * scale_;
				scaledBodyFontSize_ = Mathf.RoundToInt(bodyfontSize_ * scale_);
			}

			// Logiques du panneau de gauche
			var leftPanelObjWidth_ = scaledTouchSurfaceSize_.x + scaledMarginSize_ * 2f;

			leftPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftPanelObjWidth_);

			leftPanelScrollViewContentObjVLG_.padding = new RectOffset((int)scaledMarginSize_, (int)scaledMarginSize_, (int)scaledMarginSize_, (int)scaledMarginSize_);
			leftPanelScrollViewContentObjVLG_.spacing = scaledMarginSize_;

			foreach (var leftPanelScrollViewContentChildObjRT_ in leftPanelScrollViewContentChildObjRTs_)
			{
				leftPanelScrollViewContentChildObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
				leftPanelScrollViewContentChildObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			}

			//// Si au-dessus la boucle, la boucle annule son effet.
			leftPanelScrollViewContentObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftPanelObjWidth_);

			// Logique du panneau de droite
			rightPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledRightPanelWidth_);
			rightPanelHandleObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x * 0.5f);

			//// Bouton d'annulation
			buttonCancelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonCancelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonCancelObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, -scaledMarginSize_);

			//// Champ du nom
			inputFieldNameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			inputFieldNameObjRT_.offsetMin = new Vector2(scaledTouchSurfaceSize_.x + scaledMarginSize_ * 2f, inputFieldNameObjRT_.offsetMin.y);
			inputFieldNameObjRT_.offsetMax = new Vector2((scaledTouchSurfaceSize_.x + scaledMarginSize_ * 2f) * -1, -scaledMarginSize_);

			foreach (var inputFieldNameObjT_ in inputFieldNameObjTs_)
			{
				inputFieldNameObjT_.fontSize = scaledBodyFontSize_;
			}

			//// Bouton de validation
			buttonOkObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonOkObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonOkObjRT_.anchoredPosition = new Vector2(-scaledMarginSize_, -scaledMarginSize_);

			//// Bouton d'ajount d'object
			buttonAddObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);
			buttonAddObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y);
			buttonAddObjectObjRT_.anchoredPosition = new Vector2(scaledMarginSize_, (scaledMarginSize_ * 2f + scaledTouchSurfaceSize_.y) * -1);

			//// Liste des objets
			scrollViewObjectsObjRT_.offsetMin = new Vector2(scaledMarginSize_, scaledMarginSize_);
			scrollViewObjectsObjRT_.offsetMax = new Vector2(-scaledMarginSize_, (scaledTouchSurfaceSize_.y * 2 + scaledMarginSize_ * 3f) * -1f);
			scrollViewObjectsContentObjVLG_.spacing = scaledMarginSize_;

			// Logique du panneau de vue
			if (workerObjAFCW_.MeshCameraPivotObj_ != null)
			{
				var MeshCameraPivotObjAFCC_ = workerObjAFCW_.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
				viewerPanelObjRI_.texture = MeshCameraPivotObjAFCC_.RenderTexture_;
				var MeshCameraPivotObjRatio_ = MeshCameraPivotObjAFCC_.RenderTexture_.width / MeshCameraPivotObjAFCC_.RenderTexture_.height;

				viewerPanelObjWidth_ = Screen.width - leftPanelObjWidth_ - scaledRightPanelWidth_ - ScaledTouchSurfaceSize_.x * 0.5f;
				viewerPanelObjHeight_ = (float)Screen.height;
				var viewerPanelObjRatio_ = viewerPanelObjWidth_ / viewerPanelObjHeight_;

				var ratioCorrection_ = MeshCameraPivotObjRatio_ - viewerPanelObjRatio_;

				viewerPanelObjXOffset_ = 0f;

				if (ratioCorrection_ >= 0f)
				{
					viewerPanelObjXOffset_ = leftPanelObjWidth_;
					viewerPanelObjHeight_ *= 1f - ratioCorrection_;
				}
				else
				{
					viewerPanelObjXOffset_ = leftPanelObjWidth_ + (viewerPanelObjWidth_ - viewerPanelObjHeight_) * 0.5f;
					viewerPanelObjWidth_ = viewerPanelObjHeight_;
				}

				viewerPanelObjYOffset_ = (Screen.height - viewerPanelObjHeight_) * -0.5f;

				viewerPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewerPanelObjWidth_);
				viewerPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, viewerPanelObjHeight_);
				viewerPanelObjRT_.anchoredPosition = new Vector2(viewerPanelObjXOffset_, viewerPanelObjYOffset_);

				//// Silder vertical
				var sliderVerticalBackgroundObj_ = sliderVerticalObj_.transform.Find("Background").gameObject;
				var sliderVerticalFillAreaObj_ = sliderVerticalObj_.transform.Find("FillArea").gameObject;
				var sliderVerticalHandleSlideAreaObj_ = sliderVerticalObj_.transform.Find("HandleSlideArea").gameObject;
				var sliderVerticalHandleObj_ = sliderVerticalHandleSlideAreaObj_.transform.Find("Handle").gameObject;

				var sliderVerticalObjRT_ = sliderVerticalObj_.GetComponent<RectTransform>();
				var sliderVerticalBackgroundObjRT_ = sliderVerticalBackgroundObj_.GetComponent<RectTransform>();
				var sliderVerticalHandleSlideAreaObjRT_ = sliderVerticalHandleSlideAreaObj_.GetComponent<RectTransform>();
				var sliderVerticalHandleObjRT_ = sliderVerticalHandleObj_.GetComponent<RectTransform>();

				sliderVerticalBackgroundObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);

				sliderVerticalObjRT_.offsetMin = new Vector2(0f, scaledMarginSize_);
				sliderVerticalObjRT_.offsetMax = new Vector2(0f, scaledMarginSize_ * -1f);
				sliderVerticalObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledTouchSurfaceSize_.x);

				sliderVerticalHandleSlideAreaObjRT_.offsetMin = new Vector2(0f, scaledTouchSurfaceSize_.y * 0.5f);
				sliderVerticalHandleSlideAreaObjRT_.offsetMax = new Vector2(0f, scaledTouchSurfaceSize_.y * -0.5f);

				sliderVerticalHandleObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledTouchSurfaceSize_.y * 0.5f);
			}

			// Dessine les éléments enfants
			Draw?.Invoke(scale_);
		}

		/// <summary>
		/// Force le système à dessiner l'interface.
		/// </summary>
		public void ForceDraw()
		{
			if (isInitialized_)
			{
				var canvasObjUICS_ = canvasObj_.GetComponent<UICanvasScaler>();
				canvasObjUICS_.InvokeChangedScaleEvent();
			}
		}

		/// <summary>
		/// Coroutine qui configure le composant après la phase d'initialisation de Unity. 
		/// </summary>
		/// <returns></returns>
		private IEnumerator SetupCoroutine()
		{
			yield return new WaitUntil(() => isInitialized_ && workerObj_.GetComponent<AugmentedFaceCreatorWorker>().IsInitialized_);

			AddUIObject(faceMeshPrefab_, 0.5f, true, true, false, true, false);

			SetupTouch();

			SetupButton();
		}

		private void Start()
		{
			canvasObj_ = GameObject.Find("Canvas");
			backgroundObj_ = transform.Find("Background").gameObject;
			leftPanelObj_ = transform.Find("LeftPanel").gameObject;
			leftPanelScrollViewObj_ = leftPanelObj_.transform.Find("ScrollView").gameObject;
			leftPanelScrollViewContentObj_ = leftPanelScrollViewObj_.transform.Find("Viewport").Find("Content").gameObject;
			buttonResetObj_ = leftPanelScrollViewContentObj_.transform.Find("Button-Reset").gameObject;
			buttonTranslateObj_ = leftPanelScrollViewContentObj_.transform.Find("Button-Translate").gameObject;
			buttonRotateObj_ = leftPanelScrollViewContentObj_.transform.Find("Button-Rotate").gameObject;
			buttonScaleObj_ = leftPanelScrollViewContentObj_.transform.Find("Button-Scale").gameObject;
			rightPanelObj_ = transform.Find("RightPanel").gameObject;
			rightPanelHandleObj_ = rightPanelObj_.transform.Find("RightPanelHandle").gameObject;
			buttonCancelObj_ = rightPanelObj_.transform.Find("Button-Cancel").gameObject;
			inputFieldNameObj_ = rightPanelObj_.transform.Find("InputField-Name").gameObject;
			buttonOkObj_ = rightPanelObj_.transform.Find("Button-Ok").gameObject;
			buttonAddObjectObj_ = rightPanelObj_.transform.Find("Button-AddObject").gameObject;
			scrollViewObjectsObj_ = rightPanelObj_.transform.Find("ScrollView-Objects").gameObject;
			scrollViewObjectsContentObj_ = scrollViewObjectsObj_.transform.Find("Viewport").Find("Content").gameObject;
			viewerPanelObj_ = transform.Find("ViewerPanel").gameObject;
			sliderVerticalObj_ = viewerPanelObj_.transform.Find("SliderVertical").gameObject;

			uiObjectsObjs_ = new Dictionary<string, GameObject>();

			if (workerObj_ == null)
			{
				workerObj_ = GameObject.Find("Worker");
			}

			var canvasObjUICS_ = canvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();

			Initialize?.Invoke();

			isInitialized_ = true;

			StartCoroutine(SetupCoroutine());
		}
	}
}
