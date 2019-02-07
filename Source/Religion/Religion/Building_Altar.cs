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
    public class Building_Altar : Building_Storage, IAssignableTrait
    {
        public List<TraitDef> religion = new List<TraitDef>();
        public Building_Lectern lectern;
        public Thing relic;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ReligionDefOf.ReligionKnowlegde, KnowledgeAmount.Total);
            if (lectern == null)
            {
                Building_Lectern l = ReligionUtility.FindLecternToAltar(this, map);
                if (l != null)
                {
                    lectern = l;
                    lectern.altar = this;
                }
            }
        }

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
            UnassginAll();
            if (!religion.Contains(trait))
            {
                religion.Add(trait);
                ThingDef bookDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.comps.Any(y => y is CompProperties_CompRelic && (y as CompProperties_CompRelic).religionTrait == trait));
                if (bookDef != null)
                this.GetStoreSettings().filter.SetAllow(bookDef, true);
                if (lectern != null)
                    lectern.TryAssignTrait(trait);
            }
            else return;
        }

        public void TryUnassignTrait(TraitDef trait)
        {
            UnassginAll();
        }

        public void UnassginAll()
        {
            this.GetStoreSettings().filter.SetDisallowAll();
            religion.Clear();
            if (relic != null)
                relic = null;
            if (lectern != null)
            {
                lectern.religion.Clear();
                lectern.owners.Clear();
            }

        }
        #endregion

        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);
            if(newItem.def.comps.Any(x=> x is CompProperties_CompRelic))
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
            if (base.Faction == Faction.OfPlayer)
            {
                yield return new Command_Action
                {
                    defaultLabel = "AssignReligion".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Things/Symbols/AssignReligion", true),
                    defaultDesc = "AssignReligionDesc".Translate(),
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_AssignTrait(this));
                    },
                    hotKey = KeyBindingDefOf.Misc4
                };

            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<TraitDef>(ref this.religion, "religions", LookMode.Def);
            Scribe_References.Look<Building_Lectern>(ref this.lectern, "AltarLectern");
            Scribe_References.Look<Thing>(ref this.relic, "relic");
        }
    }
}
