using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Religion
{
    class ThoughtWorker_BothNonTheist : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike)
                return false;
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
                return false;
            if (other.def != p.def)
                return false;
            for(int i = 0; i < def.requiredTraits.Count; ++i)
            {
                TraitDef t = def.requiredTraits[i];
                if (p.story.traits.HasTrait(t) && other.story.traits.HasTrait(t))
                    return true;
            }
            return false;
        }
    }
}
