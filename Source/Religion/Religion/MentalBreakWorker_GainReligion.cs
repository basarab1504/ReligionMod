using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace Religion
{
    class MentalBreakWorker_GainReligion : MentalBreakWorker
    {
        readonly Trait Religion = new Trait(DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait).RandomElement());

        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            float baseCommonality = def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && def.commonalityFactorPerPopulationCurve != null)
                baseCommonality *= def.commonalityFactorPerPopulationCurve.Evaluate(PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
            if (pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait || x.def is TraitDef_NonReligion))
                return 0;
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
