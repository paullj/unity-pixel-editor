using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensionMethods
{
	private static Rect ClampToScreen (this Rect r, int border)
	{
		r.x = Mathf.Clamp (r.x, border, Screen.width - r.width - border);
		r.y = Mathf.Clamp (r.y, border, Screen.height - r.height - border);
		return r;
	}
}
