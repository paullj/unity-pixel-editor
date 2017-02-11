using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace toinfiniityandbeyond.PixelEditor
{
	public class PixelEditorUtility
	{
		public static Image CreateImage (int width, int height)
		{
			string path = EditorUtility.SaveFilePanelInProject ("Create Image", "New Image", "asset", "");
			if (path == "")
				return null;

			path = FileUtil.GetProjectRelativePath (path);
			return CreateImage (path, width, height);
		}

		public static Image CreateImage(string path, int width, int height)
		{
			if (path == "")
				return null;

			//if (File.Exists (path) && !EditorUtility.DisplayDialog ("Replace Existing Image?", "An image with the same name already exists in this directory. Do you want to replace it?", "Yes", "Cancel"))
			//return null;

			Image image = ScriptableObject.CreateInstance<Image> ();
			AssetDatabase.CreateAsset (image, path);
			AssetDatabase.SaveAssets ();

			image.Init (width, height);

			EditorUtility.SetDirty (image);
			return image;
		}

		public static void AddImageAsPersistant (Image image)
		{
			string path = AssetDatabase.GetAssetPath (image);
			if (string.IsNullOrEmpty (path))
				return;

			string value = EditorPrefs.GetString ("PixelEditor_OpenTabs");
			string [] paths = value.Split ('\n');
			if(!paths.Contains(path))
				value += path + "\n";
			EditorPrefs.SetString ("PixelEditor_OpenTabs", value);
		}
		public static void RemoveImageAsPersistant (Image image)
		{
			string path = AssetDatabase.GetAssetPath (image);
			if (string.IsNullOrEmpty (path))
				return;

			string value = EditorPrefs.GetString ("PixelEditor_OpenTabs");
			string [] paths = value.Split ('\n');
			value = value.Replace (path + "\n", "");
			EditorPrefs.SetString ("PixelEditor_OpenTabs", value);
		}
		public static List<Image> GetPersistantImages ()
		{
			List<Image> results = new List<Image> ();
			string value = EditorPrefs.GetString ("PixelEditor_OpenTabs");
			string [] paths = value.Split ('\n');
			value = "";
			for (int i = 0; i < paths.Length; i++)
			{
				Image image = AssetDatabase.LoadAssetAtPath (paths [i], typeof (Image)) as Image;
				if (string.IsNullOrEmpty (paths [i]) || !image)
					paths [i] = "";
				else
					results.Add (image);
				value += paths [i] + "\n";
			}
			EditorPrefs.SetString ("PixelEditor_OpenTabs", value);
			return results;
		}
		public static Image OpenImage ()
		{
			string path = EditorUtility.OpenFilePanel ("Find an Image (.asset)"/* | .png | .jpg)"*/, "Assets/", "Image Files;*.asset;"/**.jpg;*.png"*/);

			if (path.Length != 0)
			{
				// Check if the loaded file is an Asset or Image
				if (path.EndsWith (".asset"))
				{
					path = FileUtil.GetProjectRelativePath (path);
					Image image = AssetDatabase.LoadAssetAtPath (path, typeof (Image)) as Image;
					return image;
				}
				//else
				//{
				//	// Load Texture from file
				//	Texture2D tex = LoadImageFromFile (path);
				//	// Create a new Image with textures dimensions
				//	Image img = CreateImage (tex.width, tex.height);
				//	// Set pixel colors
				//	img.layers [0].colors = tex.GetPixels ();
				//	img.layers [0].RebuildTextureFromColors ();
				//	return img;
				//}
			}
			return null;
		}
		
		public static Texture2D LoadImageFromFile (string path)
		{
			Texture2D tex = null;
			byte [] fileData;
			if (File.Exists (path))
			{
				fileData = File.ReadAllBytes (path);
				tex = new Texture2D (2, 2);
				tex.LoadImage (fileData); //..this will auto-resize the texture dimensions.
			}
			return tex;
		}

		public static Image OpenImageByAsset (Image img)
		{
			if (img == null)
			{
				Debug.LogWarning ("Image is null. Returning null.");
				//EditorPrefs.SetString ("currentImgPath", "");
				return null;
			}

			string path = AssetDatabase.GetAssetPath (img);

			return img;
		}

		public static Image OpenImageAtPath (string path)
		{
			if (path.Length != 0)
			{
				Image img = AssetDatabase.LoadAssetAtPath (path, typeof (Image)) as Image;

				//if (img == null)
				//{
				//	EditorPrefs.SetString ("currentImgPath", "");
				//	return null;
				//}

				//EditorPrefs.SetString ("currentImgPath", path);
				return img;
			}

			return null;
		}
		/*
		public static bool ExportImage (UPAImage img, TextureType type, TextureExtension extension)
		{
			string path = EditorUtility.SaveFilePanel (
				"Export image as " + extension.ToString (),
				"Assets/",
				img.name + "." + extension.ToString ().ToLower (),
				extension.ToString ().ToLower ());

			if (path.Length == 0)
				return false;

			byte [] bytes;
			if (extension == TextureExtension.PNG)
			{
				// Encode texture into PNG
				bytes = img.GetFinalImage (true).EncodeToPNG ();
			}
			else
			{
				// Encode texture into JPG

#if UNITY_4_2
			bytes = img.GetFinalImage(true).EncodeToPNG();
#elif UNITY_4_3
			bytes = img.GetFinalImage(true).EncodeToPNG();
#elif UNITY_4_5
			bytes = img.GetFinalImage(true).EncodeToJPG();
#else
				bytes = img.GetFinalImage (true).EncodeToJPG ();
#endif
			}

			path = FileUtil.GetProjectRelativePath (path);

			//Write to a file in the project folder
			File.WriteAllBytes (path, bytes);
			AssetDatabase.Refresh ();

			TextureImporter texImp = AssetImporter.GetAtPath (path) as TextureImporter;

			if (type == TextureType.texture)
				texImp.textureType = TextureImporterType.Default;
			else if (type == TextureType.sprite)
			{
				texImp.textureType = TextureImporterType.Sprite;

#if UNITY_4_2
			texImp.spritePixelsToUnits = 10;
#elif UNITY_4_3
			texImp.spritePixelsToUnits = 10;
#elif UNITY_4_5
			texImp.spritePixelsToUnits = 10;
#else
				texImp.spritePixelsPerUnit = 10;
#endif
			}

			texImp.filterMode = FilterMode.Point;
			texImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;

			AssetDatabase.ImportAsset (path);

			return true;
		}*/
	}
}