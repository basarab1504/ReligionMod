using RimWorld;
using Verse;

namespace Religion
{
    internal class ThoughtWorker_ReligiousHateAnti : ThoughtWorker
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

            if (other.story.traits.HasTrait(ReligionDefOf.Antitheist))
            {
                return true;
            }

            return false;
        }
    }
}