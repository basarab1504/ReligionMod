using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace Religion
{
    class MentalBreakWorker_BecomeAtheist : MentalBreakWorker
    {
        public override float CommonalityFor(Pawn pawn)
        {
            float baseCommonality = this.def.baseCommonality;
            if (pawn.Faction == Faction.OfPlayer && this.def.commonalityFactorPerPopulationCurve != null)
                baseCommonality *= this.def.commonalityFactorPerPopulationCurve.Evaluate((float)PawnsFinder.AllMaps_FreeColonists.Count<Pawn>());
            if (pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait || x.def is TraitDef_NonReligion))
                return 0;
            return baseCommonality;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            if(Rand.Value < 0.5f)
            pawn.story.traits.GainTrait(new Trait(ReligionDefOf.Atheist));
            else
                pawn.story.traits.GainTrait(new Trait(ReligionDefOf.Antitheist));
            return base.TryStart(pawn, reason, causedByMood);
        }
    }
}
