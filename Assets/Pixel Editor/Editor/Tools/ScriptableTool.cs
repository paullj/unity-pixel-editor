using UnityEngine;
using System;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	// The base class for a ScriptableTool
	[Serializable]
	public abstract class ScriptableTool
	{
		//Public variables will automatically be exposed in the tile editor
		//This can be very useful, however it only works with some Types including:
		//
		//	- bool								- AnimationCurve
		//	- float								- Color
		//	- int								- Sprite, Texture2D
		//	- Vector2, Vector3					- GameObject, Object
		//	- Enum

		protected List<Vector2> region = new List<Vector2> ();

		// The default constructor where you can set up default variable values
		public ScriptableTool ()
		{

		}

		// The name (which is the class name)
		public string Name { get { return this.GetType ().Name; } }
		// Unique ID of this ScriptableBrush
		public int ID { get { return Animator.StringToHash (Name); } }
		// The shortcut used in the tile editor
		public virtual KeyCode Shortcut { get { return KeyCode.None; } }
		// Small summary on what the ScriptableTool does
		public virtual string Description { get { return string.Empty; } }

		//Called when LMB is held down
		public abstract void OnClick (Vector2 point, int index, Color color, Image image);
		//Called when LMB is clicked down	
		public virtual void OnClickDown (Vector2 point, int index, Color tile, Image image) { }
		//Called when LMB is released		
		public virtual void OnClickUp (Vector2 point, int index, Color tile, Image image) { }
		//The region to draw a tool preview for
		public virtual List<Vector2> GetRegion (Vector2 point, int index, Color tile, Image image)
		{
			region = new List<Vector2>
			{
				point
			};
			return region;
		}
	}
}