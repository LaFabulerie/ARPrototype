using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class UIAugmentedImageBody : MonoBehaviour
	{
		public GameObject ArCoreDeviceObj_ { get => GameObject.Find("ARCore Device"); }

		public GameObject CanvasObj_ { get => GameObject.Find("Canvas"); }

		public GameObject WorkerObj_ { get => GameObject.Find("Worker"); }

		public GameObject LoadingObj_ { get => transform.Find("Loading").gameObject; }

		private Vector2 beginPos_;

		private Vector2 beginOffset_;

		private GameObject currentSurfaceObj_;

		private Coroutine loadingCoroutine_;

		[SerializeField]
		private Vector2 iconSize_;

		private void OnDrag(Vector2 pos_)
		{
			var arCoreDeviceC_ = ArCoreDeviceObj_.GetComponentInChildren<Camera>();
			var ray_ = arCoreDeviceC_.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray_, out RaycastHit hit_))
			{
				var currentSurfaceObj_ = hit_.transform.gameObject;

				beginPos_ = pos_;

				var as_ = currentSurfaceObj_.GetComponent<AugmentedImageSurface>();
				beginOffset_ = as_.matchingImage_.offset_;

				this.currentSurfaceObj_ = currentSurfaceObj_;
			}
		}

		private void Drag(Vector2 pos_, Vector2 dir_, float mag_, UIDragDirection cardDir_)
		{

			if (currentSurfaceObj_ != null)
			{
				var posNorm_ = (pos_ - beginPos_) / new Vector2(Screen.width, Screen.height);
				posNorm_ *= 0.25f;

				var as_ = currentSurfaceObj_.GetComponent<AugmentedImageSurface>();
				as_.matchingImage_.offset_ = beginOffset_ + new Vector2(posNorm_.x, posNorm_.y);
			}
		}

		private void OffDrag(Vector2 pos_, Vector2 dir_, float mag_, UIDragDirection cardDir_)
		{
			currentSurfaceObj_ = null;
		}

		private IEnumerator DisplayLoading()
		{
			var loadingRT_ = LoadingObj_.GetComponent<RectTransform>();

			var angle_ = 360f / 25f;
			var delay_ = 1f / 25f;

			var workerObjAIM_ = WorkerObj_.GetComponent<AugmentedImageWorker>();

			while (true)
			{
				LoadingObj_.gameObject.SetActive(true);

				while (workerObjAIM_.IsLoading_)
				{
					loadingRT_.Rotate(Vector3.back, angle_);
					yield return new WaitForSeconds(delay_);
				}

				LoadingObj_.gameObject.SetActive(false);

				yield return new WaitWhile(() => !workerObjAIM_.IsLoading_);
			}
		}

		private void DrawUI(float scale_)
		{
			var LoadingObjRT_ = LoadingObj_.GetComponent<RectTransform>();
			var scaledIconSize_ = iconSize_ * scale_;
			LoadingObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledIconSize_.x);
			LoadingObjRT_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledIconSize_.y);
		}

		private void Start()
		{
			loadingCoroutine_ = StartCoroutine(DisplayLoading());

			var uit_ = GetComponentInChildren<UITouch>();

			uit_.OneBeginDrag += OnDrag;
			uit_.OneDrag += Drag;
			uit_.OneEndDrag += OffDrag;

			var canvasObjUICS_ = CanvasObj_.GetComponent<UICanvasScaler>();
			canvasObjUICS_.ChangedScale += DrawUI;
			canvasObjUICS_.InvokeChangedScaleEvent();
		}

		private void OnDestroy()
		{
			StopCoroutine(loadingCoroutine_);
		}
	}
}
