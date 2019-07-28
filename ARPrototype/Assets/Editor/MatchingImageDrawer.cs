using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
	[CustomPropertyDrawer(typeof(MatchingImage))]
	public class MatchingImageDrawer : PropertyDrawer
	{
		private float height_;

		public override void OnGUI(Rect position_, SerializedProperty property_, GUIContent label_)
		{
			EditorGUI.BeginProperty(position_, label_, property_);

			// Reinitialise les valeurs.
			var indentLevel_ = EditorGUI.indentLevel;
			height_ = 0f;

			// Label
			EditorGUI.indentLevel = 0;
			position_.height = 16f;
			EditorGUI.PrefixLabel(position_, new GUIContent($"Matching Image {label_.text.Split(' ').Last()}"));
			position_.y += position_.height;
			height_ += position_.height;

			// Label du détecteur
			EditorGUI.indentLevel = 1;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("databaseIndex_"));
			EditorGUI.LabelField(position_, new GUIContent($"Detector {property_.FindPropertyRelative("databaseIndex_").intValue}"));
			position_.y += position_.height;
			height_ += position_.height;

			// Visuel du détecteur
			EditorGUI.indentLevel = 2;
			var texture_ = (Texture2D)property_.FindPropertyRelative("databaseEntryTexture_").objectReferenceValue;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("databaseEntryTexture_")) * 5f;
			EditorGUI.LabelField(position_, new GUIContent(texture_));
			position_.y += position_.height;
			height_ += position_.height;

			// Nom du détecteur
			EditorGUI.indentLevel = 2;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("databaseEntryName_"));
			EditorGUI.LabelField(position_, new GUIContent(property_.FindPropertyRelative("databaseEntryName_").stringValue));
			position_.y += position_.height;
			height_ += position_.height;

			// Qualité et largeur du détecteur
			EditorGUI.indentLevel = 2;
			position_.height = Mathf.Max(EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("databaseEntryQuality_")), EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("databaseEntryWidth_")));
			EditorGUI.LabelField(position_, new GUIContent($"Quality: {property_.FindPropertyRelative("databaseEntryQuality_").stringValue} \\ Width: {property_.FindPropertyRelative("databaseEntryWidth_").floatValue}"));
			position_.y += position_.height;
			height_ += position_.height;

			// Champ de type
			EditorGUI.indentLevel = 1;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("type_"));
			EditorGUI.PropertyField(position_, property_.FindPropertyRelative("type_"), new GUIContent("Type"));
			position_.y += position_.height;
			height_ += position_.height;

			// Champ du nom de la texture
			EditorGUI.indentLevel = 1;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("textureName_"));
			var textureName_ = property_.FindPropertyRelative("textureName_");
			if (string.IsNullOrWhiteSpace(textureName_.stringValue))
			{
				textureName_.stringValue = "_MainTex";
			}
			EditorGUI.PropertyField(position_, textureName_, new GUIContent("Texture Name"));
			position_.y += position_.height;
			height_ += position_.height;

			// Champ du materiel
			EditorGUI.indentLevel = 1;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("material_"));
			EditorGUI.PropertyField(position_, property_.FindPropertyRelative("material_"), new GUIContent("Material"));
			position_.y += position_.height;
			height_ += position_.height;

			// Champ du décallage
			EditorGUI.indentLevel = 1;
			position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("offset_"));
			EditorGUI.PropertyField(position_, property_.FindPropertyRelative("offset_"), new GUIContent("Offset (% from center)"));
			position_.y += position_.height;
			height_ += position_.height;

			// Champ(s) de donnée
			var type_ = (MatchingImageType)property_.FindPropertyRelative("type_").enumValueIndex;
			switch (type_)
			{
				case MatchingImageType.Image:
					// Champ de l'image
					EditorGUI.indentLevel = 1;
					position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("texture_"));
					EditorGUI.PropertyField(position_, property_.FindPropertyRelative("texture_"), new GUIContent("Texture"));
					position_.y += position_.height;
					height_ += position_.height * 3f;
					break;
				case MatchingImageType.Images:
					//public List<Texture2D> textures_;
					// Champ des images
					EditorGUI.indentLevel = 1;
					position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("texturesFolderPath_"));
					EditorGUI.PropertyField(position_, property_.FindPropertyRelative("texturesFolderPath_"), new GUIContent("Textures Folder Path"));
					position_.y += position_.height;
					height_ += position_.height;
					// Champ de validation
					var assetFolderPath_ = property_.FindPropertyRelative("texturesFolderPath_").stringValue;
					var info_ = "Please tape a valid relative path of asset folder.";
					var style_ = new GUIStyle();
					style_.normal.textColor = Color.HSVToRGB(0f, 1f, 0.5f);
					if (AssetDatabase.IsValidFolder(assetFolderPath_))
					{
						info_ = "The folder exists but it not contains images.";
						style_.normal.textColor = Color.HSVToRGB(0.72f, 1f, 0.5f);

						var texs_ = property_.FindPropertyRelative("textures_");

						texs_.ClearArray();

						var assetsPaths_ = Directory.GetFiles(assetFolderPath_);

						for (int i_ = 0, j_ = 0; i_ < assetsPaths_.Length; i_++)
						{
							var assetPath_ = assetsPaths_[i_];
							var tex_ = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath_);

							if (tex_ != null)
							{
								texs_.InsertArrayElementAtIndex(j_);
								texs_.GetArrayElementAtIndex(j_).objectReferenceValue = tex_;
								j_++;
							}
						}

						if (texs_.arraySize > 0)
						{
							info_ = "The folder exists and it contains images.";
							style_.normal.textColor = Color.HSVToRGB(0.36f, 1f, 0.5f);
						}
					}
					EditorGUI.indentLevel = 2;
					position_.height = 16f;
					EditorGUI.LabelField(position_, new GUIContent(info_), style_);
					position_.y += position_.height;
					height_ += position_.height;
					// Champ du temps entre les images.
					EditorGUI.indentLevel = 1;
					position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("timeInterFrame_"));
					EditorGUI.PropertyField(position_, property_.FindPropertyRelative("timeInterFrame_"), new GUIContent("Time Inter Frame"));
					position_.y += position_.height;
					height_ += position_.height;
					break;
				case MatchingImageType.Video:
					// Champ de la vidéo
					EditorGUI.indentLevel = 1;
					position_.height = EditorGUI.GetPropertyHeight(property_.FindPropertyRelative("video_"));
					EditorGUI.PropertyField(position_, property_.FindPropertyRelative("video_"), new GUIContent("Video"));
					position_.y += position_.height;
					height_ += position_.height * 3f;
					break;
			}

			EditorGUI.indentLevel = indentLevel_;

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return height_;
		}
	}
}
