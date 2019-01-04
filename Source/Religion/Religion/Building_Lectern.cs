using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace Religion
{
    public class Building_Lectern : Building, IAssignableTrait, IAssignableBuilding
    {
        public List<Pawn> owners = new List<Pawn>();
        public List<Pawn> listeners = new List<Pawn>();
        public List<TraitDef> religion = new List<TraitDef>();
        public List<bool> daysOfLectures = Enumerable.Repeat(false, 15).ToList();
        public int timeOfLecture = 9;
        public string timeOfbuffer;
        public bool didLecture;
        Building_Altar altar;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (altar == null)
            {
                Building_Altar a;
                a = ReligionUtility.FindAtlarToLectern(this, map);
                if (a != null)
                {
                    this.altar = a;
                    altar.lectern = this;
                    if (!a.religion.NullOrEmpty())
                        TryAssignTrait(a.religion[0]);
                }
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (altar != null)
                altar.lectern = null;
            base.Destroy(mode);           
        }

        public override void TickRare()
        {
            if (!Spawned) return;
            // Don't forget the base work
            base.TickRare();
            if (ReligionUtility.TimeToLecture(Map, timeOfLecture) && daysOfLectures[(GenLocalDate.DayOfQuadrum(Map))] && didLecture == false)
            {
                //Messages.Message("is true", MessageTypeDefOf.PositiveEvent);                
                ReligionUtility.TryLecture(this,false);
            }
            if (ReligionUtility.IsEvening(Map) && didLecture == true)
            {
                didLecture = false;
                //Messages.Message("is false", MessageTypeDefOf.PositiveEvent);
            }
        } //проверка почти каждый тик, ужас

        #region IBuilding
        public IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!this.Spawned || this.religion.NullOrEmpty())
                    return Enumerable.Empty<Pawn>();
                return Map.mapPawns.FreeColonists.Where(x => x.story.traits.HasTrait(religion[0]) && !Map.listerBuildings.allBuildingsColonist.Any(u => u is Building_Lectern && (u as Building_Lectern).owners.Contains(x)));
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

        public void TryAssignPawn(Pawn pawn)
        {
            owners.Clear();
            if (!owners.Contains(pawn))
            {
                owners.Add(pawn);
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
            ReligionUtility.Wipe(this);
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
            if (base.Faction == Faction.OfPlayer)
            {

                //yield return new Command_Action
                //{
                //    defaultLabel = "AssignReligion".Translate(),
                //    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                //    defaultDesc = "AssignReligionDesc".Translate(),
                //    action = delegate
                //    {
                //        Find.WindowStack.Add(new Dialog_AssignTrait(this));
                //    },
                //    hotKey = KeyBindingDefOf.Misc4
                //};

                //yield return new Command_Action
                //{
                //    defaultLabel = "CommandBedSetOwnerLabel".Translate(),
                //    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                //    defaultDesc = "CommandBedSetOwnerDesc".Translate(),
                //    action = delegate
                //    {
                //        if (religion.Count == 0)
                //            Messages.Message("Select a religion first".Translate(), MessageTypeDefOf.NegativeEvent);
                //        else
                //            Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                //    },
                //    hotKey = KeyBindingDefOf.Misc3
                //};

                var command_Action = new Command_Action
                {
                    action = delegate
                    {
                        if (religion.NullOrEmpty() || owners.NullOrEmpty())
                            Messages.Message("Select a religion and preacher first".Translate(), MessageTypeDefOf.NegativeEvent);
                        else
                        {
                            ReligionUtility.TryLecture(this,true);
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timeOfLecture, "timeoflecture");
            Scribe_Values.Look<bool>(ref this.didLecture, "morning", false, false);
            Scribe_Values.Look<int>(ref this.timeOfLecture, "timeOfLecture");
            Scribe_Collections.Look<TraitDef>(ref this.religion, "religions", LookMode.Def);
            Scribe_Collections.Look<Pawn>(ref this.owners, "owners", LookMode.Reference);
            Scribe_Collections.Look<bool>(ref this.daysOfLectures, "daysOfLectures");
            Scribe_References.Look<Building_Altar>(ref this.altar, "LecternAltar");
        }
    }
}