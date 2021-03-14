using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Religion
{
    public class Building_Altar : Building_Storage, IAssignableTrait
    {
        public Building_Lectern lectern;
        public Thing relic;
        public List<TraitDef> religion = new();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ReligionDefOf.ReligionKnowlegde, KnowledgeAmount.Total);
            if (lectern != null)
            {
                return;
            }

            var buildingLectern = ReligionUtility.FindLecternToAltar(this, map);
            if (buildingLectern == null)
            {
                return;
            }

            lectern = buildingLectern;
            lectern.altar = this;
        }

        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);
            if (newItem.def.comps.Any(x => x is CompProperties_CompRelic))
            {
                relic = newItem;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (lectern != null)
            {
                ReligionUtility.Wipe(lectern);
            }

            base.Destroy(mode);
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Faction == Faction.OfPlayer)
            {
                yield return new Command_Action
                {
                    defaultLabel = "AssignReligion".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Things/Symbols/AssignReligion"),
                    defaultDesc = "AssignReligionDesc".Translate(),
                    action = delegate { Find.WindowStack.Add(new Dialog_AssignTrait(this)); },
                    hotKey = KeyBindingDefOf.Misc4
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref religion, "religions", LookMode.Def);
            Scribe_References.Look(ref lectern, "AltarLectern");
            Scribe_References.Look(ref relic, "relic");
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
            UnassignAll();
            if (religion.Contains(trait))
            {
                return;
            }

            religion.Add(trait);
            var bookDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(x =>
                x.comps.Any(y =>
                    y is CompProperties_CompRelic compRelic && compRelic.religionTrait == trait));
            if (bookDef != null)
            {
                GetStoreSettings().filter.SetAllow(bookDef, true);
            }

            lectern?.TryAssignTrait(trait);
        }

        public void TryUnassignTrait(TraitDef trait)
        {
            UnassignAll();
        }

        private void UnassignAll()
        {
            GetStoreSettings().filter.SetDisallowAll();
            religion.Clear();
            relic = null;

            if (lectern == null)
            {
                return;
            }

            lectern.religion.Clear();
            lectern.CompAssignableToPawn.UnassignAllPawns();
        }

        #endregion
    }
}