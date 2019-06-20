using GoogleARCore;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "New Matching Image Database", menuName = "MatchingImageDatabase", order = 0)]
	public sealed class MatchingImageDatabase : ScriptableObject
	{
#if (UNITY_EDITOR)
		[SerializeField]
		private AugmentedImageDatabase augmentedImages_;
#endif

		[SerializeField]
		private List<MatchingImage> matchingImages_ = new List<MatchingImage>();

		public MatchingImage this[int index_]
		{
			get
			{
				return matchingImages_[index_];
			}
		}

#if (UNITY_EDITOR)
		public void LoadReferenceImages()
		{
			matchingImages_.Clear();

			for (int i_ = 0; i_ < augmentedImages_.Count; i_++)
			{
				matchingImages_.Add(new MatchingImage(i_, augmentedImages_[i_]));
			}
		}
#endif
	}
}