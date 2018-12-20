using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;
using Verse.AI;

namespace Religion
{
    public static class ReligionUtility
    {
        public static void GiveAttendJob(Building_Lectern lectern, Pawn attendee)
        {
            //Log.Message("1");
            if (lectern.owners[0] == attendee) return;
            if (attendee.Drafted) return;
            if (attendee.IsPrisoner) return;
            if (attendee.jobs.curJob.def == ReligionDefOf.AttendLecture) return;

            IntVec3 result;
            Building chair;
            if (!WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, true, out result, out chair))           
            {
                    return;
            }
            Job J = new Job(ReligionDefOf.AttendLecture, (LocalTargetInfo)lectern, (LocalTargetInfo)result, (LocalTargetInfo)((Thing)chair));
            J.playerForced = true;
            J.ignoreJoyTimeAssignment = true;
            J.expiryInterval = 9999;
            J.ignoreDesignations = true;
            J.ignoreForbidden = true;
            attendee.jobs.EndCurrentJob(JobCondition.Incompletable);
            attendee.jobs.TryTakeOrderedJob(J);
            //Log.Message("2");
            //else
            //{
            //    //Log.Message("3b");

            //    IntVec3 newPos = result + GenAdj.CardinalDirections[dir];

            //    Job J = new Job(ReligionDefOf.AttendLecture, lectern, newPos, result);
            //    J.playerForced = true;
            //    J.ignoreJoyTimeAssignment = true;
            //    J.expiryInterval = 9999;
            //    J.ignoreDesignations = true;
            //    J.ignoreForbidden = true;
            //    attendee.jobs.EndCurrentJob(JobCondition.Incompletable);
            //    attendee.jobs.TryTakeOrderedJob(J);
            //}
        }
    }
}
