using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace Religion
{
    class MentalBreakWorker_LoseAtheism : MentalBreakWorker
    {
        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            float baseCommonality = def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && def.commonalityFactorPerPopulationCurve != null)
                baseCommonality *= def.commonalityFactorPerPopulationCurve.Evaluate(PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
            if (!pawn.story.traits.allTraits.Any(x => x.def is TraitDef_NonReligion))
                return 0;
            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            Trait religion = pawn.story.traits.allTraits.Find(x => x.def is TraitDef_NonReligion);
            pawn.story.traits.allTraits.Remove(religion);
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}
