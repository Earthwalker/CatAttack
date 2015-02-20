using System.Collections.Generic;

namespace Ini
{
    class IniSection
    {
        string _name;
        List<IniKey> _keyList = new List<IniKey>();

        public IniSection(string name)
        {
            _name = name;
        }

        public void AddKey(string name)
        {
            _keyList.Add(new IniKey(name));
        }

        public bool AddKey(IniKey key)
        {
            if (!KeyExists(key.name))
            {
                _keyList.Add(key);
                return true;
            }
            else
                return false;
        }

        public bool RemoveKey(string name)
        {
            return _keyList.Remove(_keyList.Find(key => key.name == name));
        }

        public bool RemoveKey(IniKey key)
        {
            return _keyList.Remove(key);
        }

        public bool KeyExists(string name)
        {
            return _keyList.Find(key => key.name == name) != null;
        }

        public IniKey this[int ind]
        {
            get
            {
                return _keyList[ind];
            }
            set
            {
                _keyList[ind] = value;
            }
        }

        public IniKey this[string name]
        {
            get
            {
                return _keyList.Find(key => key.name == name);
            }
            set
            {
                _keyList[_keyList.FindIndex(key => key.name == name)] = value;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public IniKey[] GetKeys()
        {
            return _keyList.ToArray();
        }
        
        public string[] ToArray()
        {
            string[] array = new string[_keyList.Count];

            for (int i = 0; i < array.Length; i++)
                array[i] = _keyList[i].ToFormattedString();

            return array;
        }
    }
}
