using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	public partial class PixelEditorWindow : EditorWindow
	{
		public static PixelEditorWindow window;
		public static bool IsShowing
		{
			get { return window != null; }
		}

		public static List<Image> openImages = new List<Image> ();
		public int selectedImageIndex = -1;
		public Image SelectedImage
		{
			get { return selectedImageIndex >= 0 && selectedImageIndex < openImages.Count ? openImages [selectedImageIndex] : null; }
		}
		public Vector2 mousePosition;
	
		[MenuItem ("Window/Pixel Editor")]
		static void Init ()
		{
			PixelEditorWindow.window = (PixelEditorWindow)EditorWindow.GetWindow (typeof (PixelEditorWindow));
			window.name = "Pixel Editor";
			window.Show ();
		}
		private void OnEnable ()
		{
			OnLayerWindowEnable ();
			openImages = PixelEditorUtility.GetPersistantImages();
		}
		private void OnGUI ()
		{
			Rect toolbarRect = new Rect (0, 0, this.position.width, EditorGUIUtility.singleLineHeight);
			Rect canvasRect = new Rect (0, 0, this.position.width, this.position.height/* - EditorGUIUtility.singleLineHeight*/);

			DrawCanvas (canvasRect);
			DrawToolbar (toolbarRect);
			
			BeginWindows ();
			if (SelectedImage)
			{
				layerWindowRect = GUI.Window (1, layerWindowRect, DrawLayerWindow, "Layers");
			}
			if (showCreateNewWindow)
			{
				GUI.Window (2, new Rect (canvasRect.width / 2 - canvasRect.width / 6, canvasRect.height / 2 - canvasRect.height / 6, canvasRect.width / 3, canvasRect.height / 3), DrawCreateNewWindow, "Create New...");
			}
			EndWindows ();

			mousePosition = GetMousePosition (canvasRect);
			HandleToolbarEvents (Event.current);
		}

		private void OpenTab (Image image)
		{
			if(openImages.Contains(image))
			{
				selectedImageIndex = openImages.IndexOf (image);
				return;
			}
			openImages.Add (image);
			selectedImageIndex = openImages.Count - 1;
			//RefreshLayerList ();
			PixelEditorUtility.AddImageAsPersistant (image);
		}
		private void CloseTab (Image image)
		{
			selectedImageIndex = openImages.Count - 1;
			PixelEditorUtility.RemoveImageAsPersistant (image);
			openImages.Remove (image);
			//RefreshLayerList ();
		}
		private void DrawCanvas (Rect rect)
		{
			GUI.backgroundColor = new Color (0.4f, 0.4f, 0.4f);
			GUI.Box (rect, GUIContent.none, new GUIStyle ("InnerShadowBg"));
			GUI.backgroundColor = Color.white;
			if (SelectedImage) {
				Rect texPos = SelectedImage.GetRect(rect);

				Texture2D bg = new Texture2D (1, 1);
				bg.SetPixel (0, 0, Color.clear);
				bg.Apply ();
				EditorGUI.DrawTextureTransparent (texPos, bg);
				DestroyImmediate (bg);

				//Calculate the final image from the layers list
				Texture2D _result = SelectedImage.GetTexture2D();

				//Draw the image
				_result.SetPixel (1, 1, Color.black);
				GUI.DrawTexture (texPos, _result);

				//// Draw a grid above the image (y axis first)
				//for (int x = 0; x <= image.width; x += 1)
				//{
				//	float posX = texPos.xMin + ((float)texPos.width / (float)image.width) * x - 0.2f;
				//	EditorGUI.DrawRect (new Rect (posX, texPos.yMin, 1, texPos.height), Color.white);
				//}
				//// Then x axis
				//for (int y = 0; y <= image.height; y += 1)
				//{
				//	float posY = texPos.yMin + ((float)texPos.height / (float)image.height) * y - 0.2f;
				//	EditorGUI.DrawRect (new Rect (texPos.xMin, posY, texPos.width, 1), Color.white);
				//}
				SelectedImage.canvasPixelScale = GUI.VerticalSlider(new Rect (rect.width - 20, rect.height - 100, 20, 100), SelectedImage.canvasPixelScale, 100, 1);
			}
			DoDragAndDrop (rect);
			GUI.backgroundColor = Color.white;
		}
		private void DrawToolbar (Rect rect)
		{
			//Event Handler
			Event e = Event.current;
			if(rect.Contains (e.mousePosition))
				Repaint ();

			GUI.Box (rect, GUIContent.none, EditorStyles.toolbar);
			//Tabs
			Rect tabsRect = new Rect (rect.x, rect.y, rect.width - rect.width / 4 - 10, rect.height);
			for (int i = 0; i < openImages.Count; i++)
			{
				bool isValid = openImages [i] != null;
				if(!isValid)
				{
					openImages.RemoveAt (i);
					continue;
				}

				Color tabColor = (i == selectedImageIndex) ? new Color (0.55f, 0.55f, 0.55f) : new Color (0.85f, 0.85f, 0.85f);
				GUI.backgroundColor = tabColor;

				float tabWidth = Mathf.Clamp (tabsRect.width / openImages.Count, 75, 200);
				bool isDisplayable = tabsRect.x + tabWidth * (i + 1.5f) < rect.width;

				if (isDisplayable)
				{
					Rect currentTabRect = new Rect (tabsRect.x + tabWidth * i, tabsRect.y, tabWidth, tabsRect.height);
					currentTabRect.width -= 25;
					if (GUI.Button (currentTabRect, openImages [i].name, EditorStyles.toolbarButton))
					{
						selectedImageIndex = i;
						//RefreshLayerList ();
					}
					currentTabRect.x += currentTabRect.width;
					currentTabRect.width = 20;

					GUI.backgroundColor = (currentTabRect.Contains (e.mousePosition)) ? new Color (0.7f, 0.25f, 0.25f) : tabColor;
					if (GUI.Button (currentTabRect, "X", EditorStyles.toolbarButton))
					{
						CloseTab (openImages[i]);
						continue;
					}
				}
				GUI.backgroundColor = Color.white;
			}

			//Buttons
			GUI.contentColor = Color.black;
			Rect buttonsRect = new Rect (rect.width - rect.width / 4, rect.y, rect.width / 4, rect.height);
			if (GUI.Button (new Rect (buttonsRect.x, buttonsRect.y, buttonsRect.width / 4, buttonsRect.height), Resources.Load<Texture2D>("Icons/new"), EditorStyles.toolbarButton))
			{
				defaultPath = "";
				showCreateNewWindow = true;
			}
			buttonsRect.x += buttonsRect.width / 4;
			if (GUI.Button (new Rect (buttonsRect.x, buttonsRect.y, buttonsRect.width / 4, buttonsRect.height), Resources.Load<Texture2D> ("Icons/save"), EditorStyles.toolbarButton))
			{

			}
			buttonsRect.x += buttonsRect.width / 4;
			if (GUI.Button (new Rect (buttonsRect.x, buttonsRect.y, buttonsRect.width / 4, buttonsRect.height), Resources.Load<Texture2D> ("Icons/open"), EditorStyles.toolbarButton))
			{
				Image image = PixelEditorUtility.OpenImage ();
				if(image)
				{
					OpenTab (image);
				}
			}
			buttonsRect.x += buttonsRect.width / 4;
			if (GUI.Button (new Rect (buttonsRect.x, buttonsRect.y, buttonsRect.width / 4, buttonsRect.height), Resources.Load<Texture2D> ("Icons/settings"), EditorStyles.toolbarButton))
			{

			}
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.white;
		}

		private void HandleToolbarEvents (Event e)
		{
			if (SelectedImage.selectedTool >= 0 && SelectedImage.selectedTool < scriptableToolCache.Count)
			{
				if (e.button == 0)
				{
					if (e.type == EventType.MouseDown)
					{
						scriptableToolCache [SelectedImage.selectedTool].OnClickDown (mousePosition, selectedLayerIndex, SelectedImage.primaryColor, SelectedImage);
					}
					if (e.type == EventType.MouseDrag)
					{
						scriptableToolCache [SelectedImage.selectedTool].OnClick (mousePosition, selectedLayerIndex, SelectedImage.primaryColor, SelectedImage);
					}
					if (e.type == EventType.MouseUp)
					{
						scriptableToolCache [SelectedImage.selectedTool].OnClickUp (mousePosition, selectedLayerIndex, SelectedImage.primaryColor, SelectedImage);
					}
					List<Vector2> region = scriptableToolCache [SelectedImage.selectedTool].GetRegion (mousePosition, selectedLayerIndex, SelectedImage.primaryColor, SelectedImage);
					Repaint ();
				}
			}
		}
		public Vector2 GetMousePosition (Rect window)
		{
			Vector2 screenPosition = Event.current.mousePosition;
			Rect imageRect = SelectedImage.GetRect (window);
			float x = Mathf.Lerp (0, SelectedImage.width, (screenPosition.x - imageRect.position.x) / imageRect.width);
			float y = Mathf.Lerp (0, SelectedImage.height, (imageRect.height - screenPosition.y + imageRect.position.y) / imageRect.height);
			return new Vector2 (x, y);
		}


		private void DoDragAndDrop (Rect dropArea)
		{
			Event e = Event.current;
			EventType currentEventType = e.type;

			// The DragExited event does not have the same mouse position data as the other events,
			// so it must be checked now:
			if (currentEventType == EventType.DragExited) DragAndDrop.PrepareStartDrag ();// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)

			if (!dropArea.Contains (e.mousePosition)) return;

			switch (currentEventType)
			{
				case EventType.MouseDown:
					DragAndDrop.PrepareStartDrag ();// reset data
				//	e.Use ();

					break;
				case EventType.MouseDrag:
					break;
				case EventType.DragUpdated:
					if (DragAndDrop.objectReferences.OfType<Image> ().Any ())
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

					if (DragAndDrop.objectReferences.Length > 0)
						e.Use ();

					break;
				case EventType.DragPerform:
					DragAndDrop.AcceptDrag ();

					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						if(DragAndDrop.objectReferences [i].GetType() == typeof(Image))
							OpenTab (DragAndDrop.objectReferences [i] as Image);
					}
					e.Use ();
					break;
				case EventType.MouseUp:
					// Clean up, in case MouseDrag never occurred:
					DragAndDrop.PrepareStartDrag ();
					break;
			}

		}
	}
}