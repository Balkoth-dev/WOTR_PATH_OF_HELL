﻿using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Dungeon.Blueprints;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOTR_BOAT_BOAT_BOAT.MechanicsChanges;
using WOTR_BOAT_BOAT_BOAT.Utilities;

namespace WOTR_BOAT_BOAT_BOAT.BlueprintPatches
{
    class DLC3_ElementalDamageElectricityBuff
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                DLC3_ElementalDamageElectricityBuff_Patch();

            }

            private static void DLC3_ElementalDamageElectricityBuff_Patch()
            {
                var dLC3_ElementalDamageElectricityBuff = BlueprintTool.Get<BlueprintBuff>("84420fb8d0034378b69ba7e912d1ff15");
                var dungeonBoon_Electric = BlueprintTool.Get<BlueprintDungeonBoon>("c955ab1f11c646fc8bbb00248040024f");
                var callLightningAbility = BlueprintTool.Get<BlueprintAbility>("0bd54216d38852947930320f6269a9d7");

                var newDescription = "All electricity damage dealt by your party members is increased by 25%.\nIn addition you can cast Call Lighting as a swift action that scales with your character level.";

                var callLightningSwift = Helpers.CreateCopy(callLightningAbility);
                callLightningSwift.AssetGuid = new BlueprintGuid(new Guid("4a59f8ec-fa5a-4e60-b125-dd2efc6dfa4c"));
                callLightningSwift.RemoveComponents<AbilityExecuteActionOnCast>();
                callLightningSwift.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Swift;

                callLightningSwift.AddComponent<ContextRankConfig>(c =>
                {
                    c.m_Type = AbilityRankType.DamageDice;
                    c.m_BaseValueType = ContextRankBaseValueType.CharacterLevel;
                    c.m_StepLevel = 3;
                });

                Helpers.AddBlueprint(callLightningSwift, callLightningSwift.AssetGuid);

                dLC3_ElementalDamageElectricityBuff.AddComponent<AddFacts>(c =>
                {
                    c.m_Facts = new BlueprintUnitFactReference[]{
                        callLightningSwift.ToReference<BlueprintUnitFactReference>()
                    };
                });

                dLC3_ElementalDamageElectricityBuff.m_Description = Helpers.CreateString(dLC3_ElementalDamageElectricityBuff + ".Description", newDescription);
                dungeonBoon_Electric.m_Description = Helpers.CreateString(dungeonBoon_Electric + ".Description", newDescription);

                Main.AddBoonOnAreaLoad(dungeonBoon_Electric, false);

                var p = dungeonBoon_Electric;
                Main.Log(p.Name + " - " + p.Description);
            }
        }
    }
}
