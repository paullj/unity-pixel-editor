using UnityEngine;
using System.Linq;

namespace toinfiniityandbeyond.PixelEditor
{
	[System.Serializable]
	public class Layer
	{
		public string name;
		public float opacity;

		public bool isEnabled;
		public bool isLocked;
		public BlendMode mode;

		public Color [] colors;
		private Texture2D texture;
		public Texture2D Texture
		{
			get {
				bool valid = (texture && texture.width == parent.width && texture.height == parent.height);
				if(!valid)
				{
					RebuildTextureFromColors ();
				}
				return texture;
			}
		}
		public Image parent;

		public Layer (Image parent)
		{
			this.parent = parent;
			this.name = "New Layer";
			this.opacity = 1;
			this.mode = BlendMode.Normal;

			this.isEnabled = true;
			this.isLocked = false;
			this.parent = parent;

			this.colors = Enumerable.Repeat (Color.clear, parent.width * parent.height).ToArray ();
			this.RebuildTextureFromColors ();
		}
		public Layer (Layer original)
		{
			name = original.name + " (Clone)";
			opacity = original.opacity;
			mode = original.mode;

			isEnabled = original.isEnabled;
			isLocked = original.isLocked;
			parent = original.parent;

			colors = original.colors;
			RebuildTextureFromColors ();
		}

		public void RebuildTextureFromColors ()
		{
			texture = new Texture2D (parent.width, parent.height);
			texture.SetPixels (colors);
			texture.filterMode = FilterMode.Point;
			texture.Apply ();
		}

		public Color GetPixel (int x, int y)
		{
			//Should be the same.
			return colors [x + y * parent.width];
			//return Texture.GetPixel (x, y);
		}
		public void SetPixel (int x, int y, Color color)
		{
			if (!isLocked)
			{
				colors [x + y * parent.width] = color;
				Texture.SetPixel (x, y, color);
				Texture.Apply ();
			}
		}

		public int GetOrder ()
		{
			return parent.layers.IndexOf (this);
		}
	}
}