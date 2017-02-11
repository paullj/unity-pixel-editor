using UnityEngine;
using System;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	[Serializable]
	public class Pencil : ScriptableTool
	{
		public Pencil () : base () { }

		// Optional override to set a shortcut used in the tile editor
		public override KeyCode Shortcut { get { return KeyCode.P; } }

		// Optional override to set a description for the tool
		public override string Description { get { return "As simple as they come"; } }

		// Called by the tilemap editor to paint tiles
		public override void OnClick (Vector2 point, int index , Color color, Image image)
		{
			if (image == null)
				return;

			image.SetPixel ((int)point.x, (int)point.y, index, color);
		}

		public override void OnClickDown (Vector2 point, int index, Color color, Image image)
		{
			OnClick(point, index, color, image);
		}
	}
}
