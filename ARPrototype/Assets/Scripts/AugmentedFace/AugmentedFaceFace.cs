using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class AugmentedFaceFace : MonoBehaviour
	{
		public void UpdateTexture(Text debug_, FacePartInfo facePartInfo_)
		{
			var mr_ = GetComponent<MeshRenderer>();
			mr_.material.mainTexture = FacePartInfo.GetImage(facePartInfo_.TexturePath_, facePartInfo_.TextureWidth_, facePartInfo_.TextureHeight);
		}

		public void UpdateMesh(Text debug_, List<Vector3>  vertices_, List<int> triangles_, List<Vector3> normals_, List<Vector2> uvs_)
		{
			// Initialise le maillage du vissage

			var mf_ = GetComponent<MeshFilter>();
			mf_.mesh.SetVertices(vertices_);
			mf_.mesh.SetTriangles(triangles_, 0, true);
			mf_.mesh.SetNormals(normals_);
			mf_.mesh.SetUVs(0, uvs_);
			mf_.mesh.RecalculateTangents();
		}

		private void Start()
		{
			var mf_ = GetComponent<MeshFilter>();

			mf_.mesh = new Mesh();
			mf_.mesh.MarkDynamic();
		}
	}
}