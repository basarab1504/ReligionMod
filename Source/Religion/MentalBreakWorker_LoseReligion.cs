using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class MentalBreakWorker_LoseReligion : MentalBreakWorker
    {
        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            var baseCommonality = def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && def.commonalityFactorPerPopulationCurve != null)
            {
                baseCommonality *=
                    def.commonalityFactorPerPopulationCurve.Evaluate(PawnsFinder.AllMaps_FreeColonists.Count);
            }

            if (!pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return 0;
            }

            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            var religion = pawn.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            pawn.story.traits.allTraits.Remove(religion);
            var h = pawn.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionAddiction);
            if (h != null)
            {
                pawn.health.hediffSet.hediffs.Remove(h);
            }

            if (Rand.Value < 0.10)
            {
                pawn.story.traits.GainTrait(new Trait(ReligionDefOf.Atheist));
            }

            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}