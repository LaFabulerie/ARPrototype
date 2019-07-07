using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class AugmentedFaceObject : MonoBehaviour
	{
		public void Initialiaze(Text debug_, FacePartInfo objectPartInfo_, AugmentedFace face_)
		{
			var px_ = objectPartInfo_.PositionOffset_.x * 1f / objectPartInfo_.ReferenceScale_.x;
			var py_ = objectPartInfo_.PositionOffset_.y * 1f / objectPartInfo_.ReferenceScale_.y;
			var pz_ = objectPartInfo_.PositionOffset_.z * 1f / objectPartInfo_.ReferenceScale_.z;

			transform.localPosition = new Vector3(px_, py_, pz_);
			transform.localRotation = objectPartInfo_.Rotation_;

			if (objectPartInfo_.Is2D_)
			{
				transform.localScale = objectPartInfo_.Scale_;

				var mf_ = GetComponent<MeshFilter>();
				mf_.mesh = new Mesh();

				var hW_ = objectPartInfo_.Dimension_.x * 0.5f * (1f / objectPartInfo_.ReferenceScale_.x);
				var hH_ = objectPartInfo_.Dimension_.y * 0.5f * (1f / objectPartInfo_.ReferenceScale_.y);

				mf_.mesh.SetVertices(new List<Vector3>()
				{
					new Vector3(-hW_, -hH_),
					new Vector3(-hW_, hH_),
					new Vector3(hW_, hH_),
					new Vector3(hW_, -hH_)
				});

				mf_.mesh.SetTriangles(new List<int>()
				{
					0,
					1,
					3,
					1,
					2,
					3,
				}, 0);

				mf_.mesh.SetNormals(new List<Vector3>()
				{
					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
					Vector3.forward
				});

				mf_.mesh.SetUVs(0, new List<Vector2>()
				{
					new Vector2(0f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
					new Vector2(1f, 0f)
				});
			}
			else
			{
				var sx_ = objectPartInfo_.Scale_.x * 1f / objectPartInfo_.ReferenceScale_.x;
				var sy_ = objectPartInfo_.Scale_.y * 1f / objectPartInfo_.ReferenceScale_.y;
				var sz_ = objectPartInfo_.Scale_.z * 1f / objectPartInfo_.ReferenceScale_.z;

				transform.localScale = new Vector3(sx_, sy_, sz_);

				if (objectPartInfo_.MeshObjIndex_ < AppManager.Current_.Meshs_.Count)
				{
					var mesh_ = AppManager.Current_.Meshs_[objectPartInfo_.MeshObjIndex_];
					GetComponent<MeshFilter>().mesh = mesh_;
				}
			}

			var mr_ = GetComponent<MeshRenderer>();
			mr_.material.mainTexture = FacePartInfo.GetImage(objectPartInfo_.TexturePath_, objectPartInfo_.TextureWidth_, objectPartInfo_.TextureHeight);
		}
	}
}
