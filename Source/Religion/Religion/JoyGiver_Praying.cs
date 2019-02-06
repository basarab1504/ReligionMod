using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    class JoyGiver_Praying : JoyGiver_InteractBuilding
    {

        protected override bool CanInteractWith(Pawn pawn, Thing t, bool inBed)
        {
            Building_Altar altar = t as Building_Altar;
            //if (!(t is Building_Altar))
            //    return false;
            if (!base.CanInteractWith(pawn, t, inBed))
                return false;
            if (altar.religion == null || altar.relic == null || altar.religion.Count == 0)
                return false;
            if (pawn.story.traits.HasTrait(altar.religion[0]))
                return true;
            //if (inBed == false)
            //    return false;
            return false;
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            IntVec3 result;
            Building chair;
            if (!WatchBuildingUtility.TryFindBestWatchCell(t, pawn, this.def.desireSit, out result, out chair))
            {
                WatchBuildingUtility.TryFindBestWatchCell(t, pawn, false, out result, out chair);
            }
            return new Job(this.def.jobDef, (LocalTargetInfo)t, (LocalTargetInfo)result, (LocalTargetInfo)((Thing)chair));
        }
    }
}
