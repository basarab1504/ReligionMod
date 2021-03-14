using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class JoyGiver_Praying : JoyGiver_InteractBuilding
    {
        protected override bool CanInteractWith(Pawn pawn, Thing t, bool inBed)
        {
            var altar = t as Building_Altar;
            //if (!(t is Building_Altar))
            //    return false;
            if (!base.CanInteractWith(pawn, t, inBed))
            {
                return false;
            }

            if (altar != null && (altar.religion == null || altar.relic == null || altar.religion.Count == 0))
            {
                return false;
            }

            if (altar != null && pawn.story.traits.HasTrait(altar.religion[0]))
            {
                return true;
            }

            //if (inBed == false)
            //    return false;
            return false;
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            if (!WatchBuildingUtility.TryFindBestWatchCell(t, pawn, def.desireSit, out var result, out var chair))
            {
                WatchBuildingUtility.TryFindBestWatchCell(t, pawn, false, out result, out chair);
            }

            return new Job(def.jobDef, t, result, chair);
        }
    }
}