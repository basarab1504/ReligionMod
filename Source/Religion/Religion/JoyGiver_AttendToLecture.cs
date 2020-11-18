using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    class JoyGiver_AttendToWorship : JoyGiver_InteractBuilding
    {
        protected override bool CanInteractWith(Pawn pawn, Thing t, bool inBed)
        {
            Building_Lectern lectern = t as Building_Lectern;
            if (!(t is Building_Lectern))
                return false;
            if (lectern.CompAssignableToPawn.AssignedPawnsForReading.NullOrEmpty())
                return false;
            if (lectern.religion.NullOrEmpty())
                return false;
            Pawn preacher = lectern.CompAssignableToPawn.AssignedPawnsForReading[0];
            if (pawn.story.traits.HasTrait(lectern.religion[0]) && preacher.CurJobDef == ReligionDefOf.HoldWorship)
                return true;
            return false;
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            if (!WatchBuildingUtility.TryFindBestWatchCell(t, pawn, def.desireSit, out IntVec3 result, out Building chair))
                return null;
            return new Job(def.jobDef, t, result, chair);
        }
    }
}
