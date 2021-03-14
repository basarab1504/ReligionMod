using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Religion
{
    public class Building_Lectern : Building, IAssignableTrait
    {
        public readonly List<Pawn> listeners = new();
        public Building_Altar altar;
        public List<bool> daysOfWorships = Enumerable.Repeat(false, 15).ToList();
        public bool didWorship;
        public List<TraitDef> religion = new();
        public string timeOfbuffer;
        public int timeOfWorship = 9;

        public Pawn AssignedPawn
        {
            get
            {
                if (CompAssignableToPawn == null || !CompAssignableToPawn.AssignedPawnsForReading.Any())
                {
                    return null;
                }

                return CompAssignableToPawn.AssignedPawnsForReading[0];
            }
        }

        // Token: 0x17000E1E RID: 3614
        // (get) Token: 0x0600504E RID: 20558 RVA: 0x001AFD8A File Offset: 0x001ADF8A
        public CompAssignableToPawn_Lectern CompAssignableToPawn => GetComp<CompAssignableToPawn_Lectern>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ReligionDefOf.WorshipKnowlegde, KnowledgeAmount.Total);
            if (altar != null)
            {
                return;
            }

            var buildingAltar = ReligionUtility.FindAtlarToLectern(this, map);
            if (buildingAltar == null)
            {
                return;
            }

            altar = buildingAltar;
            altar.lectern = this;
            if (!altar.religion.NullOrEmpty())
            {
                TryAssignTrait(altar.religion[0]);
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
            if (!Spawned)
            {
                return;
            }

            base.TickRare();
            if (ReligionUtility.TimeToWorship(Map, timeOfWorship) && daysOfWorships[GenLocalDate.DayOfQuadrum(Map)] &&
                didWorship == false)
            {
                ReligionUtility.TryWorship(this, false);
            }

            if (ReligionUtility.IsEvening(Map) && didWorship)
            {
                didWorship = false;
            }
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            if (Faction != Faction.OfPlayer || !Prefs.DevMode)
            {
                yield break;
            }

            var command_Action = new Command_Action
            {
                action = delegate { ReligionUtility.TryWorship(this, true); },
                defaultLabel = "Worship".Translate(),
                defaultDesc = "WorshipDesc".Translate(),
                hotKey = KeyBindingDefOf.Misc2,
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner")
            };
            yield return command_Action;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref timeOfWorship, "timeofWorship");
            Scribe_Values.Look(ref didWorship, "morning");
            Scribe_Values.Look(ref timeOfWorship, "timeOfWorship");
            Scribe_Collections.Look(ref religion, "religions", LookMode.Def);
            Scribe_Collections.Look(ref daysOfWorships, "daysOfWorships");
            Scribe_References.Look(ref altar, "LecternAltar");
        }


        #region ITrait

        public IEnumerable<TraitDef> AssigningTrait
        {
            get
            {
                if (!Spawned)
                {
                    return Enumerable.Empty<TraitDef>();
                }

                return DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            }
        }

        public IEnumerable<TraitDef> AssignedTraits => religion;

        public int MaxAssignedTraitsCount => 1;

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
        }

        public void TryUnassignTrait(TraitDef trait)
        {
            ReligionUtility.Wipe(this);
        }

        #endregion
    }
}