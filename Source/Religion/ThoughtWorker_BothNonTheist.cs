using RimWorld;
using Verse;

namespace Religion
{
    internal class ThoughtWorker_BothNonTheist : ThoughtWorker
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

            foreach (var traitDef in def.requiredTraits)
            {
                if (p.story.traits.HasTrait(traitDef) && other.story.traits.HasTrait(traitDef))
                {
                    return true;
                }
            }

            return false;
        }
    }
}