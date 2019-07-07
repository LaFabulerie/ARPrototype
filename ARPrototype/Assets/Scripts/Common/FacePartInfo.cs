using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts
{
	[DataContract]
	public class FacePartInfo
	{
		public static Texture2D GetImage(string path_, int width_ = 1024, int height_ = 1024)
		{
			if (!File.Exists(path_))
			{
				return null;
			}

			var bytes_ = File.ReadAllBytes(path_);

			var tex_ = new Texture2D(width_, height_, TextureFormat.ARGB32, false);
			tex_.LoadImage(bytes_);

			return tex_;
		}

		[DataMember()]
		public bool IsReference_ { get; set; }

		[DataMember]
		public ReferenceAnchor Anchor_ { get; set; }

		[DataMember()]
		public bool Is2D_ { get; set; }

		[DataMember]
		public Vector3 PositionOffset_ { get; set; }

		[DataMember]
		public Quaternion Rotation_ { get; set; }

		[DataMember]
		public Vector3 ReferenceScale_ { get; set; }

		[DataMember]
		public Vector3 Scale_ { get; set; }

		[DataMember]
		public Vector2 Dimension_ { get; set; }

		[DataMember()]
		public int MeshObjIndex_ { get; set; }

		[DataMember()]
		public int TextureWidth_ { get; set; }

		[DataMember()]
		public int TextureHeight { get; set; }

		[DataMember]
		public string TexturePath_ { get; set; }
	}
}
