using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class ImageLayerWorker : MonoBehaviour
	{
		// Mesh
		private MeshFilter mf_;
		private MeshRenderer mr_;
		private BoxCollider2D bc2d_;

		// Arguments
		[SerializeField]
		private Texture2D texture_;

		[SerializeField]
		private Texture2D mask_;

		[SerializeField]
		private AugmentedFaceCreatorWorkerOld afcw_;

		public Texture2D Texture_
		{
			get => texture_;
			set
			{
				texture_ = value;
				ComputeVertex();
				mr_.material.SetTexture("_MainTex", value);
			}
		}

		public Texture2D Mask_
		{
			get => mask_;
			set
			{
				mask_ = value;
				mr_.material.SetTexture("_MaskTex", value);
			}
		}

		public AugmentedFaceCreatorWorkerOld AFCW_
		{
			get => afcw_;
			set
			{
				afcw_ = value;
			}
		}

		public Vector2 TranslationOffset { get; set; }

		public float AngleOffset { get; set; }

		public Vector2 ScalingOffset { get; set; }

		public void ComputeVertex()
		{
			if (AFCW_ == null ||
				Texture_ == null)
			{
				return;
			}

			var r_ = (float)Texture_.height / Texture_.width;
			var halfW_ = AFCW_.StackCameraRectBounds.width * 0.5f;
			var h_ = AFCW_.StackCameraRectBounds.height * r_;
			var halfH_ = h_ * 0.5f;

			mf_.mesh.SetVertices(new List<Vector3>
			{
				new Vector3(-halfW_, -halfH_),
				new Vector3(-halfW_, halfH_),
				new Vector3(halfW_, -halfH_),
				new Vector3(halfW_, halfH_)
			});

			bc2d_.size = new Vector2(AFCW_.StackCameraRectBounds.width, h_);
		}

		public void ComputeTriangles()
		{
			if (mf_.mesh.vertexCount != 4)
			{
				return;
			}

			mf_.mesh.SetTriangles(new int[6]
			{
				0, 1, 2,
				3, 2, 1
			}, 0);
		}

		public void ComputeUVs()
		{
			if (mf_.mesh.vertexCount != 4)
			{
				return;
			}

			mf_.mesh.SetUVs(0, new List<Vector2>()
			{
				new Vector2(0, 0),
				new Vector2(0, 1),
				new Vector2(1, 0),
				new Vector2(1, 1)
			});
		}

		private void OnEnable()
		{
			mf_ = GetComponent<MeshFilter>();
			mr_ = GetComponent<MeshRenderer>();
			bc2d_ = GetComponent<BoxCollider2D>();

			mf_.mesh = new Mesh();
			mf_.mesh.MarkDynamic();

			TranslationOffset = Vector2.zero;
			ScalingOffset = Vector2.one;
			AngleOffset = 0f;
		}
	}
}