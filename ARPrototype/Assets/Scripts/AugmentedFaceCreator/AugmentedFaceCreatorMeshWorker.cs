using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorMeshWorker : MonoBehaviour
	{
		/// <summary>
		/// Gestionnaire de travailleur.
		/// </summary>
		public GameObject WorkerObj_
		{
			get => GameObject.Find("Worker");
		}

		/// <summary>
		/// Objet du pivot.
		/// </summary>
		public GameObject PivotObj_ => transform.Find("Pivot").gameObject;

		/// <summary>
		/// Objet du maillage.
		/// </summary>
		public GameObject MeshObj_ { get; private set; }

		/// <summary>
		/// Indique si l'enregistrement est en cours.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Etendu de profondeur dans laquelle les maillages peuvent être situer.
		/// </summary>
		public float DepthRange_
		{
			get
			{
				var cameraObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();
				return cameraObjAFCC_.CameraObjC_.farClipPlane - cameraObjAFCC_.CameraObjC_.nearClipPlane;
			}
		}

		/// <summary>
		/// Ratio de la position du maillage pa rapport à l'étendu de la profondeur. 
		/// </summary>
		public float PosRatio_
		{
			get
			{
				var cameraObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

				var min_ = cameraObjAFCC_.CameraObjC_.nearClipPlane * 1.001f;

				return (PivotObj_.transform.localPosition.z - min_) / (DepthRange_ * 0.999f);
			}
			set
			{
				var posRatio_ = Mathf.Clamp01(value);

				var cameraObjAFCC_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>();

				PivotObj_.transform.localPosition = Vector3.forward * (cameraObjAFCC_.CameraObjC_.nearClipPlane * 1.001f + DepthRange_ * 0.999f * posRatio_);
			}
		}

		/// <summary>
		/// Indique si le maillage est masqué ou pas.
		/// </summary>
		public bool IsHide
		{
			get => !PivotObj_.activeSelf;
		}

		/// <summary>
		/// Retourne les objets des ancres.
		/// </summary>
		/// <returns>Liste des objets des ancres.</returns>
		public List<GameObject> GetAnchorObjs()
		{
			var anchorsObj_ = MeshObj_.transform.Find("Anchors");

			var anchorsObjs_ = new List<GameObject>();
			for (int i_ = 0; i_ < anchorsObj_.childCount; i_++)
			{
				anchorsObjs_.Add(anchorsObj_.GetChild(i_).gameObject);
			}

			return anchorsObjs_;
		}

		/// <summary>
		/// Retourne les noms des ancres.
		/// </summary>
		/// <returns>Liste des noms des ancres.</returns>
		public List<string> GetAnchorNames_()
		{
			var anchorsObjs_ = GetAnchorObjs();

			return (from anchorsObj_ in anchorsObjs_
					select anchorsObj_.name).ToList();
		}

		/// <summary>
		/// Retourne la position par rapport à une ancre.
		/// </summary>
		/// <param name="anchorObj_">Objet portant l'ancre.</param>
		/// <returns>Décalage par rapport à l'objet.</returns>
		public Vector3 GetPositionOffset(GameObject anchorObj_)
		{
			var referencePivot_ = anchorObj_.transform.parent.parent.parent;

			var to_ = anchorObj_.transform.localPosition;
			to_.x *= referencePivot_.transform.localScale.x;
			to_.y *= referencePivot_.transform.localScale.y;
			to_.z *= referencePivot_.transform.localScale.z;
			to_ += referencePivot_.localPosition;

			return to_ - PivotObj_.transform.localPosition;
		}

		/// <summary>
		/// Initialise.
		/// </summary>
		/// <param name="meshObj_">Objet portant le maillage.</param>
		/// <param name="dir_">Objet possédant le maillage.</param>
		/// <param name="zRatio_">Ratio de la position.</param>
		public void Initialize(GameObject meshObj_, float zRatio_)
		{
			name = $"MeshWorker-{transform.GetSiblingIndex():D3}";

			MeshObj_ = Instantiate(meshObj_, Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up), PivotObj_.transform);
			MeshObj_.name = "Mesh";

			PosRatio_ = zRatio_;

			IsInitialized_ = true;
		}

		/// <summary>
		/// Met le maillage à l'échelle des limites de la caméra.
		/// </summary>
		public void ScaleMeshToCameraBounds()
		{
			var meshObjMC_ = MeshObj_.GetComponent<MeshCollider>();

			meshObjMC_.sharedMesh.RecalculateBounds();

			var meshMin_ = meshObjMC_.bounds.min;
			var meshMax_ = meshObjMC_.bounds.max;

			var cameraMin_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().MinimumBounds_;
			var cameraMax_ = AugmentedFaceCreatorWorker.MeshCameraPivotObj_.GetComponent<AugmentedFaceCreatorCamera>().MaximumBounds_;

			if (meshMin_.x <= 0 && meshMin_.y <= 0 &&
				meshMax_.x <= 0 && meshMax_.y <= 0)
			{
				Debug.LogError($"Verify the integrity of {MeshObj_}");
			}

			var minRatio_ = new Vector2
				(
					cameraMin_.x / meshMin_.x,
					cameraMin_.y / meshMin_.y
				);

			var maxRatio_ = new Vector2
				(
					cameraMax_.x / meshMax_.x,
					cameraMax_.y / meshMax_.y
				);

			var scale_ = Mathf.Abs(Mathf.Min(minRatio_.x, minRatio_.y, maxRatio_.x, maxRatio_.y));

			PivotObj_.transform.localScale *= scale_;

			var xOffset_ = (Mathf.Abs(meshMax_.x) - Mathf.Abs(meshMin_.x)) * 0.5f * scale_;
			var yOffset_ = (Mathf.Abs(meshMax_.y) - Mathf.Abs(meshMin_.y)) * 0.5f * scale_;

			PivotObj_.transform.localPosition += Vector3.left * xOffset_ + Vector3.down * yOffset_;
		}

		/// <summary>
		/// Définit la texture sur le maillage. 
		/// </summary>
		/// <param name="texture_">Texture à appliquer.</param>
		public void SetTexture(Texture texture_)
		{
			var mr_ = MeshObj_.GetComponent<MeshRenderer>();
			mr_.material.mainTexture = texture_;
		}
	}
}
