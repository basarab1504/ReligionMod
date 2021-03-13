using RimWorld;
using Verse;

namespace Religion
{
    internal class ThoughtWorker_NoPreacher : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.needs.AllNeeds.Any(x => x.def == ReligionDefOf.Religion_Need))
            {
                return ThoughtState.Inactive;
            }

            var pawnReligion = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
            var altars = p.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ReligionDefOf.Altar));
            if (altars.NullOrEmpty())
            {
                return ThoughtState.Inactive;
            }

            foreach (var thing in altars)
            {
                var a = (Building_Altar) thing;
                if (a.religion.Contains(pawnReligion) && a.relic != null && a.lectern != null &&
                    !a.lectern.CompAssignableToPawn.AssignedPawnsForReading.NullOrEmpty())
                {
                    return ThoughtState.Inactive;
                }
            }

            return true;
            return ThoughtState.Inactive;
        }
    }
}