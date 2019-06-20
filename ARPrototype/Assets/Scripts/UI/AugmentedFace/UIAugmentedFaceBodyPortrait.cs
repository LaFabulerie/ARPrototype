using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class UIAugmentedFaceBodyPortrait : MonoBehaviour
	{
		private GameObject canvasObj_;

		private GameObject scrollViewFacesObj_;
		private GameObject scrollViewFacesContentObj_;

		private UITouch uit_;

		[SerializeField]
		private float margin_;

		[SerializeField]
		private Vector2 buttonSize_;

		[SerializeField]
		private GameObject facePrefab_;

		public List<GameObject> faceObjs_ { get; private set; }

		private void LoadFaces()
		{
			var rootPath_ = Path.Combine(Application.persistentDataPath, "faces");

			if (!Directory.Exists(rootPath_))
			{
				return;
			}

			var foldersNames_ = Directory.EnumerateDirectories(rootPath_);

			foreach (var folderName_ in foldersNames_)
			{
				var jsonPath_ = Path.Combine(folderName_, $"{Path.GetFileName(folderName_)}.json");

				var jsonBytes_ = File.ReadAllBytes(jsonPath_);

				var stream_ = new MemoryStream(jsonBytes_);
				var serializer_ = new DataContractJsonSerializer(typeof(FaceInfo));
				var faceInfo_ = serializer_.ReadObject(stream_) as FaceInfo;
				stream_.Close();

				var faceObj_ = Instantiate(facePrefab_, Vector3.zero, Quaternion.identity, scrollViewFacesContentObj_.transform);
				faceObj_.GetComponent<AugmentedFaceFaceData>().Initialiaze(Path.GetFileName(folderName_), faceInfo_);
				faceObjs_.Add(faceObj_);
			}
		}

		private void DrawUI(float scale_)
		{
		}

		private void Start()
		{
			canvasObj_ = GameObject.Find("Canvas");

			scrollViewFacesObj_ = transform.Find("ScrollView-Faces").gameObject;
			scrollViewFacesContentObj_ = scrollViewFacesObj_.transform.Find("Viewport").Find("Content").gameObject;

			var canvasObjUICS_ = canvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();

			faceObjs_ = new List<GameObject>();

			LoadFaces();
		}
	}
}