using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Religion
{
    internal class IncidentWorker_PilgrimsGroup : IncidentWorker_NeutralGroup
    {
        private const float TraderChance = 0.75f;

        private static readonly SimpleCurve PointsCurve = new()
        {
            new CurvePoint(45f, 0.0f),
            new CurvePoint(50f, 1f),
            new CurvePoint(100f, 1f),
            new CurvePoint(200f, 0.25f),
            new CurvePoint(300f, 0.1f),
            new CurvePoint(500f, 0.0f)
        };

        private TraitDef religionDef;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var target = (Map) parms.target;
            if (!TryResolveParms(parms))
            {
                return false;
            }

            var pawns = SpawnPawnsReligion(parms);
            if (pawns.Count == 0)
            {
                return false;
            }

            RCellFinder.TryFindRandomSpotJustOutsideColony(pawns[0], out var result);
            var lordJobVisitColony = new LordJob_VisitColony(parms.faction, result);
            LordMaker.MakeNewLord(parms.faction, lordJobVisitColony, target, pawns);
            var flag = false;
            if (Rand.Value < TraderChance)
            {
                flag = TryConvertOnePawnToSmallTrader(pawns, parms.faction, target);
            }

            var pawn = pawns.Find(x => parms.faction.leader == x);
            TaggedString letterLabel;
            TaggedString letterText;
            if (pawns.Count == 1)
            {
                var str1 = !flag
                    ? TaggedString.Empty
                    : "\n\n" + "SinglePilgrimVisitorArrivesTraderInfo".Translate(pawns[0].Named("PAWN"))
                        .AdjustedFor(pawns[0]);
                var str2 = pawn == null
                    ? TaggedString.Empty
                    : "\n\n" + "SingleVisitorArrivesLeaderInfo".Translate(pawns[0].Named("PAWN")).AdjustedFor(pawns[0]);
                letterLabel = "LetterLabelSingleVisitorArrives".Translate();
                letterText = "SingleVisitorArrives".Translate(pawns[0].story.Title, parms.faction.Name,
                    pawns[0].Name.ToStringFull, str1, str2, pawns[0].Named("PAWN")).AdjustedFor(pawns[0]);
            }
            else
            {
                var str1 = !flag ? TaggedString.Empty : "\n\n" + "GroupPilgrimVisitorsArriveTraderInfo".Translate();
                var str2 = pawn == null
                    ? TaggedString.Empty
                    : "\n\n" + "GroupVisitorsArriveLeaderInfo".Translate(pawn.LabelShort, pawn);
                letterLabel = "LetterLabelGroupVisitorsArrive".Translate();
                letterText = "GroupVisitorsArrive".Translate(parms.faction.Name, str1, str2);
            }

            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText,
                "LetterRelatedPawnsNeutralGroup".Translate(Faction.OfPlayer.def.pawnsPlural), true);
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, pawns[0], parms.faction);
            return true;
        }

        private List<Pawn> SpawnPawnsReligion(IncidentParms parms)
        {
            var rels = DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait);
            religionDef = rels.RandomElement();
            var religion = new Trait(religionDef);
            var target = (Map) parms.target;
            var list = PawnGroupMakerUtility
                .GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDef, parms, true), false)
                .ToList();
            foreach (var newThing in list)
            {
                newThing.story.traits.GainTrait(religion);
                GenSpawn.Spawn(newThing, CellFinder.RandomClosewalkCellNear(parms.spawnCenter, target, 5), target);
            }

            return list;
        }

        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            if (parms.points >= 0.0)
            {
                return;
            }

            parms.points = Rand.ByCurve(PointsCurve);
        }

        private bool TryConvertOnePawnToSmallTrader(List<Pawn> pawns, Faction faction, Map map)
        {
            if (faction.def.visitorTraderKinds.NullOrEmpty())
            {
                return false;
            }

            var pawn = pawns.RandomElement();
            var lord = pawn.GetLord();
            pawn.mindState.wantsToTradeWithColony = true;
            PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, true);
            var traderKindDef = ReligionDefOf.Pilgrim_Standart;
            pawn.trader.traderKind = traderKindDef;
            pawn.inventory.DestroyAll();
            //ThingDef n = DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.comps.Any(y => y is CompProperties_CompRelic && (y as CompProperties_CompRelic).religionTrait == religionDef));
            var n = DefDatabase<ThingDef>.AllDefsListForReading.Find(x =>
                x.comps.Any(y => y is CompProperties_CompReligionBook book && book.religionTrait == religionDef));
            var forBook = new StockGenerator_Book {thingDef = n, countRange = {min = 1, max = 1}};
            traderKindDef.stockGenerators.Add(forBook);
            var things = ThingSetMakerDefOf.TraderStock.root.Generate(new ThingSetMakerParams
            {
                traderDef = traderKindDef,
                tile = map.Tile,
                makingFaction = faction
            });
            foreach (var thing in things)
            {
                if (thing is Pawn p)
                {
                    if (p.Faction != pawn.Faction)
                    {
                        p.SetFaction(pawn.Faction);
                    }

                    var loc = CellFinder.RandomClosewalkCellNear(pawn.Position, map, 5);
                    GenSpawn.Spawn(p, loc, map);
                    lord.AddPawn(p);
                }
                else if (!pawn.inventory.innerContainer.TryAdd(thing))
                {
                    thing.Destroy();
                }
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