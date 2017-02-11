using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace toinfiniityandbeyond.PixelEditor
{
	public partial class PixelEditorWindow : EditorWindow
	{
		private Rect layerWindowRect;
		private ReorderableList layerList;
		private Vector2 layerListScroll;

		private void OnLayerWindowEnable ()
		{
			layerWindowRect = new Rect (this.position.width / 4 * 3 - 10, EditorGUIUtility.singleLineHeight + 10, this.position.width / 4, this.position.height / 2);

			RefreshScriptableToolCache ();
			RefreshLayerList ();
		}

		private void RefreshLayerList ()
		{
			if (SelectedImage == null)
				return;
			
			layerList = new ReorderableList (SelectedImage.layers, typeof (Layer), false, true, true, true);
			layerList.headerHeight = 0;
			layerList.drawFooterCallback = (Rect rect) =>
			{
				
			};
			layerList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = SelectedImage.layers [index];
				rect.x += 10;
				rect.width -= 10;

				rect.y += 0;
				float block = EditorGUIUtility.singleLineHeight;

				GUI.enabled = !element.isLocked;

				GUI.Label (new Rect (rect.x, rect.y, block * 2, block * 2), element.Texture);

				Texture2D enabledIcon = element.isEnabled ? Resources.Load<Texture2D> ("Icons/eye-visible") : Resources.Load<Texture2D> ("Icons/eye-hidden");
				GUI.contentColor = new Color (0.2f, 0.2f, 0.2f);
				EditorGUI.BeginChangeCheck ();
				element.isEnabled = GUI.Toggle (new Rect (rect.x, rect.y, block * 2, block * 2), element.isEnabled, enabledIcon, "Label");
				if (EditorGUI.EndChangeCheck ())
				{
					SelectedImage.IsDirty = true;
				}
				GUI.contentColor = Color.white;

				rect.x += block * 2 + 5;
				rect.y += block / 2;
				element.name = GUI.TextField (
					new Rect (rect.x, rect.y, rect.width - block * 4, EditorGUIUtility.singleLineHeight),
					element.name);
				rect.x += rect.width - block * 4;
				rect.y -= block / 2;

				GUI.enabled = true;
				GUI.contentColor = new Color (0.2f, 0.2f, 0.2f);
				Texture2D lockedIcon = element.isLocked ? Resources.Load<Texture2D> ("Icons/lock") : Resources.Load<Texture2D> ("Icons/unlock");
				element.isLocked = GUI.Toggle (
					new Rect (rect.x, rect.y, block * 2, block * 2),
					element.isLocked, lockedIcon, "Label");
				GUI.contentColor = Color.white;
			};
			layerList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
		}
		private void DrawLayerWindow (int id)
		{
			if (layerList == null)
				OnLayerWindowEnable();

			int layerCount = SelectedImage.layers.Count;

			Rect listRect = new Rect (2.5f, EditorGUIUtility.singleLineHeight + 2.5f, layerWindowRect.width - 5, layerWindowRect.height * 0.65f);
			Rect viewRect = new Rect (0, 0, layerWindowRect.width - 5, EditorGUIUtility.singleLineHeight * 2 * layerCount);
			layerListScroll = GUI.BeginScrollView (listRect, layerListScroll, viewRect);
			for(int i = 0; i < layerCount; i++)
			{
				//GUI.Label (new Rect (listRect.x, listRect.y + EditorGUIUtility.singleLineHeight * 2 * i, listRect.width, EditorGUIUtility.singleLineHeight * 2), "Test");
				var element = SelectedImage.layers [i];
				int elementY = (int)EditorGUIUtility.singleLineHeight * 2 * i;

				GUI.enabled = !element.isLocked;
				GUI.Label (new Rect (0, elementY, viewRect.width, EditorGUIUtility.singleLineHeight * 2f), GUIContent.none, new GUIStyle ("RL Background"));
				
				EditorGUI.BeginChangeCheck ();
				Texture2D enabledIcon = element.isEnabled ? Resources.Load<Texture2D> ("Icons/eye-visible") : Resources.Load<Texture2D> ("Icons/eye-hidden");
				GUI.contentColor = new Color (0.2f, 0.2f, 0.2f);
				element.isEnabled = GUI.Toggle (new Rect (0, elementY, viewRect.width * 0.15f, EditorGUIUtility.singleLineHeight * 2f), element.isEnabled, enabledIcon, "Label");
				GUI.contentColor = Color.white;

				if (EditorGUI.EndChangeCheck ())
					SelectedImage.IsDirty = true;

				GUI.Label (new Rect (viewRect.width * 0.15f, elementY + 1.5f, viewRect.width * 0.2f, EditorGUIUtility.singleLineHeight * 2 - 5), element.Texture);

				element.name = GUI.TextField (new Rect ((int)(viewRect.width * 0.35f), (int)(elementY + 0.45f * EditorGUIUtility.singleLineHeight), (int)(viewRect.width * 0.5f), EditorGUIUtility.singleLineHeight), element.name);

				GUI.enabled = true;
				GUI.contentColor = new Color (0.2f, 0.2f, 0.2f);
				Texture2D lockedIcon = element.isLocked ? Resources.Load<Texture2D> ("Icons/lock") : Resources.Load<Texture2D> ("Icons/unlock");
				element.isLocked = GUI.Toggle (
					new Rect (viewRect.width * 0.85f, elementY, viewRect.width * 0.15f, EditorGUIUtility.singleLineHeight * 2f),
					element.isLocked, lockedIcon, "Label");
				GUI.contentColor = Color.white;
			}
			GUI.EndScrollView ();
		/*
			EditorGUI.BeginChangeCheck ();
			GUI.enabled = layerList.index > 0 && layerList.count > 1 && !SelectedImage.layers [layerList.index].isLocked;
			if (GUI.Button (new Rect (2.5f, layerWindowRect.height * 0.65f, layerWindowRect.width / 8, EditorGUIUtility.singleLineHeight), "▲", EditorStyles.miniButtonLeft))
			{
				Layer temp = SelectedImage.layers [layerList.index];
				SelectedImage.layers.RemoveAt (layerList.index);
				SelectedImage.layers.Insert (layerList.index - 1, temp);
				layerList.index--;
			}
			GUI.enabled = layerList.index >= 0 && layerList.index < layerList.count - 1 && layerList.count > 1 && !SelectedImage.layers [layerList.index].isLocked;
			if (GUI.Button (new Rect (2.5f + layerWindowRect.width / 8, layerWindowRect.height * 0.65f, layerWindowRect.width / 8, EditorGUIUtility.singleLineHeight), "▼", EditorStyles.miniButtonRight))
			{
				Layer temp = SelectedImage.layers [layerList.index];
				SelectedImage.layers.RemoveAt (layerList.index);
				SelectedImage.layers.Insert (layerList.index + 1, temp);
				layerList.index++;
			}
			GUI.enabled = true;
			if (GUI.Button (new Rect (layerWindowRect.width - layerWindowRect.width / 2, layerWindowRect.y, layerWindowRect.width / 8, EditorGUIUtility.singleLineHeight), "+", EditorStyles.miniButtonLeft))
			{
				layerList.list.Add (new Layer (SelectedImage));
				layerList.index = layerList.count - 1;
			}
			GUI.enabled = layerList.index >= 0 && layerList.count > 1 && !SelectedImage.layers [layerList.index].isLocked;
			if (GUI.Button (new Rect (layerWindowRect.width - layerWindowRect.width / 8 * 3, layerWindowRect.y, layerWindowRect.width / 8, EditorGUIUtility.singleLineHeight), "-", EditorStyles.miniButtonMid))
			{
				if (EditorUtility.DisplayDialog ("Warning!",
									"Are you sure you want to delete this layer?", "Yes", "No"))
				{
					ReorderableList.defaultBehaviours.DoRemoveButton (layerList);
				}
			}
			GUI.enabled = layerList.index >= 0;
			if (GUI.Button (new Rect (2.5f + layerWindowRect.width / 5 * 4, layerWindowRect.y, layerWindowRect.width / 5, EditorGUIUtility.singleLineHeight), "Clone", EditorStyles.miniButtonRight))
			{
				layerList.list.Add (new Layer (SelectedImage.layers [layerList.index]));
				layerList.index = layerList.count - 1;
			}
			if (EditorGUI.EndChangeCheck ())
				SelectedImage.IsDirty = true;
			GUI.enabled = true;



			if (layerList.index >= 0 && layerList.index < SelectedImage.layers.Count)
			{
				EditorGUI.BeginChangeCheck ();
				var element = SelectedImage.layers [layerList.index];
				GUI.enabled = !element.isLocked;

				float height = layerWindowRect.height - 2.5f - EditorGUIUtility.singleLineHeight * 2;
				GUI.Label (new Rect (2.5f, height, layerWindowRect.width / 7 * 2, EditorGUIUtility.singleLineHeight), "Opacity", EditorStyles.miniLabel);
				element.opacity = GUI.HorizontalSlider (new Rect (2.5f + layerWindowRect.width / 7 * 2, height, layerWindowRect.width / 7 * 2, EditorGUIUtility.singleLineHeight), element.opacity, 0, 1);
				element.mode = (BlendMode)EditorGUI.EnumPopup (new Rect (2.5f + layerWindowRect.width / 7 * 4 + 5, height, layerWindowRect.width / 7 * 3 - 5, EditorGUIUtility.singleLineHeight), element.mode);

				if (EditorGUI.EndChangeCheck ())
					SelectedImage.IsDirty = true;
				GUI.enabled = true;
			}
			*/
			GUI.DragWindow ();
		}

		//Move this;
		private List<ScriptableTool> scriptableToolCache = new List<ScriptableTool> ();
		private void RefreshScriptableToolCache ()
		{
			List<ScriptableTool> toRemove = new List<ScriptableTool> ();
			for (int i = 0; i < scriptableToolCache.Count; i++)
			{
				if (scriptableToolCache [i] == null)
					toRemove.Add (scriptableToolCache [i]);
			}
			scriptableToolCache = scriptableToolCache.Except (toRemove).ToList ();
			foreach (Type type in Assembly.GetAssembly (typeof (ScriptableTool)).GetTypes ()
					 .Where (myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf (typeof (ScriptableTool))))
			{
				bool containsType = false;
				for (int i = 0; i < scriptableToolCache.Count; i++)
				{
					if (scriptableToolCache [i].GetType () == type)
					{
						containsType = true;
						break;
					}
				}
				if (!containsType)
					scriptableToolCache.Add ((ScriptableTool)Activator.CreateInstance (type));
			}
		}
	}
}


/*
						Rect rect = new Rect (2.5f, EditorGUIUtility.singleLineHeight + 2.5f, toolbarRect.width - 5, toolbarRect.height - EditorGUIUtility.singleLineHeight - 5);
						GUI.SelectionGrid()
						float toolSize = (rect.width-2) / 5;
						int maxRows = Mathf.CeilToInt ((float)scriptableToolCache.Count / 5);
						for (int x = 0; x < 5; x++)
						{
							for (int y = 0; y < maxRows; y++)
							{
								int index = x + y * 5;
								GUI.contentColor = new Color (0.2f, 0.2f, 0.2f);
								bool exists = (index < scriptableToolCache.Count);
								if (exists)
								{
									Rect toolButtonRect = new Rect (rect.x + x * toolSize + 1, rect.y + y * toolSize + 1, toolSize - 1, toolSize + 1);
									GUI.Toggle (toolButtonRect, SelectedImage.selectedTool == index, Resources.Load<Texture2D> ("Icons/" + scriptableToolCache [index].Name), "Button");
									if (GUI.Button (toolButtonRect, "", "Label"))
									{
										if (SelectedImage.selectedTool == index)
											index = -1;
										else
											SelectedImage.selectedTool = index;
									}
								}
								GUI.contentColor = Color.white;
							}
						}
						rect.y += maxRows * (toolSize + 2) + EditorGUIUtility.singleLineHeight / 3;
						*/
