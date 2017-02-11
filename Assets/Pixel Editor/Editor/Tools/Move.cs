using UnityEngine;
using System;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	[Serializable]
	public class Move : ScriptableTool
	{
		private Vector2 previousPosition;

		public Move () : base ()
		{
		}

		public override KeyCode Shortcut { get { return KeyCode.Space; } }

		public override string Description { get { return "Used to navigate the canvas"; } }

		public override void OnClick (Vector2 position, int index , Color color, Image image)
		{
			if (image == null)
				return;

			Vector2 delta = previousPosition - position;
			delta.x = -delta.x;
			delta *= image.canvasPixelScale / 2  * (Event.current.control ? 0.1f : 1);
			image.canvasPosition += delta;

			previousPosition = position;
		}
		public override void OnClickDown (Vector2 point, int index, Color tile, Image image)
		{
			previousPosition = point;
		}
		public override void OnClickUp (Vector2 point, int index, Color tile, Image image)
		{
			previousPosition = Vector2.zero;
		}
	}
}
