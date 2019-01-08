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

        [HarmonyPatch(typeof(Pawn_Ownership), "UnclaimAll")]
        private static class Patch_UnclaimAll
        {
            private static void Postfix(Pawn_Ownership __instance)
            {
                Pawn pawn = Traverse.Create((object)__instance).Field("pawn").GetValue<Pawn>();
                if (pawn == null)
                    return;
                if(pawn.story.traits.allTraits.Any(x=>x.def is TraitDef_ReligionTrait))
                ReligionUtility.Unclaim(pawn);
            }
        }
    }
}
