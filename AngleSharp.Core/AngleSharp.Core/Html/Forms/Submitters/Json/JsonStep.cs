#nullable disable

namespace AngleSharp.Html.Forms.Submitters.Json
{
    using AngleSharp.Text;
    using System;
    using System.Collections.Generic;

    abstract class JsonStep
    {
        public Boolean Append { get; set; }

        public JsonStep Next { get; set; }

        public static IEnumerable<JsonStep> Parse(String path)
        {
            var steps = new List<JsonStep>();
            var index = 0;

            while (index < path.Length && path[index] != Symbols.SquareBracketOpen)
            {
                index++;
            }

            if (index == 0)
            {
                return FailedJsonSteps(path);
            }

            steps.Add(new ObjectStep(path.Substring(0, index)));

            while (index < path.Length)
            {
                if (index + 1 >= path.Length || path[index] != Symbols.SquareBracketOpen)
                {
                    return FailedJsonSteps(path);
                }
                else if (path[index + 1] == Symbols.SquareBracketClose)
                {
                    steps[steps.Count - 1].Append = true;
                    index += 2;

                    if (index < path.Length)
                    {
                        return FailedJsonSteps(path);
                    }
                }
                else if (path[index + 1].IsDigit())
                {
                    var start = ++index;

                    while (index < path.Length && path[index] != Symbols.SquareBracketClose)
                    {
                        if (!path[index].IsDigit())
                        {
                            return FailedJsonSteps(path);
                        }

                        index++;
                    }

                    if (index == path.Length)
                    {
                        return FailedJsonSteps(path);
                    }

                    steps.Add(new ArrayStep(path.Substring(start, index - start).ToInteger(0)));
                    index++;
                }
                else
                {
                    var start = ++index;

                    while (index < path.Length && path[index] != Symbols.SquareBracketClose)
                    {
                        index++;
                    }

                    if (index == path.Length)
                    {
                        return FailedJsonSteps(path);
                    }

                    steps.Add(new ObjectStep(path.Substring(start, index - start)));
                    index++;
                }
            }

            var n = steps.Count - 1;

            for (var i = 0; i < n; i++)
            {
                steps[i].Next = steps[i + 1];
            }

            return steps;
        }

        static IEnumerable<JsonStep> FailedJsonSteps(String original) => new[] { new ObjectStep(original) };

        protected abstract JsonElement CreateElement();

        protected abstract JsonElement SetValue(JsonElement context, JsonElement value);

        protected abstract JsonElement GetValue(JsonElement context);

        protected abstract JsonElement ConvertArray(JsonArray value);

        public JsonElement Run(JsonElement context, JsonElement value, Boolean file = false)
        {
            if (Next is null)
            {
                return JsonEncodeLastValue(context, value, file);
            }
            else
            {
                return JsonEncodeValue(context, value, file);
            }
        }

        private JsonElement JsonEncodeValue(JsonElement context, JsonElement value, Boolean file)
        {
            var currentValue = GetValue(context);

            if (currentValue is null)
            {
                var newValue = Next.CreateElement();
                return SetValue(context, newValue);
            }
            else if (currentValue is JsonObject)
            {
                return currentValue;
            }
            else if (currentValue is JsonArray)
            {
                return SetValue(context, Next.ConvertArray((JsonArray)currentValue));
            }
            else
            {
                var obj = new JsonObject { [String.Empty] = currentValue };
                return SetValue(context, obj);
            }
        }

        private JsonElement JsonEncodeLastValue(JsonElement context, JsonElement value, Boolean file)
        {
            var currentValue = GetValue(context);

            //undefined
            if (currentValue is null)
            {
                if (Append)
                {
                    var arr = new JsonArray(1) { value };

                    value = arr;
                }

                SetValue(context, value);
            }
            else if (currentValue is JsonArray jsonArray)
            {
                jsonArray.Push(value);
            }
            else if (currentValue is JsonObject && !file)
            {
                var step = new ObjectStep(String.Empty);
                return step.JsonEncodeLastValue(currentValue, value, file: true);
            }
            else
            {
                var arr = new JsonArray(2) { currentValue, value };

                SetValue(context, arr);
            }

            return context;
        }

        private sealed class ObjectStep : JsonStep
        {
            public ObjectStep(String key)
            {
                Key = key;
            }

            public String Key { get; }

            protected override JsonElement GetValue(JsonElement context) => context[Key];

            protected override JsonElement SetValue(JsonElement context, JsonElement value)
            {
                context[Key] = value;
                return value;
            }

            protected override JsonElement CreateElement() => new JsonObject();

            protected override JsonElement ConvertArray(JsonArray values)
            {
                var obj = new JsonObject();

                for (var i = 0; i < values.Length; i++)
                {
                    var item = values[i];

                    if (item != null)
                    {
                        obj[i.ToString()] = item;
                    }
                }

                return obj;
            }
        }

        private sealed class ArrayStep : JsonStep
        {
            public ArrayStep(Int32 key)
            {
                Key = key;
            }

            public Int32 Key { get; }

            protected override JsonElement GetValue(JsonElement context)
            {
                if (context is JsonArray array)
                {
                    return array[Key];
                }

                return context[Key.ToString()];
            }

            protected override JsonElement SetValue(JsonElement context, JsonElement value)
            {

                if (context is JsonArray array)
                {
                    array[Key] = value;
                }
                else
                {
                    context[Key.ToString()] = value;
                }

                return value;
            }

            protected override JsonElement CreateElement() => new JsonArray();

            protected override JsonElement ConvertArray(JsonArray value) => value;
        }
    }
}
