using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvalScript.Evaluating
{
    public class EvalObject : Dictionary<string, object>
    {
        public static EvalObject PopulateFromJson(string json)
        {
            var jObj = JObject.Parse(json);
            return GetJObjectValue(jObj);
        }

        private static EvalObject GetJObjectValue(JObject jObj)
        {
            EvalObject output = new EvalObject();

            foreach (var property in jObj)
                output[property.Key] = GetJTokenValue(property.Value);

            return output;
        }

        private static object GetJTokenValue(JToken token)
        {
            if (token is JArray)
                return GetJArrayValue(token as JArray);

            else if (token is JObject)
                return GetJObjectValue(token as JObject);

            else if (token is JValue)
                return (token as JValue).Value;

            throw new Exception($"Failure when reading json data, couldn't handle token type {token.GetType().Name}");
        }

        private static List<object> GetJArrayValue(JArray array)
        {
            List<object> output = new List<object>();

            foreach (var item in array)
                output.Add(GetJTokenValue(item));

            return output;
        }



        public static List<EvalObject> PopulateFromCsv(string csv)
        {
            var lines = csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            var headers = SplitLine(lines[0]);
            FormatLineData(headers);

            lines = lines.GetRange(1, lines.Count - 1);
            var output = new List<EvalObject>();
            foreach(var line in lines)
            {
                var data = SplitLine(line);
                FormatLineData(data);

                var model = new EvalObject();
                for (int i = 0; i < headers.Count; i++)
                    model[headers[i]] = (i < data.Count) ? data[i] : null;
                output.Add(model);
            }
            return output;
        }

        private static void FormatLineData(List<string> data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                var s = data[i].Trim();
                if (s.StartsWith("\"") && s.EndsWith("\""))
                    s = s.Substring(1, s.Length - 2);

                s = s.Replace("\"\"", "\"");

                data[i] = s;
            }
        }

        private static List<string> SplitLine(string line)
        {
            var data = new List<string>();
            int lastCommaIndex = -1;
            int commaIndex = -1;
            while (true)
            {
                commaIndex = line.IndexOf(',', commaIndex + 1);
                if(commaIndex < 0)
                {
                    data.Add(line.Substring(lastCommaIndex + 1));
                    break;
                }
                string lineVal = line.Substring(lastCommaIndex + 1, commaIndex - lastCommaIndex - 1);
                if (lineVal.Count(x => x == '"') % 2 == 1)
                    continue;
                data.Add(lineVal);
                lastCommaIndex = commaIndex;
            }
            return data;
        }
    }
}
