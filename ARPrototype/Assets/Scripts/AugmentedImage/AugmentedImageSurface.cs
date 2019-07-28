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

		public AugmentedImage augmentedImage_;
		public MatchingImage matchingImage_;

		private MeshCollider mc_;

		public bool IsInitialized_ { get; private set; }

		public bool IsRunning_ { get; private set; }

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

			GetComponent<MeshCollider>().sharedMesh = mesh_;
		}

		public void Play()
		{
			if (!IsInitialized_ ||
				IsRunning_)
			{
				return;
			}

			gameObject.SetActive(true);

			switch (matchingImage_.type_)
			{
				case MatchingImageType.Image:
					GetComponent<ImagePlayer>().images_ = new List<Texture2D>(1) { matchingImage_.texture_ };
					GetComponent<ImagePlayer>().textureName_ = matchingImage_.textureName_;
					GetComponent<ImagePlayer>().timeInterFrame_ = matchingImage_.timeInterFrame_;
					GetComponent<ImagePlayer>().isLooping_ = true;
					GetComponent<ImagePlayer>().Play();
					break;
				case MatchingImageType.Images:
					GetComponent<MeshRenderer>().material = matchingImage_.material_;
					GetComponent<ImagePlayer>().images_ = matchingImage_.textures_;
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

			IsRunning_ = true;
		}

		public void Stop()
		{
			if (!IsRunning_)
			{
				return;
			}

			gameObject.SetActive(false);

			switch (matchingImage_.type_)
			{
				case MatchingImageType.Image:
				case MatchingImageType.Images:
					GetComponent<ImagePlayer>().Stop();
					break;
				case MatchingImageType.Video:
					GetComponent<VideoPlayer>().Stop();
					break;
			}

			IsRunning_ = false;
		}

		private void Update()
		{
			if (IsRunning_)
			{
				transform.localPosition = new Vector3(matchingImage_.offset_.x * augmentedImage_.ExtentX, 0f, matchingImage_.offset_.y * augmentedImage_.ExtentZ);

				ComputeVertices();
			}
		}
	}
}
