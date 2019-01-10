using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI;
using Verse.Sound;
using System;

namespace Religion
{
    [StaticConstructorOnStartup]
    public static class ReligionUtility
    {
        public static readonly Texture2D faith = ContentFinder<Texture2D>.Get("Things/Symbols/Religion", true);
        public static bool IsMorning(Map map) => GenLocalDate.HourInteger(map) > 6 && GenLocalDate.HourInteger(map) < 10;
        public static bool IsEvening(Map map) => GenLocalDate.HourInteger(map) > 18 && GenLocalDate.HourInteger(map) < 22;
        public static bool TimeToLecture(Map map, int time) => GenLocalDate.HourInteger(map) > time - 1 && GenLocalDate.HourInteger(map) < time + 1;
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
            Building_Altar l;
            for (int index = 0; index < andAdjacentThings.Count; ++index)
            {
                if (andAdjacentThings[index] is Building_Altar)
                {
                    l = andAdjacentThings[index] as Building_Altar;
                    if (l.lectern == null)
                        return l;
                }
            }
            return null;
        }

        public static Building_Lectern FindLecternToAltar(Building_Altar altar, Map map)
        {
            Room room = altar.GetRoom(RegionType.Set_Passable);
            List<Thing> andAdjacentThings = room.ContainedAndAdjacentThings;
            Building_Lectern l;
            for (int index = 0; index < andAdjacentThings.Count; ++index)
            {
                if (andAdjacentThings[index] is Building_Lectern)
                {
                    l = andAdjacentThings[index] as Building_Lectern;
                    if (l.altar == null)
                        return l;
                }                  
            }
            return null;
        }

        #region LecternManagement

        public static void Unclaim(Pawn p)
        {
            Building_Lectern b = p.Map.listerBuildings.allBuildingsColonist.Find(x => x is Building_Lectern && (x as Building_Lectern).owners.Contains(p)) as Building_Lectern;
            if (b == null)
                return;
            b.TryUnassignPawn(p);
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
            foreach(Thing t in p.Map.listerThings.AllThings.FindAll(x => x is ThingWithComps_Book))
            {
                if (t.TryGetComp<CompReligionBook>().religion == religionOfPawn)
                    return t;
            }
            return null;
        }

        //public static Thing AppropriateRelic(Pawn p, TraitDef religionOfPawn)
        //{
        //    TraitDef rel;
        //    foreach (Thing t in p.Map.listerThings.AllThings.FindAll(x => x.TryGetComp<CompRelic>() != null))
        //    {
        //        rel = t.TryGetComp<CompRelic>().religion;
        //        if (rel == religionOfPawn)
        //            if (p.CanReach((LocalTargetInfo)t, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
        //                return t;
        //    }
        //    return null;
        //}

        //public static Thing AppropriateBookInInventory(Pawn p, TraitDef religionOfPawn)
        //{
        //    foreach (Thing t in p.inventory.innerContainer)
        //        if (t.def is ReligionBook_ThingDef && (t.def as ReligionBook_ThingDef).religion == religionOfPawn)
        //            return t;
        //    return null;
        //}

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
            lectern.altar = null;
        }

        public static void Listeners(Building_Lectern lectern, List<Pawn> listenersOfLectern)
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
            if(lectern == null)
            {
                Messages.Message("ErrorLectern".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            Building_Lectern lectern_ = lectern;

            if (lectern_.religion.NullOrEmpty())
            {
                Messages.Message("SelectAReligion".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            if (lectern_.owners.NullOrEmpty())
            {
                Messages.Message("SelectAPreacher".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            Pawn preacher = lectern_.owners[0];

            if (lectern_.altar.relic == null)
            {
                Messages.Message("NoRelicOnAltar".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }
            if (preacher.Dead || preacher.Drafted || preacher.IsPrisoner
                || preacher.jobs.curJob.def == ReligionDefOf.HoldLecture 
                || preacher.InMentalState || preacher.InAggroMentalState)
            {
                Messages.Message("CantGiveLectureJobToPreacher".Translate(), MessageTypeDefOf.NegativeEvent);
                if (preacher.Dead)
                {
                    Messages.Message("PreacherIsDead".Translate(), MessageTypeDefOf.NegativeEvent);
                    lectern_.owners.Clear();
                }
                return;
            }

            if (AppropriateBook(preacher, lectern_.religion[0]) == null)
            {
                Messages.Message("NoBookAvaliable".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            Thing book = AppropriateBook(preacher, lectern_.religion[0]);
            //Log.Message(book.Position.ToString());
                //if (book == null)
                //{
                //    Messages.Message("NoBookOfReligion".Translate(), MessageTypeDefOf.NegativeEvent);
                //    return;
                //}

            ////if (AppropriateBookInInventory(preacher, lectern.religion[0]) != null)
            ////    book = AppropriateBookInInventory(preacher, lectern.religion[0]);

            if (!forced)
                lectern_.didLecture = true;

            Job J = new Job(ReligionDefOf.HoldLecture, (LocalTargetInfo)lectern_, (LocalTargetInfo)book);
            J.playerForced = true;
            preacher.jobs.EndCurrentJob(JobCondition.Incompletable);
            preacher.jobs.TryTakeOrderedJob(J);
        }
        #endregion

        #region BookReading
        private static List<CompGatherSpot> workingSpots = new List<CompGatherSpot>();
        private static readonly int NumRadiusCells = GenRadial.NumCellsInRadius(3.9f);
        private static readonly List<IntVec3> RadialPatternMiddleOutward = ((IEnumerable<IntVec3>)GenRadial.RadialPattern).Take<IntVec3>(NumRadiusCells).OrderBy<IntVec3, float>((Func<IntVec3, float>)(c => Mathf.Abs((c - IntVec3.Zero).LengthHorizontal - 1.95f))).ToList<IntVec3>();
        private static List<ThingDef> nurseableDrugs = new List<ThingDef>();
        private const float GatherRadius = 3.9f;

        //public static Job PlaceToRead(Pawn pawn, Thing t)
        //{

        //    if (pawn.Map.gatherSpotLister.activeSpots.Count == 0)
        //        return (Job)null;
        //    workingSpots.Clear();
        //    for (int index = 0; index < pawn.Map.gatherSpotLister.activeSpots.Count; ++index)
        //        workingSpots.Add(pawn.Map.gatherSpotLister.activeSpots[index]);
        //    CompGatherSpot result1;
        //    while (workingSpots.TryRandomElement<CompGatherSpot>(out result1))
        //    {
        //        workingSpots.Remove(result1);
        //        if (!result1.parent.IsForbidden(pawn) && pawn.CanReach((LocalTargetInfo)((Thing)result1.parent), PathEndMode.Touch, Danger.None, false, TraverseMode.ByPawn) && (result1.parent.IsSociallyProper(pawn) && result1.parent.IsPoliticallyProper(pawn)))
        //        {
        //            if (result1.parent.def.surfaceType == SurfaceType.Eat)
        //            {
        //                Messages.Message("1", MessageTypeDefOf.NegativeEvent);
        //                Thing chair;
        //                if (!TryFindChairBesideTable((Thing)result1.parent, pawn, out chair))
        //                    return (Job)null;
        //                return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)chair, (LocalTargetInfo)t);
        //            }
        //            else
        //            {
        //                Thing chair;
        //                if (TryFindChairNear(result1.parent.Position, pawn, out chair))
        //                {
        //                    Messages.Message("2", MessageTypeDefOf.NegativeEvent);
        //                    return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)chair, (LocalTargetInfo)t);
        //                }
        //                else
        //                {
        //                    IntVec3 result2;
        //                    if (!TryFindSitSpotOnGroundNear(result1.parent.Position, pawn, out result2))
        //                        return (Job)null;
        //                    Messages.Message("3", MessageTypeDefOf.NegativeEvent);
        //                    return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)result2, (LocalTargetInfo)t);
        //                }
        //            }
        //        }
        //    }
        //    return (Job)null;
        //}

        private static bool TryFindChairBesideTable(Thing table, Pawn sitter, out Thing chair)
        {
            for (int index = 0; index < 30; ++index)
            {
                Building edifice = table.RandomAdjacentCellCardinal().GetEdifice(table.Map);
                if (edifice != null && edifice.def.building.isSittable && sitter.CanReserve((LocalTargetInfo)((Thing)edifice), 1, -1, (ReservationLayerDef)null, false))
                {
                    chair = (Thing)edifice;
                    return true;
                }
            }
            chair = (Thing)null;
            return false;
        }

        private static bool TryFindSitSpotOnGroundNear(IntVec3 center, Pawn sitter, out IntVec3 result)
        {
            for (int index = 0; index < 30; ++index)
            {
                IntVec3 intVec3 = center + GenRadial.RadialPattern[Rand.Range(1, NumRadiusCells)];
                if (sitter.CanReserveAndReach((LocalTargetInfo)intVec3, PathEndMode.OnCell, Danger.None, 1, -1, (ReservationLayerDef)null, false) && intVec3.GetEdifice(sitter.Map) == null && GenSight.LineOfSight(center, intVec3, sitter.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    result = intVec3;
                    return true;
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        private static bool TryFindChairNear(IntVec3 center, Pawn sitter, out Thing chair)
        {
            for (int index = 0; index < RadialPatternMiddleOutward.Count; ++index)
            {
                Building edifice = (center + RadialPatternMiddleOutward[index]).GetEdifice(sitter.Map);
                if (edifice != null && edifice.def.building.isSittable && (sitter.CanReserve((LocalTargetInfo)((Thing)edifice), 1, -1, (ReservationLayerDef)null, false) && !edifice.IsForbidden(sitter)) && GenSight.LineOfSight(center, edifice.Position, sitter.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    chair = (Thing)edifice;
                    return true;
                }
            }
            chair = (Thing)null;
            return false;
        }
        #endregion

        #region Misc
        private static readonly Color InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);

        static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            Color color = GUI.color;
            if (disabled)
                GUI.color = InactiveColor;
            Texture2D texture2D = !active ? (!((UnityEngine.Object)texUnchecked != (UnityEngine.Object)null) ? Widgets.CheckboxOffTex : texUnchecked) : (!((UnityEngine.Object)texChecked != (UnityEngine.Object)null) ? Widgets.CheckboxOnTex : texChecked);
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
    }
}