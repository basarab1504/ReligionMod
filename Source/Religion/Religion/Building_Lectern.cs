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

        public void Listeners()
        {
            if(religion[0] != null)
            foreach (Pawn p in Map.mapPawns.FreeColonists)
                if (p.story.traits.HasTrait(religion[0]) && p != owners[0])
                    listeners.Add(p);
        }

        public void TryLecture()
        {
            if (owners.Count == 0)
                return;
            Job job = new Job(ReligionDefOf.HoldLecture, this);
            Job attend = new Job(ReligionDefOf.AttendLecture, this);
            job.playerForced = true;
            owners[0].jobs.TryTakeOrderedJob(job);
            foreach (Pawn p in listeners)
                ReligionUtility.GiveAttendJob(this, p);
        }

        #region IBuilding
        public IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!this.Spawned)
                    return Enumerable.Empty<Pawn>();
                return this.Map.mapPawns.FreeColonists;
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
            if (!owners.Contains(owner))
            {
                owners.Add(owner);
                //religion = owner.story.traits.GetTrait(ReligionDefOf.)
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
                return DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait); //наверное тут
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

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (base.Faction == Faction.OfPlayer)
            {
                var command_Action = new Command_Action
                {
                    action = delegate
                    {
                        Messages.Message("Ok, we're trying".Translate(), MessageTypeDefOf.PositiveEvent);
                        Listeners();
                        TryLecture();
                    },
                    defaultLabel = "Worship".Translate(),
                    defaultDesc = "WorshipDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc2,
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                };
                yield return command_Action;

                yield return new Command_Action
                {
                    defaultLabel = "CommandBedSetOwnerLabel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                    defaultDesc = "CommandBedSetOwnerDesc".Translate(),
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                    },
                    hotKey = KeyBindingDefOf.Misc3
                };

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
            }
        }
    }
}