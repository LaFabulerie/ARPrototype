using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Assets.Scripts
{
	[DataContract]
	public class FaceInfo
	{
		[DataMember()]
		public string Name_ { get; set; }

		[DataMember()]
		public string PreviewPath_ { get; set; }

		[DataMember()]
		public List<FacePartInfo> FaceParts_ { get; set; }
	}
}
