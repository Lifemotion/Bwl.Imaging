using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bwl.Imaging
{

    [JsonConverter(typeof(ParametersDictionaryJsonConverter))]
    public class ParametersDictionary : IEnumerable<Parameter>
    {

        private Dictionary<string, Parameter> _data = new Dictionary<string, Parameter>();
        private object _syncRoot = new object();

        public ParametersDictionary()
        {
        }

        public ParametersDictionary(IEnumerable<Parameter> values)
        {
            AddRange(values);
        }

        public virtual IEnumerator<Parameter> GetEnumerator()
        {
            IEnumerable<Parameter> values = null;
            lock (_syncRoot)
                values = _data.Values.ToArray();
            return values.GetEnumerator();
        }

        private IEnumerator IEnumerable_GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => IEnumerable_GetEnumerator();

        public Parameter this[string key]
        {
            get
            {
                lock (_syncRoot)
                    return _data[key];
            }
            set
            {
                lock (_syncRoot)
                    _data[key] = value;
            }
        }

        public string ItemValue(string key)
        {
            lock (_syncRoot)
                return _data[key].Value;
        }

        public void Add(string key, string value, bool visible = false)
        {
            lock (_syncRoot)
                _data.Add(key, new Parameter(key, value, visible));
        }

        public void Add(Parameter value)
        {
            lock (_syncRoot)
                _data.Add(value.Key, value);
        }

        public void AddRange(IEnumerable<Parameter> values)
        {
            lock (_syncRoot)
            {
                foreach (var value in values)
                    _data.Add(value.Key, value);
            }
        }

        public void Remove(string key)
        {
            lock (_syncRoot)
                _data.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            lock (_syncRoot)
                return _data.ContainsKey(key);
        }

        public void Clear()
        {
            lock (_syncRoot)
                _data.Clear();
        }

        public Parameter[] ToArray()
        {
            lock (_syncRoot)
                return _data.Values.ToArray();
        }
    }

    internal sealed class ParametersDictionaryJsonConverter : JsonConverter<ParametersDictionary>
    {
        public override ParametersDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var map = JsonSerializer.Deserialize<Dictionary<string, Parameter>>(ref reader, options)
                      ?? new Dictionary<string, Parameter>();

            var result = new ParametersDictionary();
            foreach (var kv in map)
                result.Add(kv.Value ?? new Parameter(kv.Key, string.Empty));
            return result;
        }

        public override void Write(Utf8JsonWriter writer, ParametersDictionary value, JsonSerializerOptions options)
        {
            var map = value.ToArray().ToDictionary(p => p.Key, p => p);
            JsonSerializer.Serialize(writer, map, options);
        }
    }
}