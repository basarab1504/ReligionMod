using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Religion
{
    public class Building_Lectern : Building, IAssignableTrait, IAssignableBuilding
    {
        public List<Pawn> owners = new List<Pawn>();
        public List<Pawn> listeners = new List<Pawn>();
        public List<TraitDef> religion = new List<TraitDef>();

        public int duration;
        public List<bool> daysOfLectures = Enumerable.Repeat(false, 15).ToList();

        public int timeOfLecture = 9;
        public bool didLecture;

        public void Listeners()
        {
            if (listeners.Count != 0)
                listeners.Clear();
            if (listeners.Count == 0)
                foreach (Pawn x in Map.mapPawns.FreeColonists)
                    if (x.story.traits.HasTrait(religion[0]) && x != owners[0]
                            &&
                               x.RaceProps.Humanlike &&
                               !x.IsPrisoner &&
                               x.Faction == Faction &&
                               x.RaceProps.intelligence == Intelligence.Humanlike &&
                               !x.Downed && !x.Dead &&
                               !x.InMentalState && !x.InAggroMentalState &&
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
                        listeners.Add(x);
        }

        public void TryLecture()
        {
            Listeners();
            ReligionUtility.GiveLectureJob(this, owners[0]);
            foreach (Pawn p in listeners)
                ReligionUtility.GiveAttendJob(this, p);
        }

        public void TryForcedLecture()
        {
            Listeners();
            ReligionUtility.GiveLectureJob(this, owners[0]);
            foreach (Pawn p in listeners)
                ReligionUtility.GiveAttendJob(this, p);
        }

        #region IBuilding
        public IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!this.Spawned || this.religion.Count == 0)
                    return Enumerable.Empty<Pawn>();
                return Map.mapPawns.FreeColonists.Where(x => x.story.traits.HasTrait(religion[0]));
            }
        }

        public IEnumerable<Pawn> AssignedPawns
        {
            get
            {
                return this.owners;
            }
        }

        public int MaxAssignedPawnsCount
        {
            get
            {
                return 1;
            }
        }

        public bool AssignedAnything(Pawn pawn)
        {
            return false;
        }

        public void TryAssignPawn(Pawn owner)
        {
            owners.Clear();
            if (!owners.Contains(owner))
            {
                owners.Add(owner);
            }
            else return;
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            if (owners.Contains(pawn))
            {
                owners.Remove(pawn);
            }
            else return;
        }
        #endregion

        #region ITrait
        public IEnumerable<TraitDef> AssigningTrait
        {
            get
            {
                if (!this.Spawned)
                    return Enumerable.Empty<TraitDef>();
                return DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            }
        }

        public IEnumerable<TraitDef> AssignedTraits
        {
            get
            {
                return this.religion;
            }
        }

        public int MaxAssignedTraitsCount
        {
            get
            {
                return 1;
            }
        }

        public bool AssignedAnything(TraitDef trait)
        {
            return false;
        }

        public void TryAssignTrait(TraitDef trait)
        {
            religion.Clear();
            if (!religion.Contains(trait))
            {
                religion.Add(trait);
            }
            else return;
        }

        public void TryUnassignTrait(TraitDef trait)
        {
            if (religion.Contains(trait))
            {
                religion.Remove(trait);
            }
            else return;
        }
        #endregion

        #region BillGiver

        #endregion

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (base.Faction == Faction.OfPlayer)
            {

                yield return new Command_Action
                {
                    defaultLabel = "AssignReligion".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                    defaultDesc = "AssignReligionDesc".Translate(),
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_AssignTrait(this));
                    },
                    hotKey = KeyBindingDefOf.Misc4
                };

                yield return new Command_Action
                {
                    defaultLabel = "CommandBedSetOwnerLabel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                    defaultDesc = "CommandBedSetOwnerDesc".Translate(),
                    action = delegate
                    {
                        if (religion.Count == 0)
                            Messages.Message("Select a religion first".Translate(), MessageTypeDefOf.NegativeEvent);
                        else
                            Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                    },
                    hotKey = KeyBindingDefOf.Misc3
                };

                var command_Action = new Command_Action
                {
                    action = delegate
                    {
                        if (religion.Count == 0 || owners.Count == 0)
                            Messages.Message("Select a religion and preacher first".Translate(), MessageTypeDefOf.NegativeEvent);
                        else
                        {
                            TryForcedLecture();
                        }
                    },
                    defaultLabel = "Worship".Translate(),
                    defaultDesc = "WorshipDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc2,
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                };
                yield return command_Action;
            }
        }

        public override void TickRare()
        {
            if (!Spawned) return;
            // Don't forget the base work
            base.TickRare();
            if (ReligionUtility.TimeToLecture(Map, timeOfLecture) && daysOfLectures[(GenLocalDate.DayOfQuadrum(Map))] && didLecture == false)
            {
                Messages.Message("is true", MessageTypeDefOf.PositiveEvent);
                didLecture = true;
                TryLecture();
            }
            if (ReligionUtility.IsEvening(Map) && didLecture == true)
            {
                didLecture = false;
                Messages.Message("is false", MessageTypeDefOf.PositiveEvent);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timeOfLecture, "timeoflecture");
            Scribe_Values.Look<bool>(ref this.didLecture, "morning", false, false);
            Scribe_Collections.Look<TraitDef>(ref this.religion, "religions", LookMode.Def);
            Scribe_Collections.Look<Pawn>(ref this.owners, "owners", LookMode.Reference, new object[0]);
        }
    }
}