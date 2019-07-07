using GoogleARCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class AugmentedFaceWorker : MonoBehaviour
	{
		private List<AugmentedFace> foundFaces_;
		private List<Vector3> vertices_;
		private List<int> triangles_;
		private List<Vector3> normals_;
		private List<Vector2> uvs_;

		[SerializeField]
		private Text debug_;

		[SerializeField]
		private GameObject objectPrefab_;

		public GameObject FaceObj_ => transform.Find("Face").gameObject;

		public FaceInfo FaceInfo_ { get; private set; }

		public bool IsInitialized_ { get; private set; }

		public void SetFace(FaceInfo faceInfo_)
		{
			FaceInfo_ = faceInfo_;

			for (int i_ = 0; i_ < FaceObj_.transform.childCount; i_++)
			{
#if UNITY_EDITOR
				DestroyImmediate(FaceObj_.transform.GetChild(i_).gameObject);
#else
				Destroy(FaceObj_.transform.GetChild(i_).gameObject);
#endif
			}

			IsInitialized_ = false;
		}

		private void Start()
		{
			foundFaces_ = new List<AugmentedFace>();
			vertices_ = new List<Vector3>();
			triangles_ = new List<int>();
			normals_ = new List<Vector3>();
			uvs_ = new List<Vector2>();
		}

		private void Update()
		{
			// Récupère le vissage.
			Session.GetTrackables(foundFaces_, TrackableQueryFilter.All);

			// Vérifie si un vissage est bien récupéré.
			var foundFace_ = foundFaces_.FirstOrDefault();

			if (FaceInfo_ != null &&
				foundFace_ != null)
			{
				var faceObjAFF_ = FaceObj_.GetComponent<AugmentedFaceFace>();

				if (!IsInitialized_)
				{
					var facePartInfo_ = (from facePart_ in FaceInfo_.FaceParts_
										 where facePart_.IsReference_ == true
										 select facePart_).FirstOrDefault();

					var objectPartInfos_ = from objectPart_ in FaceInfo_.FaceParts_
										   where objectPart_ != facePartInfo_
										   select objectPart_;

					if (facePartInfo_ == null)
					{
						return;
					}

					faceObjAFF_.UpdateTexture(debug_, facePartInfo_);

					foreach (var objectPartInfo_ in objectPartInfos_)
					{
						var objectObj_ = Instantiate(objectPrefab_, Vector3.zero, Quaternion.identity, FaceObj_.transform);
						var objectObjAFO_ = objectObj_.GetComponent<AugmentedFaceObject>();
						objectObjAFO_.Initialiaze(debug_, objectPartInfo_, foundFace_);
					}

					FaceObj_.SetActive(true);

					IsInitialized_ = true;
				}

				foundFace_.GetVertices(vertices_);
				foundFace_.GetTriangleIndices(triangles_);
				foundFace_.GetNormals(normals_);
				foundFace_.GetTextureCoordinates(uvs_);

				faceObjAFF_.UpdateMesh(debug_, vertices_, triangles_, normals_, uvs_);

				// Définit la position et la rotation de l'objet portant le vissage.
				FaceObj_.transform.SetPositionAndRotation(foundFace_.CenterPose.position, foundFace_.CenterPose.rotation);
			}
			else
			{				
				for (int i_ = 0; i_ < FaceObj_.transform.childCount; i_++)
				{
#if UNITY_EDITOR
					DestroyImmediate(FaceObj_.transform.GetChild(i_).gameObject);
#else
					Destroy(FaceObj_.transform.GetChild(i_).gameObject);
#endif
				}

				FaceObj_.SetActive(false);

				IsInitialized_ = false;
			}
		}
	}
}