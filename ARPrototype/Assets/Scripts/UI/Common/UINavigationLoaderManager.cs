using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class UINavigationLoaderManager : MonoBehaviour
	{
		private Navigation navigation_;

		private GameObject backgroundObj_;
		private GameObject hourglassObj_;

		private Coroutine rotateCoroutine_;

		private IEnumerator Rotate(AsyncOperation op_)
		{
			var hourglassRT_ = hourglassObj_.GetComponent<RectTransform>();

			var angle_ = 360f / 25f;
			var delay_ = 1f / 25f;

			backgroundObj_.gameObject.SetActive(true);

			while (true)
			{
				hourglassRT_.Rotate(Vector3.back, angle_);
				yield return new WaitForSeconds(delay_);
			}

			//backgroundObj_.gameObject.SetActive(false);

			//rotateCoroutine_ = null;
		}

		private void Run(AsyncOperation op_)
		{
			if (rotateCoroutine_ == null)
			{
				rotateCoroutine_ = StartCoroutine(Rotate(op_));
			}
		}

		private void OnEnable()
		{
			navigation_ = Navigation.Current_;

			backgroundObj_ = transform.Find("Background")?.gameObject;
			hourglassObj_ = backgroundObj_.transform.Find("Hourglass")?.gameObject;

			navigation_.Change += Run;
		}

		private void OnDisable()
		{
			StopAllCoroutines();

			navigation_.Change -= Run;
		}
	}
}