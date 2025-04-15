using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using SexyFramework.Drivers;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.PIL;

namespace SexyFramework
{
	public static class GlobalMembers
	{
		public delegate int GetIdByImageFunc(Image img);

		public delegate Image GetImageByIdFunc(int id);

		public delegate KeyFrameData KFDInstantiateFunc();

		public static SexyAppBase gSexyAppBase = null;

		public static SexyApp gSexyApp = null;

		public static bool gIs3D = false;

		public static IFileDriver gFileDriver = null;

		public static int gTotalGraphicsMemory = 0;

		public static float SEXYMATH_PI = (float)Math.PI;

		public static float SEXYMATH_2PI = (float)Math.PI * 2f;

		public static float SEXYMATH_E = 2.71828f;

		public static float SEXYMATH_EPSILON = 0.001f;

		public static float SEXYMATH_EPSILONSQ = 1E-06f;

		public static bool IsBackButtonPressed = false;

		public static Vector2 NO_TOUCH_MOUSE_POS = new Vector2(-1f, -1f);

		public static int[,] gButtonWidgetColors = new int[6, 3]
		{
			{ 0, 0, 0 },
			{ 0, 0, 0 },
			{ 0, 0, 0 },
			{ 255, 255, 255 },
			{ 132, 132, 132 },
			{ 212, 212, 212 }
		};

		public static int[,] gDialogButtonColors = new int[6, 3]
		{
			{ 255, 255, 255 },
			{ 255, 255, 255 },
			{ 0, 0, 0 },
			{ 255, 255, 255 },
			{ 132, 132, 132 },
			{ 212, 212, 212 }
		};

		public static int[,] gDialogColors = new int[7, 3]
		{
			{ 255, 255, 255 },
			{ 255, 255, 0 },
			{ 255, 255, 255 },
			{ 255, 255, 255 },
			{ 255, 255, 255 },
			{ 80, 80, 80 },
			{ 255, 255, 255 }
		};

		public static string DIALOG_YES_STRING = "YES";

		public static string DIALOG_NO_STRING = "NO";

		public static string DIALOG_OK_STRING = "OK";

		public static string DIALOG_CANCEL_STRING = "CANCEL";

		public static string _ID(string strDefault, int id)
		{
			return gSexyApp.mPopLoc.GetString(id, strDefault);
		}

		public static int sexyatoi(string str)
		{
			int result;
			if (string.IsNullOrEmpty(str) || !int.TryParse(str, out result))
			{
				return 0;
			}
			return result;
		}

		public static int sexyatoi(Dictionary<string, string> dict, string key)
		{
			int result = 0;
			if (dict != null && dict.ContainsKey(key))
			{
				result = sexyatoi(dict[key]);
			}
			return result;
		}

		public static float sexyatof(Dictionary<string, string> dict, string key)
		{
			float result = 0f;
			if (dict != null && dict.ContainsKey(key))
			{
				float.TryParse(dict[key], NumberStyles.Any, CultureInfo.InvariantCulture, out result);
			}
			return result;
		}

		public static Rect sexyatof(Dictionary<char, Rect> dict, char key)
		{
			Rect result = default(Rect);
			if (dict != null && dict.ContainsKey(key))
			{
				return dict[key];
			}
			return result;
		}
	}
}
