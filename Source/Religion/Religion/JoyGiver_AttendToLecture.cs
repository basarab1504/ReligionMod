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
            Pawn preacher = FindPreacher(pawn);
            JoyGiverDef_ReligionJoyGiverDef relDef = this.def as JoyGiverDef_ReligionJoyGiverDef;
            if (preacher == null)
                return false;
            if (relDef.traitDefs == null)
                return false;
            if (pawn.story.traits.HasTrait(relDef.traitDefs[0]))
                return true;
            if (!base.CanInteractWith(pawn, t, inBed))
                return false;
            if (inBed == false)
                return false;
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

        Pawn FindPreacher(Pawn p)
        {
            foreach (Pawn pawn in p.Map.mapPawns.FreeColonistsSpawned)
            {
                if (pawn.CurJob.def == ReligionDefOf.HoldLecture)
                    return pawn;
            }
            return null;
        }
    }
}
