using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTR_PATH_OF_HELL.BlueprintPatches
{
    class DLC3_ArmorPenaltyReduceBuff
    {
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            private static BlueprintProgression devilProgression = BlueprintTool.Get<BlueprintProgression>("87bc9abf00b240a44bb344fea50ec9bc");
            private static BlueprintUnitFactReference aeonGazeFeature = BlueprintTool.Get<BlueprintFeature>("2689d57a889951241912f4bc7d6bb77e").ToReference<BlueprintUnitFactReference>();
            private static BlueprintFeatureBaseReference hellsPowersFeature = BlueprintTool.Get<BlueprintFeature>("77759cac4da9f2144b831cb8ad8a32de").ToReference<BlueprintFeatureBaseReference>();
            private static BlueprintFeatureBaseReference aeonGazeFeatureEndless = BlueprintTool.Get<BlueprintFeature>("a2f5852d76a165f4d8d6fe670e8013fb").ToReference<BlueprintFeatureBaseReference>();
            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                DevilProgression_Patch();
                Main.Log("DevilProgression_Patch");

            }

            private static void DevilProgression_Patch()
            {
                if (Settings.Settings.GetSetting<bool>("aeongazepatch"))
                {
                    AeonGaze_Patch();
                    Main.Log("aeongazepatch");
                }
                if (Settings.Settings.GetSetting<bool>("hellsauthoritypatch"))
                {
                    HellsAuthority_Patch();
                    Main.Log("hellsauthoritypatch");
                }

            }

            private static void AeonGaze_Patch()
            {

                foreach (RemoveFeatureOnApply component in devilProgression.GetComponents<RemoveFeatureOnApply>())
                {
                    if (component.m_Feature == aeonGazeFeature)
                    {
                        devilProgression.SetComponents(devilProgression.ComponentsArray.RemoveFromArray(component));
                    }
                }

                devilProgression.LevelEntries[0].m_Features.Add(aeonGazeFeatureEndless);

            }

            private static void HellsAuthority_Patch()
            {

                foreach (var levelEntry in devilProgression.LevelEntries)
                {
                    foreach (var feature in levelEntry.m_Features)
                    {
                        if (feature == hellsPowersFeature)
                        {
                            levelEntry.m_Features.Remove(feature);
                        }
                    }
                }

                devilProgression.LevelEntries[0].m_Features.Add(hellsPowersFeature);

            }

        }
    }
}
