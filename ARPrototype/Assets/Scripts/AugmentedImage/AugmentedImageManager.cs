using GoogleARCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts
{
	public class AugmentedImageManager : MonoBehaviour
	{
		private List<AugmentedImage> updatedImages_;
		private Dictionary<int, GameObject> trackedObjects_;

		public MatchingImageDatabase matchingImages_;

		void Start()
		{
			updatedImages_ = new List<AugmentedImage>();
			trackedObjects_ = new Dictionary<int, GameObject>();
		}

		void Update()
		{
			// Retourne si le tracking n'est pas en cours.
			if (Session.Status != SessionStatus.Tracking)
			{
				return;
			}

			// Récupère les objets traçable mis à jour.
			Session.GetTrackables(updatedImages_, TrackableQueryFilter.Updated);

			// Pour chacun des objets traçable...
			foreach (var updatedImage_ in updatedImages_)
			{
				// Si l'image est suivi et qu'aucun objet de suivi existe.
				if (!trackedObjects_.ContainsKey(updatedImage_.DatabaseIndex) &&
					updatedImage_.TrackingState == TrackingState.Tracking)
				{
					// Instancit un nouvelle objet suivi au niveau de l'ancre.
					trackedObjects_[updatedImage_.DatabaseIndex] = new GameObject($"Surface {updatedImage_.DatabaseIndex}", typeof(AugmentedImageSurface), typeof(MeshFilter), typeof(MeshRenderer), typeof(ImagePlayer), typeof(VideoPlayer), typeof(MeshCollider));

					// Crée l'ancre de l'objet à instancier.
					var anchor_ = updatedImage_.CreateAnchor(updatedImage_.CenterPose);

					//
					trackedObjects_[updatedImage_.DatabaseIndex].transform.SetParent(anchor_.transform);
					trackedObjects_[updatedImage_.DatabaseIndex].transform.localPosition = Vector3.zero;
					trackedObjects_[updatedImage_.DatabaseIndex].transform.localRotation = Quaternion.identity;
					trackedObjects_[updatedImage_.DatabaseIndex].transform.localScale = Vector3.one;

					// Initialise...
					trackedObjects_[updatedImage_.DatabaseIndex].GetComponent<AugmentedImageSurface>().Inititialize(updatedImage_, matchingImages_[updatedImage_.DatabaseIndex]);
				}

				// Met à jour le temps.
				if (trackedObjects_.ContainsKey(updatedImage_.DatabaseIndex))
				{
					//
					trackedObjects_[updatedImage_.DatabaseIndex].GetComponent<AugmentedImageSurface>().LastUpdatedTime_ = Time.time;
				}
			}

			var keyToDeletes_ = from item_ in trackedObjects_ where item_.Value.GetComponent<AugmentedImageSurface>().IsOld_ select item_.Key;

			foreach (var keyToDelete_ in keyToDeletes_)
			{
				Destroy(trackedObjects_[keyToDelete_]);
				trackedObjects_.Remove(keyToDelete_);
			}
		}
	}
}