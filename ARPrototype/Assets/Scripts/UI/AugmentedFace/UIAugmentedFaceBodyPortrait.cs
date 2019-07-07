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
		public GameObject CanvasObj_ => GameObject.Find("Canvas");

		public GameObject ScrollViewFacesObj_ => transform.Find("ScrollView-Faces").gameObject;

		public GameObject ScrollViewFacesContentObj_ => ScrollViewFacesObj_.transform.Find("Viewport").Find("Content").gameObject;

		[SerializeField]
		private float margin_;

		[SerializeField]
		private Vector2 buttonSize_;

		[SerializeField]
		private GameObject facePrefab_;

		private void LoadFaces()
		{
			for (int i_ = 0; i_ < ScrollViewFacesContentObj_.transform.childCount; i_++)
			{
#if UNITY_EDITOR
				DestroyImmediate(ScrollViewFacesContentObj_.transform.GetChild(i_).gameObject);
#else
				Destroy(ScrollViewFacesContentObj_.transform.GetChild(i_).gameObject);
#endif
			}

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

				var faceObj_ = Instantiate(facePrefab_, Vector3.zero, Quaternion.identity, ScrollViewFacesContentObj_.transform);
				faceObj_.GetComponent<AugmentedFaceFaceData>().Initialiaze(Path.GetFileName(folderName_), faceInfo_);
			}
		}

		private void DrawUI(float scale_)
		{
		}

		private void Start()
		{
			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();

			LoadFaces();
		}
	}
}