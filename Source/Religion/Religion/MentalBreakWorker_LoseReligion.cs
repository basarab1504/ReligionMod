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
        public override float CommonalityFor(Pawn pawn)
        {
            float baseCommonality = this.def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && this.def.commonalityFactorPerPopulationCurve != null)
                baseCommonality *= this.def.commonalityFactorPerPopulationCurve.Evaluate((float)PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
            if (!pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait || x.def == ReligionDefOf.Atheist))
                return 0;
            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            Trait religion = pawn.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait);
            pawn.story.traits.allTraits.Remove(religion);
            if (Rand.Value < 0.25)
                pawn.story.traits.GainTrait(new Trait(ReligionDefOf.Atheist));
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}
