using System;
using System.Collections.Generic;
using System.IO;

namespace Ini
{
    class IniFile
    {
        List<IniSection> _sectionList = new List<IniSection>();

        public bool LoadFromString(string str)
        {
            try
            {
                using (StringReader reader = new StringReader(str))
                {
                    string line;
                    string section = "";

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line[0] == '[')
                        {
                            section = line.Remove(0, 1).Split(']')[0];
                            AddSection(section);
                        }

                        if (line[0] != ';')
                            this[section].AddKey(new IniKey(line.Split(new char[] { '=', ';' })));
                    }

                    reader.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool LoadFromFile(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                using(StreamReader reader = new StreamReader(path))
                {
                    string line;
                    string section = "";

                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line[0] == '[')
                            {
                                section = line.Remove(0, 1).Split(']')[0];
                                AddSection(section);
                            }
                            else
                            {
                                if (line[0] != ';')
                                {
                                    string[] stringArray = line.Split(new char[] { '=', ';' });

                                    if (stringArray.Length > 0)
                                        this[section].AddKey(new IniKey(stringArray));
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public string SaveToString()
        {
            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    foreach (IniSection section in _sectionList)
                    {
                        writer.WriteLine("[" + section.name + "]");

                        for (int i = 0; i < section.ToArray().Length; i++)
                            writer.WriteLine(section.ToArray()[i]);

                        writer.WriteLine();
                    }

                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void SaveToFile(string path)
        {
            try
            {
                using(StreamWriter writer = new StreamWriter(path))
                {
                    foreach(IniSection section in _sectionList)
                    {
                        writer.WriteLine("[" + section.name + "]");

                        for (int i = 0; i < section.ToArray().Length; i++)
                            writer.WriteLine(section.ToArray()[i]);

                        writer.WriteLine();
                    }

                    writer.Close();
                }
            }
            catch(Exception)
            {
                return;
            }
        }

        public bool AddSection(string name)
        {
            if (!SectionExists(name))
            {
                _sectionList.Add(new IniSection(name));
                return true;
            }
            else
                return false;
        }

        public bool RemoveSection(string name)
        {
            return _sectionList.Remove(_sectionList.Find(section => section.name == name));
        }

        public bool SectionExists(string name)
        {
            return _sectionList.Find(section => section.name == name) != null;
        }

        public IniSection this[int ind]
        {
            get
            {
                return _sectionList[ind];
            }
            set
            {
                _sectionList[ind] = value;
            }
        }

        public IniSection this[string name]
        {
            get
            {
                return _sectionList.Find(section => section.name == name);
            }
            set
            {
                _sectionList[_sectionList.FindIndex(section => section.name == name)] = value;
            }
        }

        public IniKey GetKey(string name, string section="")
        {
            if (section != "")
                return this[section][name];
            else
            {
                foreach(IniSection sect in _sectionList)
                {
                    if (sect[name] != null)
                        return sect[name];
                }
            }

            return null;
        }

        public IniSection[] GetSections()
        {
            return _sectionList.ToArray();
        }
    }
}
