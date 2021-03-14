using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Religion
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        private static readonly Harmony harmony = new("ReligionMod");

        static HarmonyPatches()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(TraitDef), "ConflictsWith", typeof(Trait))]
        private static class Patch_ConflictsWith
        {
            private static bool Prefix(TraitDef __instance, Trait other, ref bool __result)
            {
                if (__instance is TraitDef_ReligionTrait &&
                    (other.def is TraitDef_ReligionTrait || other.def is TraitDef_NonReligion))
                {
                    __result = true;
                    return false;
                }

                if (__instance is not TraitDef_NonReligion ||
                    other.def is not TraitDef_ReligionTrait && other.def is not TraitDef_NonReligion)
                {
                    return true;
                }

                __result = true;
                return false;
            }
        }
    }
}