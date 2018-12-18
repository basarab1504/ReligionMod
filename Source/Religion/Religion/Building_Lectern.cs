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
using RimWorld;

namespace Religion
{
    class Building_Lectern : Building, IAssignableBuilding
    {
        public List<Pawn> owners = new List<Pawn>();

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
                owners.Add(owner);
            else return;
        }

        public void TryUnassignPawn(Pawn pawn)
        {
            if (owners.Contains(pawn))
                owners.Remove(pawn);
            else return;
        }

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
                    defaultLabel = "CommandBedSetOwnerLabel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                    defaultDesc = "CommandBedSetOwnerDesc".Translate(),
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                    },
                    hotKey = KeyBindingDefOf.Misc3
                };
            }
        }
    }
}