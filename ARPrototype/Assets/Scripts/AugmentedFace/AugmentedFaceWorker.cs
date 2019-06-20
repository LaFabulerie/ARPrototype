using GoogleARCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class AugmentedFaceWorker : MonoBehaviour
	{
		private List<AugmentedFace> foundFaces_;
		private List<Vector3> vertices_;
		private List<int> triangles_;
		private List<Vector3> normals_;
		private List<Vector2> uvs_;

		private GameObject faceObj_;
		private MeshFilter faceMF_;
		private MeshRenderer faceMR_;

		private Texture2D faceTexture_;

		[SerializeField]
		private Material faceMaterial_;

		public Texture2D FaceTexture_
		{
			get => faceTexture_;
			set
			{
				faceTexture_ = value;
				faceMR_.material.SetTexture("_MainTex", faceTexture_);
			}
		}

		public void ChangeFace(FaceInfo faceInfo_)
		{


		}

		private void Start()
		{
			foundFaces_ = new List<AugmentedFace>();
			vertices_ = new List<Vector3>();
			triangles_ = new List<int>();
			normals_ = new List<Vector3>();
			uvs_ = new List<Vector2>();

			faceObj_ = new GameObject("Face", typeof(MeshFilter), typeof(MeshRenderer));
			faceMF_ = faceObj_.GetComponent<MeshFilter>();
			faceMR_ = faceObj_.GetComponent<MeshRenderer>();

			faceObj_.transform.SetParent(transform);
			faceMF_.mesh = new Mesh();
			faceMR_.material = faceMaterial_;
		}

		private void Update()
		{

			// Récupère le vissage.
			Session.GetTrackables(foundFaces_, TrackableQueryFilter.All);

			// Vérifie si un vissage est bien récupéré.
			var foundFace_ = foundFaces_.FirstOrDefault();

			if (FaceTexture_ != null &&
				foundFace_ != null)
			{
				// Définit la position et la rotation de l'objet portant le vissage.
				faceObj_.transform.SetPositionAndRotation(foundFace_.CenterPose.position, foundFace_.CenterPose.rotation);

				// Récupères les données.
				foundFace_.GetVertices(vertices_);
				foundFace_.GetTriangleIndices(triangles_);
				foundFace_.GetNormals(normals_);
				foundFace_.GetTextureCoordinates(uvs_);

				// Recalcul le maillage du vissage.
				faceMF_.mesh.SetVertices(vertices_);
				faceMF_.mesh.SetTriangles(triangles_, 0);
				faceMF_.mesh.SetNormals(normals_);
				faceMF_.mesh.SetUVs(0, uvs_);
				faceMF_.mesh.RecalculateTangents();
				faceMF_.mesh.RecalculateBounds();

				// Affiche le vissage.
				faceObj_.SetActive(true);
			}
			else
			{
				// Masque le vissage.
				faceObj_.SetActive(false);
			}
		}
	}
}