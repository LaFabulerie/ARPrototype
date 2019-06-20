using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(ImagePlayer))]
	[RequireComponent(typeof(VideoPlayer))]
	[RequireComponent(typeof(MeshCollider))]
	public class AugmentedImageSurface : MonoBehaviour
	{
		private Mesh mesh_;
		private Coroutine currentCoroutine_;
		private Vector3 positon_;

		public float disparitionDelay_ = 2f;
		public AugmentedImage augmentedImage_;
		public MatchingImage matchingImage_;

		private MeshCollider mc_;

		public bool IsInitialized_ { get; private set; }
		public float LastUpdatedTime_ { get; set; }
		public bool IsOld_ { get; private set; }

		private void ComputeVertices()
		{
			var halfWidth_ = augmentedImage_.ExtentX * 0.5f;
			var halfHeight_ = augmentedImage_.ExtentZ * 0.5f;

			mesh_.SetVertices(new List<Vector3>(4)
			{
				new Vector3(-halfWidth_, 0, halfHeight_),
				new Vector3(halfWidth_, 0, -halfHeight_),
				new Vector3(-halfWidth_, 0, -halfHeight_),
				new Vector3(halfWidth_, 0, halfHeight_)
			});
		}

		private void ComputeTriangles()
		{
			mesh_.SetTriangles(new int[6]
			{
				0, 
				1,
				2,
				0,
				3,
				1
			}, 0);
		}

		private void ComputeNormals()
		{
			mesh_.SetNormals(new List<Vector3>(4)
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			});
		}

		private void ComputeUVs()
		{
			mesh_.SetUVs(0, new List<Vector2>(4)
			{
				new Vector2(0, 1),
				new Vector2(1, 0),
				new Vector2(0, 0),
				new Vector2(1, 1)
			});
		}

		public void Inititialize(AugmentedImage augmentedImage_, MatchingImage matchingImage_)
		{
			this.augmentedImage_ = augmentedImage_;
			this.matchingImage_ = matchingImage_;

			LastUpdatedTime_ = Time.time;
			IsInitialized_ = true;

			mesh_ = new Mesh();

			ComputeVertices();
			ComputeTriangles();
			ComputeNormals();
			ComputeUVs();
			mesh_.RecalculateTangents();
			mesh_.MarkDynamic();

			GetComponent<MeshFilter>().mesh = mesh_;
			GetComponent<MeshRenderer>().material = matchingImage_.material_;

			switch (matchingImage_.type_)
			{
				case MatchingImageType.Image:
					GetComponent<ImagePlayer>().images_ = new List<Texture2D>(1) { matchingImage_.image_ };
					GetComponent<ImagePlayer>().textureName_ = matchingImage_.textureName_;
					GetComponent<ImagePlayer>().timeInterFrame_ = matchingImage_.timeInterFrame_;
					GetComponent<ImagePlayer>().isLooping_ = true;
					GetComponent<ImagePlayer>().Play();
					break;
				case MatchingImageType.Images:
					GetComponent<MeshRenderer>().material = matchingImage_.material_;
					GetComponent<ImagePlayer>().images_ = matchingImage_.images_;
					GetComponent<ImagePlayer>().textureName_ = matchingImage_.textureName_;
					GetComponent<ImagePlayer>().timeInterFrame_ = matchingImage_.timeInterFrame_;
					GetComponent<ImagePlayer>().isLooping_ = true;
					GetComponent<ImagePlayer>().Play();
					break;
				case MatchingImageType.Video:
					GetComponent<VideoPlayer>().clip = matchingImage_.video_;
					GetComponent<VideoPlayer>().renderMode = VideoRenderMode.MaterialOverride;
					GetComponent<VideoPlayer>().targetMaterialProperty = matchingImage_.textureName_;
					GetComponent<VideoPlayer>().isLooping = true;
					GetComponent<VideoPlayer>().Play();
					break;
			}

			GetComponent<MeshCollider>().sharedMesh = mesh_;
		}

		private void Update()
		{
			IsOld_ = IsInitialized_ && Time.time >= LastUpdatedTime_ + disparitionDelay_;

			if (IsOld_)
			{
				gameObject.SetActive(false);

				switch (matchingImage_.type_)
				{
					case MatchingImageType.Images:
						if (currentCoroutine_ != null)
						{
							StopCoroutine(currentCoroutine_);
						}
						break;
					case MatchingImageType.Video:
						if (GetComponent<VideoPlayer>().isPlaying)
						{
							GetComponent<VideoPlayer>().Stop();
						}
						break;
				}

				return;
			}

			transform.localPosition = new Vector3(matchingImage_.offset_.x * augmentedImage_.ExtentX, 0f, matchingImage_.offset_.y * augmentedImage_.ExtentZ);

			ComputeVertices();

			gameObject.SetActive(true);
		}
	}
}
