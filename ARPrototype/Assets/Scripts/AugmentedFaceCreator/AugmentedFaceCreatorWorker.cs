using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorWorker : MonoBehaviour
	{
		/// <summary>
		/// Largeur des textures de travail.
		/// </summary>
		[SerializeField]
		private int textureWidth_ = 1024;

		/// <summary>
		/// Hauteur des textures de travail.
		/// </summary>
		[SerializeField]
		private int textureHeight_ = 1024;

		/// <summary>
		/// Couleur d'effacement de la caméra du maillage.
		/// </summary>
		[SerializeField]
		private Color backgroundMeshCamera_ = Color.clear;

		/// <summary>
		/// Couleur d'effacement de la caméra des calques.
		/// </summary>
		[SerializeField]
		private Color backgroundLayerCamera_ = Color.clear;

		/// <summary>
		/// Préfabriqué du travailleur de maillage.
		/// </summary>
		[SerializeField]
		private GameObject meshWorkerPrefab_;

		/// <summary>
		/// Préfabriqué du travailleur de calque.
		/// </summary>
		[SerializeField]
		private GameObject layerWorkerPrefab_;

		public static AugmentedFaceCreatorWorker Instance_ { get; private set; }

		/// <summary>
		/// Canvas.
		/// </summary>
		public static GameObject CanvasObj_ => GameObject.Find("Canvas");

		/// <summary>
		/// Vue en mode paysage.
		/// </summary>
		public static GameObject BodyLandscapeObj_ => CanvasObj_.transform.Find("Body").Find("Body-Landscape").gameObject;

		/// <summary>
		/// Racine du pivot de la caméra de maillage.
		/// </summary>
		public static GameObject MeshCameraPivotObj_ => Instance_.transform.Find("MeshCamera-Pivot").gameObject;

		/// <summary>
		/// Racine du pivot de la caméra de calque.
		/// </summary>
		public static GameObject LayerCameraPivotObj_ => Instance_.transform.Find("LayerCamera-Pivot").gameObject;

		/// <summary>
		/// Lumière gauche.
		/// </summary>
		public static GameObject LeftLightObj_ => Instance_.transform.Find("LeftLight").gameObject;

		/// <summary>
		/// LUmière droite.
		/// </summary>
		public static GameObject RightLightObj_ => Instance_.transform.Find("RightLight").gameObject;

		/// <summary>
		/// Contenant des travailleurs de maillage.
		/// </summary>
		public static GameObject MeshWorkersObj_ => Instance_.transform.Find("MeshWorkers").gameObject;

		/// <summary>
		/// Contenant des travailleurs de calque.
		/// </summary>
		public static GameObject LayerWorkersObj_ => Instance_.transform.Find("LayerWorkers").gameObject;

		/// <summary>
		/// Indique si l'objet est initialisé.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Largeur des textures de travail.
		/// </summary>
		public int TextureWidth_ { get => textureWidth_; }

		/// <summary>
		/// Hauteur des textures de travail.
		/// </summary>
		public int TextureHeight_ { get => textureHeight_; }

		/// <summary>
		/// 
		/// </summary>
		public int SubWorkerCount
		{
			get
			{
				return MeshWorkersObj_.transform.childCount;
			}
		}

		/// <summary>
		/// Peut ajouter un sous traitant.
		/// </summary>
		public bool CanAddSubWorker
		{
			get
			{
				return SubWorkerCount < 1000;
			}
		}

		/// <summary>
		/// Ajoute un travailleur de maillage associé à un travailleur de calque.
		/// </summary>
		/// <param name="meshPrefab_">Préfabriqué du maillage.</param>
		/// <param name="zRatio_">Ratio de la position.</param>
		/// <returns>Travailleur de maillage et travailleur de calque.</returns>
		public (GameObject, GameObject) AddSubWorker(GameObject meshPrefab_, float zRatio_, int width_, int height_)
		{
			if (!CanAddSubWorker)
			{
				return (null, null);
			}

			// Travailleur de maillage
			var meshWorkerObj_ = Instantiate(meshWorkerPrefab_, Vector3.zero, Quaternion.identity, MeshWorkersObj_.transform);
			var workerObjAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			workerObjAFCMW_.Initialize(meshPrefab_, zRatio_);

			// Travailleur de calque
			var layerCameraPivotObjAFCC_ = LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var layerWorkerObj_ = Instantiate(layerWorkerPrefab_, layerCameraPivotObjAFCC_.Forward_ * (layerCameraPivotObjAFCC_.ClippingPlane_.far_ * LayerWorkersObj_.transform.childCount), Quaternion.identity, LayerWorkersObj_.transform);
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
			layerWorkerObjAFCLW_.Initialize(width_, height_);

			// Commun
			return (meshWorkerObj_, layerWorkerObj_);
		}

		/// <summary>
		/// Suprime les travailleurs.
		/// </summary>
		/// <param name="meshWorker_">Travailleur du maillage.</param>
		/// <param name="layerWorker_">Travailleur de calque.</param>
		public void RemoveSubWorker(GameObject meshWorker_, GameObject layerWorker_)
		{
#if UNITY_EDITOR
			if (meshWorker_ != null)
			{
				DestroyImmediate(meshWorker_);
			}

			if (layerWorker_ != null)
			{
				DestroyImmediate(layerWorker_);
			}
#else
			if (meshWorker_ != null)
			{
				Destroy(meshWorker_);
			}

			if (layerWorker_ != null)
			{
				Destroy(layerWorker_);
			}
#endif
		}

		/// <summary>
		/// Configure les lumières.
		/// </summary>
		public void SetupLights(GameObject meshWorkerObj_)
		{
			var meshWorkerObjAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

			var leftLightObjL_ = LeftLightObj_.GetComponent<Light>();
			var rightLightObjL_ = RightLightObj_.GetComponent<Light>();

			var a_ = meshWorkerObjAFCMW_.PivotObj_.transform.localPosition.z;
			var b_ = Mathf.Sqrt(a_ * a_ * 2f) * 2f;

			LeftLightObj_.transform.localPosition = Vector3.right * a_;
			RightLightObj_.transform.localPosition = Vector3.left * a_;

			LeftLightObj_.transform.localEulerAngles = Vector3.up * -45f;
			RightLightObj_.transform.localEulerAngles = Vector3.up * 45f;

			leftLightObjL_.spotAngle = rightLightObjL_.spotAngle = 90f;
			leftLightObjL_.range = rightLightObjL_.range = b_;
		}

		private void Awake()
		{
			Instance_ = this;
		}

		private void Start()
		{
			var meshCameraPivotObjAFCC_ = MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var layerCameraPivotObjAFCC_ = LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			meshCameraPivotObjAFCC_.Initialize(textureWidth_, textureHeight_, backgroundMeshCamera_, 0.1f, 5f, Vector3.zero, Vector3.forward);
			layerCameraPivotObjAFCC_.Initialize(textureWidth_, textureHeight_, backgroundLayerCamera_, 0.1f, 10f, Vector3.zero, Vector3.back);

			IsInitialized_ = true;

			var bodyLandscapeAFCBL_ = BodyLandscapeObj_.GetComponent<UIAugmentedFaceCreatorBodyLandscape>();
			bodyLandscapeAFCBL_.Setup();
		}
	}
}