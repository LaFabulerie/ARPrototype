using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	public class AugmentedFaceCreatorSurfaceMesh : MonoBehaviour
	{
		public Vector2 Size_ { get; private set; }

		/// <summary>
		/// Indique si le composant est initialisé.
		/// </summary>
		public bool IsInitialized_ { get; private set; }

		/// <summary>
		/// Intialise le composant.
		/// </summary>
		/// <param name="cameraObj_"></param>
		/// <param name="widthRatio_">Le ratio de largeur de la surface par rapport à la caméra.</param>
		/// <param name="heightRatio_">Le ratio de hauteur de la surface par rapport à la caméra</param>
		public void Initialize(GameObject cameraObj_, float widthRatio_  = 1f, float heightRatio_ = 1f)
		{
			var cameraObjAFCC_ = cameraObj_.GetComponent<AugmentedFaceCreatorCamera>();

			var cameraRect_ = cameraObjAFCC_.RectBounds_;

			Size_ = new Vector2(cameraRect_.width * widthRatio_, cameraRect_.height * heightRatio_);

			var w_ = Size_.x * 0.5f;
			var h_ = Size_.y * 0.5f;

			var mesh_ = new Mesh();

			mesh_.SetVertices(new List<Vector3>()
			{
				new Vector3(-w_, -h_),
				new Vector3(-w_, h_),
				new Vector3(w_, -h_),
				new Vector3(w_, h_)
			});

			mesh_.SetTriangles(new int[]
			{
				0,
				1,
				2,
				1,
				3,
				2
			}, 0);

			mesh_.SetUVs(0, new List<Vector2>()
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f)
			});

			GetComponent<MeshFilter>().mesh = mesh_;
			GetComponent<MeshCollider>().sharedMesh = mesh_;

			IsInitialized_ = true;
		}

		public void ChangeTexture(Texture2D texture_)
		{

			GetComponent<MeshRenderer>().material.mainTexture = texture_;
		}
	}
}
