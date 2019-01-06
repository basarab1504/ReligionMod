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
            if (p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait) != null)
            {
                TraitDef pawnReligion = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
                List<Thing> altars = p.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ReligionDefOf.Altar));
                Building_Altar better = null;
                if(!altars.NullOrEmpty())
                {
                    foreach (Building_Altar a in altars)
                    {
                        if (a.religion.Contains(pawnReligion) && a.lectern != null && !a.lectern.owners.NullOrEmpty())
                            return ThoughtState.Inactive;
                        if (a.religion.Contains(pawnReligion) && (a.lectern == null || a.lectern.owners.NullOrEmpty()))
                            better = a;
                    }
                    if(better != null)
                    return ThoughtState.ActiveAtStage(1);
                    else
                        return ThoughtState.ActiveAtStage(0);
                }
                else
                    return ThoughtState.ActiveAtStage(0);
            }
            return ThoughtState.Inactive;
        }
    }
}
