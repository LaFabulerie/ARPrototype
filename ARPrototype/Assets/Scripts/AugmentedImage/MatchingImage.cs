using System;
using System.Collections.Generic;
using GoogleARCore;
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
		public Texture2D databaseEntryTexture_;
		public string databaseEntryName_;
		public string databaseEntryQuality_;
		public float databaseEntryWidth_;
		public AugmentedImageDatabaseEntry databaseEntry_;
		public bool isChanged_;
#endif

		public MatchingImageType type_;
		public string textureName_;
		public Texture2D texture_;
		public string texturesFolderPath_;
		public List<Texture2D> textures_;
		public float timeInterFrame_;
		public VideoClip video_;
		public Material material_;
		public Vector2 offset_;

#if UNITY_EDITOR
		public MatchingImage(int databaseIndex_, AugmentedImageDatabaseEntry entry_)
		{
			hashCode_ = GetHashCode();
			this.databaseIndex_ = databaseIndex_;
			databaseEntry_ = entry_;
			databaseEntryTexture_ = entry_.Texture;
			databaseEntryName_ = entry_.Name;
			databaseEntryQuality_ = entry_.Quality;
			databaseEntryWidth_ = entry_.Width;
		}
#endif
	}
}
