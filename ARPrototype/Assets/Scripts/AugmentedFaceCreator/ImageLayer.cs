using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class ImageLayer : MonoBehaviour
	{
		private Vector2 offset_;

		[SerializeField]
		private Camera camera_;

		[SerializeField]
		private Texture texture_;

		[SerializeField]
		private Texture2D mask_;

		public Camera Camera_
		{
			get => camera_;
			set
			{
				camera_ = value;
			}
		}

		public Texture Texture_
		{
			get => texture_;
			set
			{
				texture_ = value;
			}
		}

		public Texture2D Mask_
		{
			get => mask_;
			set
			{
				mask_ = value;
			}
		}

		public MeshFilter MeshFilter_ { get; private set; }

		public MeshRenderer MeshRenderer_ { get; private set; }

		public BoxCollider2D BoxCollider_ { get; private set; }

		public bool IsInitialized_ { get; private set; }

		public bool IsMoving_ { get; private set; }

		public void Initialize(Camera camera_, Texture texture_, Texture2D mask_, Material material_)
		{
			MeshFilter_ = GetComponent<MeshFilter>();
			MeshRenderer_ = GetComponent<MeshRenderer>();
			BoxCollider_ = GetComponent<BoxCollider2D>();

			Camera_ = camera_;
			Texture_ = texture_;
			Mask_ = mask_;

			MeshFilter_.mesh = new Mesh();
			MeshRenderer_.material = material_;

			ComputeVertice();
			ComputeTriangles();
			ComputeUV();

			IsInitialized_ = true;
		}

		private void Start()
		{
			Mask_ = new Texture2D(1024, 1024);
		}

		private void Update()
		{
			if (!IsInitialized_)
			{
				return;
			}

			MeshRenderer_.material.SetTexture("_Mask", Mask_);
			MeshRenderer_.material.SetTexture("_MainTex", Texture_);

			ComputeVertice();
		}

		private void ComputeVertice()
		{
			var width_ = 0f;
			var height_ = 0f;

			if (texture_.width > camera_.pixelWidth)
			{
				width_ = camera_.pixelWidth;
			}
			else
			{
				width_ = texture_.width;
			}

			if (texture_.height > camera_.pixelHeight)
			{
				height_ = camera_.pixelHeight * ((float)texture_.height / texture_.width);
			}
			else
			{
				height_ = texture_.height;
			}

			var a_ = Camera_.ScreenToWorldPoint(new Vector3(0, height_));
			var b_ = Camera_.ScreenToWorldPoint(new Vector3(width_, 0));
			var c_ = Camera_.ScreenToWorldPoint(new Vector3(0, 0));
			var d_ = Camera_.ScreenToWorldPoint(new Vector3(width_, height_));

			a_.z = b_.z = c_.z = d_.z = 0;

			MeshFilter_.mesh.SetVertices(new List<Vector3>()
		{
			a_,
			b_,
			c_,
			d_
		});

			MeshFilter_.mesh.RecalculateBounds();

			var boundOffset_ = new Vector2();
			width_ = MeshFilter_.mesh.bounds.size.x;
			height_ = MeshFilter_.mesh.bounds.size.y;

			if (texture_.width >= camera_.pixelWidth)
			{
				boundOffset_.x = 0;
			}
			else
			{
				boundOffset_.x = width_ * 0.5f;
			}

			if (texture_.height >= camera_.pixelHeight)
			{
				boundOffset_.y = 0;
			}
			else
			{
				boundOffset_.y = height_ * -0.5f;
			}

			BoxCollider_.offset = boundOffset_;
			BoxCollider_.size = new Vector2(width_, height_);
		}

		private void ComputeTriangles()
		{
			MeshFilter_.mesh.SetTriangles(new int[6]
			{
			0, 1, 2,
			0, 3, 1
			}, 0);
		}

		private void ComputeUV()
		{
			MeshFilter_.mesh.SetUVs(0, new List<Vector2>()
		{
			new Vector2(0, 1),
			new Vector2(1, 0),
			new Vector2(0, 0),
			new Vector2(1, 1)
		});
		}
	}
}
