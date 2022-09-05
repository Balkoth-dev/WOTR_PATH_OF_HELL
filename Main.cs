﻿using HarmonyLib;
using System;
using System.IO;
using UnityModManagerNet;
using WOTR_BOAT_BOAT_BOAT.Utilities;
using ModKit;
using Kingmaker;
using Kingmaker.PubSubSystem;
using WOTR_BOAT_BOAT_BOAT.MechanicsChanges;
using Kingmaker.Dungeon.Blueprints;
using Kingmaker.Blueprints.Root;
using System.Linq;

namespace WOTR_BOAT_BOAT_BOAT
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
        public static Settings settings; 
        private static bool enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            AssetLoader.ModEntry = modEntry;
            modInfo = modEntry;
            settings = Settings.Load<Settings>(modEntry);
            var settingsFile = Path.Combine(modEntry.Path, "Settings.bak");
            var copyFile = Path.Combine(modEntry.Path, "Settings.xml");
            if (File.Exists(settingsFile) && !File.Exists(copyFile))
            {
                File.Copy(settingsFile, copyFile, false);
            }
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            harmony.PatchAll();
            EventBus.Subscribe(new InjectStuffOnLoad());
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            UI.AutoWidth(); UI.Div(0, 15);
            using (UI.VerticalScope())
            {
                UI.Label("SETTINGS WILL NOT BE UPDATED UNTIL YOU RESTART YOUR GAME.".grey().bold().size(20));
                /*   UI.Toggle("Gold Dragon Spell Damage Fix".bold(), ref settings.PatchGoldDragonSpellDamage);
                   if(settings.PatchGoldDragonSpellDamage)
                   {
                       UI.Label("Spell Damage Dice Progression is changed to work as written. In addition, if an enemy has any energy vulnerability they'll be vulnerable to the attack at mythic rank 10.".green().size(10));
                   }
                   else
                   {
                       UI.Label("Spell Damage Dice Progression is unchanged.".red().size(10));
                   }*/

            }
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
        public static void AddBoonOnAreaLoad(BlueprintDungeonBoon dungeonBoon, bool inject)
        {
#if DEBUG
            if (inject)
            {
                InjectStuffOnLoad.Injections.Add(() => { SetBoon(dungeonBoon); });
                //DungeonRoot
            }
#endif
        }
        public static void SetBoon(BlueprintDungeonBoon dungeonBoon)
        {
            Log(dungeonBoon.Name);
            if (dungeonBoon == null)
            {
                Log(dungeonBoon.Name + " is null");
            }
            Game.Instance.Player.DungeonState.m_IsBoonApplied = false;
            Game.Instance.Player.DungeonState.StageIndex = 999;
            Game.Instance.Player.DungeonState.Statistic.StageIndexBest = 998;
            Game.Instance.Player.DungeonState.SelectBoon(dungeonBoon);
            if (Game.Instance.Player.DungeonState.Boon != dungeonBoon)
            {
                Log("Failed to set boon to " + dungeonBoon.Name);
            }
            Log("Boon set to " + dungeonBoon.Name + ".");
            Game.Instance.Player.DungeonState.ApplyBoon(Game.Instance.Player.MainCharacter);

        }

        public bool GetSettingValue(string b)
        {
            return true;
        }
        public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

    }
}
