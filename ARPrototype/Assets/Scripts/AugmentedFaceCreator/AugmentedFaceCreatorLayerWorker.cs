using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorLayerWorker : MonoBehaviour
	{
		/// <summary>
		/// Gestionnaire de travailleur.
		/// </summary>
		public GameObject WorkerObj_
		{
			get => GameObject.Find("Worker");
		}

		/// <summary>
		/// Contenant des calques.
		/// </summary>
		public GameObject LayersObj_ => transform.Find("Layers").gameObject;

		/// <summary>
		/// Indique si l'enregistrement est en cours.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Indique si l'enregistrement est en cours.
		/// </summary>
		public bool IsRecording_ { get; private set; }

		/// <summary>
		/// Texture de sortie.
		/// </summary>
		public Texture2D Texture_ { get; private set; }

		/// <summary>
		/// Nombre de calque maximum.
		/// </summary>
		public int MaxLayer_ { get; } = 100;

		/// <summary>
		/// Nombre de calque maximum.
		/// </summary>
		public float LayerOffset_ => AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().Depth_ / MaxLayer_;

		/// <summary>
		/// Largeur de la texture de sortie.
		/// </summary>
		public int Width_ { get; set; }

		/// <summary>
		/// Hauteur de la texture de sortie.
		/// </summary>
		public int Height_ { get; set; }

		/// <summary>
		/// Peut ajouter de nouveau calque.
		/// </summary>
		public bool CanAddLayer_ => LayersObj_.transform.childCount < MaxLayer_;

		/// <summary>
		/// Intialise le composant.
		/// </summary>
		/// <param name="width_">Largeur de la texture de sortie</param>
		/// <param name="height_">hauteur de la texture de sortie</param>
		public void Initialize(int width_, int height_)
		{
			Width_ = width_;
			Height_ = height_;
			var layerCameraPivotObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			name = $"LayerWorker-{transform.GetSiblingIndex():D3}";
			Texture_ = new Texture2D(Width_, Height_, TextureFormat.ARGB32, false, false);

			layerCameraPivotObjAFCC_.SetRenderTexture(Width_, Height_);

			IsInitialized_ = true;
		}

		public Texture2D Save()
		{
			IsRecording_ = true;

			var cameraObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			AugmentedFaceCreatorWorker.LayerCameraPivotObj_.transform.position = transform.position;

			var cullingMask_ = cameraObjAFCC_.CameraObjC_.cullingMask;
			cameraObjAFCC_.CameraObjC_.cullingMask = 1 << 0;

			cameraObjAFCC_.CameraObjC_.Render();

			Save(cameraObjAFCC_.RenderTexture_);

			cameraObjAFCC_.CameraObjC_.cullingMask = cullingMask_;

			IsRecording_ = false;

			return Texture_;
		}

		/// <summary>
		/// Prépare l'enregistrement.
		/// </summary>
		/// <param name="cameraObj_">Caméra de calque</param>
		public RenderTexture StartRecording()
		{
			if (!IsInitialized_)
			{
				return null;
			}

			IsRecording_ = true;

			AugmentedFaceCreatorWorker.LayerCameraPivotObj_.transform.position = transform.position;

			var cameraObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			return cameraObjAFCC_.RenderTexture_;
		}

		/// <summary>
		/// Sauvegarde la texture.
		/// </summary>
		private void Save(RenderTexture texture_)
		{
			var current_ = RenderTexture.active;

			RenderTexture.active = texture_;
			Texture_.ReadPixels(new Rect(0f, 0f, texture_.width, texture_.height), 0, 0, false);
			Texture_.Apply();

			RenderTexture.active = current_;
		}

		/// <summary>
		/// Arrête l'enregistrement.
		/// </summary>
		public Texture2D StopRecording()
		{
			var cameraObjAFCC_ = AugmentedFaceCreatorWorker.LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			Save(cameraObjAFCC_.RenderTexture_);

			IsRecording_ = false;

			return Texture_;
		}

		/// <summary>
		/// Ajoute le calque au travailleur.
		/// </summary>
		/// <param name="layerSupportObj_">Maillage de support de calque.</param>
		/// <returns>Calque créé</returns>
		public GameObject AddLayer(GameObject layerSupportObj_, Texture2D texture_)
		{
			if (!CanAddLayer_)
			{
				return null;
			}

			var layerObj_ = Instantiate(layerSupportObj_, Vector3.zero, Quaternion.identity, LayersObj_.transform);
			layerObj_.name = $"Layer-{layerObj_.transform.GetSiblingIndex():D3}";

			var layerObjAFCSM_ = layerObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

			layerObjAFCSM_.Initialize(AugmentedFaceCreatorWorker.LayerCameraPivotObj_);
			layerObjAFCSM_.ChangeTexture(texture_);

			return layerObj_;
		}

		/// <summary>
		/// Supprime le calque du travailleur.
		/// </summary>
		/// <param name="layerWorkerObj_">Travailleur de calque à supprimer.</param>
		public void Remove()
		{
#if UNITY_EDITOR
			DestroyImmediate(gameObject);
#else
			Destroy(gameObject);
#endif
		}
	}
}
