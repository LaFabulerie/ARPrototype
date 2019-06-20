using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorCamera : MonoBehaviour
	{
		// Objets enfants
		private GameObject yawObj_;
		private GameObject pitchObj_;
		private GameObject cameraObj_;

		private Quaternion tempQuaternion1_;
		private Quaternion tempQuaternion2_;

		/// <summary>
		/// Indique si l'enregistrement est en cours.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Sortie de la caméra.
		/// </summary>
		public RenderTexture RenderTexture_ { get; private set; }

		/// <summary>
		/// Limites minimum de la caméra dans l'espace mondial.
		/// </summary>
		public Vector3 MinimumBounds_ { get; private set; }

		/// <summary>
		/// Limites maximum de la caméra dans l'espace mondial.
		/// </summary>
		public Vector3 MaximumBounds_ { get; private set; }

		/// <summary>
		/// Limites du cadre de la caméra dans l'espace mondial.
		/// </summary>
		public Rect RectBounds_ { get; private set; }

		/// <summary>
		/// Composant caméra.
		/// </summary>
		public Camera CameraObjC_
		{
			get => cameraObj_.GetComponent<Camera>();
		}

		/// <summary>
		///Initialise et définit la sortie de la caméra sur la texture de rendu.
		/// </summary>
		/// <param name="width_">Largeur de la texture de sortie</param>
		/// <param name="height_">Hauteur de la texture de sortie</param>
		/// <param name="depth_">Profondeur de la texture de sortie</param>
		public void SetRenderTexture(int width_, int height_)
		{
			if (RenderTexture_ != null)
			{
				RenderTexture_.Release();
			}

			RenderTexture_ = new RenderTexture(width_, height_, 24);

			CameraObjC_.targetTexture = RenderTexture_;
			CameraObjC_.forceIntoRenderTexture = true;

			ComputeBounds();
		}

		/// <summary>
		/// Définit le mode orthographique.
		/// </summary>
		private void SetOrthographicMode()
		{
			CameraObjC_.orthographic = true;
			CameraObjC_.orthographicSize = 0.5f;
		}

		/// <summary>
		/// Définit la couleur d'effacement.
		/// </summary>
		/// <param name="color_">Couleur d'effacement</param>
		private void SetBackground(Color color_)
		{
			CameraObjC_.clearFlags = CameraClearFlags.SolidColor;
			CameraObjC_.backgroundColor = color_;
		}

		/// <summary>
		/// Définit les distances de coupures.
		/// </summary>
		/// <param name="near_">Distance de coupure proche</param>
		/// <param name="far_">Distance de coupure éloigné</param>
		private void SetClippingPlanes(float near_, float far_)
		{
			CameraObjC_.nearClipPlane = near_;
			CameraObjC_.farClipPlane = far_;
		}

		/// <summary>
		/// Calcul les limites.
		/// </summary>
		private void ComputeBounds()
		{
			MinimumBounds_ = CameraObjC_.ViewportToWorldPoint(Vector3.forward * CameraObjC_.nearClipPlane);

			MaximumBounds_ = CameraObjC_.ViewportToWorldPoint(Vector3.one);

			RectBounds_ = new Rect(0f, 0f, MaximumBounds_.x - MinimumBounds_.x, MaximumBounds_.y - MinimumBounds_.y);
		}

		/// <summary>
		/// Définit la position de la caméra.
		/// </summary>
		/// <param name="localPos_">Position locale</param>
		public void SetPosition(Vector3 localPos_)
		{
			transform.localPosition = localPos_;

			ComputeBounds();
		}

		public float GetZRatio()
		{
			return pitchObj_.transform.localPosition.z / CameraObjC_.farClipPlane;
		}

		public void TranslateOnZ(float zRatio)
		{
			var zPos_ = zRatio * (CameraObjC_.farClipPlane - CameraObjC_.nearClipPlane);

			pitchObj_.transform.localPosition = new Vector3(pitchObj_.transform.localPosition.x, pitchObj_.transform.localPosition.y, zPos_);

			ComputeBounds();
		}

		public void SetupRotationXY()
		{
			tempQuaternion1_ = yawObj_.transform.localRotation;
			tempQuaternion2_ = pitchObj_.transform.localRotation;
		}

		public void RotateOnXY(Vector2 rot_)
		{
			yawObj_.transform.localRotation = tempQuaternion1_ * Quaternion.AngleAxis(rot_.x, Vector3.up);
			pitchObj_.transform.localRotation = tempQuaternion2_ * Quaternion.AngleAxis(rot_.y, Vector3.right);
		}

		/// <summary>
		/// Initialise le composant.
		/// </summary>
		/// <param name="renderWidth_">Largeur de la texture de sortie</param>
		/// <param name="renderHeight_">Hauteur de la texture de sortie</param>
		public void Initialize(int renderWidth_, int renderHeight_, Color color_, float near_, float far_, Vector3 localPos_, Vector3 localDir_)
		{
			yawObj_ = transform.Find("Yaw").gameObject;
			pitchObj_ = yawObj_.transform.Find("Pitch").gameObject;
			cameraObj_ = pitchObj_.transform.Find("Camera").gameObject;

			SetRenderTexture(renderWidth_, renderHeight_);

			SetOrthographicMode();

			SetBackground(color_);

			SetClippingPlanes(near_, far_);

			transform.localRotation = Quaternion.LookRotation(localDir_, Vector3.up);

			SetPosition(localPos_);

			IsInitialized_ = true;
		}

		private void OnDisable()
		{
			if (RenderTexture_ != null)
			{
				RenderTexture_.Release();
			}

			CameraObjC_.targetTexture = null;
			CameraObjC_.forceIntoRenderTexture = false;
		}
	}
}
