using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Religion
{
    [StaticConstructorOnStartup]
    public static class ReligionUtility
    {
        public static readonly Texture2D faith = ContentFinder<Texture2D>.Get("Things/Symbols/Religion");

        public static bool IsMorning(Map map)
        {
            return GenLocalDate.HourInteger(map) > 6 && GenLocalDate.HourInteger(map) < 10;
        }

        public static bool IsEvening(Map map)
        {
            return GenLocalDate.HourInteger(map) > 18 && GenLocalDate.HourInteger(map) < 22;
        }

        public static bool TimeToWorship(Map map, int time)
        {
            return GenLocalDate.HourInteger(map) > time - 1 && GenLocalDate.HourInteger(map) < time + 1;
        }

        public static bool IsNight(Map map)
        {
            return GenLocalDate.HourInteger(map) > 22;
        }

        public static Building_Altar FindAtlarToLectern(Building_Lectern lectern, Map map)
        {
            var room = lectern.GetRoom();
            var andAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var thing in andAdjacentThings)
            {
                if (thing is not Building_Altar)
                {
                    continue;
                }

                var adjacentThing = thing as Building_Altar;
                if (adjacentThing.lectern == null)
                {
                    return adjacentThing;
                }
            }

            return null;
        }

        public static Building_Lectern FindLecternToAltar(Building_Altar altar, Map map)
        {
            var room = altar.GetRoom();
            var andAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var thing in andAdjacentThings)
            {
                if (thing is not Building_Lectern)
                {
                    continue;
                }

                var buildingLectern = thing as Building_Lectern;
                if (buildingLectern.altar == null)
                {
                    return buildingLectern;
                }
            }

            return null;
        }

        public static void TryWorshipInteraction(Pawn preacher, Pawn recipient, InteractionDef intDef)
        {
            if (preacher == recipient)
            {
                Log.Warning(preacher + " tried to interact with self, interaction=" + intDef.defName);
                return;
            }

            //if (!tracker.CanInteractNowWith(recipient))
            //    return false;
            var extraSentencePacks = new List<RulePackDef>();
            //if (intDef.initiatorThought != null)
            //    Pawn_InteractionsTracker.AddInteractionThought(preacher, recipient, intDef.initiatorThought);
            //if (intDef.recipientThought != null && recipient.needs.mood != null)
            //    Pawn_InteractionsTracker.AddInteractionThought(recipient, preacher, intDef.recipientThought);
            //if (intDef.initiatorXpGainSkill != null)
            //    preacher.skills.Learn(intDef.initiatorXpGainSkill, (float)intDef.initiatorXpGainAmount, false);
            //if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike)
            //    recipient.skills.Learn(intDef.recipientXpGainSkill, (float)intDef.recipientXpGainAmount, false);
            var flag = false;
            if (recipient.RaceProps.Humanlike)
            {
                flag = recipient.interactions.CheckSocialFightStart(intDef, preacher);
            }

            if (!flag)
            {
                intDef.Worker.Interacted(preacher, recipient, extraSentencePacks, out _, out _, out _, out _);
            }

            MoteMaker.MakeInteractionBubble(preacher, recipient, intDef.interactionMote, intDef.Symbol);
            if (flag)
            {
                extraSentencePacks.Add(RulePackDefOf.Sentence_SocialFightStarted);
            }

            var entryInteraction = new PlayLogEntry_Interaction(intDef, preacher, recipient, extraSentencePacks);
            Find.PlayLog.Add(entryInteraction);
        }

        #region Addiction

        public static void TryAddAddiction(Pawn p, Pawn preacher)
        {
            var n = p.needs.AllNeeds.Find(x => x.def == ReligionDefOf.Religion_Need);
            if (n == null)
            {
                if (preacher.skills.GetSkill(SkillDefOf.Social).levelInt >= 8)
                {
                    p.health.AddHediff(ReligionDefOf.ReligionAddiction);
                }
            }
            else
            {
                n.CurLevel = 1f;
            }

            //Hediff h = p.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionTolerance);
            //if (h == null)
            //    p.health.AddHediff(ReligionDefOf.ReligionTolerance);
            //else
            //    h.Severity += 0.1f;
        }

        public static void TryAddAddictionForPreacher(Pawn p)
        {
            var n = p.needs.AllNeeds.Find(x => x.def == ReligionDefOf.Religion_Need);
            if (n == null)
            {
                p.health.AddHediff(ReligionDefOf.ReligionAddiction);
            }
            else
            {
                n.CurLevel = 1f;
            }

            //Hediff h = p.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionTolerance);
            //if (h == null)
            //    p.health.AddHediff(ReligionDefOf.ReligionTolerance);
            //else
            //    h.Severity += 0.1f;
        }

        #endregion

        #region Thoughts

        public static void TryGainTempleRoomThought(Pawn pawn)
        {
            var room = pawn.GetRoom();
            var def = ReligionDefOf.PrayedInImpressiveTemple;
            if (pawn == null)
            {
                return;
            }

            if (room?.Role == null)
            {
                return;
            }

            if (def == null)
            {
                return;
            }

            if (room.Role != ReligionDefOf.Church)
            {
                return;
            }

            var scoreStageIndex =
                RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
            if (def.stages[scoreStageIndex] == null)
            {
                return;
            }

            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, scoreStageIndex));
        }

        public static void AttendedWorshipThought(Pawn pawn, Pawn preacher)
        {
            var def = ReligionDefOf.AttendedWorship;
            if (pawn == null)
            {
                return;
            }

            if (preacher == null)
            {
                return;
            }

            if (def == null)
            {
                return;
            }

            var skill = preacher.skills.GetSkill(SkillDefOf.Social).levelInt;
            switch (skill)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 0));
                    break;
                }
                case 6:
                case 7:
                {
                    break;
                }
                case 8:
                case 9:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 1));
                    break;
                }
                case 10:
                case 11:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 2));
                    break;
                }
                case 12:
                case 13:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 3));
                    break;
                }
                case 14:
                case 15:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 4));
                    break;
                }
                case 16:
                case 17:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 5));
                    break;
                }
                case 18:
                case 19:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 6));
                    break;
                }
                case 20:
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(def, 7));
                    break;
                }
            }
        }

        public static void HeldWorshipThought(Pawn preacher)
        {
            if (preacher == null)
            {
                return;
            }

            TryGainTempleRoomThought(preacher);
            var newThought = ReligionDefOf.HeldWorship; // DefDatabase<ThoughtDef>.GetNamed("HeldSermon");
            if (newThought != null)
            {
                preacher.needs.mood.thoughts.memories.TryGainMemory(newThought);
            }
        }

        //public static void AttendedWorshipThought(Pawn prayer)
        //{
        //    if (prayer == null) return;
        //    TryGainTempleRoomThought(prayer);
        //    ThoughtDef newThought = ReligionDefOf.AttendedWorship; // DefDatabase<ThoughtDef>.GetNamed("HeldSermon");
        //    if (newThought != null)
        //    {
        //        prayer.needs.mood.thoughts.memories.TryGainMemory(newThought);
        //    }
        //}

        #endregion

        #region LecternManagement

        public static void GiveAttendJob(Building_Lectern lectern, Pawn attendee)
        {
            if (attendee.Drafted)
            {
                return;
            }

            if (attendee.IsPrisoner)
            {
                return;
            }

            if (attendee.jobs.curJob.def == ReligionDefOf.AttendWorship)
            {
                return;
            }

            if (!WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, true, out var result, out var chair))
            {
                WatchBuildingUtility.TryFindBestWatchCell(lectern, attendee, false, out result, out chair);
            }

            var J = new Job(ReligionDefOf.AttendWorship, lectern, result, chair)
            {
                playerForced = true,
                ignoreJoyTimeAssignment = true,
                expiryInterval = 9999,
                ignoreDesignations = true,
                ignoreForbidden = true
            };
            //attendee.jobs.EndCurrentJob(JobCondition.Incompletable);
            attendee.jobs.TryTakeOrderedJob(J);
        }

        private static Thing AppropriateBook(Pawn p, TraitDef religionOfPawn)
        {
            foreach (var t in p.Map.listerThings.AllThings.FindAll(x => x is ThingWithComps_Book))
            {
                if (t.TryGetComp<CompReligionBook>().Religion == religionOfPawn)
                {
                    return t;
                }
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
            lectern.CompAssignableToPawn.UnassignAllPawns();
            lectern.religion.Clear();
            lectern.listeners.Clear();
            for (var i = 0; i < lectern.daysOfWorships.Count; ++i)
            {
                lectern.daysOfWorships[i] = false;
            }

            lectern.timeOfWorship = 9;
            lectern.timeOfbuffer = string.Empty;
            lectern.didWorship = false;
            lectern.altar = null;
        }

        public static void Listeners(Building_Lectern lectern, List<Pawn> listenersOfLectern)
        {
            if (listenersOfLectern.Count != 0)
            {
                listenersOfLectern.Clear();
            }

            if (listenersOfLectern.Count != 0)
            {
                return;
            }

            foreach (var x in lectern.Map.mapPawns.FreeColonists)
            {
                if (x.story.traits.HasTrait(lectern.religion[0]) &&
                    x != lectern.CompAssignableToPawn.AssignedPawnsForReading[0] &&
                    x.RaceProps.Humanlike &&
                    !x.IsPrisoner &&
                    x.Faction == Faction.OfPlayer &&
                    x.RaceProps.intelligence == Intelligence.Humanlike &&
                    !x.Downed && !x.Dead &&
                    !x.InMentalState && !x.InAggroMentalState &&
                    x.CurJob.def != ReligionDefOf.HoldWorship &&
                    x.CurJob.def != ReligionDefOf.AttendWorship /* &&*/
                    //x.CurJob.def != JobDefOf.Capture &&
                    //x.CurJob.def != JobDefOf.ExtinguishSelf && //Oh god help
                    //x.CurJob.def != JobDefOf.Rescue && //Saving lives is more important
                    //x.CurJob.def != JobDefOf.TendPatient && //Saving lives is more important
                    //x.CurJob.def != JobDefOf.BeatFire && //Fire?! This is more important
                    //x.CurJob.def != JobDefOf.Lovin && //Not ready~~
                    //x.CurJob.def != JobDefOf.LayDown && //They're resting
                    //x.CurJob.def != JobDefOf.FleeAndCower //They're not cowering
                )
                {
                    listenersOfLectern.Add(x);
                }
            }
        }

        public static void TryWorship(Building_Lectern lectern, bool forced)
        {
            if (lectern == null)
            {
                Messages.Message("ErrorLectern".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            var lectern_ = lectern;

            if (lectern_.religion.NullOrEmpty())
            {
                Messages.Message("CantGiveWorshipJobToPreacher".Translate() + " : " + "SelectAReligion".Translate(),
                    MessageTypeDefOf.NegativeEvent);
                lectern_.didWorship = true;
                return;
            }

            if (lectern_.CompAssignableToPawn.AssignedPawnsForReading.NullOrEmpty())
            {
                Messages.Message("CantGiveWorshipJobToPreacher".Translate() + " : " + "SelectAPreacher".Translate(),
                    MessageTypeDefOf.NegativeEvent);
                lectern_.didWorship = true;
                return;
            }

            var preacher = lectern_.CompAssignableToPawn.AssignedPawnsForReading[0];

            if (lectern_.altar.relic == null)
            {
                Messages.Message("CantGiveWorshipJobToPreacher".Translate() + " : " + "NoRelicOnAltar".Translate(),
                    MessageTypeDefOf.NegativeEvent);
                lectern_.didWorship = true;
                return;
            }

            if (preacher.Dead || preacher.Drafted || preacher.IsPrisoner
                || preacher.jobs.curJob.def == ReligionDefOf.HoldWorship
                || preacher.InMentalState || preacher.InAggroMentalState)
            {
                Messages.Message("CantGiveWorshipJobToPreacher".Translate(), MessageTypeDefOf.NegativeEvent);
                if (preacher.Dead)
                {
                    Messages.Message("PreacherIsDead".Translate(), MessageTypeDefOf.NegativeEvent);
                    lectern_.CompAssignableToPawn.UnassignAllPawns();
                }

                lectern_.didWorship = true;
                return;
            }

            if (AppropriateBook(preacher, lectern_.religion[0]) == null)
            {
                Messages.Message("CantGiveWorshipJobToPreacher".Translate() + " : " + "NoBookAvaliable".Translate(),
                    MessageTypeDefOf.NegativeEvent);
                lectern_.didWorship = true;
                return;
            }

            var book = AppropriateBook(preacher, lectern_.religion[0]);
            //Log.Message(book.Position.ToString());
            //if (book == null)
            //{
            //    Messages.Message("NoBookOfReligion".Translate(), MessageTypeDefOf.NegativeEvent);
            //    return;
            //}

            ////if (AppropriateBookInInventory(preacher, lectern.religion[0]) != null)
            ////    book = AppropriateBookInInventory(preacher, lectern.religion[0]);

            if (!forced)
            {
                lectern_.didWorship = true;
            }

            var J = new Job(ReligionDefOf.HoldWorship, lectern_, book) {count = 1, playerForced = true};
            //preacher.jobs.EndCurrentJob(JobCondition.Incompletable);
            preacher.jobs.TryTakeOrderedJob(J);
        }

        #endregion

        #region BookReading

        private static readonly List<CompGatherSpot> workingSpots = new();
        private static readonly int NumRadiusCells = GenRadial.NumCellsInRadius(3.9f);

        private static readonly List<IntVec3> RadialPatternMiddleOutward = GenRadial.RadialPattern.Take(NumRadiusCells)
            .OrderBy(c => Mathf.Abs((c - IntVec3.Zero).LengthHorizontal - 1.95f)).ToList();

        private static readonly List<ThingDef> nurseableDrugs = new();
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
            for (var index = 0; index < 30; ++index)
            {
                var edifice = table.RandomAdjacentCellCardinal().GetEdifice(table.Map);
                if (edifice == null || !edifice.def.building.isSittable || !sitter.CanReserve(edifice))
                {
                    continue;
                }

                chair = edifice;
                return true;
            }

            chair = null;
            return false;
        }

        private static bool TryFindSitSpotOnGroundNear(IntVec3 center, Pawn sitter, out IntVec3 result)
        {
            for (var index = 0; index < 30; ++index)
            {
                var intVec3 = center + GenRadial.RadialPattern[Rand.Range(1, NumRadiusCells)];
                if (!sitter.CanReserveAndReach(intVec3, PathEndMode.OnCell, Danger.None) ||
                    intVec3.GetEdifice(sitter.Map) != null || !GenSight.LineOfSight(center, intVec3, sitter.Map, true))
                {
                    continue;
                }

                result = intVec3;
                return true;
            }

            result = IntVec3.Invalid;
            return false;
        }

        private static bool TryFindChairNear(IntVec3 center, Pawn sitter, out Thing chair)
        {
            foreach (var intVec3 in RadialPatternMiddleOutward)
            {
                var edifice = (center + intVec3).GetEdifice(sitter.Map);
                if (edifice == null || !edifice.def.building.isSittable || !sitter.CanReserve(edifice) ||
                    edifice.IsForbidden(sitter) || !GenSight.LineOfSight(center, edifice.Position, sitter.Map, true))
                {
                    continue;
                }

                chair = edifice;
                return true;
            }

            chair = null;
            return false;
        }

        #endregion

        #region Misc

        public static bool CanBeReligious(Pawn pawn, Trait religion)
        {
            var traitsOfPawn = pawn.story.traits.allTraits;
            var conflicts = religion.def.conflictingTraits;
            if (traitsOfPawn.Any(x => conflicts.Any(y => x.def == y)))
            {
                return false;
            }

            //foreach (Trait t in traitsOfPawn)
            //    foreach (TraitDef td in conflicts)
            //        if (t.def == td)
            //            return false;
            return true;
        }

        private static readonly Color InactiveColor = new(0.37f, 0.37f, 0.37f, 0.8f);

        private static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f,
            Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            var color = GUI.color;
            if (disabled)
            {
                GUI.color = InactiveColor;
            }

            var texture2D = !active ? !(texUnchecked != null) ? Widgets.CheckboxOffTex :
                texUnchecked :
                !(texChecked != null) ? Widgets.CheckboxOnTex : texChecked;
            GUI.DrawTexture(new Rect(x, y, size, size), texture2D);
            if (!disabled)
            {
                return;
            }

            GUI.color = color;
        }

        public static void CheckboxLabeled(Rect rect, Building_Lectern lectern, int i, string label, bool checkOn,
            bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null,
            bool placeCheckboxNearText = false)
        {
            var anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            if (placeCheckboxNearText)
            {
                rect.width = Mathf.Min(rect.width, (float) (Text.CalcSize(label).x + 24.0 + 10.0));
            }

            Widgets.Label(rect, label);
            if (!disabled && Widgets.ButtonInvisible(rect, false))
            {
                lectern.daysOfWorships[i] = !checkOn;
                if (checkOn)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                }
            }

            CheckboxDraw((float) (rect.x + (double) rect.width - 24.0), rect.y, checkOn, disabled);
            Text.Anchor = anchor;
        }

        #endregion
    }
}