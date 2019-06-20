using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
	[CustomPropertyDrawer(typeof(MatchingImage))]
	public class MatchingImageEditor : PropertyDrawer
	{
		private int lineCount_ = 13;

		private static Dictionary<int, Color> colors_ = new Dictionary<int, Color>();

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * lineCount_;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var hashCode_ = property.FindPropertyRelative("hashCode_").intValue;

			if (!colors_.ContainsKey(hashCode_))
			{
				colors_.Add(hashCode_, Color.red);
			}

			EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.25f;

			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, new GUIContent($"Index: {property.FindPropertyRelative("databaseIndex_").intValue.ToString()}"));

			position.height /= lineCount_;

			EditorGUI.LabelField(position, new GUIContent($"Name: {property.FindPropertyRelative("entry_").FindPropertyRelative("Name").stringValue}"));

			position.y += position.height;
			EditorGUI.LabelField(position, new GUIContent($"Quality: {property.FindPropertyRelative("entry_").FindPropertyRelative("Quality").stringValue}"));

			position.y += position.height;
			EditorGUI.LabelField(position, new GUIContent($"Width (in meter): {property.FindPropertyRelative("entry_").FindPropertyRelative("Width").floatValue.ToString()}"));

			position.y += position.height;
			var width_ = position.width;
			var height_ = position.height;
			position.height *= 5;
			GUIStyle style = new GUIStyle();
			var referenceTexture_ = (Texture2D)property.FindPropertyRelative("referenceImage").objectReferenceValue;
			style.normal.background = referenceTexture_;
			position.width = position.height * ((float)referenceTexture_.width / referenceTexture_.height);
			EditorGUI.LabelField(position, GUIContent.none, style);
			position.width = width_;
			position.height = height_;

			EditorGUI.BeginChangeCheck();
			position.y += position.height * 5;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("type_"), new GUIContent("Media type"));
			property.FindPropertyRelative("changed_").boolValue = EditorGUI.EndChangeCheck();
			if (property.FindPropertyRelative("changed_").boolValue)
			{
				colors_[hashCode_] = Color.red;
				colors_[hashCode_] = Color.red;
				property.FindPropertyRelative("image_").objectReferenceValue = null;
				property.FindPropertyRelative("imagesFolderPath_").stringValue = string.Empty;
				property.FindPropertyRelative("images_").ClearArray();
				property.FindPropertyRelative("video_").objectReferenceValue = null;
			}

			var x_ = position.x;
			position.y += position.height;
			position.width -= position.height;

			switch ((MatchingImageType)property.FindPropertyRelative("type_").enumValueIndex)
			{
				case MatchingImageType.Image:
					EditorGUI.BeginChangeCheck();
					EditorGUI.PropertyField(position, property.FindPropertyRelative("image_"), GUIContent.none);
					property.FindPropertyRelative("changed_").boolValue = EditorGUI.EndChangeCheck();
					if (property.FindPropertyRelative("image_").objectReferenceValue != null)
					{
						colors_[hashCode_] = Color.green;
					}
					position.x = EditorGUIUtility.labelWidth + width_;
					position.width = position.height;
					EditorGUI.DrawRect(position, colors_[hashCode_]);
					position.width = width_;
					position.x = x_;
					break;
				case MatchingImageType.Images:
					EditorGUI.BeginChangeCheck();
					EditorGUI.PropertyField(position, property.FindPropertyRelative("imagesFolderPath_"), new GUIContent("Images folder path"));
					property.FindPropertyRelative("changed_").boolValue = EditorGUI.EndChangeCheck();
					if (property.FindPropertyRelative("changed_").boolValue)
					{
						var path_ = property.FindPropertyRelative("imagesFolderPath_").stringValue;

						if (Directory.Exists(path_))
						{
							colors_[hashCode_] = Color.blue;

							var filePaths_ = Directory.GetFiles(path_);
							Array.Sort(filePaths_);

							property.FindPropertyRelative("images_").ClearArray();

							for (int i_ = 0, j_ = 0; i_ < filePaths_.Length; i_++)
							{
								var texture_ = AssetDatabase.LoadAssetAtPath<Texture2D>(filePaths_[i_]);

								if (texture_ == null)
								{
									continue;
								}

								property.FindPropertyRelative("images_").InsertArrayElementAtIndex(j_);
								property.FindPropertyRelative("images_").GetArrayElementAtIndex(j_).objectReferenceValue = texture_;
								++j_;
							}
						}
						else
						{
							colors_[hashCode_] = Color.red;
						}
					}
					if (property.FindPropertyRelative("images_").arraySize > 0)
					{
						colors_[hashCode_] = Color.green;
					}
					position.x = EditorGUIUtility.labelWidth + width_;
					position.width = position.height;
					EditorGUI.DrawRect(position, colors_[hashCode_]);
					position.width = width_;
					position.x = x_;
					if (property.FindPropertyRelative("images_").arraySize > 0)
					{
						lineCount_ = 15;
						position.y += position.height;
						EditorGUI.LabelField(position, new GUIContent($"Images found: {property.FindPropertyRelative("images_").arraySize}"));
					}
					else
					{
						lineCount_ = 14;
					}
					position.y += position.height;
					EditorGUI.PropertyField(position, property.FindPropertyRelative("timeInterFrame_"), new GUIContent("Time inter frame (in second)"));
					break;
				case MatchingImageType.Video:
					EditorGUI.BeginChangeCheck();
					EditorGUI.PropertyField(position, property.FindPropertyRelative("video_"), GUIContent.none);
					property.FindPropertyRelative("changed_").boolValue = EditorGUI.EndChangeCheck();
					if (property.FindPropertyRelative("video_").objectReferenceValue != null)
					{
						colors_[hashCode_] = Color.green;
					}
					position.x = EditorGUIUtility.labelWidth + width_;
					position.width = position.height;
					EditorGUI.DrawRect(position, colors_[hashCode_]);
					position.width = width_;
					position.x = x_;
					break;
			}

			if (property.FindPropertyRelative("material_").objectReferenceValue == null)
			{
				property.FindPropertyRelative("material_").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Screen.mat");
			}
			position.y += position.height;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("material_"), new GUIContent("Material"));

			if (string.IsNullOrWhiteSpace(property.FindPropertyRelative("textureName_").stringValue))
			{
				property.FindPropertyRelative("textureName_").stringValue = "_MainTex";
			}
			position.y += position.height;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("textureName_"), new GUIContent("Texture Name"));

			position.y += position.height;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("offset_"), new GUIContent("Offset (in ratio)"));

			EditorGUI.EndProperty();
		}
	}
}