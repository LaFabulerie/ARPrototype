using GoogleARCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts
{
	public enum MatchingImageType
	{
		Image,
		Images,
		Video
	}

	[Serializable]
	public sealed class MatchingImage
	{
#if UNITY_EDITOR
		public int hashCode_;
		public int databaseIndex_;
		public AugmentedImageDatabaseEntry entry_;
		public Texture2D referenceImage;
		public string imagesFolderPath_;
		public bool changed_;
#endif

		public MatchingImageType type_;
		public Texture2D image_;
		public List<Texture2D> images_;
		public float timeInterFrame_;
		public VideoClip video_;
		public Material material_;
		public string textureName_;
		public Vector2 offset_;

#if UNITY_EDITOR
		public MatchingImage(int databaseIndex_, AugmentedImageDatabaseEntry entry_)
		{
			hashCode_ = GetHashCode();
			this.databaseIndex_ = databaseIndex_;
			this.entry_ = entry_;
			referenceImage = entry_.Texture;
		}
#endif
	}
}
