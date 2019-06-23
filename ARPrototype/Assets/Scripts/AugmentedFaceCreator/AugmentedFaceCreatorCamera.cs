using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorCamera : MonoBehaviour
	{
		/// <summary>
		/// Gestionnaire de travailleur.
		/// </summary>
		public GameObject WorkerObj_
		{
			get => GameObject.Find("Worker");
		}

		/// <summary>
		/// Objet de yaw.
		/// </summary>
		public GameObject YawObj_ => transform.Find("Yaw").gameObject;

		/// <summary>
		/// Objet de pitch.
		/// </summary>
		public GameObject PitchObj_ => YawObj_.transform.Find("Pitch").gameObject;

		/// <summary>
		/// Objet portant la caméra.
		/// </summary>
		public GameObject CameraObj_ => PitchObj_.transform.Find("Camera").gameObject;

		/// <summary>
		/// Indique si l'initialisation est terminé.
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
			get => CameraObj_.GetComponent<Camera>();
		}

		/// <summary>
		/// Arrière plan.
		/// </summary>
		private Color Background_
		{
			get => CameraObjC_.backgroundColor;
			set
			{
				CameraObjC_.clearFlags = CameraClearFlags.SolidColor;
				CameraObjC_.backgroundColor = value;
			}
		}

		/// <summary>
		/// Définit les distances de coupures.
		/// </summary>
		public (float near_, float far_) ClippingPlane_
		{
			get => (CameraObjC_.nearClipPlane, CameraObjC_.farClipPlane);
			set
			{
				CameraObjC_.nearClipPlane = value.near_;
				CameraObjC_.farClipPlane = value.far_;
			}
		}

		/// <summary>
		/// Position.
		/// </summary>
		public Vector3 Position_
		{
			get => transform.localPosition;
			set
			{
				transform.localPosition = value;

				ComputeBounds();
			}
		}

		/// <summary>
		/// Direction du regard.
		/// </summary>
		public Vector3 Forward_ => CameraObjC_.transform.forward;

		/// <summary>
		/// Profondeur de l'espace.
		/// </summary>
		public float Depth_ => ClippingPlane_.far_ - ClippingPlane_.near_;

		/// <summary>
		/// Z Ratio
		/// </summary>
		public float ZRatio
		{
			get => PitchObj_.transform.localPosition.z / ClippingPlane_.far_;
			set
			{
				value = Mathf.Clamp01(value);

				PitchObj_.transform.localPosition = Vector3.forward * (value * Depth_ + ClippingPlane_.near_);

				ComputeBounds();
			}
		}

		/// <summary>
		/// Rotation.
		/// </summary>
		public (float yaw_, float pitch_) Rotation
		{
			get => (YawObj_.transform.localEulerAngles.y, PitchObj_.transform.localEulerAngles.x);
			set
			{
				YawObj_.transform.localRotation = Quaternion.AngleAxis(value.yaw_, Vector3.up);
				PitchObj_.transform.localRotation = Quaternion.AngleAxis(value.pitch_, Vector3.right);
			}
		}

		/// <summary>
		///Initialise et définit la sortie de la caméra sur la texture de rendu.
		/// </summary>
		/// <param name="width_">Largeur de la texture de sortie</param>
		/// <param name="height_">Hauteur de la texture de sortie</param>
		public void SetRenderTexture(int width_, int height_)
		{
			if (RenderTexture_ != null)
			{
				RenderTexture_.Release();
			}

			RenderTexture_ = new RenderTexture(width_, height_, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

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
		/// Calcul les limites.
		/// </summary>
		private void ComputeBounds()
		{
			MinimumBounds_ = CameraObjC_.ViewportToWorldPoint(Vector3.forward * CameraObjC_.nearClipPlane);

			MaximumBounds_ = CameraObjC_.ViewportToWorldPoint(Vector3.one);

			RectBounds_ = new Rect(0f, 0f, MaximumBounds_.x - MinimumBounds_.x, MaximumBounds_.y - MinimumBounds_.y);
		}

		/// <summary>
		/// Initialisation.
		/// </summary>
		/// <param name="renderWidth_">Largeur de la texture de sortie</param>
		/// <param name="renderHeight_">Hauteur de la texture de sortie</param>
		public void Initialize(int renderWidth_, int renderHeight_, Color color_, float near_, float far_, Vector3 pos_, Vector3 dir_)
		{
			if (RenderTexture_ != null)
			{
				RenderTexture_.Release();
			}

			transform.localRotation = Quaternion.LookRotation(dir_, Vector3.up);

			SetRenderTexture(renderWidth_, renderHeight_);
			SetOrthographicMode();

			Background_ = color_;
			ClippingPlane_ = (near_, far_);
			Position_ = pos_;

			IsInitialized_ = true;
		}
	}
}
