using RimWorld;
using Verse;

namespace Religion
{
    internal class ThoughtWorker_AntitheistThought : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (other.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait))
            {
                return true;
            }

            return false;
        }
    }
}