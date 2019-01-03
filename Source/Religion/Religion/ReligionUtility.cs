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
        public static readonly Texture2D faith = ContentFinder<Texture2D>.Get("Things/Symbols/Religion", true);
        public static bool IsMorning(Map map) => GenLocalDate.HourInteger(map) > 6 && GenLocalDate.HourInteger(map) < 10;
        public static bool IsEvening(Map map) => GenLocalDate.HourInteger(map) > 18 && GenLocalDate.HourInteger(map) < 22;
        public static bool TimeToLecture(Map map, int time) => GenLocalDate.HourInteger(map) > time-1 && GenLocalDate.HourInteger(map) < time+1;
        public static bool IsNight(Map map) => GenLocalDate.HourInteger(map) > 22;

        #region Thoughts
        public static void TryGainTempleRoomThought(Pawn pawn)
        {
            Room room = pawn.GetRoom();
            ThoughtDef def = ReligionDefOf.PrayedInImpressiveTemple;
            if (pawn == null) return;
            if (room == null) return;
            if (room.Role == null) return;
            if (def == null) return;
            if (room.Role == ReligionDefOf.Church)
            {
                int scoreStageIndex =
                    RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
                Messages.Message(scoreStageIndex.ToString(), MessageTypeDefOf.NegativeEvent);
                if (def.stages[scoreStageIndex] == null) return;
                pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, scoreStageIndex), null);
            }
        }

        public static void HeldWorshipThought(Pawn preacher)
        {
            if (preacher == null) return;
            TryGainTempleRoomThought(preacher);
            ThoughtDef newThought = ReligionDefOf.HeldLecture; // DefDatabase<ThoughtDef>.GetNamed("HeldSermon");
            if (newThought != null)
            {
                preacher.needs.mood.thoughts.memories.TryGainMemory(newThought);
            }
        }
        #endregion

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

        public static Building_Lectern FindLecternToAltar(Building_Altar altar, Map map)
        {
            Room room = altar.GetRoom(RegionType.Set_Passable);
            List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
            for (int index = 0; index < andAdjacentThings.Count; ++index)
            {
                if (andAdjacentThings[index] is Building_Lectern)
                {
                    return andAdjacentThings[index] as Building_Lectern;

                }
            }
            return null;
        }

        #region LecternManagement
        public static void GiveLectureJob(Building_Lectern lectern, Pawn preacher)
        {
            if (preacher != lectern.owners[0] || preacher == null || preacher.Drafted || preacher.IsPrisoner || preacher.jobs.curJob.def == ReligionDefOf.HoldLecture)
            {
                Messages.Message("CantGiveLectureJobToPreacher".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            Thing book = null;

            if (AppropriateBookInInventory(preacher, lectern.religion[0]) != null)
                book = AppropriateBookInInventory(preacher, lectern.religion[0]);
            else if (AppropriateBook(preacher, lectern.religion[0]) != null)
                book = AppropriateBook(preacher, lectern.religion[0]);

            if(book != null)
            {
                Job J = new Job(ReligionDefOf.HoldLecture, (LocalTargetInfo)lectern, (LocalTargetInfo)book);
                J.playerForced = true;
                preacher.jobs.EndCurrentJob(JobCondition.Incompletable);
                preacher.jobs.TryTakeOrderedJob(J);
            }
            else
            Messages.Message("NoBook".Translate(), MessageTypeDefOf.NegativeEvent);
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

        public static Thing AppropriateBook(Pawn p, TraitDef religionOfPawn)
        {
            foreach (Thing t in p.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ReligionDefOf.ReligionBook)))
                if ((t.def as ReligionBook_ThingDef).religion == religionOfPawn)
                    if (p.CanReach((LocalTargetInfo)t, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                        return t;
            return null;
        }

        public static Thing AppropriateBookInInventory(Pawn p, TraitDef religionOfPawn)
        {
            foreach (Thing t in p.inventory.innerContainer)
                if (t.def is ReligionBook_ThingDef && (t.def as ReligionBook_ThingDef).religion == religionOfPawn)
                    return t;
            return null;
        }

        public static void Wipe(Building_Lectern lectern)
        {
            lectern.owners.Clear();
            lectern.religion.Clear();
            lectern.listeners.Clear();
            for (int i = 0; i < lectern.daysOfLectures.Count; ++i)
                lectern.daysOfLectures[i] = false;
            lectern.timeOfLecture = 9;
            lectern.timeOfbuffer = string.Empty;
            lectern.didLecture = false;
        }

        public static void Listeners(Building_Lectern lectern, List<Pawn>listenersOfLectern)
        {
            if (listenersOfLectern.Count != 0)
                listenersOfLectern.Clear();
            if (listenersOfLectern.Count == 0)
                foreach (Pawn x in lectern.Map.mapPawns.FreeColonists)
                    if (x.story.traits.HasTrait(lectern.religion[0]) && x != lectern.owners[0] &&
                               x.RaceProps.Humanlike &&
                               !x.IsPrisoner &&
                               x.Faction == Faction.OfPlayer &&
                               x.RaceProps.intelligence == Intelligence.Humanlike &&
                               !x.Downed && !x.Dead &&
                               !x.InMentalState && !x.InAggroMentalState &&
                               x.CurJob.def != ReligionDefOf.HoldLecture &&
                               x.CurJob.def != ReligionDefOf.AttendLecture &&
                               x.CurJob.def != JobDefOf.Capture &&
                               x.CurJob.def != JobDefOf.ExtinguishSelf && //Oh god help
                               x.CurJob.def != JobDefOf.Rescue && //Saving lives is more important
                               x.CurJob.def != JobDefOf.TendPatient && //Saving lives is more important
                               x.CurJob.def != JobDefOf.BeatFire && //Fire?! This is more important
                               x.CurJob.def != JobDefOf.Lovin && //Not ready~~
                               x.CurJob.def != JobDefOf.LayDown && //They're resting
                               x.CurJob.def != JobDefOf.FleeAndCower //They're not cowering
                            )
                        listenersOfLectern.Add(x);
        }

        public static void TryLecture(Building_Lectern lectern, bool forced)
        {
            if (!lectern.owners.NullOrEmpty())
            {
                lectern.didLecture = true;
                ;
                ReligionUtility.GiveLectureJob(lectern, lectern.owners[0]);
            }
            else
            {
                Messages.Message("No preacher assigned".Translate(), MessageTypeDefOf.NegativeEvent);
            }
            if (!forced)
                lectern.didLecture = true;
        }
        #endregion

        #region Misc
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
        #endregion
    };
}
