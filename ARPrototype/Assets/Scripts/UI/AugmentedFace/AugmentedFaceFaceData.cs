using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[ExecuteAlways()]
	public class AugmentedFaceFaceData : MonoBehaviour
	{
		public GameObject Worker_ => GameObject.Find("Worker");

		public GameObject Body_ => GameObject.Find("Body");

		public FaceInfo FaceInfo_ { get; private set; }

		private void SetupTouch()
		{
			var uit_ = GetComponent<UITouch>();
			var BodyPortraitSR_ = Body_.GetComponent<UIAugmentedFaceBody>().ScrollViewFacesObj_.GetComponent<ScrollRect>();

			uit_.BeginDrag += (e_) => { BodyPortraitSR_.OnBeginDrag(e_); };
			uit_.Drag += (e_) => { BodyPortraitSR_.OnDrag(e_); };
			uit_.EndDrag += (e_) => { BodyPortraitSR_.OnEndDrag(e_); };

			uit_.OneEndDrag += (pos_, dir_, mag_, cardDir_) =>
			{
				if (cardDir_ == UIDragDirection.Up)
				{
					var WorkerAFW_ = Worker_.GetComponent<AugmentedFaceWorker>();
					WorkerAFW_.SetFace(FaceInfo_);
				}

				if (cardDir_ == UIDragDirection.Down)
				{
					//#if UNITY_EDITOR
					//					DestroyImmediate(gameObject);
					//#else
					//					Destroy(gameObject);
					//#endif
					//					Directory.Delete(FaceInfo_.FolderPath_, true);

					var WorkerAFW_ = Worker_.GetComponent<AugmentedFaceWorker>();
					WorkerAFW_.SetFace(null);
				}
			};
		}

		public void Initialiaze(string name_, FaceInfo faceInfo_)
		{
			FaceInfo_ = faceInfo_;

			name = name_;

			var i_ = GetComponent<RawImage>();
			i_.texture = FacePartInfo.GetImage(FaceInfo_.PreviewPath_);

			SetupTouch();
		}
	}
}
