using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SexyFramework
{
	public class PopLoc
	{
		private enum FormatField
		{
			None,
			Expect
		}

		private ILocalizedStringProvider localizedString;

		private Dictionary<int, string> mIdStrings;

		private Dictionary<string, string> mNameStrings;

		public ILocalizedStringProvider LocalizedString
		{
			get
			{
				return localizedString;
			}
			set
			{
				ILocalizedStringProvider localizedString2 = localizedString;
				localizedString = value;
			}
		}

		public PopLoc()
		{
			mIdStrings = new Dictionary<int, string>();
			mNameStrings = new Dictionary<string, string>();
		}

		public string ConvertFormatFields(string fmt)
		{
			int num = 0;
			string text = string.Empty;
			FormatField formatField = FormatField.None;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < fmt.Length; i++)
			{
				char c = fmt[i];
				if (formatField == FormatField.None)
				{
					if (c == '%')
					{
						formatField = FormatField.Expect;
					}
					else
					{
						stringBuilder.Append(c);
					}
					continue;
				}
				if (!char.IsDigit(c))
				{
					switch (c)
					{
					case '-':
					case '.':
						break;
					case '%':
						stringBuilder.Append('%');
						formatField = FormatField.None;
						continue;
					default:
						stringBuilder.Append('{');
						stringBuilder.AppendFormat("{0}", new object[1] { num++ });
						if (!string.IsNullOrEmpty(text))
						{
							stringBuilder.AppendFormat(":{0}{1}", new object[2] { c, text });
						}
						stringBuilder.Append('}');
						text = string.Empty;
						formatField = FormatField.None;
						continue;
					}
				}
				text += c;
				if (text == ".")
				{
					text = "0.";
				}
				if (text == "-.")
				{
					text = "-0.";
				}
			}
			return stringBuilder.ToString();
		}

		private string readDumpTemplate(string fileName)
		{
			string result = null;
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					using (StreamReader streamReader = new StreamReader(stream, true))
					{
						result = streamReader.ReadToEnd();
					}
				}
			}
			catch
			{
			}
			return result;
		}

		private string xmlConvert(string str)
		{
			StringBuilder stringBuilder = new StringBuilder(str);
			stringBuilder.Replace("&", "&amp;");
			stringBuilder.Replace("<", "&lt;");
			stringBuilder.Replace(">", "&gt;");
			return stringBuilder.ToString();
		}

		public void dumpLocalizedTextResource(string fileName)
		{
			string text = readDumpTemplate("StringsTemplate.resx");
			if (text == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int key in mIdStrings.Keys)
			{
				stringBuilder.AppendFormat("  <data name=\"IDS_{0}\" xml:space=\"preserve\"><value>{1}</value></data>\n", new object[2]
				{
					key,
					xmlConvert(mIdStrings[key])
				});
			}
			StringBuilder stringBuilder2 = new StringBuilder(text);
			stringBuilder2.Replace("%DUMPED_RESOURCE%", stringBuilder.ToString());
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
					{
						streamWriter.WriteLine(stringBuilder2.ToString());
					}
				}
			}
			catch
			{
			}
		}

		public string GetString(int id, string strDefault)
		{
			string text = null;
			if (LocalizedString != null)
			{
				text = LocalizedString.fromID(id);
			}
			if (text == null)
			{
				return strDefault;
			}
			return text;
		}

		public string GetString(string name, string strDefault)
		{
			if (!mNameStrings.ContainsKey(name))
			{
				return strDefault;
			}
			return mNameStrings[name];
		}

		public bool SetString(int id, string str, bool reset)
		{
			bool result = false;
			str = ConvertFormatFields(str);
			if (!mIdStrings.ContainsKey(id))
			{
				mIdStrings[id] = str;
				result = true;
			}
			else if (reset)
			{
				mIdStrings.Add(id, str);
				result = true;
			}
			return result;
		}

		public bool SetString(string name, string str, bool reset)
		{
			bool result = false;
			str = ConvertFormatFields(str);
			name = name.ToUpper();
			if (!mNameStrings.ContainsKey(name))
			{
				mNameStrings[name] = str;
				result = true;
			}
			else if (reset)
			{
				mNameStrings.Add(name, str);
				result = true;
			}
			return result;
		}

		public bool RemoveString(int id)
		{
			return mIdStrings.Remove(id);
		}

		public bool RemoveString(string name)
		{
			return mNameStrings.Remove(name.ToUpper());
		}

		public string Evaluate(string input)
		{
			int num = 0;
			do
			{
				int num2 = input.IndexOf('%', num);
				if (num2 < 0)
				{
					break;
				}
				int num3 = input.IndexOf('%', num2 + 1);
				if (num3 < 0)
				{
					break;
				}
				if (num3 == num2 + 1)
				{
					input.Remove(num3, 1);
					num = num3;
					continue;
				}
				string text = input.Substring(num2 + 1, num3 - (num2 + 1));
				int result = 0;
				if (!int.TryParse(text, out result))
				{
					string @string = GetString(text, string.Empty);
					input.Replace("%" + text + "%", @string);
					num = num2;
				}
				else
				{
					string string2 = GetString(result, GetString(text, string.Empty));
					input.Replace("%" + text + "%", string2);
					num = num2;
				}
			}
			while (num < input.Length);
			return input;
		}
	}
}
