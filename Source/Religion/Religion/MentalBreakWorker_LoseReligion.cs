using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;
namespace Religion
{
    class MentalBreakWorker_LoseReligion : MentalBreakWorker
    {
        public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
        {
            float baseCommonality = def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && def.commonalityFactorPerPopulationCurve != null)
                baseCommonality *= def.commonalityFactorPerPopulationCurve.Evaluate(PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
            if (!pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return 0;
            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            Trait religion = pawn.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            pawn.story.traits.allTraits.Remove(religion);
            Hediff h = pawn.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionAddiction);
            if (h != null)
            {               
                pawn.health.hediffSet.hediffs.Remove(h);
            }
            if (Rand.Value < 0.10)
                pawn.story.traits.GainTrait(new Trait(ReligionDefOf.Atheist));
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}
