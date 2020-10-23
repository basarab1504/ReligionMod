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
            if (!TryResolveParms(parms))
                return false;
            List<Pawn> pawns = SpawnPawnsReligion(parms);
            if (pawns.Count == 0)
                return false;
            RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0], out IntVec3 result);
            LordJob_VisitColony lordJobVisitColony = new LordJob_VisitColony(parms.faction, result);
            LordMaker.MakeNewLord(parms.faction, lordJobVisitColony, target, pawns);
            bool flag = false;
            if (Rand.Value < TraderChance)
                flag = TryConvertOnePawnToSmallTrader(pawns, parms.faction, target);
            Pawn pawn = pawns.Find(x => parms.faction.leader == x);
            TaggedString letterLabel;
            TaggedString letterText;
            if (pawns.Count == 1)
            {
                TaggedString str1 = !flag ? TaggedString.Empty : "\n\n" + "SinglePilgrimVisitorArrivesTraderInfo".Translate(pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
                TaggedString str2 = pawn == null ? TaggedString.Empty : "\n\n" + "SingleVisitorArrivesLeaderInfo".Translate(pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
                letterLabel = "LetterLabelSingleVisitorArrives".Translate();
                letterText = "SingleVisitorArrives".Translate(pawns[0].story.Title, parms.faction.Name, pawns[0].Name.ToStringFull, str1, str2, pawns[0].Named("PAWN")).AdjustedFor(pawns[0], "PAWN");
            }
            else
            {
                TaggedString str1 = !flag ? TaggedString.Empty : "\n\n" + "GroupPilgrimVisitorsArriveTraderInfo".Translate();
                TaggedString str2 = pawn == null ? TaggedString.Empty : "\n\n" + "GroupVisitorsArriveLeaderInfo".Translate(pawn.LabelShort, pawn);
                letterLabel = "LetterLabelGroupVisitorsArrive".Translate();
                letterText = "GroupVisitorsArrive".Translate(parms.faction.Name, str1, str2);
            }
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter((IEnumerable<Pawn>)pawns, ref letterLabel, ref letterText, "LetterRelatedPawnsNeutralGroup".Translate((NamedArgument)Faction.OfPlayer.def.pawnsPlural), true, true);
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, (LookTargets)((Thing)pawns[0]), parms.faction);
            return true;
        }

        List<Pawn> SpawnPawnsReligion(IncidentParms parms)
        {
            List<TraitDef> rels = DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            religionDef = rels.RandomElement();
            Trait religion = new Trait(religionDef);
            Map target = (Map)parms.target;
            List <Pawn> list = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDef, parms, true), false).ToList<Pawn>();
            foreach (Pawn newThing in list)
            {               
                newThing.story.traits.GainTrait(religion);
                GenSpawn.Spawn(newThing, CellFinder.RandomClosewalkCellNear(parms.spawnCenter, target, 5, null), target, WipeMode.Vanish);
            }            
            return list;
        }

        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            if (parms.points >= 0.0)
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
            TraderKindDef traderKindDef = ReligionDefOf.Pilgrim_Standart;
            pawn.trader.traderKind = traderKindDef;
            pawn.inventory.DestroyAll(DestroyMode.Vanish);
            //ThingDef n = DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.comps.Any(y => y is CompProperties_CompRelic && (y as CompProperties_CompRelic).religionTrait == religionDef));
            ThingDef n = DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.comps.Any(y => y is CompProperties_CompReligionBook && (y as CompProperties_CompReligionBook).religionTrait == religionDef));
            StockGenerator_Book forBook = new StockGenerator_Book
            {
                thingDef = n
            };
            forBook.countRange.min = 1;
            forBook.countRange.max = 1;
            traderKindDef.stockGenerators.Add(forBook);
            List<Thing> things = ThingSetMakerDefOf.TraderStock.root.Generate(new ThingSetMakerParams()
            {
                traderDef = traderKindDef,
                tile = new int?(map.Tile),
                makingFaction = faction
            });
            foreach (Thing thing in things)
            {
                if (thing is Pawn p)
                {
                    if (p.Faction != pawn.Faction)
                        p.SetFaction(pawn.Faction, null);
                    IntVec3 loc = CellFinder.RandomClosewalkCellNear(pawn.Position, map, 5, null);
                    GenSpawn.Spawn(p, loc, map, WipeMode.Vanish);
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
