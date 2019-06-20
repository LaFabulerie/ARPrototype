using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class AugmentedFaceFaceData : MonoBehaviour
	{
		public FaceInfo FaceInfo_ { get; private set; }

		private Texture2D GetImage(string path_, int width_ = 1024, int height_ = 1024)
		{
			if (!File.Exists(path_))
			{
				return null;
			}

			var previewBytes_ = File.ReadAllBytes(path_);

			var previewTex_ = new Texture2D(width_, height_, TextureFormat.ARGB32, false);
			previewTex_.LoadImage(previewBytes_);

			return previewTex_;
		}

		private void SetupTouch()
		{
			var uit_ = GetComponent<UITouch>();

			var workerObj_ = GameObject.Find("Worker");
			var workerObjAFW_ = workerObj_.GetComponent<AugmentedFaceWorker>();

			uit_.OneEndDrag += (pos_, dir_, mag_, cardDir_) =>
			{
				if (cardDir_ != UIDragDirection.Up)
				{
					return;
				}

				workerObjAFW_.ChangeFace(FaceInfo_);
			};
		}

		public void Initialiaze(string name_,  FaceInfo faceInfo_)
		{
			FaceInfo_ = faceInfo_;

			name = name_;

			var i_ = GetComponent<RawImage>();
			i_.texture = GetImage(FaceInfo_.PreviewPath_);

			SetupTouch();
		}
	}
}
