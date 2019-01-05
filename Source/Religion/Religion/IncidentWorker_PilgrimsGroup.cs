using System;
using System.Collections.Generic;
using Verse;
using System.Linq;
using Verse.AI.Group;
using RimWorld;

namespace Religion
{
    class IncidentWorker_PilgrimsGroup : IncidentWorker_NeutralGroup
    {
        private static readonly SimpleCurve PointsCurve = new SimpleCurve()
    {
      {
        new CurvePoint(45f, 0.0f),
        true
      },
      {
        new CurvePoint(50f, 1f),
        true
      },
      {
        new CurvePoint(100f, 1f),
        true
      },
      {
        new CurvePoint(200f, 0.25f),
        true
      },
      {
        new CurvePoint(300f, 0.1f),
        true
      },
      {
        new CurvePoint(500f, 0.0f),
        true
      }
    };


        private const float TraderChance = 0.75f;
        TraitDef religionDef;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            if (!this.TryResolveParms(parms))
                return false;
            List<Pawn> pawns = SpawnPawnsReligion(parms);
            if (pawns.Count == 0)
                return false;
            IntVec3 result;
            RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0], out result);
            LordJob_VisitColony lordJobVisitColony = new LordJob_VisitColony(parms.faction, result);
            LordMaker.MakeNewLord(parms.faction, (LordJob)lordJobVisitColony, target, (IEnumerable<Pawn>)pawns);
            bool flag = false;
            if ((double)Rand.Value < 0.75)
                flag = this.TryConvertOnePawnToSmallTrader(pawns, parms.faction, target);
            Pawn pawn = pawns.Find((Predicate<Pawn>)(x => parms.faction.leader == x));
            string letterLabel;
            string letterText;
            if (pawns.Count == 1)
            {
                string str1 = !flag ? string.Empty : "\n\n" + "SingleVisitorArrivesTraderInfo".Translate(pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
                string str2 = pawn == null ? string.Empty : "\n\n" + "SingleVisitorArrivesLeaderInfo".Translate(pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
                letterLabel = "LetterLabelSingleVisitorArrives".Translate();
                letterText = "SingleVisitorArrives".Translate((NamedArgument)pawns[0].story.Title, (NamedArgument)parms.faction.Name, (NamedArgument)pawns[0].Name.ToStringFull, (NamedArgument)str1, (NamedArgument)str2, pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
            }
            else
            {
                string str1 = !flag ? string.Empty : "\n\n" + "GroupVisitorsArriveTraderInfo".Translate();
                string str2 = pawn == null ? string.Empty : "\n\n" + "GroupVisitorsArriveLeaderInfo".Translate((NamedArgument)pawn.LabelShort, (NamedArgument)((Thing)pawn));
                letterLabel = "LetterLabelGroupVisitorsArrive".Translate();
                letterText = "GroupVisitorsArrive".Translate((NamedArgument)parms.faction.Name, (NamedArgument)str1, (NamedArgument)str2);
            }
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter((IEnumerable<Pawn>)pawns, ref letterLabel, ref letterText, "LetterRelatedPawnsNeutralGroup".Translate((NamedArgument)Faction.OfPlayer.def.pawnsPlural), true, true);
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, (LookTargets)((Thing)pawns[0]), parms.faction, (string)null);
            return true;
        }

        List<Pawn> SpawnPawnsReligion(IncidentParms parms)
        {
            List<TraitDef> rels = DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            religionDef = ReligionDefOf.TynanReligion;
            Trait religion = new Trait(religionDef);
            Map target = (Map)parms.target;
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(this.PawnGroupKindDef, parms, true), false).ToList<Pawn>();
            foreach(Pawn p in list)
            {
                p.story.traits.GainTrait(religion);
            }
            foreach (Thing newThing in list)
            {               
                GenSpawn.Spawn(newThing, CellFinder.RandomClosewalkCellNear(parms.spawnCenter, target, 5, (Predicate<IntVec3>)null), target, WipeMode.Vanish);
            }            
            return list;
        }

        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            if ((double)parms.points >= 0.0)
                return;
            parms.points = Rand.ByCurve(PointsCurve);
        }

        private bool TryConvertOnePawnToSmallTrader(List<Pawn> pawns, Faction faction, Map map)
        {
            if (faction.def.visitorTraderKinds.NullOrEmpty<TraderKindDef>())
                return false;
            Pawn pawn = pawns.RandomElement<Pawn>();
            Lord lord = pawn.GetLord();
            pawn.mindState.wantsToTradeWithColony = true;
            PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
            TraderKindDef traderKindDef = faction.def.visitorTraderKinds.RandomElementByWeight<TraderKindDef>((Func<TraderKindDef, float>)(traderDef => traderDef.CalculatedCommonality));
            pawn.trader.traderKind = traderKindDef;
            pawn.inventory.DestroyAll(DestroyMode.Vanish);
            List<Thing> things = ThingSetMakerDefOf.TraderStock.root.Generate(new ThingSetMakerParams()
            {
                traderDef = traderKindDef,
                tile = new int?(map.Tile),
                traderFaction = faction
            });
            //ThingDef n = DefDatabase<ReligionBook_ThingDef>.AllDefsListForReading.Find(x => x.religion == religionDef);
            //things.Add(ThingMaker.MakeThing(n)); //////////////////
            foreach (Thing thing in things)
            {
                Pawn p = thing as Pawn;
                if (p != null)
                {
                    if (p.Faction != pawn.Faction)
                        p.SetFaction(pawn.Faction, (Pawn)null);
                    IntVec3 loc = CellFinder.RandomClosewalkCellNear(pawn.Position, map, 5, (Predicate<IntVec3>)null);
                    GenSpawn.Spawn((Thing)p, loc, map, WipeMode.Vanish);
                    lord.AddPawn(p);
                }
                else if (!pawn.inventory.innerContainer.TryAdd(thing, true))
                    thing.Destroy(DestroyMode.Vanish);
            }
            PawnInventoryGenerator.GiveRandomFood(pawn);
            //////if(n != null)
            ////{
            ////    Thing book = ThingMaker.MakeThing(n);
            ////    Messages.Message(book.ToString() + " " + (book.def as ReligionBook_ThingDef).religion.ToString(), MessageTypeDefOf.NegativeEvent);
            ////    Pawn a = book as Pawn;
            ////    if (a.Faction != pawn.Faction)
            ////        a.SetFaction(pawn.Faction, (Pawn)null);
            ////    IntVec3 locc = CellFinder.RandomClosewalkCellNear(pawn.Position, map, 5, (Predicate<IntVec3>)null);
            ////    GenSpawn.Spawn((Thing)a, locc, map, WipeMode.Vanish);
            ////    lord.AddPawn(a);
            ////}
            return true;
        }
    }
}
