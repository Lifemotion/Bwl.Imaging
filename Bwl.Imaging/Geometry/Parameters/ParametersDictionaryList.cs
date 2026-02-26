using System;
using System.Collections.Generic;

namespace Bwl.Imaging
{
    public class ParametersDictionaryList : List<Parameter>
    {

        public Parameter Add(string key, string value, bool visible = false)
        {
            lock (this)
            {
                if (key is null || key.Length == 0)
                    throw new Exception("ParametersDictionary::Add(): key Is Nothing OrElse key.Length = 0");
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                    {
                        throw new Exception("Parameter with this key exists");
                    }
                }
                var newParam = new Parameter(key, value, visible);
                Add(newParam);

                return newParam;
            }
        }

        public new void Add(Parameter addingParameter)
        {
            lock (this)
            {
                if (addingParameter is null)
                    throw new Exception("addingParameter nothing");
                if (addingParameter.Key is null || addingParameter.Key.Length == 0)
                    throw new Exception("ParametersDictionary::Add(): addingParameter.Key Is Nothing");
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (addingParameter.Key.ToLower() ?? ""))
                    {
                        throw new Exception("Parameter with this key exists");
                    }
                }
                base.Add(addingParameter);
            }
        }

        public void Remove(string key)
        {
            lock (this)
            {
                var items2Remove = new LinkedList<Parameter>();
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                    {
                        items2Remove.AddLast(@param);
                    }
                }

                foreach (var item2Remove in items2Remove)
                    Remove(item2Remove);
            }
        }

        public bool GetContainsKey(string key)
        {
            lock (this)
            {
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public Parameter this[string key]
        {
            get
            {
                lock (this)
                {
                    foreach (var @param in this)
                    {
                        if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                        {
                            return @param;
                        }
                    }
                    throw new Exception("ParametersDictionary::Add(): key not found");
                }
            }
            set
            {
                lock (this)
                {
                    if (value is null)
                        throw new Exception("ParametersDictionary::Add(): value Is Nothing");
                    foreach (var @param in this)
                    {
                        if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                        {
                            @param.Value = value.Value;
                            @param.Caption = value.Caption;
                            @param.Unit = value.Unit;
                            @param.AdditionalSettings = value.AdditionalSettings;
                            @param.Visible = value.Visible;
                        }
                    }
                    Add(value);
                }
            }
        }

        public string GetItemValue(string key)
        {
            lock (this)
            {
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                    {
                        return @param.Value;
                    }
                }
                throw new Exception("ParametersDictionary::ItemValue(): key not found");
            }
        }
        public void SetItemValue(string key, string value)
        {
            lock (this)
            {
                if (value is null)
                    throw new Exception("ParametersDictionary::ItemValue(): value is Nothing");
                foreach (var @param in this)
                {
                    if ((@param.Key.ToLower() ?? "") == (key.ToLower() ?? ""))
                    {
                        @param.Value = value;
                    }
                }
                Add(key, value);
            }
        }
    }
}