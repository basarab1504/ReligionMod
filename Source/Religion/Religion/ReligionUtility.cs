using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI;

namespace Religion
{
    public static class ReligionUtility
    {
        public static readonly Texture2D Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true);

        public static bool IsMorning(Map map) => GenLocalDate.HourInteger(map) > 6 && GenLocalDate.HourInteger(map) < 10;

        public static bool IsEvening(Map map) => GenLocalDate.HourInteger(map) > 18 && GenLocalDate.HourInteger(map) < 22;

        public static bool IsNight(Map map) => GenLocalDate.HourInteger(map) > 22;

        public static void GiveAttendJob(Building_Lectern lectern, Pawn attendee)
        {
            //Log.Message("1");
            //if (lectern.owners[0] == attendee) return;
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
        }

        public static void GiveLectureJob(Building_Lectern lectern, Pawn preacher)
        {
            if (preacher != lectern.owners[0])
                return;
            if (preacher.Drafted) return;
            if (preacher.IsPrisoner) return;
            if (preacher.jobs.curJob.def == ReligionDefOf.HoldLecture) return;

            Job J = new Job(ReligionDefOf.HoldLecture, (LocalTargetInfo)lectern);
            J.playerForced = true;
            preacher.jobs.EndCurrentJob(JobCondition.Incompletable);
            preacher.jobs.TryTakeOrderedJob(J);
        }
    }
}
