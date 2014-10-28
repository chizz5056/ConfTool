using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FixSchema
{
    public class Field
    {
        public int Tag { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        private SortedDictionary<string, string> _fieldValues;

        public Field(int number, string name, string type)
        {
            Tag = number;
            Name = name;
            Type = type;
            _fieldValues = new SortedDictionary<string, string>();
        }

        public void AddFieldValue(string name, string description)
        {
            _fieldValues.Add(name, description);
        }

        public bool ContainsFieldValue(string value)
        {
            return _fieldValues.ContainsKey(value);
        }

        public string GetFieldValue(string value)
        {
            return _fieldValues[value];
        }
    }
}
