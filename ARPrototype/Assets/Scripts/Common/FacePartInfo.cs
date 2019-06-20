using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts
{
	[DataContract]
	public class FacePartInfo
	{
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
		public Vector3 Scale_ { get; set; }

		[DataMember]
		public Vector2 Dimension_ { get; set; }

		[DataMember()]
		public string MeshAssetPath_ { get; set; }

		[DataMember()]
		public int TextureWidth_ { get; set; }

		[DataMember()]
		public int TextureHeight { get; set; }

		[DataMember]
		public string TexturePath_ { get; set; }
	}
}
