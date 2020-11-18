using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Religion
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        private static readonly Harmony harmony = new Harmony("ReligionMod");

        static HarmonyPatches()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(TraitDef), "ConflictsWith", new Type[] { typeof(Trait) })]
        private static class Patch_ConflictsWith
        {
            private static bool Prefix(TraitDef __instance, Trait other, ref bool __result)
            {
                if (__instance is TraitDef_ReligionTrait && (other.def is TraitDef_ReligionTrait || other.def is TraitDef_NonReligion))
                {
                    __result = true;
                    return false;
                }
                if(__instance is TraitDef_NonReligion && (other.def is TraitDef_ReligionTrait || other.def is TraitDef_NonReligion))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
