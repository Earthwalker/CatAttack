using System;
using System.Collections.Generic;

namespace DynamicSettings
{
    public class Setting
    {
        public string name;
        public string value;
        public string comment;
        public string section;

        public Setting(string name, string value, string comment, string section)
        {
            this.name = name;
            this.value = value;
            this.comment = comment;
            this.section = section;
        }

        public Setting(string[] data)
        {
            name = data[0];
            value = data[1];

            if (data.Length >= 3)
                comment = data[2];
            else
                comment = "";

            if (data.Length >= 4)
                section = data[3];
            else
                section = "";
        }
    }
}
