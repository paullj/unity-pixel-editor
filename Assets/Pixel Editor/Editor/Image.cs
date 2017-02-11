using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace toinfiniityandbeyond.PixelEditor
{
	public class Image : ScriptableObject
	{
		public int width, height;
		public List<Layer> layers = new List<Layer> ();

		public bool IsDirty = true;
		private Texture2D texture = null;

		[SerializeField]
		public int selectedTool = -1;
		public Color primaryColor, secondaryColor;
		public Vector2 canvasPosition = new Vector2(0, 0);
		public float canvasPixelScale = 20f;

		public void Init (int w, int h)
		{
			width = w;
			height = h;

			layers.Add (new Layer (this));
			IsDirty = true;
		}

		public void SetPixel(int x, int y, int layer, Color c)
		{
			if (!InBounds (x, y) || layer < 0  || layer >= layers.Count)
				return;

			layers [layer].SetPixel (x, y, c);
			IsDirty = true;
		}
		public Color GetPixel(int x, int y, int layer)
		{
			if (!InBounds (x, y) || layer < 0 || layer >= layers.Count)
				return default(Color);

			return layers [layer].GetPixel (x, y);
		}

		public bool InBounds(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		// Get the rect of the image as displayed in the editor
		public Rect GetRect (Rect window)
		{
			float ratio = (float)height / (float)width;
			float w = canvasPixelScale * 30;
			float h = ratio * canvasPixelScale * 30;
			float xPos = window.width / 2f - w / 2f + canvasPosition.x;
			float yPos = window.height / 2f - h / 2f + 20 + canvasPosition.y;

			return new Rect (xPos, yPos, w, h);
		}
		public Texture2D GetTexture2D ()
		{
			if (IsDirty != true && texture != null)
				return texture;

			IsDirty = false;
			if (layers.Count > 0)
			{
				// Calculate blended image
				texture = null;
				for (int i = layers.Count - 1; i >= 0; i--)
				{
					if (!layers [i].isEnabled)
						continue;

					if (texture == null)
					{
						texture = BlendModeUtility.Blend (layers [i].Texture, layers [i].opacity, layers [i].Texture, layers [i].opacity, layers [i].mode);
					}
					else
					{
						texture = BlendModeUtility.Blend (texture, 1, layers [i].Texture, layers [i].opacity, layers [i].mode);
					}
				}
				if (texture == null)
				{
					Texture2D _empty = new Texture2D (1, 1);
					_empty.SetPixel (0, 0, Color.clear);
					_empty.Apply ();
					return _empty;
				}
				return texture;
			}
			else
			{
				Texture2D _empty = new Texture2D (1, 1);
				_empty.SetPixel (0, 0, Color.clear);
				_empty.Apply ();
				return _empty;
			}
		}
	}
}