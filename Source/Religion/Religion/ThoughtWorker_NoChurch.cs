using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace Religion
{
    public class ThoughtWorker_NoChurch : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait)!=null && p.IsColonist)
            {
                TraitDef pawnReligion = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
                List<Thing> altars = p.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ReligionDefOf.Altar));
                if(!altars.NullOrEmpty())
                {
                    foreach (Building_Altar a in altars)
                    {
                        if (a.religion.Contains(pawnReligion) && a.relic != null /* && a.lectern != null*/)
                            return ThoughtState.Inactive;
                    }
                }
                return true;
            }
            return ThoughtState.Inactive;
        }
    }
}
