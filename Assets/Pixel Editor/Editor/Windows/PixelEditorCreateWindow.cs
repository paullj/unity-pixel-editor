using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	public partial class PixelEditorWindow : EditorWindow
	{
		bool showCreateNewWindow = false;
		string defaultPath = string.Empty;
		int defaultWidth = 32;
		int defaultHeight = 32;

		void DrawCreateNewWindow (int id)
		{
			Rect labelRect = new Rect (2.5f, EditorGUIUtility.singleLineHeight + 2.5f, this.position.width / 3 - 5, EditorGUIUtility.singleLineHeight);
			GUI.Label (new Rect (labelRect.x, labelRect.y, labelRect.width / 6 - 2.5f, labelRect.height), "Path:");
			labelRect.x += labelRect.width / 6;

			defaultPath = GUI.TextField (new Rect (labelRect.x, labelRect.y, labelRect.width / 6 * 4 - 5, labelRect.height), defaultPath);
			labelRect.x += labelRect.width / 6 * 4;
			if (GUI.Button (new Rect (labelRect.x, labelRect.y, labelRect.width / 6 - 5, labelRect.height), "..."))
			{
				defaultPath = EditorUtility.SaveFilePanelInProject ("Create Image", "New Image", "asset", "");
			}
			labelRect.x = 2.5f;
			labelRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
			GUI.Label (labelRect, "Settings", EditorStyles.boldLabel);
			labelRect.y += EditorGUIUtility.singleLineHeight;
			GUI.Label (new Rect (labelRect.x, labelRect.y, labelRect.width / 8 * 1.5f - 2.5f, labelRect.height), "Width:");
			labelRect.x += labelRect.width / 8 * 1.5f;
			defaultWidth = EditorGUI.IntSlider (new Rect (labelRect.x, labelRect.y, labelRect.width / 8 * 6f - 2.5f, labelRect.height), defaultWidth, 1, 512);
			labelRect.x += labelRect.width / 8 * 6f - 2.5F;
			GUI.Label (new Rect (labelRect.x, labelRect.y, labelRect.width / 8, labelRect.height), "px");
			labelRect.x = 2.5f;
			labelRect.y += EditorGUIUtility.singleLineHeight * 1.1f;
			GUI.Label (new Rect (labelRect.x, labelRect.y, labelRect.width / 8 * 1.5f - 2.5f, labelRect.height), "Height:");
			labelRect.x += labelRect.width / 8 * 1.5f;
			defaultHeight = EditorGUI.IntSlider (new Rect (labelRect.x, labelRect.y, labelRect.width / 8 * 6f - 2.5f, labelRect.height), defaultHeight, 1, 512);
			labelRect.x += labelRect.width / 8 * 6f - 2.5F;
			GUI.Label (new Rect (labelRect.x, labelRect.y, labelRect.width / 8, labelRect.height), "px");
			labelRect.x = 2.5f;


			Rect buttonRect = new Rect (2.5f, this.position.height / 3 - EditorGUIUtility.singleLineHeight * 1.5f - 7.5f, this.position.width / 6 - 5f, EditorGUIUtility.singleLineHeight * 1.5f);
			GUI.backgroundColor = new Color (0.25f, 0.75f, 0.25f);
			GUI.enabled = !string.IsNullOrEmpty (defaultPath);
			if (GUI.Button (buttonRect, "Create"))
			{
				Image image = PixelEditorUtility.CreateImage (defaultPath, defaultWidth, defaultHeight);
				if (image)
				{
					OpenTab (image);
					showCreateNewWindow = false;
				}
			}
			GUI.enabled = true;
			buttonRect.x += this.position.width / 6 - 2.5f;
			GUI.backgroundColor = new Color (0.7f, 0.25f, 0.25f);
			if (GUI.Button (buttonRect, "Cancel"))
			{
				showCreateNewWindow = false;
			}
		}
	}
}