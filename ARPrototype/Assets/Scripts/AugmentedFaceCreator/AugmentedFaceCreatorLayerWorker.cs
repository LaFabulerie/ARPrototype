using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorLayerWorker : MonoBehaviour
	{
		// Objets enfants
		private GameObject layersObj_;

		/// <summary>
		/// Nombre de calque initialisé.
		/// </summary>
		private int layerCount_;

		/// <summary>
		/// Objet de caméra en cours.
		/// </summary>
		private GameObject cameraObj_;

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
		public int MaxLayer_ { get; } = 20;

		/// <summary>
		/// Largeur de la texture.
		/// </summary>
		public int Width_ { get; set; }

		/// <summary>
		/// Hauteur de la texture.
		/// </summary>
		public int Height_ { get; set; }

		/// <summary>
		/// Intialise le composant.
		/// </summary>
		public void Initialize()
		{
			IsInitialized_ = true;
		}

		/// <summary>
		/// Prépare l'enregistrement.
		/// </summary>
		/// <param name="cameraObj_">Objet</param>
		/// <param name="width_">Largeur de la texture</param>
		/// <param name="height_">Jauteur de la texture</param>
		public RenderTexture PrepareRecording(GameObject cameraObj_, int width_, int height_)
		{
			if (!IsInitialized_)
			{
				return null;
			}

			Width_ = width_;
			Height_ = height_;

			IsRecording_ = true;

			this.cameraObj_ = cameraObj_;

			this.cameraObj_.transform.position = transform.position;

			var cameraObjAFCC_ = this.cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

			cameraObjAFCC_.SetRenderTexture(width_, height_);

			Texture_ = new Texture2D(width_, height_, TextureFormat.ARGB32, false, false);

			return cameraObjAFCC_.RenderTexture_;
		}

		/// <summary>
		/// Sauvegarde la texture.
		/// </summary>
		/// <param name="aflc_">Composant</param>
		private void SaveToTexture(AugmentedFaceCreatorCamera aflc_)
		{
			var current_ = RenderTexture.active;

			RenderTexture.active = aflc_.RenderTexture_;

			Texture_.ReadPixels(new Rect(0f, 0f, aflc_.RenderTexture_.width, aflc_.RenderTexture_.height), 0, 0, false);
			Texture_.Apply();

			RenderTexture.active = current_;
		}

		/// <summary>
		/// Arrête l'enregistrement.
		/// </summary>
		public Texture2D StopRecording(GameObject camObj_)
		{
			var camObjAFLC_ = camObj_.GetComponent<AugmentedFaceCreatorCamera>();

			SaveToTexture(camObjAFLC_);

			IsRecording_ = false;

			return Texture_;
		}

		public Vector2 GetPositionXY(GameObject layerObj_)
		{
			return layerObj_.transform.localPosition;
		}

		/// <summary>
		/// Définit la position du calque sur le plan XY.
		/// </summary>
		/// <param name="layerObj_">Calque à déplacer.</param>
		/// <param name="localPos_">Nouvelle position.</param>
		public void SetPositionXY(GameObject layerObj_, Vector2 localPos_)
		{
			if (cameraObj_ == null)
			{
				return;
			}

			var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

			localPos_.x = Mathf.Clamp(localPos_.x, Mathf.Min(cameraObjAFCC_.MinimumBounds_.x, cameraObjAFCC_.MaximumBounds_.x), Mathf.Max(cameraObjAFCC_.MinimumBounds_.x, cameraObjAFCC_.MaximumBounds_.x));

			localPos_.y = Mathf.Clamp(localPos_.y, Mathf.Min(cameraObjAFCC_.MinimumBounds_.y, cameraObjAFCC_.MaximumBounds_.y), Mathf.Max(cameraObjAFCC_.MinimumBounds_.y, cameraObjAFCC_.MaximumBounds_.y));

			layerObj_.transform.localPosition = new Vector3(localPos_.x, localPos_.y, layerObj_.transform.localPosition.z);
		}

		/// <summary>
		/// Retourne l'angle de l'objet sur le plan XY.
		/// </summary>
		/// <returns>Angle sur le plan XY.</returns>
		public Quaternion GetRotation(GameObject layerObj_)
		{
			return layerObj_.transform.localRotation;
		}

		/// <summary>
		/// Définit la rotation du maillage sur le plan XY.
		/// </summary>
		/// <param name="rotation_">Rotation.</param>
		public void SetRotation(GameObject layerObj_, Quaternion rotation_)
		{
			layerObj_.transform.localRotation = rotation_;
		}

		/// <summary>
		/// Définit la position du calque sur l'axe Z.
		/// </summary>
		/// <param name="layerObj_">Calque à déplacer.</param>
		public void SetPositionOnZ(GameObject layerObj_)
		{
			var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var distance_ = cameraObjAFCC_.MaximumBounds_.z - cameraObjAFCC_.MinimumBounds_.z;
			var unit_ = Mathf.Abs(distance_ / MaxLayer_);

			var newZ_ = ((layerObj_.transform.GetSiblingIndex() + 1) * unit_ + cameraObjAFCC_.CameraObjC_.nearClipPlane) * -1f;

			layerObj_.transform.localPosition = new Vector3(layerObj_.transform.localPosition.x, layerObj_.transform.localPosition.y, newZ_);
		}

		/// <summary>
		/// Retourne l'échelle de l'objet sur le plan XY.
		/// </summary>
		/// <returns>Angle sur le plan XY.</returns>
		public Vector2 GetScale(GameObject layerObj_)
		{
			return layerObj_.transform.localScale;
		}

		/// <summary>
		/// Définit la mise à l'échelle du maillage sur le plan XY.
		/// </summary>
		/// <param name="scale_">Echelle.</param>
		public void SetScale(GameObject layerObj_, Vector2 scale_)
		{
			layerObj_.transform.localScale = new Vector3(scale_.x, scale_.y, layerObj_.transform.localScale.z);
		}

		/// <summary>
		/// Ajoute le calque au travailleur.
		/// </summary>
		/// <param name="supportMeshObject_">Maillage de support de calque.</param>
		/// <param name="localPos_">Position locale du calque</param>
		/// <returns></returns>
		public (int, GameObject) AddLayer(GameObject supportMeshObject_, Vector3 localPos_)
		{
			var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var newLayerObj_ = Instantiate(supportMeshObject_, Vector3.zero, Quaternion.identity, layersObj_.transform);

			newLayerObj_.name = $"Layer-{layerCount_:D3}";

			SetPositionXY(newLayerObj_, new Vector2(cameraObjAFCC_.transform.localPosition.x, cameraObjAFCC_.transform.localPosition.y));

			SetPositionOnZ(newLayerObj_);

			return (layerCount_++, newLayerObj_);
		}

		/// <summary>
		/// Supprime le calque du travailleur.
		/// </summary>
		/// <param name="layerWorkerObj_">Calque à supprimer.</param>
		public void RemoveLayer(GameObject layerWorkerObj_)
		{
#if UNITY_EDITOR
			DestroyImmediate(layerWorkerObj_);
#else
			Destroy(layerWorkerObj_);
#endif
		}

		/// <summary>
		/// Avant le calque vers l'avant d'une unité.
		/// </summary>
		/// <param name="layerObj_">Calque à avancer.</param>
		public void MoveForwardLayer(GameObject layerObj_)
		{
			var currentIndex_ = layerObj_.transform.GetSiblingIndex();
			var newIndex_ = Mathf.Clamp(currentIndex_ - 1, 0, layersObj_.transform.childCount - 1);

			var currentPos_ = layerObj_.transform.localPosition;
			var newPos_ = layersObj_.transform.GetChild(newIndex_).localPosition;

			layersObj_.transform.GetChild(newIndex_).localPosition = new Vector3(newPos_.x, newPos_.y, currentPos_.z);
			layerObj_.transform.localPosition = new Vector3(currentPos_.x, currentPos_.y, newPos_.z);

			layersObj_.transform.GetChild(newIndex_).SetSiblingIndex(currentIndex_);
			layerObj_.transform.SetSiblingIndex(newIndex_);
		}

		/// <summary>
		/// Recule le calque vers l'arrière d'une unité.
		/// </summary>
		/// <param name="layerWorkerObj_">Calque à reculer.</param>
		public void MoveBackwardLayer(GameObject layerObj_)
		{
			var currentIndex_ = layerObj_.transform.GetSiblingIndex();
			var newIndex_ = Mathf.Clamp(currentIndex_ + 1, 0, layersObj_.transform.childCount - 1);

			var currentPos_ = layerObj_.transform.localPosition;
			var newPos_ = layersObj_.transform.GetChild(newIndex_).localPosition;

			layersObj_.transform.GetChild(newIndex_).localPosition = new Vector3(newPos_.x, newPos_.y, currentPos_.z);
			layerObj_.transform.localPosition = new Vector3(currentPos_.x, currentPos_.y, newPos_.z);

			layersObj_.transform.GetChild(newIndex_).SetSiblingIndex(currentIndex_);
			layerObj_.transform.SetSiblingIndex(newIndex_);
		}

		private void OnEnable()
		{
			layersObj_ = transform.Find("Layers").gameObject;
		}
	}
}
