using System;
using System.IO;
using System.Collections.Generic;
using Ini;

namespace DynamicSettings
{
    /// <summary>
    /// Configuration settings that are loaded from a file and can be accessed by others
    /// 
    /// Attached to GameManager
    /// </summary>
    public class SettingsManager
    { 
        List<Setting> settingsList = new List<Setting>();

        /// <summary>
        /// Loads from an ini
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadFromIni(string path)
        {
            IniFile iniFile = new IniFile();

            if (iniFile.LoadFromFile(path))
            {
                foreach(IniSection section in iniFile.GetSections())
                {
                    foreach(IniKey key in section.GetKeys())
                    {
                        List<string> list = new List<string>(key.ToArray());
                        list.Add(section.name);

                        settingsList.Add(new Setting(list.ToArray()));
                    }
                }

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Saves to an ini
        /// </summary>
        /// <param name="path"></param>
        public void SaveToIni(string path)
        {
            IniFile iniFile = new IniFile();

            foreach(Setting setting in settingsList)
            {
                iniFile.AddSection(setting.section);
                iniFile[setting.section].AddKey(new IniKey(setting.name, setting.value, setting.comment));
            }

            iniFile.SaveToFile(path);
        }

        /// <summary>
        /// Gets the data of a setting
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The setting if it exists, otherwise, an empty setting</returns>
        public Setting GetSetting(string name)
        {
            return settingsList.Find(setting => setting.name == name);
        }

        /// <summary>
        /// Modifies an existing setting or adds a new one
        /// </summary>
        /// <param name="newSetting"></param>
        public void ModifySetting(Setting newSetting)
        {
            Setting setting = settingsList.Find(item => item.name == newSetting.name);

            if (setting == null)
                settingsList.Add(newSetting);
            else
            {
                if (newSetting.value != "")
                    setting.value = newSetting.value;
                if (newSetting.comment != "")
                    setting.comment = newSetting.comment;
                if (newSetting.section != "")
                    setting.section = newSetting.section;
            }
        }

        public List<Setting> GetSettingsList()
        {
            return settingsList;
        }
    }
}