using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DynamicSettings;
//using Wenzil.Console;

/// <summary>
/// Configuration settings that are loaded from a file and can be accessed by others
/// 
/// Attached to GameManager
/// </summary>
public class Settings : MonoBehaviour
{
    //prefabs

	//read only
	const string gameName = "Game";
	const string gameVersion = "0.0.0.1";

    SettingsManager settingsManager = new SettingsManager();

    string path = "settings";

    /// <summary>
    /// Sets variables for starting
    /// </summary>
	void Awake()
	{
        LoadSettings();
	}

    public void LoadSettings()
    {
        //load the file from our directory
        if (settingsManager.LoadFromIni(Path.Combine(Application.streamingAssetsPath, path + ".ini")))
        {
            //TODO: Uncomment console stuff here
            //foreach (Setting setting in settingsManager.GetSettingsList())
            //    ConsoleCommandsDatabase.RegisterCommand("/" + setting.name, setting.comment, "", ConsoleSetting );
        }
        else
        {
            //display error
        }
    }

    /// <summary>
    /// Loads settings from a file
    /// </summary>
    /// <param name="path"></param>
	public void LoadSettings(string path)
	{
        this.path = path;

        LoadSettings();
	}

    public void SaveSettings()
    {
		//save the file to our directory
		settingsManager.SaveToIni(Path.Combine(Application.streamingAssetsPath, path + ".ini"));
    }
	
    /// <summary>
    /// Saves settings to a file
    /// </summary>
    /// <param name="path"></param>
	public void SaveSettings(string path)
	{
        this.path = path;

        SaveSettings();
	}

    /// <summary>
    /// Gets the data of a setting
    /// </summary>
    /// <param name="name"></param>
    /// <returns>The setting if it exists, otherwise, an empty setting</returns>
    public Setting GetSetting(string name)
    {
        return settingsManager.GetSetting(name);
    }

    /// <summary>
    /// Modifies an existing setting or adds a new one
    /// </summary>
    /// <param name="newSetting"></param>
    public void ModifySetting(Setting newSetting)
    {
        settingsManager.ModifySetting(newSetting);
    }

    public string GetSettingValueAsString(string name)
    {
        return settingsManager.GetSetting(name).value;
    }

    public bool GetSettingValueAsBool(string name)
    {
        Setting setting = settingsManager.GetSetting(name);

        if(setting != null)
            return Convert.ToBoolean(setting.value);

        //TODO: throw exception
        return false;
    }

    public int GetSettingValueAsInt(string name)
    {
        Setting setting = settingsManager.GetSetting(name);

        if (setting != null)
            return Convert.ToInt32(setting.value);

        //TODO: throw exception
        return 0;
    }

    public float GetSettingValueAsFloat(string name)
    {
        Setting setting = settingsManager.GetSetting(name);

        if (setting != null)
            return (float)Convert.ToDouble(setting.value);

        //TODO: throw exception
        return 0;
    }

    string ConsoleSetting(params string[] args)
    {
        if (args.Length == 1)
        {
            Setting setting = GetSetting(args[0]);

            if (setting != null)
                return setting.value;
        }
        else
            ModifySetting(new Setting(args));

        return "";
    }
}
