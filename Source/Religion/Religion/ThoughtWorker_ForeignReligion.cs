using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Religion
{
    class ThoughtWorker_ForeignReligion : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike)
                return (ThoughtState)false;
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
                return (ThoughtState)false;
            if (other.def != p.def)
                return (ThoughtState)false;
            if (!p.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return (ThoughtState)false;
            if (!other.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
                return (ThoughtState)false;
            TraitDef_ReligionTrait pawnRel = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def as TraitDef_ReligionTrait;
            TraitDef otherRel = other.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
            if (pawnRel.foreignReligions.Contains(otherRel))
                return (ThoughtState)true;
            return (ThoughtState)false;
        }
    }
}
