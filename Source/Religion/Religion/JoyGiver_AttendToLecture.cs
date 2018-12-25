using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    class JoyGiver_AttendToLecture : JoyGiver_InteractBuilding
    {
        protected override bool CanInteractWith(Pawn pawn, Thing t, bool inBed)
        {
            Building_Lectern lectern = t as Building_Lectern;
            if (!(t is Building_Lectern))
                return false;
            if (lectern.owners.NullOrEmpty())
                return false;
            if (lectern.religion.NullOrEmpty())
                return false;
            Pawn preacher = lectern.owners[0];
            if (pawn.story.traits.HasTrait(lectern.religion[0]) && preacher.CurJobDef == ReligionDefOf.HoldLecture)
                return true;
            return false;
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
