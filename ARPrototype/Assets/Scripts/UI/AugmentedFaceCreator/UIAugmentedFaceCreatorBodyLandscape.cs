using System;
using System.Collections.Generic;
using System.IO;
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
		/// <summary>
		/// Vector2 temporaire.
		/// </summary>
		private Vector2 translationInitialPosition_;

		/// <summary>
		/// Nombre flotant temporaire.
		/// </summary>
		private float rotationAng_;

		/// <summary>
		/// Direction cardinale temporaire.
		/// </summary>
		private UIDragDirection scalingCardDir_;

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
		/// Préfabriqué d'un objet de maillage d'interface.
		/// </summary>
		[SerializeField]
		private GameObject uiObjectPrefab_;

		/// <summary>
		/// Préfabriqué du maillage du vissage.
		/// </summary>
		[SerializeField]
		private GameObject referenceMeshPrefab_;

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
		/// Texture de référence du maillage de référence.
		/// </summary>
		[SerializeField]
		private Texture2D referenceLayer_;

		public static UIAugmentedFaceCreatorBodyLandscape Instance_ { get; private set; }

		/// <summary>
		/// Taille de la vue.
		/// </summary>
		public float ViewerPanelObjWidth_ { get; set; }

		/// <summary>
		/// Taille de la vue.
		/// </summary>
		public float ViewerPanelObjHeight_ { get; set; }

		/// <summary>
		/// Position sur l'axe X de la vue.
		/// </summary>
		public float ViewerPanelObjXOffset_ { get; set; }

		/// <summary>
		/// Position sur l'axe X de la vue.
		/// </summary>
		public float ViewerPanelObjYOffset_ { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public GameObject CanvasObj_ => GameObject.Find("Canvas");

		/// <summary>
		/// 
		/// </summary>
		public GameObject WorkerObj_ => GameObject.Find("Worker");

		/// <summary>
		/// 
		/// </summary>
		public GameObject BackgroundObj_ => transform.Find("Background").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject LeftPanelObj_ => transform.Find("LeftPanel").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject LeftPanelScrollViewObj_ => LeftPanelObj_.transform.Find("ScrollView").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject LeftPanelScrollViewContentObj_ => LeftPanelScrollViewObj_.transform.Find("Viewport").Find("Content").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonResetObj_ => LeftPanelScrollViewContentObj_.transform.Find("Button-Reset").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonTranslateObj_ => LeftPanelScrollViewContentObj_.transform.Find("Button-Translate").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonRotateObj_ => LeftPanelScrollViewContentObj_.transform.Find("Button-Rotate").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonScaleObj_ => LeftPanelScrollViewContentObj_.transform.Find("Button-Scale").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject RightPanelObj_ => transform.Find("RightPanel").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject RightPanelHandleObj_ => RightPanelObj_.transform.Find("RightPanelHandle").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonCancelObj_ => RightPanelObj_.transform.Find("Button-Cancel").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject InputFieldNameObj_ => RightPanelObj_.transform.Find("InputField-Name").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonOkObj_ => RightPanelObj_.transform.Find("Button-Ok").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ButtonAddObjectObj_ => RightPanelObj_.transform.Find("Button-AddObject").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ScrollViewObjectsObj_ => RightPanelObj_.transform.Find("ScrollView-Objects").gameObject;


		/// <summary>
		/// 
		/// </summary>
		public GameObject ScrollViewObjectsContentObj_ => ScrollViewObjectsObj_.transform.Find("Viewport").Find("Content").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject ViewerPanelObj_ => transform.Find("ViewerPanel").gameObject;

		/// <summary>
		/// 
		/// </summary>
		public GameObject SliderVerticalObj_ => ViewerPanelObj_.transform.Find("SliderVertical").gameObject;

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		public Vector2 ScaledTouchSurfaceSize_ { get; private set; }

		/// <summary>
		/// Taille des surfaces à toucher mis à l'échelle mis à l'échelle.
		/// </summary>
		public float ScaledRightPanelWidth_ { get; private set; }

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		public float ScaledMarginSize_ { get; set; }

		/// <summary>
		/// Largeur du panneau de droite avant transformation mis à l'échelle.
		/// </summary>
		public float BeginRightPanelWidth_ { get; private set; }

		/// <summary>
		/// Taille des marges mis à l'échelle.
		/// </summary>
		public int ScaledBodyFontSize_ { get; private set; }

		/// <summary>
		/// Indique si le composant est initialisé.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Action de l'utilisateur en cours.
		/// </summary>
		public UIUserAction CurrentUserAction_ { get; private set; }

		/// <summary>
		/// Travailleur de maillage actuel.
		/// </summary>
		public GameObject CurrentMeshWokerObj_ { get; private set; }

		/// <summary>
		/// Travailleur de calque actuel.
		/// </summary>
		public GameObject CurrentLayerWokerObj_ { get; private set; }

		/// <summary>
		/// Appeler lorsque des éléments enfants doivent être dessinner.
		/// </summary>
		public event Action<float> DrawUI;

		/// <summary>
		/// Débute le redimensionnement du panneaux de droite. 
		/// </summary>
		/// <param name="pos_">Position du curseur.</param>
		private void OneBeginDragOnRightPanelHandleObj(Vector2 pos_)
		{
			BeginRightPanelWidth_ = Screen.width - pos_.x - RightPanelObj_.GetComponent<RectTransform>().sizeDelta.x;
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
			var leftPanelObjRT_ = LeftPanelObj_.GetComponent<RectTransform>();

			var min_ = rightPanelWidth_ * CanvasObj_.GetComponent<UICanvasScaler>().Scale_;
			var max_ = Screen.width - leftPanelObjRT_.sizeDelta.x - ScaledTouchSurfaceSize_.x * 0.5f;

			ScaledRightPanelWidth_ = Mathf.Clamp(Screen.width - pos_.x - BeginRightPanelWidth_, min_, max_);

			Draw(0f);
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

				translationInitialPosition_ = pos_;

				currentSelectedObjectObjUIAFCO_.SetupTranslation();
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				translationInitialPosition_ = pos_;

				currentSelectedObjectObjUIAFCL_.SetupTranslation();
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

			var meshCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			if (pos_.x >= ViewerPanelObjXOffset_ &&
				pos_.x <= ViewerPanelObjXOffset_ + ViewerPanelObjWidth_ &&
				pos_.y >= ViewerPanelObjYOffset_ &&
				pos_.y <= ViewerPanelObjYOffset_ + ViewerPanelObjHeight_)
			{
				pos_ -= translationInitialPosition_;

				pos_.x /= ViewerPanelObjWidth_ + ViewerPanelObjXOffset_;
				pos_.y /= ViewerPanelObjHeight_ + ViewerPanelObjYOffset_;
				pos_ *= meshCameraPivotObjAFCC_.RectBounds_.size;

				if (currentSelectedObjectObjUIAFCO_ != null)
				{
					if (!currentSelectedObjectObjUIAFCO_.IsMoveable_)
					{
						return;
					}

					currentSelectedObjectObjUIAFCO_.Translate(pos_);
				}
				else if (currentSelectedObjectObjUIAFCL_ != null)
				{
					currentSelectedObjectObjUIAFCL_.Translate(pos_);
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

				currentSelectedObjectObjUIAFCO_.SetupRotation();

				rotationAng_ = ang_;
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				currentSelectedObjectObjUIAFCL_.SetupRotation();

				rotationAng_ = ang_;
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

				currentSelectedObjectObjUIAFCO_.Rotate(ang_);
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				currentSelectedObjectObjUIAFCL_.Rotate(ang_);
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

				currentSelectedObjectObjUIAFCO_.SetupScaling();
				scalingCardDir_ = cardDir_;
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				currentSelectedObjectObjUIAFCL_.SetupScaling();
				scalingCardDir_ = cardDir_;
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

				if (cardDir2_ == UIDragDirection.Up ||
					cardDir2_ == UIDragDirection.Down)
				{
					currentSelectedObjectObjUIAFCO_.Scale(new Vector2(1f, mag2_));
				}

				if (cardDir2_ == UIDragDirection.Left ||
					cardDir2_ == UIDragDirection.Right)
				{
					currentSelectedObjectObjUIAFCO_.Scale(new Vector2(mag2_, 1f));
				}

				if (cardDir2_ == UIDragDirection.Angle)
				{
					currentSelectedObjectObjUIAFCO_.Scale(new Vector2(mag2_, mag2_));
				}
			}
			else if (currentSelectedObjectObjUIAFCL_ != null)
			{
				if (cardDir2_ == UIDragDirection.Up ||
					cardDir2_ == UIDragDirection.Down)
				{
					currentSelectedObjectObjUIAFCL_.Scale(new Vector2(1f, mag2_));
				}

				if (cardDir2_ == UIDragDirection.Left ||
					cardDir2_ == UIDragDirection.Right)
				{
					currentSelectedObjectObjUIAFCL_.Scale(new Vector2(mag2_, 1f));
				}

				if (cardDir2_ == UIDragDirection.Angle)
				{
					currentSelectedObjectObjUIAFCL_.Scale(new Vector2(mag2_, mag2_));
				}
			}
		}

		/// <summary>
		/// Configure la gestion du toucher (icompris de la souris).
		/// </summary>
		private void SetupTouch()
		{
			// Composants du panneau droit
			var rightPanelHandleObjUIT_ = RightPanelHandleObj_.GetComponent<UITouch>();

			rightPanelHandleObjUIT_.OneBeginDrag += OneBeginDragOnRightPanelHandleObj;
			rightPanelHandleObjUIT_.OneDrag += OneDragOnRightPanelHandleObj;

			// Composants du panneau de vue
			var viewerPanelObjUIT_ = ViewerPanelObj_.GetComponent<UITouch>();

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
			var rightPanelHandleObjUIT_ = RightPanelHandleObj_.GetComponent<UITouch>();

			rightPanelHandleObjUIT_.OneBeginDrag -= OneBeginDragOnRightPanelHandleObj;
			rightPanelHandleObjUIT_.OneDrag -= OneDragOnRightPanelHandleObj;

			// Composants du panneau de vue
			var viewerPanelObjUIT_ = ViewerPanelObj_.GetComponent<UITouch>();

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
		public GameObject AddObject(GameObject meshPrefab_, float posRatio_, bool isReference_, bool isPermanent_, bool isMoveable_, bool resize_, bool hide_)
		{
			var workerObjAFCW_ = AugmentedFaceCreatorWorker.Instance_;

			if (!workerObjAFCW_.CanAddSubWorker)
			{
				return null;
			}

			if (CurrentMeshWokerObj_ != null && CurrentLayerWokerObj_ != null)
			{
				CurrentMeshWokerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().SetTexture(CurrentLayerWokerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>().StopRecording());
			}

			(CurrentMeshWokerObj_, CurrentLayerWokerObj_) = workerObjAFCW_.AddSubWorker(meshPrefab_, isReference_ ? 0.5f : posRatio_, workerObjAFCW_.TextureWidth_, workerObjAFCW_.TextureHeight_);

			var uiObjectObj_ = Instantiate(uiObjectPrefab_, Vector3.zero, Quaternion.identity, ScrollViewObjectsContentObj_.transform);
			var uiObjectObjAFCUIO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			uiObjectObjAFCUIO_.Initialize(CurrentMeshWokerObj_, CurrentLayerWokerObj_, isReference_, isPermanent_, isMoveable_);

			if (meshPrefab_ != surfaceMeshPrefab_ &&
				!AppManager.MeshObjs_.ContainsValue(meshPrefab_))
			{
				AppManager.MeshObjs_.Add(meshPrefab_.name, meshPrefab_);
				uiObjectObjAFCUIO_.MeshObjName_ = meshPrefab_.name;
			}

			var meshWorkerObjAFCMW_ = CurrentMeshWokerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

			if (resize_)
			{
				meshWorkerObjAFCMW_.ScaleMeshToCameraBounds();
			}

			meshWorkerObjAFCMW_.PivotObj_.SetActive(!hide_);

			return uiObjectObj_;
		}

		private void EnableButtonOfLeftPanel(GameObject buttonObj_)
		{
			var buttonObjIs_ = LeftPanelScrollViewContentObj_.GetComponentsInChildren<Image>();

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
			var buttonObjIs_ = LeftPanelScrollViewContentObj_.GetComponentsInChildren<Image>();

			foreach (var buttonObjI_ in buttonObjIs_)
			{
				buttonObjI_.color = Color.HSVToRGB(0f, 0f, 1f);

				if (buttonObjI_.gameObject == ButtonTranslateObj_)
				{
					SliderVerticalObj_.SetActive(false);
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

			faceInfo_.Name_ = InputFieldNameObj_.GetComponentInChildren<Text>().text;
			faceInfo_.Name_ = string.IsNullOrWhiteSpace(faceInfo_.Name_) ? "Unknow face" : faceInfo_.Name_;

			var referenceObj_ = UIAugmentedFaceCreatorObject.ReferenceObjectObj_;
			var referenceObjUIAFCO_ = referenceObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			var referenceMeshWorkerObj_ = referenceObjUIAFCO_.MeshWorkerObj_;
			var referenceLayerWorkerObj_ = referenceObjUIAFCO_.LayerWorkerObj_;
			var referenceAnchorObjs_ = referenceMeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetAnchorObjs();
			var meshCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var layerCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var previewTexture_ = new Texture2D(meshCameraPivotObjAFCC_.RenderTexture_.width, meshCameraPivotObjAFCC_.RenderTexture_.height, TextureFormat.ARGB32, false);

			meshCameraPivotObjAFCC_.ZRatio = 0f;

			if (UIAugmentedFaceCreatorObject.ReferenceObjectObj_ != null)
			{
				var referenceObjectObjUIAFCO_ = UIAugmentedFaceCreatorObject.ReferenceObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

				Focus(referenceObjectObjUIAFCO_.MeshWorkerObj_, referenceObjectObjUIAFCO_.LayerWorkerObj_);

				var cullingMask_ = layerCameraPivotObjAFCC_.CameraObjC_.cullingMask;
				layerCameraPivotObjAFCC_.CameraObjC_.cullingMask = 1 << 0;
				layerCameraPivotObjAFCC_.CameraObjC_.Render();
				layerCameraPivotObjAFCC_.CameraObjC_.cullingMask = cullingMask_;

				Focus(referenceObjectObjUIAFCO_.MeshWorkerObj_, referenceObjectObjUIAFCO_.LayerWorkerObj_);
			}

			var currentRenderTexture_ = RenderTexture.active;
			meshCameraPivotObjAFCC_.CameraObjC_.Render();
			RenderTexture.active = meshCameraPivotObjAFCC_.RenderTexture_;
			previewTexture_.ReadPixels(new Rect(0, 0, meshCameraPivotObjAFCC_.RenderTexture_.width, meshCameraPivotObjAFCC_.RenderTexture_.height), 0, 0);
			previewTexture_.Apply();
			RenderTexture.active = currentRenderTexture_;

			var previewBytes_ = previewTexture_.EncodeToPNG();

			var previewName_ = $"preview-{faceIndex_:D4}.png";
			var previewPath_ = Path.Combine(faceFolderPath_, previewName_);

			File.WriteAllBytes(previewPath_, previewBytes_);

			faceInfo_.PreviewPath_ = previewPath_;

			faceInfo_.FaceParts_ = new List<FacePartInfo>();

			int facePartIndex_ = 0;
			for (int i_ = 0; i_ < ScrollViewObjectsContentObj_.transform.childCount; i_++)
			{
				var objectObj_ = ScrollViewObjectsContentObj_.transform.GetChild(i_);
				var uiObjectObjUIAFCO_ = objectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var objectObjUIAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

				if (!objectObjUIAFCMW_.PivotObj_.activeSelf)
				{
					continue;
				}

				var facePart_ = new FacePartInfo();

				facePart_.IsReference_ = uiObjectObjUIAFCO_.IsReference_;

				facePart_.Anchor_ = (ReferenceAnchor)uiObjectObjUIAFCO_.AnchorIndex_;

				var MeshObjAFCSM_ = objectObjUIAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

				facePart_.Is2D_ = MeshObjAFCSM_ != null;

				if (MeshObjAFCSM_ != null)
				{
					facePart_.Dimension_ = MeshObjAFCSM_.Size_;
				}

				facePart_.MeshObjName_ = uiObjectObjUIAFCO_.MeshObjName_;

				facePart_.PositionOffset_ = objectObjUIAFCMW_.GetPositionOffset(referenceAnchorObjs_[uiObjectObjUIAFCO_.AnchorIndex_]);

				facePart_.Rotation_ = objectObjUIAFCMW_.transform.localRotation;

				facePart_.Scale_ = objectObjUIAFCMW_.transform.localScale;

				var layerWorkerObjAFCLW_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

				var texture_ = layerWorkerObjAFCLW_.Save();

				if (texture_ != null)
				{
					var texBytes_ = texture_.EncodeToPNG();

					var texName_ = $"FacePartTexture-{faceIndex_:D4}-{facePartIndex_:D4}.png";
					var texPath_ = Path.Combine(faceFolderPath_, texName_);

					File.WriteAllBytes(texPath_, texBytes_);

					facePart_.TextureWidth_ = texture_.width;

					facePart_.TextureHeight = texture_.height;

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

		/// <summary>
		/// Change la position de la caméra de calque en fonction des travailleurs de maillage et de calque tout en redimentionant la caméra de calque à la dimension de l'objet.
		/// </summary>
		/// <param name="meshWorkerObj_">Travailleur de maillage.</param>
		/// <param name="layerWorkerObj_">Travailleur de calque.</param>
		public void Focus(GameObject meshWorkerObj_, GameObject layerWorkerObj_)
		{
			if (CurrentMeshWokerObj_ != meshWorkerObj_ && CurrentLayerWokerObj_ != layerWorkerObj_ &&
				CurrentMeshWokerObj_ != null && CurrentLayerWokerObj_ != null)
			{
				var currentMeshWokerObjAFCMW_ = CurrentMeshWokerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var currentLayerWokerObjAFCLW_ = CurrentLayerWokerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

				currentMeshWokerObjAFCMW_.SetTexture(currentLayerWokerObjAFCLW_.StopRecording());
			}

			CurrentMeshWokerObj_ = meshWorkerObj_;
			CurrentLayerWokerObj_ = layerWorkerObj_;

			var meshWokerObjAFCMW_ = CurrentMeshWokerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			var layerWokerObjAFCLW_ = CurrentLayerWokerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			meshWokerObjAFCMW_.SetTexture(layerWokerObjAFCLW_.StartRecording());
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
				uiObjectObjUIAFCO_ = uiObjectObjUIAFCL_.ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var meshWorkerObj_ = uiObjectObjUIAFCO_.MeshWorkerObj_;
				var meshWorkerAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var meshWorkerAFCSM_ = meshWorkerAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();


				var layerObj_ = uiObjectObjUIAFCL_.LayerObj_;
				var layerObjAFCSM_ = layerObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();
				var workerAFCW_ = WorkerObj_.GetComponent<AugmentedFaceCreatorWorker>();
				var layerCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

				var texture_ = layerObj_.GetComponent<MeshRenderer>().material.mainTexture;

				if (texture_.width > texture_.height)
				{
					var r_ = (float)texture_.width / texture_.height;
					layerCameraPivotObjAFCC_.SetRenderTexture(Mathf.FloorToInt(workerAFCW_.TextureWidth_ * r_), workerAFCW_.TextureHeight_);
					if (meshWorkerAFCSM_ != null)
					{
						meshWorkerAFCSM_.Initialize(AugmentedFaceCreatorWorker.MeshCameraPivotObj_, 1f, 1f / r_);
					}
				}
				else
				{
					var r_ = (float)texture_.height / texture_.width;
					layerCameraPivotObjAFCC_.SetRenderTexture(workerAFCW_.TextureWidth_, Mathf.FloorToInt(workerAFCW_.TextureHeight_ * r_));
					if (meshWorkerAFCSM_ != null)
					{
						meshWorkerAFCSM_.Initialize(AugmentedFaceCreatorWorker.MeshCameraPivotObj_, 1f / r_, 1f);
					}
				}

				layerObj_.transform.localPosition = new Vector3(0f, 0f, layerObj_.transform.localPosition.z);
				layerObj_.transform.localRotation = Quaternion.identity;
				layerObj_.transform.localScale = Vector3.one;

				if (!uiObjectObjUIAFCO_.IsReference_)
				{
					meshWorkerObj_.transform.localPosition = new Vector3(0f, 0f, layerObj_.transform.localPosition.z);
					meshWorkerObj_.transform.localRotation = Quaternion.identity;
					meshWorkerObj_.transform.localScale = Vector3.one;
				}

				layerObjAFCSM_.Initialize(AugmentedFaceCreatorWorker.LayerCameraPivotObj_);
				Focus(uiObjectObjUIAFCO_.MeshWorkerObj_, uiObjectObjUIAFCO_.LayerWorkerObj_);
			}
			else if (uiObjectObjUIAFCO_ != null)
			{
				if (uiObjectObjUIAFCO_.IsReference_)
				{
					return;
				}

				var meshWorkerObj_ = uiObjectObjUIAFCO_.MeshWorkerObj_;
				var meshWorkerAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var referenceObjUIAFCO_ = UIAugmentedFaceCreatorObject.ReferenceObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

				var offset_ = meshWorkerAFCMW_.GetPositionOffset(referenceObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>().GetAnchorObjs()[uiObjectObjUIAFCO_.AnchorIndex_]);

				meshWorkerAFCMW_.PivotObj_.transform.Translate(offset_);

				meshWorkerObj_.transform.localPosition = new Vector3(0f, 0f, meshWorkerObj_.transform.localPosition.z);
				meshWorkerAFCMW_.transform.localRotation = Quaternion.identity;
				meshWorkerAFCMW_.transform.localScale = Vector3.one;
			}
		}

		private void ToggleVerticalSlider(GameObject uiObjectObj_)
		{
			if (uiObjectObj_ == null)
			{
				return;
			}

			if (CurrentUserAction_ == UIUserAction.Translate)
			{
				var uiObjectObjAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();

				if (uiObjectObjAFCO_ != null)
				{
					SliderVerticalObj_.SetActive(true);

					var sliderVerticalObjS_ = SliderVerticalObj_.GetComponent<Slider>();

					if (uiObjectObjAFCO_.IsReference_)
					{
						var value_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().ZRatio;

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
					SliderVerticalObj_.SetActive(false);
				}
			}
			else
			{
				SliderVerticalObj_.SetActive(false);
			}
		}

		/// <summary>
		/// Configure la gestion des boutons.
		/// </summary>
		private void SetupButton()
		{
			// Composants annexes
			var meshCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			// Butons du panneau gauche
			var buttonResetObjB_ = ButtonResetObj_.GetComponent<Button>();
			var buttonTranslateObjB_ = ButtonTranslateObj_.GetComponent<Button>();
			var sliderVerticalObjS_ = SliderVerticalObj_.GetComponent<Slider>();
			var buttonRotateObjB_ = ButtonRotateObj_.GetComponent<Button>();
			var buttonScaleObjB_ = ButtonScaleObj_.GetComponent<Button>();

			UISelection.SelectionChanged += (oldObj_, newObj_) =>
			{
				var newObjAFCL_ = newObj_.GetComponent<UIAugmentedFaceCreatorLayer>();

				if (newObjAFCL_ != null)
				{
					var currentSelectionObjAFCO_ = newObjAFCL_.ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
					Focus(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
				}

				ToggleVerticalSlider(newObj_);
			};

			//// Bouton de d'annulation
			buttonResetObjB_.onClick.AddListener(() =>
			{
				CurrentUserAction_ = UIUserAction.Reset;
				EnableButtonOfLeftPanel(ButtonResetObj_);

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
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						Focus(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Translate;
					EnableButtonOfLeftPanel(ButtonTranslateObj_);
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
						meshCameraPivotObjAFCC_.ZRatio = sliderVerticalObjS_.normalizedValue;
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
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						Focus(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Rotate;
					EnableButtonOfLeftPanel(ButtonRotateObj_);
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
						var currentSelectionObjAFCO_ = currentSelectionObjAFCL_.ObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
						Focus(currentSelectionObjAFCO_.MeshWorkerObj_, currentSelectionObjAFCO_.LayerWorkerObj_);
					}

					CurrentUserAction_ = UIUserAction.Scale;
					EnableButtonOfLeftPanel(ButtonScaleObj_);
				}
			});

			//// Butons du panneau droit
			var buttonCancelObjB_ = ButtonCancelObj_.GetComponent<Button>();
			var buttonOkObjB_ = ButtonOkObj_.GetComponent<Button>();
			var buttonAddObjectObjB_ = ButtonAddObjectObj_.GetComponent<Button>();

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
				var uiObjectObj_ = AddObject(surfaceMeshPrefab_, 0.25f, false, false, true, false, false);
				var uiObjectObjUIAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
				var uiObjectObjAFCMW_ = uiObjectObjUIAFCO_.MeshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
				var uiObjectObjAFCSM_ = uiObjectObjAFCMW_.MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

				uiObjectObjUIAFCO_.AddLayer(texture_ =>
				{
					var w_ = (float)texture_.width / meshCameraPivotObjAFCC_.RenderTexture_.width;
					var h_ = (float)texture_.height / meshCameraPivotObjAFCC_.RenderTexture_.height;

					var LayerWorkerObjAFCLW_ = uiObjectObjUIAFCO_.LayerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
					LayerWorkerObjAFCLW_.Width_ = texture_.width;
					LayerWorkerObjAFCLW_.Height_ = texture_.height;

					Focus(uiObjectObjUIAFCO_.MeshWorkerObj_, uiObjectObjUIAFCO_.LayerWorkerObj_);
					uiObjectObjAFCSM_.Initialize(AugmentedFaceCreatorWorker.MeshCameraPivotObj_, w_, h_);
				});
			});
		}

		/// <summary>
		/// Annule les événements associés à la gestion des boutons.
		/// </summary>
		private void ResetButtonEvents()
		{
			// Butons du panneau gauche
			var buttonResetObjB_ = ButtonResetObj_.GetComponent<Button>();
			var buttonTranslateObjB_ = ButtonTranslateObj_.GetComponent<Button>();
			var buttonRotateObjB_ = ButtonRotateObj_.GetComponent<Button>();
			var buttonScaleObjB_ = ButtonScaleObj_.GetComponent<Button>();

			//// Butons du panneau droit
			var buttonCancelObjB_ = ButtonCancelObj_.GetComponent<Button>();
			var buttonOkObjB_ = ButtonOkObj_.GetComponent<Button>();
			var buttonAddObjectObjB_ = ButtonAddObjectObj_.GetComponent<Button>();

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
		private void Draw(float scale_)
		{
			// Composants du panneau de gauche
			var leftPanelObjRT_ = LeftPanelObj_.GetComponent<RectTransform>();
			var leftPanelScrollViewContentObjRT_ = LeftPanelScrollViewContentObj_.GetComponent<RectTransform>();
			var leftPanelScrollViewContentObjVLG_ = LeftPanelScrollViewContentObj_.GetComponent<VerticalLayoutGroup>();
			var leftPanelScrollViewContentChildObjRTs_ = LeftPanelScrollViewContentObj_.GetComponentsInChildren<RectTransform>();

			// Composants du panneau de droite
			var rightPanelObjRT_ = RightPanelObj_.GetComponent<RectTransform>();
			var rightPanelHandleObjRT_ = RightPanelHandleObj_.GetComponent<RectTransform>();
			var buttonCancelObjRT_ = ButtonCancelObj_.GetComponent<RectTransform>();
			var inputFieldNameObjRT_ = InputFieldNameObj_.GetComponent<RectTransform>();
			var inputFieldNameObjTs_ = InputFieldNameObj_.GetComponentsInChildren<Text>();
			var buttonOkObjRT_ = ButtonOkObj_.GetComponent<RectTransform>();
			var buttonAddObjectObjRT_ = ButtonAddObjectObj_.GetComponent<RectTransform>();
			var scrollViewObjectsObjRT_ = ScrollViewObjectsObj_.GetComponent<RectTransform>();
			var scrollViewObjectsContentObjRT_ = ScrollViewObjectsContentObj_.GetComponent<RectTransform>();
			var scrollViewObjectsContentObjVLG_ = ScrollViewObjectsContentObj_.GetComponent<VerticalLayoutGroup>();
			var scrollViewObjectsContentChildObjRTs_ = ScrollViewObjectsContentObj_.GetComponentsInChildren<RectTransform>();

			// Compoants du panneau de vue
			var viewerPanelObjRT_ = ViewerPanelObj_.GetComponent<RectTransform>();
			var viewerPanelObjRI_ = ViewerPanelObj_.GetComponent<RawImage>();
			var workerObjAFCW_ = WorkerObj_.GetComponent<AugmentedFaceCreatorWorker>();

			// Constantes
			if (scale_ > 0)
			{
				ScaledTouchSurfaceSize_ = touchSurfaceSize_ * scale_;
				ScaledRightPanelWidth_ = rightPanelWidth_ * scale_;
				ScaledMarginSize_ = marginSize_ * scale_;
				ScaledBodyFontSize_ = Mathf.RoundToInt(bodyfontSize_ * scale_);
			}

			// Logiques du panneau de gauche
			var leftPanelObjWidth_ = ScaledTouchSurfaceSize_.x + ScaledMarginSize_ * 2f;

			leftPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftPanelObjWidth_);

			leftPanelScrollViewContentObjVLG_.padding = new RectOffset((int)ScaledMarginSize_, (int)ScaledMarginSize_, (int)ScaledMarginSize_, (int)ScaledMarginSize_);
			leftPanelScrollViewContentObjVLG_.spacing = ScaledMarginSize_;

			foreach (var leftPanelScrollViewContentChildObjRT_ in leftPanelScrollViewContentChildObjRTs_)
			{
				leftPanelScrollViewContentChildObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);
				leftPanelScrollViewContentChildObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y);
			}

			//// Si au-dessus la boucle, la boucle annule son effet.
			leftPanelScrollViewContentObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftPanelObjWidth_);

			// Logique du panneau de droite
			rightPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledRightPanelWidth_);
			rightPanelHandleObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x * 0.5f);

			//// Bouton d'annulation
			buttonCancelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);
			buttonCancelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y);
			buttonCancelObjRT_.anchoredPosition = new Vector2(ScaledMarginSize_, -ScaledMarginSize_);

			//// Champ du nom
			inputFieldNameObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y);
			inputFieldNameObjRT_.offsetMin = new Vector2(ScaledTouchSurfaceSize_.x + ScaledMarginSize_ * 2f, inputFieldNameObjRT_.offsetMin.y);
			inputFieldNameObjRT_.offsetMax = new Vector2((ScaledTouchSurfaceSize_.x + ScaledMarginSize_ * 2f) * -1, -ScaledMarginSize_);

			foreach (var inputFieldNameObjT_ in inputFieldNameObjTs_)
			{
				inputFieldNameObjT_.fontSize = ScaledBodyFontSize_;
			}

			//// Bouton de validation
			buttonOkObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);
			buttonOkObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y);
			buttonOkObjRT_.anchoredPosition = new Vector2(-ScaledMarginSize_, -ScaledMarginSize_);

			//// Bouton d'ajount d'object
			buttonAddObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);
			buttonAddObjectObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y);
			buttonAddObjectObjRT_.anchoredPosition = new Vector2(ScaledMarginSize_, (ScaledMarginSize_ * 2f + ScaledTouchSurfaceSize_.y) * -1);

			//// Liste des objets
			scrollViewObjectsObjRT_.offsetMin = new Vector2(ScaledMarginSize_, ScaledMarginSize_);
			scrollViewObjectsObjRT_.offsetMax = new Vector2(-ScaledMarginSize_, (ScaledTouchSurfaceSize_.y * 2 + ScaledMarginSize_ * 3f) * -1f);
			scrollViewObjectsContentObjVLG_.spacing = ScaledMarginSize_;

			// Logique du panneau de vue
			if (AugmentedFaceCreatorWorker.MeshCameraPivotObj_ != null)
			{
				var MeshCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
				viewerPanelObjRI_.texture = MeshCameraPivotObjAFCC_.RenderTexture_;
				viewerPanelObjRI_.material.mainTexture = MeshCameraPivotObjAFCC_.RenderTexture_;

				var MeshCameraPivotObjRatio_ = MeshCameraPivotObjAFCC_.RenderTexture_.width / MeshCameraPivotObjAFCC_.RenderTexture_.height;

				ViewerPanelObjWidth_ = Screen.width - leftPanelObjWidth_ - ScaledRightPanelWidth_ - ScaledTouchSurfaceSize_.x * 0.5f;
				ViewerPanelObjHeight_ = (float)Screen.height;
				var viewerPanelObjRatio_ = ViewerPanelObjWidth_ / ViewerPanelObjHeight_;

				var ratioCorrection_ = MeshCameraPivotObjRatio_ - viewerPanelObjRatio_;

				ViewerPanelObjXOffset_ = 0f;

				if (ratioCorrection_ >= 0f)
				{
					ViewerPanelObjXOffset_ = leftPanelObjWidth_;
					ViewerPanelObjHeight_ *= 1f - ratioCorrection_;
				}
				else
				{
					ViewerPanelObjXOffset_ = leftPanelObjWidth_ + (ViewerPanelObjWidth_ - ViewerPanelObjHeight_) * 0.5f;
					ViewerPanelObjWidth_ = ViewerPanelObjHeight_;
				}

				ViewerPanelObjYOffset_ = (Screen.height - ViewerPanelObjHeight_) * -0.5f;

				viewerPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ViewerPanelObjWidth_);
				viewerPanelObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ViewerPanelObjHeight_);
				viewerPanelObjRT_.anchoredPosition = new Vector2(ViewerPanelObjXOffset_, ViewerPanelObjYOffset_);

				//// Silder vertical
				var sliderVerticalBackgroundObj_ = SliderVerticalObj_.transform.Find("Background").gameObject;
				var sliderVerticalFillAreaObj_ = SliderVerticalObj_.transform.Find("FillArea").gameObject;
				var sliderVerticalHandleSlideAreaObj_ = SliderVerticalObj_.transform.Find("HandleSlideArea").gameObject;
				var sliderVerticalHandleObj_ = sliderVerticalHandleSlideAreaObj_.transform.Find("Handle").gameObject;

				var sliderVerticalObjRT_ = SliderVerticalObj_.GetComponent<RectTransform>();
				var sliderVerticalBackgroundObjRT_ = sliderVerticalBackgroundObj_.GetComponent<RectTransform>();
				var sliderVerticalHandleSlideAreaObjRT_ = sliderVerticalHandleSlideAreaObj_.GetComponent<RectTransform>();
				var sliderVerticalHandleObjRT_ = sliderVerticalHandleObj_.GetComponent<RectTransform>();

				sliderVerticalBackgroundObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);

				sliderVerticalObjRT_.offsetMin = new Vector2(0f, ScaledMarginSize_);
				sliderVerticalObjRT_.offsetMax = new Vector2(0f, ScaledMarginSize_ * -1f);
				sliderVerticalObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScaledTouchSurfaceSize_.x);

				sliderVerticalHandleSlideAreaObjRT_.offsetMin = new Vector2(0f, ScaledTouchSurfaceSize_.y * 0.5f);
				sliderVerticalHandleSlideAreaObjRT_.offsetMax = new Vector2(0f, ScaledTouchSurfaceSize_.y * -0.5f);

				sliderVerticalHandleObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScaledTouchSurfaceSize_.y * 0.5f);
			}

			DrawUI?.Invoke(scale_);
		}

		/// <summary>
		/// Force le système à dessiner l'interface.
		/// </summary>
		public void ForceDraw()
		{
			if (IsInitialized_)
			{
				var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
				canvasObjUICS_.InvokeChangedScaleEvent();
			}
		}

		public void Setup()
		{
			SetupTouch();

			SetupButton();

			IsInitialized_ = true;

			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += Draw;
			canvasObjUICS_.InvokeChangedScaleEvent();

			var uiObjectObj_ = AddObject(referenceMeshPrefab_, 0.5f, true, true, false, true, false);
			var uiObjectObjUIAFCO_ = uiObjectObj_.GetComponent<UIAugmentedFaceCreatorObject>();
			uiObjectObjUIAFCO_.AddLayer(referenceLayer_, true, true, false);

			// Ajoute les objets pré-enregistré en les masquants
			foreach (var objectPrefab_ in objectsPrefabs_)
			{
				AddObject(objectPrefab_, 0.25f, false, true, true, true, true);
			}

			Focus(uiObjectObjUIAFCO_.MeshWorkerObj_, uiObjectObjUIAFCO_.LayerWorkerObj_);
		}

		private void Awake()
		{
			Instance_ = this;
		}
	}
}
