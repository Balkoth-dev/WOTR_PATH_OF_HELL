﻿using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Cheats;
using Kingmaker.Dungeon;
using Kingmaker.Dungeon.Blueprints;
using Kingmaker.Localization;
using Kingmaker.UI;
using ModMenu.Settings;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WOTR_PATH_OF_HELL.Utilities;

namespace WOTR_PATH_OF_HELL.Settings
{
    public static class Settings
    {
        public static BlueprintGuid BoonGuid;
        public static readonly string RootKey = "wotr-path-of-hell.settings";
        public static List<BlueprintDungeonBoon> editedBoons = new List<BlueprintDungeonBoon>();
        public static T GetSetting<T>(string key)
        {
            try
            {
                return ModMenu.ModMenu.GetSettingValue<T>(GetKey(key));
            }
            catch(Exception ex)
            {
                Main.Log(ex.ToString());
                return default(T);
            }
        }
        private static string GetKey(string partialKey)
        {
            Regex rgx = new Regex("[^a-z0-9-]");
            partialKey = rgx.Replace(partialKey.ToLower(), "");
            return $"{RootKey}.{partialKey}";
        }
    }
    [HarmonyPatch(typeof(BlueprintsCache))]
    static class BlueprintsCache_Postfix
    {
        static bool Initialized;

        [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        static void Postfix()
        {
            if (Initialized) return;
            Initialized = true;
            
            SettingsUI.Initialize();
            Main.Log("Settings Initialized");
        }
        class SettingsUI
        {
            private static readonly string RootKey = Settings.RootKey;
            private static readonly SettingsBuilder sb = SettingsBuilder.New(RootKey, Helpers.CreateString(GetKey("title"), "PATH OF HELL"));
            private static readonly Kingmaker.Dungeon.Blueprints.BlueprintDungeonBoonReference[] dungeonBoons = BlueprintTool.Get<BlueprintDungeonRoot>("096f36d4e55b49129ddd2211b2c50513").m_Boons;
            private static readonly List<LocalizedString> boons = new List<LocalizedString>();

            public static void Initialize()
            {
                sb.AddImage(AssetLoader.LoadInternal("Settings", "pathofhell.png", 512,212),212);

                CreateSubHeader("patchessubheader");
                CreateToggle("aeongazepatch", true);
                CreateToggle("hellsauthoritypatch", true);

                ModMenu.ModMenu.AddSettings(sb);
            }
            private static void CreateSubHeader(string key)
            {
                sb.AddSubHeader(Helpers.CreateString(GetKey(key), Helpers.GetLocalizationElement("value", key)));
            }
            private static void CreateToggle(string key, bool defaultBool = false)
            {
                sb.AddToggle(Toggle.New(GetKey(key), defaultValue: defaultBool, Helpers.CreateString(GetKey(key+"-desc"), Helpers.GetLocalizationElement("description", key)))
                    .ShowVisualConnection()
                    .OnValueChanged(OnToggle)
                    .WithLongDescription(Helpers.CreateString(GetKey(key + "-long-desc"), Helpers.GetLocalizationElement("longDescription", key))));
            }
            private static void CreatePatchToggle(string key, bool defaultBool = false)
            {
                if (Helpers.GetLocalizationElement("Name", key,".") != null)
                {
                    sb.AddToggle(Toggle.New(GetKey(key), defaultValue: defaultBool, Helpers.CreateString(GetKey(key + "-desc"), Helpers.GetLocalizationElement("Name", key,".")))
                        .ShowVisualConnection()
                        .OnValueChanged(OnToggle)
                        .WithLongDescription(Helpers.CreateString(GetKey(key + "-long-desc"), Helpers.GetLocalizationElement("Description", key, "."))));
                    Main.Log(GetKey(key) + " Created");
                }
            }

            private static void CreateButton(string key, Action action)
            {
                sb.AddButton(Button.New(Helpers.CreateString(GetKey(key + "-desc"), Helpers.GetLocalizationElement("description", key)), Helpers.CreateString(GetKey(key + "-name"), Helpers.GetLocalizationElement("name", key)), action)
                    .WithLongDescription(Helpers.CreateString(GetKey(key + "-longDesc"), Helpers.GetLocalizationElement("longDescription", key))));
            }
            private static void CreateDropdownButton(string key, Action<int> action,List<LocalizedString> list)
            {
                sb.AddDropdownButton(DropdownButton.New(GetKey(key),0,Helpers.CreateString(GetKey(key + "-desc"), Helpers.GetLocalizationElement("description", key)), Helpers.CreateString(GetKey(key + "-buttontext"), Helpers.GetLocalizationElement("buttonText", key)),action,list)
                    .WithLongDescription(Helpers.CreateString(GetKey(key + "-longDesc"), Helpers.GetLocalizationElement("longDescription", key))));
            }
            private static void CreateDropdown(string key, List<LocalizedString> values, int defaultValue = 0)
            {
                sb.AddDropdownList(DropdownList.New(
                    GetKey(key),
                    defaultValue,
                    Helpers.GetLocalizationElement("description", key),
                    values
                    )
                    .WithLongDescription(Helpers.CreateString(GetKey(key + "-longDesc"), Helpers.GetLocalizationElement("longDescription", key))));

            }
            private static void CreateKeyBinding(string key, Action action, KeyboardAccess.GameModesGroup gamesModeGroup = KeyboardAccess.GameModesGroup.All, UnityEngine.KeyCode firstKey = UnityEngine.KeyCode.None, bool withctrl = false)
            {
                sb.AddKeyBinding(KeyBinding.New(GetKey(key), gamesModeGroup, Helpers.CreateString(GetKey(key + "-desc"), Helpers.GetLocalizationElement("description", key))).SetPrimaryBinding(firstKey, withctrl)
                    .WithLongDescription(Helpers.CreateString(GetKey(key + "-longDesc"), Helpers.GetLocalizationElement("longDescription", key))),
                    action);
            }
            private static string GetKey(string partialKey)
            {
                Regex rgx = new Regex("[^a-z0-9-]");
                partialKey = rgx.Replace(partialKey.ToLower(), "");
                return $"{RootKey}.{partialKey}";
            }
            private static void OnToggle(bool value)
            {
                
            }
        }
    }
}
