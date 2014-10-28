using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FixSchema
{
    public sealed class Fields
    {
        private static readonly Lazy<Fields> lazy = new Lazy<Fields>(() => new Fields());
        public static Fields Instance { get { return lazy.Value; } }

        SortedDictionary<int, Field> _fieldsByTag;
        SortedDictionary<string, Field> _fieldsByName;

        private Fields()
        {
            _fieldsByTag = new SortedDictionary<int, Field>();
            _fieldsByName = new SortedDictionary<string, Field>();
        }

        public void AddField(Field field)
        {
            _fieldsByTag.Add(field.Tag, field);
            _fieldsByName.Add(field.Name, field);
        }

        public void AddField(int number, string name, string type)
        {
            Field f = new Field(number, name, type);
            _fieldsByTag.Add(number, f);
            _fieldsByName.Add(name, f);
        }

        public Field GetField(int tag)
        {
            if (_fieldsByTag.ContainsKey(tag))
            {
                return _fieldsByTag[tag];
            }
            else
                return null;
        }

        public Field GetField(string name)
        {
            if (_fieldsByName.ContainsKey(name))
            {
                return _fieldsByName[name];
            }
            else
                return null;
        }
    }
}
