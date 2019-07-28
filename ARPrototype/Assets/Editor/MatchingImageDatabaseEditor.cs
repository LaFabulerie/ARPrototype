using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
	[CustomEditor(typeof(MatchingImageDatabase))]
	[CanEditMultipleObjects()]
	public class MatchingImageDatabaseEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("augmentedImages_"), new GUIContent("Detectors Database"));

			var changed_ = serializedObject.ApplyModifiedProperties();

			var matchingImageDatabase_ = (MatchingImageDatabase)target;

			if (changed_)
			{
				if (!matchingImageDatabase_.NoDetectorsDatabase_)
				{
					matchingImageDatabase_.LoadReferenceImages();
				}
			}

			EditorGUILayout.Space();

			if (matchingImageDatabase_.NoDetectorsDatabase_)
			{
				EditorGUILayout.LabelField("Please select a detectors database to continue.");
			}
			else
			{
				var matchingImages_ = serializedObject.FindProperty("matchingImages_");

				for (int i_ = 0; i_ < matchingImages_.arraySize; i_++)
				{
					EditorGUILayout.PropertyField(matchingImages_.GetArrayElementAtIndex(i_));
				}

				serializedObject.ApplyModifiedProperties();
			}

			serializedObject.UpdateIfRequiredOrScript();
		}
	}
}
