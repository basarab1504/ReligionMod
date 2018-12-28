using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI;
using Verse.Sound;

namespace Religion
{
    [StaticConstructorOnStartup]
    public static class ReligionUtility
    {
        public static readonly Texture2D Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true);
        public static bool IsMorning(Map map) => GenLocalDate.HourInteger(map) > 6 && GenLocalDate.HourInteger(map) < 10;
        public static bool IsEvening(Map map) => GenLocalDate.HourInteger(map) > 18 && GenLocalDate.HourInteger(map) < 22;
        public static bool TimeToLecture(Map map, int time) => GenLocalDate.HourInteger(map) > time-1 && GenLocalDate.HourInteger(map) < time+1;
        public static bool IsNight(Map map) => GenLocalDate.HourInteger(map) > 22;

        //public static Building_Lectern FindLecternToAltar(Building_Altar lectern, Map map)
        //{
        //    Room room = lectern.GetRoom(RegionType.Set_Passable);
        //    List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
        //    for (int index = 0; index < andAdjacentThings.Count; ++index)
        //    {
        //        if (andAdjacentThings[index] is Building_Lectern)
        //        {
        //            return andAdjacentThings[index] as Building_Lectern;
        //        }
        //    }
        //    return null;
        //}

            public static Building_Altar FindAtlarToLectern(Building_Lectern lectern, Map map)
        {
            Room room = lectern.GetRoom(RegionType.Set_Passable);
            List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
            for (int index = 0; index < andAdjacentThings.Count; ++index)
            {
                if (andAdjacentThings[index] is Building_Altar)
                {
                    return andAdjacentThings[index] as Building_Altar;
                }
            }
            return null;
        }

        public static void GiveLectureJob(Building_Lectern lectern, Pawn preacher)
        {
            if (preacher != lectern.owners[0] || preacher == null || preacher.Drafted || preacher.IsPrisoner || preacher.jobs.curJob.def == ReligionDefOf.HoldLecture)
            {
                Messages.Message("CantGiveLectureJobToPreacher".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            Job J = new Job(ReligionDefOf.HoldLecture, (LocalTargetInfo)lectern);
            J.playerForced = true;
            preacher.jobs.EndCurrentJob(JobCondition.Incompletable);
            preacher.jobs.TryTakeOrderedJob(J);
        }

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
                WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, false, out result, out chair);
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

        private static readonly Color InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);

        static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            Color color = GUI.color;
            if (disabled)
                GUI.color = InactiveColor;
            Texture2D texture2D = !active ? (!((Object)texUnchecked != (Object)null) ? Widgets.CheckboxOffTex : texUnchecked) : (!((Object)texChecked != (Object)null) ? Widgets.CheckboxOnTex : texChecked);
            GUI.DrawTexture(new Rect(x, y, size, size), (Texture)texture2D);
            if (!disabled)
                return;
            GUI.color = color;
        }

        public static void CheckboxLabeled(Rect rect, Building_Lectern lectern, int i, string label, bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false)
        {
            TextAnchor anchor = Verse.Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            if (placeCheckboxNearText)
                rect.width = Mathf.Min(rect.width, (float)((double)Verse.Text.CalcSize(label).x + 24.0 + 10.0));
            Widgets.Label(rect, label);
            if (!disabled && Widgets.ButtonInvisible(rect, false))
            {
                lectern.daysOfLectures[i] = !checkOn;
                if (checkOn)
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera((Map)null);
                else
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera((Map)null);
            }
            CheckboxDraw((float)((double)rect.x + (double)rect.width - 24.0), rect.y, checkOn, disabled, 24f, (Texture2D)null, (Texture2D)null);
            Verse.Text.Anchor = anchor;
        }
    }
}
