using Harmony;
using RimWorld;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Religion
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        private static HarmonyInstance harmony = HarmonyInstance.Create("ReligionMod");

        static HarmonyPatches()
        {
            HarmonyPatches.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(TraitDef), "ConflictsWith")]
        private static class Patch_ConflictsWith
        {
            private static bool Prefix(TraitDef __instance, Trait other, ref bool __result)
            {
                if (__instance is TraitDef_ReligionTrait && other.def is TraitDef_ReligionTrait)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
