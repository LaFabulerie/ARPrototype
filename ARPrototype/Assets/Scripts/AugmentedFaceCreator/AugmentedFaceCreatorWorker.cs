using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorWorker : MonoBehaviour
	{
		public GameObject LayerCameraPivotObj_ { get; private set; }
		private GameObject leftLightObj_;
		private GameObject rightLightObj_;
		private GameObject meshWorkersObj_;
		private Dictionary<string, GameObject> meshWorkersObjs_;
		private GameObject layerWorkersObj_;
		private Dictionary<string, GameObject> layerWorkersObjs_;

		private int subworkerCount_;

		private AugmentedFaceCreatorMeshWorker meshWorkerObjAFCMW_;
		private AugmentedFaceCreatorLayerWorker layerWorkerObjAFCLW_;

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
		private Color backgroundMeshCamera_ = Color.white;

		/// <summary>
		/// Couleur d'effacement de la caméra des calques.
		/// </summary>
		[SerializeField]
		private Color backgroundLayerCamera_ = Color.white;

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
		private float tempFloatB_;
		private float tempFloat_;

		/// <summary>
		/// Retourne l'objet contenant la caméra du maillage.
		/// </summary>
		public GameObject MeshCameraPivotObj_ { get; private set; }

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
		/// Compose le nom du travailleur de maillage à partir de l'index.
		/// </summary>
		/// <param name="index_">Index.</param>
		/// <returns>Nom.</returns>
		private string GetMeshWorkerName(int index_)
		{
			return $"MeshWorker-{index_:D3}";
		}

		/// <summary>
		/// Compose le nom du travailleur de calque à partir de l'index.
		/// </summary>
		/// <param name="index_">Index.</param>
		/// <returns>Nom.</returns>
		private string GetLayerWorkerName(int index_)
		{
			return $"LayerWorker-{index_:D3}";
		}

		/// <summary>
		/// Ajoute un travailleur de maillage associé à un travailleur de calque.
		/// </summary>
		/// <param name="meshPrefab_">Préfabriqué du maillage.</param>
		/// <param name="posRatio_">Ratio de la position.</param>
		/// <returns>Travailleur de maillage et travailleur de calque.</returns>
		public (int, GameObject, GameObject) AddSubWorker(GameObject meshPrefab_, float posRatio_)
		{
			if (subworkerCount_ >= 1000f)
			{
				return (-1, null, null);
			}

			// Travailleur de maillage
			var meshWorkerObj_ = Instantiate(meshWorkerPrefab_, meshWorkersObj_.transform);
			var workerObjAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();

			var meshWorkerName_ = GetMeshWorkerName(subworkerCount_);

			meshWorkerObj_.name = meshWorkerName_;

			workerObjAFCMW_.Initialize(MeshCameraPivotObj_, meshPrefab_, Vector3.forward, posRatio_);

			meshWorkersObjs_.Add(meshWorkerName_, meshWorkerObj_);

			// Travailleur de calque
			var layerWorkerObj_ = Instantiate(layerWorkerPrefab_, layerWorkersObj_.transform);
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			var layerCameraPivotObjAFCC_ = LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var layerWorkerName_ = GetLayerWorkerName(subworkerCount_);

			layerWorkerObj_.name = layerWorkerName_;

			layerWorkerObj_.transform.localPosition = Vector3.back * layerCameraPivotObjAFCC_.CameraObjC_.farClipPlane * subworkerCount_;

			layerWorkerObjAFCLW_.Initialize();

			layerWorkersObjs_.Add(layerWorkerName_, layerWorkerObj_);

			// Commun
			return (subworkerCount_++, meshWorkerObj_, layerWorkerObj_);
		}

		/// <summary>
		/// Suprime les travailleurs.
		/// </summary>
		/// <param name="meshWorkerName_">Nom du travailleur du maillage.</param>
		/// <param name="layerWorkerName_">Nom du travailleur de calque.</param>
		public void RemoveSubWorker(string meshWorkerName_, string layerWorkerName_)
		{
#if UNITY_EDITOR
			if (meshWorkersObjs_[meshWorkerName_] != null)
			{
				DestroyImmediate(meshWorkersObjs_[meshWorkerName_]);
			}

			if (layerWorkersObjs_[layerWorkerName_] != null)
			{
				DestroyImmediate(layerWorkersObjs_[layerWorkerName_]);
			}
#else
			if (meshWorkersObjs_[meshWorkerName_] != null)
			{
				Destroy(meshWorkersObjs_[meshWorkerName_]);
			}

			if (layerWorkersObjs_[layerWorkerName_] != null)
			{
				Destroy(layerWorkersObjs_[layerWorkerName_]);
			}
#endif

			meshWorkersObjs_.Remove(meshWorkerName_);
			layerWorkersObjs_.Remove(layerWorkerName_);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="meshWorkerObj_"></param>
		/// <param name="layerWorkerObj_"></param>
		/// <param name="supportMeshObject_"></param>
		/// <param name="texture_"></param>
		/// <returns></returns>
		public (int, GameObject) AddLayer(GameObject meshWorkerObj_, GameObject layerWorkerObj_, GameObject supportMeshObject_, Texture2D texture_)
		{
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
			layerWorkerObjAFCLW_.Width_ = texture_.width;
			layerWorkerObjAFCLW_.Height_ = texture_.height;

			SetLayerCameraPos(meshWorkerObj_, layerWorkerObj_);

			var (index_, layerObj_) = layerWorkerObjAFCLW_.AddLayer(supportMeshObject_, Vector3.zero);

			var layerObjAFCSM_ = layerObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

			layerObjAFCSM_.Initialize(LayerCameraPivotObj_);
			layerObjAFCSM_.ChangeTexture(texture_);

			return (index_, layerObj_);
		}

		/// <summary>
		/// Supprime le calque.
		/// </summary>
		public void RemoveLayer(GameObject meshWorkerObj_, GameObject layerWorkerObj_, GameObject layerObj_)
		{
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
			layerWorkerObjAFCLW_.RemoveLayer(layerObj_);
		}

		/// <summary>
		/// Déplace le calque vers l'avant.
		/// </summary>
		public void MoveForwardLayer(GameObject meshWorkerObj_, GameObject layerWorkerObj_, GameObject layerObj_)
		{
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
			layerWorkerObjAFCLW_.MoveForwardLayer(layerObj_);
		}

		/// <summary>
		/// Déplace le calque vers l'arrière.
		/// </summary>
		public void MoveBackwardLayer(GameObject meshWorkerObj_, GameObject layerWorkerObj_, GameObject layerObj_)
		{
			var layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();
			layerWorkerObjAFCLW_.MoveBackwardLayer(layerObj_);
		}

		/// <summary>
		/// Configure les lumières.
		/// </summary>
		public void SetupLights(AugmentedFaceCreatorMeshWorker afcmw_)
		{
			var leftLightObjL_ = leftLightObj_.GetComponent<Light>();
			var rightLightObjL_ = rightLightObj_.GetComponent<Light>();

			var a_ = afcmw_.PivotObj_.transform.localPosition.z;
			var b_ = Mathf.Sqrt(a_ * a_ * 2f) * 2f;

			leftLightObj_.transform.localPosition = Vector3.right * a_;
			rightLightObj_.transform.localPosition = Vector3.left * a_;

			leftLightObj_.transform.localEulerAngles = Vector3.up * -45f;
			rightLightObj_.transform.localEulerAngles = Vector3.up * 45f;

			leftLightObjL_.spotAngle = rightLightObjL_.spotAngle = 90f;
			leftLightObjL_.range = rightLightObjL_.range = b_;
		}

		public float GetZRatioOfMeshCamera()
		{
			return MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().GetZRatio();
		}

		public void TranslateMeshCameraOnZ(float zRatio_)
		{
			zRatio_ += tempFloat_;

			MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().TranslateOnZ(zRatio_);
		}

		public void SetupRotationMeshCameraOnY()
		{
			MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().SetupRotationXY();
		}

		public void RotateMeshCameraOnY(Vector2 rot_)
		{
			MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().RotateOnXY(rot_);
		}

		/// <summary>
		/// Change la position de la caméra de calque en fonction des travailleurs de maillage et de calque tout en redimentionant la caméra de calque à la dimension de l'objet.
		/// </summary>
		/// <param name="meshWorkerObj_">Travailleur de maillage.</param>
		/// <param name="layerWorkerObj_">Travailleur de calque.</param>
		/// <param name="width_">Largeur en pixel de la caméra de calque.</param>
		/// <param name="height_">Hauteur en pixel de la caméra de calque</param>
		public void SetLayerCameraPos(GameObject meshWorkerObj_, GameObject layerWorkerObj_)
		{
			Texture texture_ = null;

			if (meshWorkerObjAFCMW_ != null && layerWorkerObjAFCLW_ != null)
			{
				texture_ = layerWorkerObjAFCLW_.StopRecording(LayerCameraPivotObj_);
				meshWorkerObjAFCMW_.SetTexture(texture_);
			}

			meshWorkerObjAFCMW_ = meshWorkerObj_.GetComponent<AugmentedFaceCreatorMeshWorker>();
			layerWorkerObjAFCLW_ = layerWorkerObj_.GetComponent<AugmentedFaceCreatorLayerWorker>();

			texture_ = layerWorkerObjAFCLW_.PrepareRecording(LayerCameraPivotObj_, layerWorkerObjAFCLW_.Width_, layerWorkerObjAFCLW_.Height_);

			meshWorkerObjAFCMW_.SetTexture(texture_);
		}

		private void OnEnable()
		{
			MeshCameraPivotObj_ = transform.Find("MeshCamera-Pivot").gameObject;
			LayerCameraPivotObj_ = transform.Find("LayerCamera-Pivot").gameObject;
			leftLightObj_ = transform.Find("LeftLight").gameObject;
			rightLightObj_ = transform.Find("RightLight").gameObject;
			meshWorkersObj_ = transform.Find("MeshWorkers").gameObject;
			meshWorkersObjs_ = new Dictionary<string, GameObject>();
			layerWorkersObj_ = transform.Find("LayerWorkers").gameObject;
			layerWorkersObjs_ = new Dictionary<string, GameObject>();

			// Caméras
			var meshCameraPivotObjAFCC_ = MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
			var layerCameraPivotObjAFCC_ = LayerCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

			meshCameraPivotObjAFCC_.Initialize(textureWidth_, textureHeight_, backgroundMeshCamera_, 0.1f, 10f, Vector3.zero, Vector3.forward);
			layerCameraPivotObjAFCC_.Initialize(textureWidth_, textureHeight_, backgroundLayerCamera_, 0.1f, 10f, Vector3.zero, Vector3.back);

			// Commun
			IsInitialized_ = true;
		}
	}
}