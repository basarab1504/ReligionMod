using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    class JobGiver_WanderChurch : JobGiver_Wander
    {
        private static List<IntVec3> gatherSpots = new List<IntVec3>();

        public JobGiver_WanderChurch()
        {
            this.wanderRadius = 7f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
            this.wanderDestValidator = (Func<Pawn, IntVec3, IntVec3, bool>)((pawn, loc, root) => true);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike && pawn.health.hediffSet.HasHediff(ReligionDefOf.ReligionAddiction))
            {
                JobGiver_WanderChurch.gatherSpots.Clear();
                for (int index = 0; index < pawn.Map.listerBuildings.allBuildingsColonist.FindAll(x => x is Building_Altar && (x as Building_Altar).relic != null).Count; ++index)
                {
                    IntVec3 position = pawn.Map.listerBuildings.allBuildingsColonist.FindAll(x => x is Building_Altar && (x as Building_Altar).relic != null)[index].Position;
                    if (!position.IsForbidden(pawn) && pawn.CanReach((LocalTargetInfo)position, PathEndMode.Touch, Danger.None, false, TraverseMode.ByPawn))
                    {
                        JobGiver_WanderChurch.gatherSpots.Add(position);                     
                    }
                }
                if (JobGiver_WanderChurch.gatherSpots.Count > 0)
                    return JobGiver_WanderChurch.gatherSpots.RandomElement<IntVec3>();
            }
            List<Building> buildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            if (buildingsColonist.Count > 0)
            {
                int num1 = 0;
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
                                building = buildingsColonist.RandomElement<Building>();
                            else
                                goto label_15;
                        }
                        while (building.def != ThingDefOf.Wall && !building.def.building.ai_chillDestination || (building.Position.IsForbidden(pawn) || !pawn.Map.areaManager.Home[building.Position]));
                        num2 = 15 + num1 * 2;
                    }
                    while ((pawn.Position - building.Position).LengthHorizontalSquared > num2 * num2);
                    c = GenAdjFast.AdjacentCells8Way((LocalTargetInfo)((Thing)building)).RandomElement<IntVec3>();
                }
                while (!c.Standable(building.Map) || c.IsForbidden(pawn) || (!pawn.CanReach((LocalTargetInfo)c, PathEndMode.OnCell, Danger.None, false, TraverseMode.ByPawn) || c.IsInPrisonCell(pawn.Map)));
                return c;
            }
            label_15:
            Pawn result;
            if (pawn.Map.mapPawns.FreeColonistsSpawned.Where<Pawn>((Func<Pawn, bool>)(c =>
            {
                if (!c.Position.IsForbidden(pawn))
                    return pawn.CanReach((LocalTargetInfo)c.Position, PathEndMode.Touch, Danger.None, false, TraverseMode.ByPawn);
                return false;
            })).TryRandomElement<Pawn>(out result))
                return result.Position;
            return pawn.Position;
        }
    }
}
