using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
	[CustomEditor(typeof(MatchingImageDatabase))]
	public class CombinedImageDatabaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("augmentedImages_"), new GUIContent("Augmented Image Database"));

			var changed_ = EditorGUI.EndChangeCheck();

			serializedObject.ApplyModifiedProperties();

			serializedObject.Update();

			var aa_ = serializedObject.FindProperty("augmentedImages_").objectReferenceValue != null;

			var matchingImages_ = serializedObject.FindProperty("matchingImages_");

			if (!aa_)
			{
				matchingImages_.ClearArray();
			}

			if (aa_ && changed_)
			{
				((MatchingImageDatabase)target).LoadReferenceImages();
			}

			for (int i_ = 0; i_ < matchingImages_.arraySize; i_++)
			{
				var matchingImage_ = matchingImages_.GetArrayElementAtIndex(i_);

				EditorGUILayout.PropertyField(matchingImage_);

				if (i_ < matchingImages_.arraySize - 1)
				{
					EditorGUILayout.Space();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}