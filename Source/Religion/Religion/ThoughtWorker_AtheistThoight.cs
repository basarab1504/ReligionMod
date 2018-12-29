using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Religion
{
    class ThoughtWorker_AtheistThoight : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if(other.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return (ThoughtState)true;
            return (ThoughtState)false;
        }
    }
}
