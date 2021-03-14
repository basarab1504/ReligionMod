using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class MentalBreakWorker_GainReligion : MentalBreakWorker
    {
        private readonly Trait Religion = new(DefDatabase<TraitDef>.AllDefsListForReading
            .FindAll(x => x is TraitDef_ReligionTrait).RandomElement());

        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            var baseCommonality = def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && def.commonalityFactorPerPopulationCurve != null)
            {
                baseCommonality *=
                    def.commonalityFactorPerPopulationCurve.Evaluate(PawnsFinder.AllMaps_FreeColonists.Count);
            }

            if (pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait || x.def is TraitDef_NonReligion))
            {
                return 0;
            }

            if (!ReligionUtility.CanBeReligious(pawn, Religion))
            {
                return 0;
            }

            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            pawn.story.traits.GainTrait(Religion);
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}