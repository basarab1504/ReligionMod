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
        //public static void GiveAttendWorshipJob(Building_Lectern lectern, Pawn attendee)
        //{
        //    //Log.Message("1");
        //    if (IsPreacher(attendee)) return;
        //    if (attendee.Drafted) return;
        //    if (attendee.IsPrisoner) return;
        //    if (attendee.jobs.curJob.def.defName == "ReflectOnWorship") return;
        //    if (attendee.jobs.curJob.def.defName == "AttendWorship") return;

        //    IntVec3 result;
        //    Building chair;
        //    if (!WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, true, out result, out chair))
        //    {
        //        if (!WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, false, out result, out chair))
        //        {
        //            return;
        //        }
        //    }
        //    //Log.Message("2");

        //    int dir = lectern.Rotation.Opposite.AsInt;

        //    if (chair != null)
        //    {
        //        IntVec3 newPos = chair.Position + GenAdj.CardinalDirections[dir];

        //        //Log.Message("3a");

        //        Job J = new Job(CultsDefOf.Cults_AttendWorship, lectern, newPos, chair);
        //        J.playerForced = true;
        //        J.ignoreJoyTimeAssignment = true;
        //        J.expiryInterval = 9999;
        //        J.ignoreDesignations = true;
        //        J.ignoreForbidden = true;
        //        attendee.jobs.EndCurrentJob(JobCondition.Incompletable);
        //        attendee.jobs.TryTakeOrderedJob(J);
        //    }
        //    else
        //    {
        //        //Log.Message("3b");

        //        IntVec3 newPos = result + GenAdj.CardinalDirections[dir];

        //        Job J = new Job(CultsDefOf.Cults_AttendWorship, lectern, newPos, result);
        //        J.playerForced = true;
        //        J.ignoreJoyTimeAssignment = true;
        //        J.expiryInterval = 9999;
        //        J.ignoreDesignations = true;
        //        J.ignoreForbidden = true;
        //        attendee.jobs.EndCurrentJob(JobCondition.Incompletable);
        //        attendee.jobs.TryTakeOrderedJob(J);
        //    }
        //}
    }
}
