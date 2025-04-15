using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using SexyFramework.Drivers;
using SexyFramework.Misc;

namespace SexyFramework
{
	public static class Common
	{
		public class Set<T> : List<T>
		{
			private new void Add(T item)
			{
			}

			public bool AddUnique(T obj)
			{
				if (typeof(T).IsValueType)
				{
					if (BinarySearch(obj) >= 0)
					{
						return false;
					}
					Add(obj);
					Sort();
				}
				else
				{
					if (IndexOf(obj) >= 0)
					{
						return false;
					}
					Add(obj);
				}
				return true;
			}
		}

		public static int V_MAX = 100;

		public static int COUNTER_CLOCKWISE = 0;

		public static int CLOCKWISE = 1;

		public static double BIG = 1E+30;

		private static MTRand gCommonMTRand = new MTRand();

		public static float JL_PI = (float)Math.PI;

		public static uint SexyTime()
		{
			return (uint)GlobalMembers.gSexyAppBase.mAppDriver.GetAppTime();
		}

		public static string StringToWString(string theString)
		{
			return theString;
		}

		public static string WStringToString(string theString)
		{
			return theString;
		}

		public static bool StringToInt(string theString, ref int theIntVal)
		{
			theIntVal = 0;
			if (theString.Length == 0)
			{
				return false;
			}
			int num = 10;
			bool flag = false;
			int i = 0;
			if (theString[i] == '-')
			{
				flag = true;
				i++;
			}
			for (; i < theString.Length; i++)
			{
				char c = theString[i];
				if (num == 10 && c >= '0' && c <= '9')
				{
					theIntVal = theIntVal * 10 + (c - 48);
				}
				else if (num == 16 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
				{
					if (c <= '9')
					{
						theIntVal = theIntVal * 16 + (c - 48);
					}
					else if (c <= 'F')
					{
						theIntVal = theIntVal * 16 + (c - 65) + 10;
					}
					else
					{
						theIntVal = theIntVal * 16 + (c - 97) + 10;
					}
				}
				else
				{
					if ((c != 'x' && c != 'X') || i != 1 || theIntVal != 0)
					{
						theIntVal = 0;
						return false;
					}
					num = 16;
				}
			}
			if (flag)
			{
				theIntVal = -theIntVal;
			}
			return true;
		}

		public static bool StringToDouble(string aTempString, ref double theDouble)
		{
			theDouble = 0.0;
			try
			{
				theDouble = double.Parse(aTempString, NumberStyles.Float, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		public static string XMLDecodeString(string theString)
		{
			StringBuilder stringBuilder = new StringBuilder("");
			for (int i = 0; i < theString.Length; i++)
			{
				char c = theString[i];
				if (c == '&')
				{
					int num = theString.IndexOf(';', i);
					if (num != -1)
					{
						string text = theString.Substring(i + 1, num - i - 1);
						i = num;
						switch (text)
						{
						case "lt":
							c = '<';
							break;
						case "amp":
							c = '&';
							break;
						case "gt":
							c = '>';
							break;
						case "quot":
							c = '"';
							break;
						case "apos":
							c = '\'';
							break;
						case "nbsp":
							c = ' ';
							break;
						case "cr":
							c = '\n';
							break;
						default:
							if (text[0] == '#' && text.Length > 1)
							{
								int theIntVal = c;
								if (text[1] == 'x')
								{
									StringToInt("0x" + text.Substring(2), ref theIntVal);
								}
								else
								{
									StringToInt(text.Substring(1), ref theIntVal);
								}
								c = (char)theIntVal;
							}
							break;
						}
					}
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static string GetFileName(string thePath, bool noExtension)
		{
			int num = Math.Max(thePath.LastIndexOf('\\'), thePath.LastIndexOf('/'));
			if (noExtension)
			{
				int num2 = thePath.LastIndexOf('.');
				if (num2 > num)
				{
					return thePath.Substring(num + 1, num2 - num - 1);
				}
			}
			if (num == -1)
			{
				return thePath;
			}
			return thePath.Substring(num + 1);
		}

		public static string RemoveTrailingSlash(string theDirectory)
		{
			int length = theDirectory.Length;
			if (length > 0 && (theDirectory[length - 1] == '\\' || theDirectory[length - 1] == '/'))
			{
				return theDirectory.Substring(0, length - 1);
			}
			return theDirectory;
		}

		public static string GetCurDir()
		{
			return GetGameFileDriver().GetCurPath();
		}

		public static string GetFullPath(string theRelPath)
		{
			return GetPathFrom(theRelPath, GetCurDir());
		}

		public static string GetFileDir(string thePath, bool withSlash)
		{
			int num = Math.Max(thePath.LastIndexOf('\\'), thePath.LastIndexOf('/'));
			if (num == -1)
			{
				return "";
			}
			if (withSlash)
			{
				return thePath.Substring(0, num + 1);
			}
			return thePath.Substring(0, num);
		}

		public static string GetPathFrom(string theRelPath, string theDir)
		{
			string text = "";
			string text2 = theDir;
			if (theRelPath.Length >= 2 && theRelPath[1] == ':')
			{
				return theRelPath;
			}
			char c = '/';
			if (theRelPath.IndexOf('\\') != -1 || theDir.IndexOf('\\') != -1)
			{
				c = '\\';
			}
			if (text2.Length >= 2 && text2[1] == ':')
			{
				text = text2.Substring(0, 2);
				text2 = text2.Remove(0, 2);
			}
			if (text2.Length > 0 && text2[text2.Length - 1] != '\\' && text2[text2.Length - 1] != '/')
			{
				text2 += c;
			}
			string text3 = theRelPath;
			while (text2.Length != 0)
			{
				int num = text3.IndexOf('\\');
				int num2 = text3.IndexOf('/');
				if (num == -1 || (num2 != -1 && num2 < num))
				{
					num = num2;
				}
				if (num == -1)
				{
					break;
				}
				string text4 = text3.Substring(0, num);
				text3 = text3.Remove(0, num + 1);
				if (text4 == "..")
				{
					string text5 = null;
					int num3 = Math.Max(text2.LastIndexOf('\\'), text2.LastIndexOf('/'));
					text5 = ((num3 == -1) ? text2 : text2.Substring(num3, text2.Length - num3 - 1));
					if (text5 == "..")
					{
						text2 += "..";
						text2 += c;
					}
					else
					{
						text2 = text2.Remove(num3);
					}
					continue;
				}
				if (text4 == "")
				{
					text2 += c;
					break;
				}
				if (!(text4 != "."))
				{
					continue;
				}
				text2 = text2 + text4 + c;
				break;
			}
			text2 = text + text2 + text3;
			if (c == '/')
			{
				text2.Replace('\\', '/');
			}
			else
			{
				text2.Replace('/', '\\');
			}
			return text2;
		}

		public static bool isSpace(char c)
		{
			if (c != ' ')
			{
				return c == '\t';
			}
			return true;
		}

		public static string Trim(string theString)
		{
			int i;
			for (i = 0; i < theString.Length && isSpace(theString[i]); i++)
			{
			}
			int num = theString.Length - 1;
			while (num >= 0 && isSpace(theString[num]))
			{
				num--;
			}
			return theString.Substring(i, num - i + 1);
		}

		public static bool DividePoly(Vector2[] v, int n, Vector2[,] theTris, int theMaxTris, ref int theNumTris)
		{
			int[] array = new int[V_MAX];
			if (n > V_MAX)
			{
				return false;
			}
			theNumTris = 0;
			int num = orientation(n, v);
			for (int i = 0; i < n; i++)
			{
				array[i] = i;
			}
			int num2 = n;
			while (num2 > 3)
			{
				float num3 = (float)BIG;
				int num4 = 0;
				int num5;
				int num6;
				for (int j = 0; j < num2; j++)
				{
					num5 = j - 1;
					num6 = j + 1;
					if (j == 0)
					{
						num5 = num2 - 1;
					}
					else if (j == num2 - 1)
					{
						num6 = 0;
					}
					float num7;
					if (determinant(array[num5], array[j], array[num6], v) == num && no_interior(array[num5], array[j], array[num6], v, array, num2, num) != 0 && (num7 = distance2(v[array[num5]].X, v[array[num5]].Y, v[array[num6]].X, v[array[num6]].Y)) < num3)
					{
						num3 = num7;
						num4 = j;
					}
				}
				if ((double)num3 == BIG)
				{
					return false;
				}
				num5 = num4 - 1;
				num6 = num4 + 1;
				if (num4 == 0)
				{
					num5 = num2 - 1;
				}
				else if (num4 == num2 - 1)
				{
					num6 = 0;
				}
				if (theNumTris >= theMaxTris)
				{
					return false;
				}
				theTris[theNumTris, 0].X = v[array[num5]].X;
				theTris[theNumTris, 0].Y = v[array[num5]].Y;
				theTris[theNumTris, 1].X = v[array[num4]].X;
				theTris[theNumTris, 1].Y = v[array[num4]].Y;
				theTris[theNumTris, 2].X = v[array[num6]].X;
				theTris[theNumTris, 2].Y = v[array[num6]].Y;
				theNumTris++;
				num2--;
				for (int i = num4; i < num2; i++)
				{
					array[i] = array[i + 1];
				}
			}
			if (theNumTris >= theMaxTris)
			{
				return false;
			}
			theTris[theNumTris, 0].X = v[array[0]].X;
			theTris[theNumTris, 0].Y = v[array[0]].Y;
			theTris[theNumTris, 1].X = v[array[1]].X;
			theTris[theNumTris, 1].Y = v[array[1]].Y;
			theTris[theNumTris, 2].X = v[array[2]].X;
			theTris[theNumTris, 2].Y = v[array[2]].Y;
			theNumTris++;
			return true;
		}

		private static int orientation(int n, Vector2[] v)
		{
			float num = v[n - 1].X * v[0].Y - v[0].X * v[n - 1].Y;
			for (int i = 0; i < n - 1; i++)
			{
				num += v[i].X * v[i + 1].Y - v[i + 1].X * v[i].Y;
			}
			if ((double)num >= 0.0)
			{
				return COUNTER_CLOCKWISE;
			}
			return CLOCKWISE;
		}

		private static int determinant(int p1, int p2, int p3, Vector2[] v)
		{
			float num = v[p1].X;
			float num2 = v[p1].Y;
			float num3 = v[p2].X;
			float num4 = v[p2].Y;
			float num5 = v[p3].X;
			float num6 = v[p3].Y;
			float num7 = (num3 - num) * (num6 - num2) - (num5 - num) * (num4 - num2);
			if ((double)num7 >= 0.0)
			{
				return COUNTER_CLOCKWISE;
			}
			return CLOCKWISE;
		}

		private static int no_interior(int p1, int p2, int p3, Vector2[] v, int[] vp, int n, int poly_or)
		{
			for (int i = 0; i < n; i++)
			{
				int num = vp[i];
				if (num != p1 && num != p2 && num != p3 && determinant(p2, p1, num, v) != poly_or && determinant(p1, p3, num, v) != poly_or && determinant(p3, p2, num, v) != poly_or)
				{
					return 0;
				}
			}
			return 1;
		}

		private static float distance2(float x1, float y1, float x2, float y2)
		{
			float num = x1 - x2;
			float num2 = y1 - y2;
			return num * num + num2 * num2;
		}

		public static int Rand()
		{
			return (int)gCommonMTRand.Next();
		}

		public static int Rand(int range)
		{
			return (int)gCommonMTRand.Next((uint)range);
		}

		public static float Rand(float range)
		{
			return gCommonMTRand.Next(range);
		}

		public static void SRand(uint theSeed)
		{
			gCommonMTRand.SRand(theSeed);
		}

		public static int SafeRand()
		{
			return (int)gCommonMTRand.Next();
		}

		public static string CommaSeperate(int theValue)
		{
			return SexyLocale.CommaSeparate(theValue);
		}

		public static string UCommaSeparate(uint theValue)
		{
			return SexyLocale.UCommaSeparate(theValue);
		}

		public static string CommaSeperate64(long theValue)
		{
			return SexyLocale.CommaSeparate64(theValue);
		}

		public static string UCommaSeparate64(ulong theValue)
		{
			return SexyLocale.UCommaSeparate64(theValue);
		}

		public static void SexySleep(int milliseconds)
		{
			try
			{
				Thread.Sleep(milliseconds);
			}
			catch (Exception)
			{
			}
		}

		public static int size<T>(List<T> list)
		{
			return list.Count;
		}

		public static T back<T>(List<T> list)
		{
			return list.Last();
		}

		public static T front<T>(List<T> list)
		{
			return list.First();
		}

		public static void Reserve<T>(List<T> list, int newSize)
		{
			if (list.Count > newSize)
			{
				list.RemoveRange(newSize, list.Count - newSize);
				return;
			}
			int num = newSize - list.Count;
			for (int i = 0; i < num; i++)
			{
				list.Add(default(T));
			}
		}

		public static void Resize<T>(List<T> list, int newSize)
		{
			if (list.Count > newSize)
			{
				list.RemoveRange(newSize, list.Count - newSize);
				return;
			}
			list.Capacity = newSize;
			int num = newSize - list.Count;
			if (typeof(T).IsValueType)
			{
				for (int i = 0; i < num; i++)
				{
					list.Add(default(T));
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					list.Add(Activator.CreateInstance<T>());
				}
			}
		}

		public static T[] CreateObjectArray<T>(int size)
		{
			T[] array = new T[size];
			if (!typeof(T).IsValueType)
			{
				for (int i = 0; i < size; i++)
				{
					array[i] = Activator.CreateInstance<T>();
				}
			}
			return array;
		}

		public static IFileDriver GetGameFileDriver()
		{
			return GlobalMembers.gFileDriver;
		}

		public static string GetAppDataFolder()
		{
			return GlobalMembers.gFileDriver.GetSaveDataPath();
		}

		public static string SetAppDataFolder(string thePath)
		{
			return "";
		}

		public static int IntRange(int min_val, int max_val)
		{
			if (min_val == max_val)
			{
				return min_val;
			}
			if (min_val < 0 && max_val < 0)
			{
				return min_val + SafeRand() % (Math.Abs(min_val) - Math.Abs(max_val));
			}
			return min_val + SafeRand() % (max_val - min_val + 1);
		}

		public static float FloatRange(float min_val, float max_val)
		{
			if (min_val == max_val)
			{
				return min_val;
			}
			if (min_val < 0f && max_val < 0f)
			{
				return min_val + (float)(SafeRand() % (int)((Math.Abs(min_val) - Math.Abs(max_val)) * 100000000f + 1f)) / 100000000f;
			}
			return min_val + (float)(SafeRand() % (int)((max_val - min_val) * 100000000f + 1f)) / 100000000f;
		}

		public static float SAFE_RAND(float val)
		{
			if (val != 0f)
			{
				return (float)Rand() % val;
			}
			return 0f;
		}

		public static bool _eq(float n1, float n2, float tolerance)
		{
			return Math.Abs(n1 - n2) <= tolerance;
		}

		public static bool _leq(float n1, float n2, float tolerance)
		{
			if (!_eq(n1, n2, tolerance))
			{
				return n1 < n2;
			}
			return true;
		}

		public static bool _geq(float n1, float n2, float tolerance)
		{
			if (!_eq(n1, n2, tolerance))
			{
				return n1 > n2;
			}
			return true;
		}

		public static bool _eq(float n1, float n2)
		{
			return Math.Abs(n1 - n2) <= float.Epsilon;
		}

		public static bool _leq(float n1, float n2)
		{
			if (!_eq(n1, n2, float.Epsilon))
			{
				return n1 < n2;
			}
			return true;
		}

		public static bool _geq(float n1, float n2)
		{
			if (!_eq(n1, n2, float.Epsilon))
			{
				return n1 > n2;
			}
			return true;
		}

		public static int Sign(int val)
		{
			if (val >= 0)
			{
				return 1;
			}
			return -1;
		}

		public static float Sign(float val)
		{
			if (!(val < 0f))
			{
				return 1f;
			}
			return -1f;
		}

		public static float AngleBetweenPoints(float p1x, float p1y, float p2x, float p2y)
		{
			return (float)Math.Atan2(0f - (p2y - p1y), p2x - p1x);
		}

		public static float AngleBetweenPoints(SexyFramework.Misc.Point p1, SexyFramework.Misc.Point p2)
		{
			return AngleBetweenPoints(p1.mX, p1.mY, p2.mX, p2.mY);
		}

		public static SexyVector2 RotatePoint(float pAngle, SexyVector2 pVector, SexyVector2 pCenter)
		{
			float num = pVector.x - pCenter.x;
			float num2 = pVector.y - pCenter.y;
			float theX = (float)((double)pCenter.x + (double)num * Math.Cos(pAngle) + (double)num2 * Math.Sin(pAngle));
			float theY = (float)((double)pCenter.y + (double)num2 * Math.Cos(pAngle) - (double)num * Math.Sin(pAngle));
			return new SexyVector2(theX, theY);
		}

		public static SexyVector2 RotatePoint(float pAngle, SexyVector2 pVector)
		{
			return RotatePoint(pAngle, pVector, new SexyVector2(0f, 0f));
		}

		public static void RotatePoint(float pAngle, ref float x, ref float y, float cx, float cy)
		{
			SexyVector2 sexyVector = RotatePoint(pAngle, new SexyVector2(x, y), new SexyVector2(cx, cy));
			x = sexyVector.x;
			y = sexyVector.y;
		}

		public static void _RotatePointClockwise(SexyFramework.Misc.Point p, float angle)
		{
			float num = (float)Math.Cos(angle);
			float num2 = (float)Math.Sin(angle);
			float num3 = p.mX;
			p.mX = (int)(num3 * num + (float)p.mY * num2);
			p.mY = (int)((0f - num3) * num2 + (float)p.mY * num);
		}

		public static bool RotatedRectsIntersect(Rect r1, float r1_angle, Rect r2, float r2_angle)
		{
			r1_angle = 0f - r1_angle;
			r2_angle = 0f - r2_angle;
			float num = r1_angle - r2_angle;
			float num2 = (float)Math.Cos(num);
			float num3 = (float)Math.Sin(num);
			SexyFramework.Misc.Point point = new SexyFramework.Misc.Point(r1.mWidth / 2, r1.mHeight / 2);
			SexyFramework.Misc.Point point2 = new SexyFramework.Misc.Point(r2.mWidth / 2, r2.mHeight / 2);
			SexyFramework.Misc.Point point3 = new SexyFramework.Misc.Point(r1.mX + point.mX, r1.mY + point.mY);
			SexyFramework.Misc.Point theTPoint = new SexyFramework.Misc.Point(r2.mX + point2.mX, r2.mY + point2.mY);
			SexyFramework.Misc.Point point4 = new SexyFramework.Misc.Point(theTPoint);
			point4 -= point3;
			_RotatePointClockwise(point4, r2_angle);
			SexyFramework.Misc.Point point5 = new SexyFramework.Misc.Point(point4);
			SexyFramework.Misc.Point point6 = new SexyFramework.Misc.Point(point4);
			point5 -= point2;
			point6 += point2;
			SexyFramework.Misc.Point point7 = default(SexyFramework.Misc.Point);
			SexyFramework.Misc.Point point8 = default(SexyFramework.Misc.Point);
			point7.mX = (int)((float)(-point.mY) * num3);
			point8.mX = point7.mX;
			float num4 = (float)point.mX * num2;
			point7.mX += (int)num4;
			point8.mX -= (int)num4;
			point7.mY = (int)((float)point.mY * num2);
			point8.mY = point7.mY;
			num4 = (float)point.mX * num3;
			point7.mY += (int)num4;
			point8.mY -= (int)num4;
			num4 = num3 * num2;
			if (num4 < 0f)
			{
				num4 = point7.mX;
				point7.mX = point8.mX;
				point8.mX = (int)num4;
				num4 = point7.mY;
				point7.mY = point8.mY;
				point8.mY = (int)num4;
			}
			if (num3 < 0f)
			{
				point8.mX = -point8.mX;
				point8.mY = -point8.mY;
			}
			if (point8.mX > point6.mX || point8.mX > -point5.mX)
			{
				return false;
			}
			float num5;
			float num6;
			if (num4 == 0f)
			{
				num5 = point7.mY;
				num6 = 0f - num5;
			}
			else
			{
				float num7 = point5.mX - point7.mX;
				float num8 = point6.mX - point7.mX;
				num5 = point7.mY;
				if (num8 * num7 > 0f)
				{
					float num9 = point7.mX;
					if (num7 < 0f)
					{
						num9 -= (float)point8.mX;
						num5 -= (float)point8.mY;
						num7 = num8;
					}
					else
					{
						num9 += (float)point8.mX;
						num5 += (float)point8.mY;
					}
					num5 *= num7;
					num5 /= num9;
					num5 += (float)point7.mY;
				}
				num7 = point5.mX + point7.mX;
				num8 = point6.mX + point7.mX;
				num6 = -point7.mY;
				if (num8 * num7 > 0f)
				{
					float num9 = -point7.mX;
					if (num7 < 0f)
					{
						num9 -= (float)point8.mX;
						num6 -= (float)point8.mY;
						num7 = num8;
					}
					else
					{
						num9 += (float)point8.mX;
						num6 += (float)point8.mY;
					}
					num6 *= num7;
					num6 /= num9;
					num6 -= (float)point7.mY;
				}
			}
			if (!(num5 < (float)point5.mY) || !(num6 < (float)point5.mY))
			{
				if (num5 > (float)point6.mY)
				{
					return !(num6 > (float)point6.mY);
				}
				return true;
			}
			return false;
		}

		public static float DistFromPointToLine(SexyFramework.Misc.Point line_p1, SexyFramework.Misc.Point line_p2, SexyFramework.Misc.Point p, ref float t)
		{
			return DistFromPointToLine(new FPoint(line_p1.mX, line_p1.mY), new FPoint(line_p2.mX, line_p2.mY), new FPoint(p.mX, p.mY), ref t);
		}

		public static float DistFromPointToLine(FPoint line_p1, FPoint line_p2, FPoint p, ref float t)
		{
			SexyVector2 v = new SexyVector2(p.mX - line_p1.mX, p.mY - line_p1.mY);
			SexyVector2 v2 = new SexyVector2(line_p2.mX - line_p1.mX, line_p2.mY - line_p1.mY);
			float num = v.Dot(v2);
			if (num <= 0f)
			{
				t = 0f;
				return v.Dot(v);
			}
			float num2 = v2.Dot(v2);
			if (num >= num2)
			{
				t = 1f;
				return v.Dot(v) - 2f * num + num2;
			}
			t = num / num2;
			return v.Dot(v) - t * num;
		}

		public static float DistFromPointToLine(Vector2 line_p1, Vector2 line_p2, Vector2 p, ref float t)
		{
			Vector2 value = line_p2 - line_p1;
			return Vector2.Distance(p, value);
		}

		public static float Distance(float p1x, float p1y, float p2x, float p2y, bool sqrt)
		{
			float num = p2x - p1x;
			float num2 = p2y - p1y;
			float num3 = num * num + num2 * num2;
			if (!sqrt)
			{
				return num3;
			}
			return (float)Math.Sqrt(num3);
		}

		public static float Distance(float p1x, float p1y, float p2x, float p2y)
		{
			return Distance(p1x, p1y, p2x, p2y, true);
		}

		public static float StrToFloat(string str)
		{
			if (str.Length == 0)
			{
				return 0f;
			}
			return float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		public static int StrToInt(string str)
		{
			if (str.Length == 0)
			{
				return 0;
			}
			return int.Parse(str);
		}

		public static bool StrEquals(string str1, string str2, bool pIgnoreCase)
		{
			if (!pIgnoreCase)
			{
				return str1 == str2;
			}
			return string.Compare(str1, str2, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		public static bool StrEquals(string str1, string str2)
		{
			return StrEquals(str1, str2, false);
		}

		public static float RadiansToDegrees(float pRads)
		{
			return pRads * 57.29694f;
		}

		public static float DegreesToRadians(float pDegs)
		{
			return pDegs * 0.017452938f;
		}

		public static float CaculatePowValume(float volume)
		{
			double num = volume;
			num *= 8.0;
			float num2 = 2f;
			double num3 = Math.Pow(num2, num) - 1.0;
			return (float)(num3 / (Math.Pow(num2, 8.0) - 1.0));
		}
	}
}
