using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace InteractionSpotOnChunk
{

    [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.InteractionCellStandable))]
    public static class Patch_InteractionCellStandable
    {
        private static ISOCModSettings Settings => LoadedModManager.GetMod<InteractionSpotOnChunk>()?.GetSettings<ISOCModSettings>() ?? InteractionSpotOnChunk.settings;

        static void Postfix(ThingDef thingDef, IntVec3 center, Rot4 rot, Map map, ref AcceptanceReport __result)
        {
            if (__result.Accepted)
                return;

            foreach (IntVec3 cell in ThingUtility.InteractionCellsWhenAt(thingDef, center, rot, map, false))
            {
                bool onlyChunks = true;

                foreach (Thing thing in map.thingGrid.ThingsListAtFast(cell))
                {
                    if (IsChunk(thing))
                        continue;

                    if (thing.def.passability != Traversability.Standable || thing.def == thingDef)
                    {
                        onlyChunks = false;
                        break;
                    }

                    BuildableDef entityDefToBuild = thing.def.entityDefToBuild;
                    if (entityDefToBuild != null && (entityDefToBuild.passability != Traversability.Standable || entityDefToBuild == thingDef))
                    {
                        onlyChunks = false;
                        break;
                    }
                }

                if (!onlyChunks)
                    return;
            }

            if (Settings.debugMode)
                Log.Message("[InteractionSpotOnChunk] Allowing placement - Interaction spot contains chunk");
            __result = AcceptanceReport.WasAccepted;
        }

        private static bool IsChunk(Thing thing)
        {
            return ThingRequestGroup.Chunk.Includes(thing.def);
        }
    }


}
