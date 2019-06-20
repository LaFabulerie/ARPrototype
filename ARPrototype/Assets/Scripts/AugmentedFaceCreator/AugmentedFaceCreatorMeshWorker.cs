using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorMeshWorker : MonoBehaviour
	{
		private GameObject cameraObj_;

		/// <summary>
		/// Objet du pivot.
		/// </summary>
		public GameObject PivotObj_ { get; private set; }

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
				var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();
				var cameraObjC_ = cameraObjAFCC_.CameraObjC_;
				var depthRange_ = cameraObjC_.farClipPlane - cameraObjC_.nearClipPlane;
				return depthRange_;
			}
		}

		/// <summary>
		/// Ratio de la position du maillage pa rapport à l'étendu de la profondeur. 
		/// </summary>
		public float PosRatio_
		{
			get
			{
				var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

				var min_ = cameraObjAFCC_.CameraObjC_.nearClipPlane * 1.001f;

				return (PivotObj_.transform.localPosition.z - min_) / (DepthRange_ * 0.999f);
			}
			set
			{
				var posRatio_ = Mathf.Clamp01(value);

				var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

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

		public List<string> GetAnchorNames_()
		{
			var anchorsObjs_ = GetAnchorObjs();

			return (from anchorsObj_ in anchorsObjs_
					select anchorsObj_.name).ToList();
		}

		public Vector3 GetPositionOffset(GameObject anchorsObj_)
		{
			var referencePivot_ = anchorsObj_.transform.parent.parent.parent;

			var to_ = anchorsObj_.transform.localPosition;
			to_.x *= referencePivot_.transform.localScale.x;
			to_.y *= referencePivot_.transform.localScale.y;
			to_.z *= referencePivot_.transform.localScale.z;
			to_ += referencePivot_.localPosition;

			return to_ - PivotObj_.transform.localPosition;
		}

		public Vector2 GetDim()
		{
			var meshObjAFCSM_ = MeshObj_.GetComponent<AugmentedFaceCreatorSurfaceMesh>();

			if (meshObjAFCSM_ == null)
			{
				return Vector2.zero;
			}

			return meshObjAFCSM_.Size_;
		}

		/// <summary>
		/// Retourne la position de l'objet sur le plan XY.
		/// </summary>
		/// <returns>Position sur le plan XY.</returns>
		public Vector2 GetPositionXY()
		{
			return PivotObj_.transform.position;
		}

		/// <summary>
		/// Retourne l'angle de l'objet sur le plan XY.
		/// </summary>
		/// <returns>Angle sur le plan XY.</returns>
		public Quaternion GetRotation()
		{
			return PivotObj_.transform.localRotation;
		}

		/// <summary>
		/// Retourne l'échelle de l'objet sur le plan XY.
		/// </summary>
		/// <returns>Angle sur le plan XY.</returns>
		public Vector2 GetScale()
		{
			return PivotObj_.transform.localScale;
		}

		/// <summary>
		/// Initialise.
		/// </summary>
		/// <param name="isPermanent_">Indique si l'objet restera permanent.</param>
		/// <param name="cameraObj_">Objet possédant la caméra.</param>
		/// <param name="meshObj_">Objet possédant le maillage.</param>
		/// <param name="posRatio_">Ratio de la position.</param>
		public void Initialize(GameObject cameraObj_, GameObject meshObj_, Vector3 localDir_, float posRatio_)
		{
			this.cameraObj_ = cameraObj_;

			MeshObj_ = Instantiate(meshObj_, PivotObj_.transform);
			MeshObj_.transform.localPosition = Vector3.zero;
			MeshObj_.transform.localRotation = Quaternion.LookRotation(localDir_, Vector3.up);
			MeshObj_.transform.localScale = Vector3.one;
			MeshObj_.name = "Mesh";

			PosRatio_ = posRatio_;

			IsInitialized_ = true;
		}

		/// <summary>
		/// Met le maillage à l'échelle des limites de la caméra.
		/// </summary>
		public void ScaleMeshToCameraBounds()
		{
			var meshObjMC_ = MeshObj_.GetComponent<MeshCollider>();

			var meshMin_ = meshObjMC_.bounds.min;
			var meshMax_ = meshObjMC_.bounds.max;

			var cameraMin_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>().MinimumBounds_;
			var cameraMax_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>().MaximumBounds_;


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

			var scale_ = Mathf.Min(minRatio_.x, minRatio_.y, maxRatio_.x, maxRatio_.y);

			Debug.Log($"{cameraMax_} {cameraMin_} {meshMin_} {meshMax_} {minRatio_} {maxRatio_}");

			PivotObj_.transform.localScale *= scale_;

			var xOffset_ = (Mathf.Abs(meshMax_.x) - Mathf.Abs(meshMin_.x)) * 0.5f * scale_;
			var yOffset_ = (Mathf.Abs(meshMax_.y) - Mathf.Abs(meshMin_.y)) * 0.5f * scale_;

			PivotObj_.transform.localPosition += Vector3.left * xOffset_ + Vector3.down * yOffset_;
		}

		/// <summary>
		/// Définit la position du maillage sur le plan XY.
		/// </summary>
		/// <param name="x_">Position de x dans l'espace mondial.</param>
		/// <param name="y_">Position de y dans l'espace mondial.</param>
		public void SetPositionXY(float x_, float y_)
		{
			PivotObj_.transform.localPosition = new Vector3(x_, y_, PivotObj_.transform.localPosition.z);
		}

		/// <summary>
		/// Définit la rotation du maillage sur le plan XY.
		/// </summary>
		/// <param name="rotation_">Rotation.</param>
		public void SetRotation(Quaternion rotation_)
		{
			PivotObj_.transform.localRotation = rotation_;
		}

		/// <summary>
		/// Définit la mise à l'échelle du maillage sur le plan XY.
		/// </summary>
		/// <param name="scale_">Echelle.</param>
		public void SetScale(Vector2 scale_)
		{
			PivotObj_.transform.localScale = new Vector3(scale_.x, scale_.y, PivotObj_.transform.localScale.z);
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

		/// <summary>
		/// Masque le maillage de l'objet.
		/// </summary>
		public void Hide()
		{
			PivotObj_.SetActive(false);
		}

		/// <summary>
		/// Montre le maillage de l'objet.
		/// </summary>
		public void Show()
		{
			PivotObj_.SetActive(true);
		}

		private void OnEnable()
		{
			PivotObj_ = transform.Find("Pivot").gameObject;
		}
	}
}
