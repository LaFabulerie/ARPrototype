using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	[RequireComponent(typeof(MeshRenderer))]
	public class ImagePlayer : MonoBehaviour
	{
		private Coroutine currentCoroutine_;

		public Text text_;
		public List<Texture2D> images_;
		public float timeInterFrame_;
		public string textureName_;
		public bool isLooping_;

		public bool IsPlaying_ { get; private set; }
		public bool IsPaused_ { get; private set; }

		private IEnumerator PlayCoroutine()
		{
			IsPlaying_ = true;

			foreach (var image_ in images_)
			{
				if (IsPaused_)
				{
					yield return null;
				}

				GetComponent<MeshRenderer>().material.SetTexture(textureName_, image_);

				yield return new WaitForSeconds(timeInterFrame_);
			}

			if (isLooping_)
			{
				currentCoroutine_ = StartCoroutine(PlayCoroutine());
			}

			IsPlaying_ = false;
		}

		public void Play()
		{
			IsPaused_ = false;

			if (currentCoroutine_ == null)
			{
				currentCoroutine_ = StartCoroutine(PlayCoroutine());
			}
		}

		public void Pause()
		{
			IsPaused_ = !IsPaused_;
		}

		public void Stop()
		{
			if (currentCoroutine_ != null)
			{
				StopCoroutine(currentCoroutine_);
				currentCoroutine_ = null;
			}
		}

		private void OnDestroy()
		{
			Stop();
		}
	}
}