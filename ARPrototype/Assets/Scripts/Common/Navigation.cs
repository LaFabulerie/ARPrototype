using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class Navigation : MonoBehaviour
	{
		public static Navigation Current_ { get => GameObject.Find("AppManager").GetComponent<Navigation>(); }

		private AsyncOperation loader_;

		public event Action<AsyncOperation> Change;

		private void ResetLoader(AsyncOperation op_)
		{
			op_.completed -= ResetLoader;
			loader_ = null;
		}

		public void LoadMainMenu()
		{
			if (loader_ == null)
			{
				loader_ = SceneManager.LoadSceneAsync("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
				loader_.completed += ResetLoader;
				Change?.Invoke(loader_);
			}
		}

		public void LoadAugmentedImageScene()
		{
			if (loader_ == null)
			{
				loader_ = SceneManager.LoadSceneAsync("Assets/Scenes/AugmentedImage.unity", LoadSceneMode.Single);
				loader_.completed += ResetLoader;
				Change?.Invoke(loader_);
			}
		}

		public void LoadAugmentedFaceScene()
		{
			if (loader_ == null)
			{
				loader_ = SceneManager.LoadSceneAsync("Assets/Scenes/AugmentedFace.unity", LoadSceneMode.Single);
				loader_.completed += ResetLoader;
				Change?.Invoke(loader_);
			}
		}

		public void LoadAugmentedFaceCreatorScene()
		{
			if (loader_ == null)
			{
				loader_ = SceneManager.LoadSceneAsync("Assets/Scenes/AugmentedFaceCreator.unity", LoadSceneMode.Single);
				loader_.completed += ResetLoader;
				Change?.Invoke(loader_);
			}
		}
	}
}
