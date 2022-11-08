using HarmonyLib;
using System;
using System.IO;
using UnityModManagerNet;
using WOTR_PATH_OF_HELL.Utilities;
using ModKit;
using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.Dungeon.Blueprints;
using Kingmaker.Blueprints.Root;
using System.Linq;
using BlueprintCore.Utils;
using UnityEngine;
using Kingmaker.Controllers.Units;
using Kingmaker.UnitLogic;
using Kingmaker.GameModes;
using Kingmaker.Dungeon;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using WOTR_PATH_OF_HELL.Settings;

namespace WOTR_PATH_OF_HELL
{
    public class Main
    {
        public class Settings : UnityModManager.ModSettings
        {
            public override void Save(UnityModManager.ModEntry modEntry)
            {
                Save(this, modEntry);
            }
        }
        public static UnityModManager.ModEntry modInfo = null;
        private static bool enabled;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            AssetLoader.ModEntry = modEntry;
            modInfo = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            harmony.PatchAll();
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
        }

        private static bool Unload(UnityModManager.ModEntry arg)
        {
            throw new NotImplementedException();
        }
        public static void Log(string msg)
        {
#if DEBUG
            modInfo.Logger.Log(msg);
#endif
        }
        public bool GetSettingValue(string b)
        {
            return true;
        }
        public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            //settings.Save(modEntry);
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

    }
}
