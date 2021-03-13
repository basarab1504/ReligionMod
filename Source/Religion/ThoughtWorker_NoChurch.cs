using RimWorld;
using Verse;

namespace Religion
{
    public class ThoughtWorker_NoChurch : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait) == null || !p.IsColonist)
            {
                return ThoughtState.Inactive;
            }

            var pawnReligion = p.story.traits.allTraits.Find(x => x.def is TraitDef_ReligionTrait).def;
            var altars = p.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ReligionDefOf.Altar));
            if (altars.NullOrEmpty())
            {
                return true;
            }

            foreach (var thing in altars)
            {
                var a = (Building_Altar) thing;
                if (a.religion.Contains(pawnReligion) && a.relic != null /* && a.lectern != null*/)
                {
                    return ThoughtState.Inactive;
                }
            }

            return true;
        }
    }
}