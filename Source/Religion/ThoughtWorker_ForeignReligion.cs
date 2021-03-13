using RimWorld;
using Verse;

namespace Religion
{
    internal class ThoughtWorker_ForeignReligion : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }

            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }

            if (other.def != p.def)
            {
                return false;
            }

            if (!p.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return false;
            }

            if (!other.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return false;
            }

            var pawnRel =
                p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def as TraitDef_ReligionTrait;
            if (pawnRel != null && pawnRel.isAgressive)
            {
                return true;
            }

            var otherRel = other.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
            if (pawnRel != null && pawnRel.foreignReligions.Contains(otherRel))
            {
                return true;
            }

            return false;
        }
    }
}