using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class JobGiver_WanderChurch : JobGiver_Wander
    {
        private static readonly List<IntVec3> gatherSpots = new();

        public JobGiver_WanderChurch()
        {
            wanderRadius = 7f;
            ticksBetweenWandersRange = new IntRange(125, 200);
            wanderDestValidator = (_, _, _) => true;
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike && pawn.health.hediffSet.HasHediff(ReligionDefOf.ReligionAddiction))
            {
                gatherSpots.Clear();
                for (var index = 0;
                    index < pawn.Map.listerBuildings.allBuildingsColonist.FindAll(x => x is Building_Altar
                    {
                        relic: { }
                    }).Count;
                    ++index)
                {
                    var position = pawn.Map.listerBuildings.allBuildingsColonist.FindAll(x => x is Building_Altar
                    {
                        relic: { }
                    })[index].Position;
                    if (!position.IsForbidden(pawn) && pawn.CanReach(position, PathEndMode.Touch, Danger.None))
                    {
                        gatherSpots.Add(position);
                    }
                }

                if (gatherSpots.Count > 0)
                {
                    return gatherSpots.RandomElement();
                }
            }

            var buildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            if (buildingsColonist.Count > 0)
            {
                var num1 = 0;
                Building building;
                IntVec3 c;
                do
                {
                    int num2;
                    do
                    {
                        do
                        {
                            ++num1;
                            if (num1 <= 20)
                            {
                                building = buildingsColonist.RandomElement();
                            }
                            else
                            {
                                goto label_15;
                            }
                        } while (building.def != ThingDefOf.Wall && !building.def.building.ai_chillDestination ||
                                 building.Position.IsForbidden(pawn) || !pawn.Map.areaManager.Home[building.Position]);

                        num2 = 15 + (num1 * 2);
                    } while ((pawn.Position - building.Position).LengthHorizontalSquared > num2 * num2);

                    c = GenAdjFast.AdjacentCells8Way(building).RandomElement();
                } while (!c.Standable(building.Map) || c.IsForbidden(pawn) ||
                         !pawn.CanReach(c, PathEndMode.OnCell, Danger.None) || c.IsInPrisonCell(pawn.Map));

                return c;
            }

            label_15:
            if (pawn.Map.mapPawns.FreeColonistsSpawned.Where(c =>
            {
                if (!c.Position.IsForbidden(pawn))
                {
                    return pawn.CanReach(c.Position, PathEndMode.Touch, Danger.None);
                }

                return false;
            }).TryRandomElement(out var result))
            {
                return result.Position;
            }

            return pawn.Position;
        }
    }
}