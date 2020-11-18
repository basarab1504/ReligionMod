using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace Religion
{
    public class Building_Lectern : Building, IAssignableTrait
    {
        public List<Pawn> listeners = new List<Pawn>();
        public List<TraitDef> religion = new List<TraitDef>();
        public List<bool> daysOfWorships = Enumerable.Repeat(false, 15).ToList();
        public int timeOfWorship = 9;
        public string timeOfbuffer;
        public bool didWorship;
        public Building_Altar altar;

        public Pawn AssignedPawn
        {
            get
            {
                if (this.CompAssignableToPawn == null || !this.CompAssignableToPawn.AssignedPawnsForReading.Any<Pawn>())
                {
                    return null;
                }
                return this.CompAssignableToPawn.AssignedPawnsForReading[0];
            }
        }

        // Token: 0x17000E1E RID: 3614
        // (get) Token: 0x0600504E RID: 20558 RVA: 0x001AFD8A File Offset: 0x001ADF8A
        public CompAssignableToPawn_Lectern CompAssignableToPawn
        {
            get
            {
                return base.GetComp<CompAssignableToPawn_Lectern>();
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ReligionDefOf.WorshipKnowlegde, KnowledgeAmount.Total);
            if (altar == null)
            {
                Building_Altar l = ReligionUtility.FindAtlarToLectern(this, map);
                if (l != null)
                {
                    altar = l;
                    altar.lectern = this;
                    if (!altar.religion.NullOrEmpty())
                        TryAssignTrait(altar.religion[0]);
                }
            } 
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (altar != null)
            {
                altar.lectern = null;
            }
            base.Destroy(mode);           
        }

        public override void TickRare()
        {
            if (!Spawned) return;
            base.TickRare();
            if (ReligionUtility.TimeToWorship(Map, timeOfWorship) && daysOfWorships[(GenLocalDate.DayOfQuadrum(Map))] && didWorship == false)
            {               
                ReligionUtility.TryWorship(this, false);
            }
            if (ReligionUtility.IsEvening(Map) && didWorship == true)
            {
                didWorship = false;
            }
        }


        #region ITrait
        public IEnumerable<TraitDef> AssigningTrait
        {
            get
            {
                if (!Spawned)
                    return Enumerable.Empty<TraitDef>();
                return DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            }
        }

        public IEnumerable<TraitDef> AssignedTraits
        {
            get
            {
                return religion;
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
            ReligionUtility.Wipe(this);
        }
        #endregion

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (Faction == Faction.OfPlayer && Prefs.DevMode)
            {
                var command_Action = new Command_Action
                {
                    action = delegate
                    {
                        ReligionUtility.TryWorship(this, true);
                    },
                    defaultLabel = "Worship".Translate(),
                    defaultDesc = "WorshipDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc2,
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                };
                yield return command_Action;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref timeOfWorship, "timeofWorship");
            Scribe_Values.Look<bool>(ref didWorship, "morning", false, false);
            Scribe_Values.Look<int>(ref timeOfWorship, "timeOfWorship");
            Scribe_Collections.Look<TraitDef>(ref religion, "religions", LookMode.Def);
            Scribe_Collections.Look<bool>(ref daysOfWorships, "daysOfWorships");
            Scribe_References.Look<Building_Altar>(ref altar, "LecternAltar");
        }
    }
}