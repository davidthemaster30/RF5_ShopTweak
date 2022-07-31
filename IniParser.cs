using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RF5_ShopTweak
{
	public class IniParser
	{
		class TypeSet
		{
			public bool boolValue = false;
			public int intValue = 0;
			public float floatValue = 0f;
			public string stringValue = "";

			public TypeSet(bool boolValue)
			{
				this.boolValue = boolValue;
				intValue = boolValue ? 1 : 0;
				floatValue = boolValue ? 1.0f : 0.0f;
				stringValue = boolValue ? "true" : "false";
			}

			public TypeSet(int intValue)
			{
				boolValue = intValue > 0;
				this.intValue = intValue;
				floatValue = intValue;
				stringValue = intValue.ToString();
			}

			public TypeSet(float floatValue)
			{
				boolValue = floatValue > 0;
				intValue = (int)floatValue;
				this.floatValue = floatValue;
				stringValue = floatValue.ToString();
			}

			public TypeSet(string stringValue)
			{
				boolValue = stringValue.Length > 0;
				int.TryParse(stringValue, out intValue);
				float.TryParse(stringValue, out floatValue);
				this.stringValue = stringValue;
			}
		}

		private Dictionary<string, Dictionary<string, TypeSet>> data = new Dictionary<string, Dictionary<string, TypeSet>>();

		public IniParser()
		{
		}

		public IniParser(string fileName)
		{
			string line = "";
			string category = "";
			using (StreamReader file = new StreamReader(fileName, Encoding.GetEncoding("utf-8")))
			{
				while ((line = file.ReadLine()) != null)
				{
					line = line.Trim();
					int delimiter = line.IndexOf(';');
					if(delimiter > -1)
						line = line.Substring(0, delimiter).Trim();
					if (line.Length <= 0)
						continue;

					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						category = line.Substring(1, line.Length - 2);
						//Main.Log.LogInfo(string.Format("category {0}", category));
					}
					else
					{
						delimiter = line.IndexOf('=');
						if (delimiter > -1)
						{
							string key = line.Substring(0, delimiter).Trim();
							string value = line.Substring(delimiter + 1);
							//Main.Log.LogInfo(string.Format("category {0} key {1} value {2}", category, key, value));

							if (!data.ContainsKey(category))
								data.Add(category, new Dictionary<string, TypeSet>());
							if (!data[category].ContainsKey(key))
								data[category].Add(key, ParseValue(value));
						}
					}
				}
			}
		}

		private TypeSet ParseValue(string value)
		{
			if(value.Length <= 0)
				return null;

			if (string.Compare(value, "true", true) == 0)
				return new TypeSet(true);
			if (string.Compare(value, "false", true) == 0)
				return new TypeSet(false);

			float fvalue;
			if(float.TryParse(value, out fvalue))
				return new TypeSet(fvalue);

			int ivalue;
			if (int.TryParse(value, out ivalue))
				return new TypeSet(ivalue);

			return new TypeSet(value);
		}

		private int InsertInt(string category, string key, int value)
		{
			if (!data.ContainsKey(category))
				data.Add(category, new Dictionary<string, TypeSet>());
			if (!data[category].ContainsKey(key))
				data[category].Add(key, new TypeSet(value));
			return data[category][key].intValue;
		}

		private float InsertFloat(string category, string key, float value)
		{
			if (!data.ContainsKey(category))
				data.Add(category, new Dictionary<string, TypeSet>());
			if (!data[category].ContainsKey(key))
				data[category].Add(key, new TypeSet(value));
			return data[category][key].floatValue;
		}

		private bool InsertBool(string category, string key, bool value)
		{
			if (!data.ContainsKey(category))
				data.Add(category, new Dictionary<string, TypeSet>());
			if (!data[category].ContainsKey(key))
				data[category].Add(key, new TypeSet(value));
			return data[category][key].boolValue;
		}

		private string InsertString(string category, string key, string value)
		{
			if (!data.ContainsKey(category))
				data.Add(category, new Dictionary<string, TypeSet>());
			if (!data[category].ContainsKey(key))
				data[category].Add(key, new TypeSet(value));
			return data[category][key].stringValue;
		}

		public int GetInt(string category, string name, int defaultValue)
		{
			return InsertInt(category, name, defaultValue);
		}

		public float GetFloat(string category, string name, float defaultValue)
		{
			return InsertFloat(category, name, defaultValue);
		}

		public bool GetBool(string category, string name, bool defaultValue)
		{
			return InsertBool(category, name, defaultValue);
		}

		public string GetString(string category, string name, string defaultValue)
		{
			return InsertString(category, name, defaultValue);
		}

		public void Save(string fileName)
		{
			using(StreamWriter file = new StreamWriter(fileName, false, Encoding.GetEncoding("utf-8")))
			{
				foreach(var category in data)
				{
					file.WriteLine(string.Format("[{0}]", category.Key));
					foreach(var keyValue in category.Value)
						file.WriteLine(string.Format("{0}={1}", keyValue.Key, keyValue.Value.stringValue));
					file.WriteLine();
				}
			}
		}
	}
}
