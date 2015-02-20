namespace Ini
{
    class IniKey
    {
        string _name;
        string _value;
        string _comment;

        public IniKey(string name)
        {
            _name = name;
            _value = "";
            _comment = "";
        }

        public IniKey(string name, string value, string comment = "")
        {
            _name = name;
            _value = value;
            _comment = comment;
        }

        public IniKey(string[] data)
        {
            _name = data[0].Trim();

            if (data.Length >= 2)
                _value = data[1].Trim();
            else
                _value = "";

            if (data.Length >= 3)
                _comment = data[2].Trim();
            else
                _comment = "";
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public string ToFormattedString()
        {
            return _name + "=" + _value + " ;" + _comment;
        }

        public string[] ToArray()
        {
            return new string[] { _name, _value, _comment };
        }
    }
}
