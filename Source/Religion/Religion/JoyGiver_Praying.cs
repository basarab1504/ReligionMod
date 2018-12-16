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
            JoyGiverDef_ReligionJoyGiverDef relDef = this.def as JoyGiverDef_ReligionJoyGiverDef;
            if (relDef.traitDefs == null)
                return false;
            if (pawn.story.traits.HasTrait(relDef.traitDefs[0]))
                return true;
            if (!base.CanInteractWith(pawn, t, inBed))
                return false;
            if (!inBed)
                return true;
            Building_Bed bed = pawn.CurrentBed();
            return WatchBuildingUtility.CanWatchFromBed(pawn, bed, t);
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            IntVec3 result;
            Building chair;
            if (!WatchBuildingUtility.TryFindBestWatchCell(t, pawn, this.def.desireSit, out result, out chair))
                return (Job)null;
            return new Job(this.def.jobDef, (LocalTargetInfo)t, (LocalTargetInfo)result, (LocalTargetInfo)((Thing)chair));
        }
    }
}
