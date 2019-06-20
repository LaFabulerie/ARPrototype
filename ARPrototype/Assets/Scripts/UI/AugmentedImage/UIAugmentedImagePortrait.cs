using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class UIAugmentedImagePortrait : MonoBehaviour
	{
		private GameObject surfaceObj;

		private UITouch uit_;

		private Vector2 beginPos_;

		private Camera arCoreDeviceC_;

		private Vector2 beginOffset_;

		private GameObject currentSurfaceObj_;

		[SerializeField]
		private GameObject arCoreDeviceObj_;

		private void OnDrag(Vector2 pos_)
		{
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

		private void Start()
		{
			surfaceObj = transform.Find("Surface").gameObject;
			var surfaceRT_ = surfaceObj.GetComponent<RectTransform>();

			//surfaceRT_.offsetMax = new Vector2(0f, 98f * -UICanvasScaler.Scale_);

			uit_ = GetComponentInChildren<UITouch>();

			uit_.OneBeginDrag += OnDrag;
			uit_.OneDrag += Drag;
			uit_.OneEndDrag += OffDrag;

			arCoreDeviceC_ = arCoreDeviceObj_.GetComponentInChildren<Camera>();
		}
	}
}
