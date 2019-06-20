using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class AugmentedFaceCreatorWorkerOld : MonoBehaviour
	{
		// AppManager
		private AppManager appManager_;

		// FaceMesh
		private GameObject faceMeshObj_;
		private GameObject meshPivotObj_;
		private GameObject meshObj_;
		private GameObject leftLightObj_;
		private GameObject rightLightObj_;
		private GameObject meshCameraYawObj_;
		private GameObject meshCameraPitchObj_;
		private GameObject meshCameraObj_;

		// TextureStack
		private GameObject textureStackObj_;
		private GameObject stackObj_;
		private Dictionary<string, GameObject> stackImageObjs_;
		private GameObject stackCameraPivotObj_;
		private GameObject stackCameraObj_;

		// Working Flied
		//
		private Vector2 workingMinimumMeshCameraRectBounds;
		private Vector2 workingMaximumMeshCameraRectBounds;
		private Rect workingMeshCameraRectBounds_;
		private Vector2 workingMinimumStackCameraRectBounds_;
		private Vector2 workingMaximumStackCameraRectBounds_;
		private Rect workingStackCameraRectBounds_;
		//
		private float prevMeshCameraUnit_;
		private float workingMeshCameraUnit_;
		//
		private Vector3 mouseBegin;
		private Vector3 prevMeshCameraLocalPos_;
		//
		private Vector3 mouseBegin2_;
		private Vector3 prevMeshCameraYawLocalEuls_;
		private Vector3 prevMeshCameraPitchLocalEuls_;
		//
		private float MininumDepth_;
		private float depthUnit_;
		//
		private GameObject stackImageObj_;
		private ImageLayerWorker stackImageWI_;
		//
		private Vector3 mouseBegin3;
		private Vector3 prevStackImageLocalPos_;
		//
		private Quaternion prevStackImageLocalRot_;
		//
		private Vector3 prevStackImageLocalScale_;
		//
		private Queue<Vector2> positions_;

		// Arguments
		[SerializeField]
		private Vector3Int meshCameraSize_;

		[SerializeField]
		private float meshCameraUnit_;

		[SerializeField]
		private float meshDistance_;

		[SerializeField]
		private Vector3Int stackCameraSize_;

		[SerializeField]
		private float stackCameraUnit_;

		[SerializeField]
		private Material imageWorkerMaterial_;

		public Vector3 MinimumMeshCameraRectBounds_ { get; private set; }

		public Vector3 MaximumMeshCameraRectBounds_ { get; private set; }

		public Rect MeshCameraRectBounds_ { get; private set; }

		public Rect StackCameraRectBounds { get; private set; }

		public RenderTexture MeshTexture_ { get; private set; }

		public RenderTexture StackTexture_ { get; private set; }

		private (Vector3, Vector3, Rect) SetupOrthographicCamera(GameObject cameraObj_, RenderTexture texture_, Vector3Int size_, float unit_)
		{
			var camera_ = cameraObj_.GetComponent<Camera>();

			if (camera_ == null)
			{
				Debug.LogError($"The {cameraObj_.name} game object doesn't contain a camera component.");
			}

			if (texture_ == null)
			{
				Debug.LogError($"The {texture_.name} render texture is null.");
			}

			camera_.targetTexture = texture_;
			camera_.orthographic = true;
			camera_.forceIntoRenderTexture = true;
			camera_.orthographicSize = size_.y / unit_ * 0.5f;

			var bl_ = camera_.ViewportToWorldPoint(new Vector3(0, 0, 0));
			var tl_ = camera_.ViewportToWorldPoint(new Vector3(0, 1, 0));
			var br_ = camera_.ViewportToWorldPoint(new Vector3(1, 0, 0));
			var tr_ = camera_.ViewportToWorldPoint(new Vector3(1, 1, 0));

			var minimum_ = new Vector3
			{
				x = Mathf.Min(bl_.x, tl_.x, br_.x, tr_.x),
				y = Mathf.Min(bl_.y, tl_.y, br_.y, tr_.y)
			};

			var maximum_ = new Vector3
			{
				x = Mathf.Max(bl_.x, tl_.x, br_.x, tr_.x),
				y = Mathf.Max(bl_.y, tl_.y, br_.y, tr_.y)
			};

			var rectBounds_ = new Rect()
			{
				x = minimum_.x,
				y = minimum_.y,
				width = maximum_.x - minimum_.x,
				height = maximum_.y - minimum_.y,
			};

			return (minimum_, maximum_, rectBounds_);
		}

		private void ScaleMeshToCameraRectBounds()
		{
			var meshCameraC_ = meshCameraObj_.GetComponent<Camera>();
			var meshMF_ = meshObj_.GetComponent<MeshFilter>();
			var meshMR_ = meshObj_.GetComponent<MeshRenderer>();
			var LeftLightL_ = leftLightObj_.GetComponent<Light>();
			var rightLightL_ = rightLightObj_.GetComponent<Light>();

			meshMF_.mesh.RecalculateBounds();
			var bounds_ = meshMR_.bounds;

			var x1_ = (MaximumMeshCameraRectBounds_.x - MinimumMeshCameraRectBounds_.x) * 0.5f;
			var y1_ = (MaximumMeshCameraRectBounds_.y - MinimumMeshCameraRectBounds_.y) * 0.5f;

			var x2_ = bounds_.extents.x;
			var y2_ = bounds_.extents.y;
			var z2_ = bounds_.extents.z;

			var sX_ = x1_ / x2_;
			var sY_ = y1_ / y2_;
			var s_ = Mathf.Min(sX_, sY_);
			var offset_ = Mathf.Max(x2_, y2_, z2_) * s_ * 1.25f;
			s_ *= 0.75f;

			meshObj_.transform.localScale = Vector3.one * s_;
			meshObj_.transform.localPosition = meshCameraYawObj_.transform.localPosition = Vector3.forward * meshDistance_;

			meshCameraObj_.transform.localPosition = Vector3.back * meshDistance_;
			meshCameraC_.nearClipPlane = meshDistance_ - offset_;
			meshCameraC_.farClipPlane = meshDistance_ + offset_;

			leftLightObj_.transform.localPosition = Vector3.left * meshDistance_;
			rightLightObj_.transform.localPosition = Vector3.right * meshDistance_;

			rightLightL_.range = LeftLightL_.range = Mathf.Sqrt(meshDistance_ * meshDistance_ * 8f);
		}

		public string GetImage(Vector3 pos_)
		{
			var stackCameraC_ = stackCameraObj_.GetComponent<Camera>();
			var ray_ = stackCameraC_.ViewportPointToRay(pos_);

			var hit_ = Physics2D.Raycast(ray_.origin, ray_.direction, stackCameraC_.farClipPlane);

			if (hit_)
			{
				return hit_.transform.name;
			}

			return string.Empty;
		}

		private void DrawFilledCircle(Texture2D texture_, Vector2Int center_, int radius_, float t_, Color color_)
		{
			for (var y_ = -radius_; y_ < radius_; y_++)
			{
				for (var x_ = -radius_; x_ < radius_; x_++)
				{
					var dist_ = Mathf.Sqrt(x_ * x_ + y_ * y_) / radius_;

					if (dist_ < 1f)
					{
						var oldColor_ = texture_.GetPixel(center_.x + x_, center_.y + y_);
						var currentColor_ = color_;

						if (t_ > 0f)
						{
							var clamped_ = 1 - Mathf.Clamp(dist_, 1f - t_, 1f);
							var mapped_ = (clamped_ / t_);
							currentColor_ = mapped_ * color_ + (1 - mapped_) * oldColor_;
						}

						if (currentColor_ != oldColor_)
						{
							texture_.SetPixel(center_.x + x_, center_.y + y_, currentColor_);
						}
					}
				}
			}

			texture_.Apply();
		}


		public Vector2? GetTexPos(Vector3 pos_)
		{
			var meshCameraC_ = meshCameraObj_.GetComponent<Camera>();
			var ray_ = meshCameraC_.ViewportPointToRay(pos_);

			if (Physics.Raycast(ray_.origin, ray_.direction, out RaycastHit hit_, meshCameraC_.farClipPlane))
			{
				return hit_.textureCoord;
			}

			return null;
		}

		public void OnDrawTexPos(string name_, Vector3 texPos_)
		{
			stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;
		}

		public void DrawTexPos(string name_, Vector2 texPos_, int radius_, float t_, Color color_)
		{
			stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			var stackImageWI_ = stackImageObj_.GetComponent<ImageLayerWorker>();

			texPos_ -= new Vector2(0.5f, 0.5f);

			texPos_ -= stackImageWI_.TranslationOffset;

			var matR_ = Matrix4x4.Rotate(Quaternion.AngleAxis(stackImageWI_.AngleOffset, Vector3.back));

			var scale_ = Vector2.zero;
			scale_.x = 1f / stackImageWI_.ScalingOffset.x;
			scale_.y = 1f / stackImageWI_.ScalingOffset.y;
			var matS_ = Matrix4x4.Scale(scale_);

			var mat_ = matR_ * matS_;

			texPos_ = mat_.MultiplyVector(texPos_);

			texPos_ += new Vector2(0.5f, 0.5f);

			Texture2D tex_ = null;

			//if (stackImageWI_.Texture_.isReadable)
			//{
			//	tex_ = stackImageWI_.Texture_;
			//}
			//else
			//{
			tex_ = stackImageWI_.Mask_;
			//}

			if (texPos_.x >= 0f && texPos_.x <= 1f &&
				texPos_.y >= 0f && texPos_.y <= 1f)
			{
				texPos_.x *= stackImageWI_.Texture_.width;
				texPos_.y *= stackImageWI_.Texture_.height;
				var texPosInt_ = new Vector2Int(Mathf.FloorToInt(texPos_.x), Mathf.FloorToInt(texPos_.y));
				DrawFilledCircle(tex_, texPosInt_, radius_, t_, color_);
			}
		}

		public void OnTranslateMeshView(Vector3 pos_)
		{
			mouseBegin = pos_;
			prevMeshCameraLocalPos_ = meshCameraObj_.transform.localPosition;
		}

		public void OnRotateMeshView(Vector3 pos_)
		{
			mouseBegin2_ = pos_;
			prevMeshCameraYawLocalEuls_ = meshCameraYawObj_.transform.localEulerAngles;
			prevMeshCameraPitchLocalEuls_ = meshCameraPitchObj_.transform.localEulerAngles;
		}

		public void OnScaleMeshView()
		{
			prevMeshCameraUnit_ = workingMeshCameraUnit_;
			prevMeshCameraLocalPos_ = meshCameraObj_.transform.localPosition;
		}

		public void TranslateMeshView(Vector3 pos_)
		{
			if (workingMeshCameraUnit_ == meshCameraUnit_)
			{
				return;
			}

			pos_ -= mouseBegin;

			pos_.x *= -MeshCameraRectBounds_.width;
			pos_.y *= -MeshCameraRectBounds_.height;

			var a_ = workingMeshCameraRectBounds_.width * 0.5f;
			var b_ = workingMeshCameraRectBounds_.height * 0.5f;

			var newPos_ = prevMeshCameraLocalPos_ + pos_;
			newPos_.x = Mathf.Clamp(newPos_.x, MinimumMeshCameraRectBounds_.x + a_, MaximumMeshCameraRectBounds_.x - a_);
			newPos_.y = Mathf.Clamp(newPos_.y, MinimumMeshCameraRectBounds_.y + a_, MaximumMeshCameraRectBounds_.y - a_);
			newPos_.z = meshCameraObj_.transform.localPosition.z;

			meshCameraObj_.transform.localPosition = newPos_;
		}

		public void RotateMeshView(Vector3 eulerAngles_)
		{
			eulerAngles_ -= mouseBegin2_;

			eulerAngles_.x = (prevMeshCameraYawLocalEuls_.x + eulerAngles_.x * -90f) % 360f;
			eulerAngles_.y = (prevMeshCameraPitchLocalEuls_.y + eulerAngles_.y * 90f) % 360f;

			if (eulerAngles_.x <= 90 || eulerAngles_.x >= 270f)
			{
				meshCameraYawObj_.transform.localEulerAngles = eulerAngles_.x * Vector3.right;
			}

			if (eulerAngles_.y <= 90 || eulerAngles_.y >= 270f)
			{
				meshCameraPitchObj_.transform.localEulerAngles = eulerAngles_.y * Vector3.up;
			}
		}

		public void ScaleMeshView(float scale_)
		{
			scale_ *= 0.001f;
			scale_++;

			if (scale_ > 0)
			{
				var meshCameraC_ = meshCameraObj_.GetComponent<Camera>();

				workingMeshCameraUnit_ = Mathf.Clamp(prevMeshCameraUnit_ * scale_, meshCameraUnit_, float.PositiveInfinity);

				meshCameraC_.orthographicSize = meshCameraSize_.y / workingMeshCameraUnit_ * 0.5f;

				var bl_ = meshCameraC_.ViewportToWorldPoint(new Vector3(0, 0, 0));
				var tl_ = meshCameraC_.ViewportToWorldPoint(new Vector3(0, 1, 0));
				var br_ = meshCameraC_.ViewportToWorldPoint(new Vector3(1, 0, 0));
				var tr_ = meshCameraC_.ViewportToWorldPoint(new Vector3(1, 1, 0));

				workingMinimumMeshCameraRectBounds = new Vector3
				{
					x = Mathf.Min(bl_.x, tl_.x, br_.x, tr_.x),
					y = Mathf.Min(bl_.y, tl_.y, br_.y, tr_.y)
				};

				workingMaximumMeshCameraRectBounds = new Vector3
				{
					x = Mathf.Max(bl_.x, tl_.x, br_.x, tr_.x),
					y = Mathf.Max(bl_.y, tl_.y, br_.y, tr_.y)
				};

				workingMeshCameraRectBounds_ = new Rect()
				{
					x = workingMinimumMeshCameraRectBounds.x,
					y = workingMinimumMeshCameraRectBounds.y,
					width = workingMaximumMeshCameraRectBounds.x - workingMinimumMeshCameraRectBounds.x,
					height = workingMaximumMeshCameraRectBounds.y - workingMinimumMeshCameraRectBounds.y,
				};

				if (scale_ < 1)
				{
					var t_ = 1f - (workingMeshCameraUnit_ - 1024f) / meshCameraUnit_;

					var newPos_ = Vector3.Lerp(prevMeshCameraLocalPos_, Vector3.zero, t_);
					newPos_.z = prevMeshCameraLocalPos_.z;

					meshCameraObj_.transform.localPosition = newPos_;
				}
			}
		}

		public void ResetMeshView()
		{
			meshCameraYawObj_.transform.localEulerAngles = Vector3.zero;
			meshCameraPitchObj_.transform.localEulerAngles = Vector3.zero;

			var meshCameraC_ = meshCameraObj_.GetComponent<Camera>();

			workingMeshCameraUnit_ = meshCameraUnit_;
			meshCameraC_.orthographicSize = meshCameraSize_.y / workingMeshCameraUnit_ * 0.5f;

			var bl_ = meshCameraC_.ViewportToWorldPoint(new Vector3(0, 0, 0));
			var tl_ = meshCameraC_.ViewportToWorldPoint(new Vector3(0, 1, 0));
			var br_ = meshCameraC_.ViewportToWorldPoint(new Vector3(1, 0, 0));
			var tr_ = meshCameraC_.ViewportToWorldPoint(new Vector3(1, 1, 0));

			workingMinimumMeshCameraRectBounds = new Vector3
			{
				x = Mathf.Min(bl_.x, tl_.x, br_.x, tr_.x),
				y = Mathf.Min(bl_.y, tl_.y, br_.y, tr_.y)
			};

			workingMaximumMeshCameraRectBounds = new Vector3
			{
				x = Mathf.Max(bl_.x, tl_.x, br_.x, tr_.x),
				y = Mathf.Max(bl_.y, tl_.y, br_.y, tr_.y)
			};

			workingMeshCameraRectBounds_ = new Rect()
			{
				x = workingMinimumMeshCameraRectBounds.x,
				y = workingMinimumMeshCameraRectBounds.y,
				width = workingMaximumMeshCameraRectBounds.x - workingMinimumMeshCameraRectBounds.x,
				height = workingMaximumMeshCameraRectBounds.y - workingMinimumMeshCameraRectBounds.y,
			};

			var newPos_ = Vector3.zero;
			newPos_.z = meshCameraObj_.transform.localPosition.z;
			meshCameraObj_.transform.localPosition = newPos_;
		}

		public void OnTranslateImage(string name_, Vector3 pos_)
		{
			stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			mouseBegin3 = pos_;
			prevStackImageLocalPos_ = stackImageObj_.transform.localPosition;
			stackImageWI_ = stackImageObj_.GetComponent<ImageLayerWorker>();
		}

		public void TranslateImage(string name_, Vector3 pos_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			pos_ -= mouseBegin3;

			pos_.x *= StackCameraRectBounds.width;
			pos_.y *= StackCameraRectBounds.height;

			var newPos_ = prevStackImageLocalPos_ + pos_;
			newPos_.z = stackImageObj_.transform.localPosition.z;

			stackImageObj_.transform.localPosition = newPos_;

			var v_ = Vector2.zero;
			v_.x = stackImageObj_.transform.localPosition.x / StackCameraRectBounds.width;
			v_.y = stackImageObj_.transform.localPosition.y / StackCameraRectBounds.height;

			stackImageWI_.TranslationOffset = v_;
		}

		public void OffTranslateImage(string name_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			stackImageObj_ = null;
			stackImageWI_ = null;
		}

		public void OnRotateImage(string name_, float angle_)
		{
			stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			prevStackImageLocalRot_ = stackImageObj_.transform.localRotation;
			stackImageWI_ = stackImageObj_.GetComponent<ImageLayerWorker>();
		}

		public void RotateImage(string name_, float angle_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			stackImageObj_.transform.localRotation = prevStackImageLocalRot_ * Quaternion.AngleAxis(angle_, Vector3.forward);
			stackImageWI_.AngleOffset = stackImageObj_.transform.localEulerAngles.z;
		}

		public void OffRotateImage(string name_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			stackImageObj_ = null;
		}

		public void OnScaleImage(string name_, float mag_)
		{
			stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			prevStackImageLocalScale_ = stackImageObj_.transform.localScale;
			stackImageWI_ = stackImageObj_.GetComponent<ImageLayerWorker>();
		}

		public void ScaleImageXY(string name_, float mag_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			mag_ *= 0.001f;
			mag_++;

			stackImageObj_.transform.localScale = prevStackImageLocalScale_ * mag_;

			stackImageWI_.ScalingOffset = stackImageObj_.transform.localScale;
		}

		public void ScaleImageY(string name_, float mag_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			mag_ *= 0.001f;
			mag_++;

			var newScale_ = new Vector3(prevStackImageLocalScale_.x, prevStackImageLocalScale_.y * mag_);

			stackImageObj_.transform.localScale = newScale_;

			stackImageWI_.ScalingOffset = stackImageObj_.transform.localScale;
		}

		public void ScaleImageX(string name_, float mag_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			mag_ *= 0.001f;
			mag_++;

			var newScale_ = new Vector3(prevStackImageLocalScale_.x * mag_, prevStackImageLocalScale_.y);

			stackImageObj_.transform.localScale = newScale_;

			stackImageWI_.ScalingOffset = stackImageObj_.transform.localScale;
		}

		public void OffScaleImage(string name_)
		{
			if (stackImageObj_ == null)
			{
				return;
			}

			stackImageObj_ = null;
		}

		public void MoveToFront(string name_)
		{
			var stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			var index_ = stackImageObj_.transform.GetSiblingIndex();

			if (index_ <= 0)
			{
				return;
			}

			var previousStackImageObj_ = stackObj_.transform.GetChild(index_ - 1).gameObject;

			previousStackImageObj_.transform.Translate(Vector3.forward * depthUnit_);
			stackImageObj_.transform.Translate(Vector3.back * depthUnit_);

			previousStackImageObj_.transform.SetSiblingIndex(index_);
			stackImageObj_.transform.SetSiblingIndex(index_ - 1);
		}

		public void MoveToBack(string name_)
		{
			var stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			var index_ = stackImageObj_.transform.GetSiblingIndex();

			if (index_ >= stackObj_.transform.childCount - 1)
			{
				return;
			}

			var previousStackImageObj_ = stackObj_.transform.GetChild(index_ + 1).gameObject;

			previousStackImageObj_.transform.Translate(Vector3.back * depthUnit_);
			stackImageObj_.transform.Translate(Vector3.forward * depthUnit_);

			previousStackImageObj_.transform.SetSiblingIndex(index_);
			stackImageObj_.transform.SetSiblingIndex(index_ + 1);
		}

		public void AddImage(string name_, Texture2D texture_)
		{
			var stackImageCount_ = stackObj_.transform.childCount;

			if (stackImageCount_ >= 100f)
			{
				return;
			}

			var stackImage_ = new GameObject(name_, typeof(ImageLayerWorker));

			stackImage_.transform.SetParent(stackObj_.transform);
			stackImage_.transform.localPosition = Vector3.forward * MininumDepth_ + Vector3.forward * depthUnit_ * stackImageCount_;
			stackImage_.transform.localRotation = Quaternion.identity;
			stackImage_.transform.localScale = Vector3.one;

			var stackImageMR_ = stackImage_.GetComponent<MeshRenderer>();
			var stackImageIW_ = stackImage_.GetComponent<ImageLayerWorker>();

			stackImageMR_.material = imageWorkerMaterial_;

			stackImageIW_.AFCW_ = this;

			stackImageIW_.Texture_ = texture_;

			var tex_ = new Texture2D(1024, 1024);
			for (int y_ = 0; y_ < 1024; y_++)
			{
				for (int x_ = 0; x_ < 1024; x_++)
				{
					tex_.SetPixel(x_, y_, Color.white);
				}
			}
			tex_.Apply();

			stackImageIW_.Mask_ = tex_;

			stackImageIW_.ComputeVertex();
			stackImageIW_.ComputeTriangles();
			stackImageIW_.ComputeUVs();
		}

		public void RemoveImage(string name_)
		{
			var stackImageObj_ = stackObj_.transform.Find(name_)?.gameObject;

			if (stackImageObj_ == null)
			{
				return;
			}

			var v_ = Vector3.back * depthUnit_;

			for (int i_ = stackObj_.transform.GetSiblingIndex() + 1; i_ < stackObj_.transform.childCount; i_++)
			{
				var childObj_ = stackObj_.transform.GetChild(i_);
				childObj_.Translate(v_);
			}

#if UNITY_EDITOR
			DestroyImmediate(stackImageObj_);
#else
			Destroy(stackImageObj_);
#endif

			stackImageObjs_.Remove(name_);
		}

		private void OnEnable()
		{
			// AppManager
			appManager_ = AppManager.Current_;

			// FaceMesh
			faceMeshObj_ = transform.Find("FaceMesh").gameObject;
			meshPivotObj_ = faceMeshObj_.transform.Find("Mesh-Pivot").gameObject;
			meshObj_ = meshPivotObj_.transform.Find("Mesh").gameObject;
			leftLightObj_ = faceMeshObj_.transform.Find("LeftLight").gameObject;
			rightLightObj_ = faceMeshObj_.transform.Find("RightLight").gameObject;
			meshCameraYawObj_ = faceMeshObj_.transform.Find("MeshCamera-Yaw").gameObject;
			meshCameraPitchObj_ = meshCameraYawObj_.transform.Find("MeshCamera-Pitch").gameObject;
			meshCameraObj_ = meshCameraPitchObj_.transform.Find("MeshCamera").gameObject;

			// TextureStack
			textureStackObj_ = transform.Find("TextureStack").gameObject;
			stackObj_ = textureStackObj_.transform.Find("Stack").gameObject;
			stackImageObjs_ = new Dictionary<string, GameObject>();
			stackCameraPivotObj_ = textureStackObj_.transform.Find("StackCamera-Pivot").gameObject;
			stackCameraObj_ = stackCameraPivotObj_.transform.Find("StackCamera").gameObject;

			// Setup Mesh Camera
			MeshTexture_ = new RenderTexture(meshCameraSize_.x, meshCameraSize_.y, meshCameraSize_.z)
			{
				name = "MeshTexture"
			};
			var a_ = SetupOrthographicCamera(meshCameraObj_, MeshTexture_, meshCameraSize_, meshCameraUnit_);
			workingMeshCameraUnit_ = meshCameraUnit_;
			workingMinimumMeshCameraRectBounds = MinimumMeshCameraRectBounds_ = a_.Item1;
			workingMaximumMeshCameraRectBounds = MaximumMeshCameraRectBounds_ = a_.Item2;
			workingMeshCameraRectBounds_ = MeshCameraRectBounds_ = a_.Item3;

			textureStackObj_.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);

			// Setup Stack Camera

			StackTexture_ = new RenderTexture(stackCameraSize_.x, stackCameraSize_.y, stackCameraSize_.z)
			{
				name = "StackTexture"
			};
			var b_ = SetupOrthographicCamera(stackCameraObj_, StackTexture_, stackCameraSize_, stackCameraUnit_);
			workingMinimumStackCameraRectBounds_ = b_.Item1;
			workingMaximumStackCameraRectBounds_ = b_.Item2;
			workingStackCameraRectBounds_ = StackCameraRectBounds = b_.Item3;

			var stackCameraC_ = stackCameraObj_.GetComponent<Camera>();
			MininumDepth_ = stackCameraC_.nearClipPlane;
			depthUnit_ = (stackCameraC_.farClipPlane - stackCameraC_.nearClipPlane) / 100f;

			// Setup Mesh
			ScaleMeshToCameraRectBounds();
			meshObj_.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", StackTexture_);

			// Setup Texture Stack
			positions_ = new Queue<Vector2>();
		}

		private void OnDisable()
		{
			// Reset
			MeshTexture_.Release();
			MeshTexture_ = null;

			StackTexture_.Release();
			StackTexture_ = null;
		}
	}
}
