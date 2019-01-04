using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class ThingWithComps_Book : ThingWithComps
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("Take a book".Translate(), (Action)(() => selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.TakeInventory, (LocalTargetInfo)this) { count = 1 })), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, null);
            yield return new FloatMenuOption("ReadABook".Translate(), (Action)(() => selPawn.jobs.TryTakeOrderedJob(PlaceToRead(selPawn, this))), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, null);
        }

        private static List<CompGatherSpot> workingSpots = new List<CompGatherSpot>();
        private static readonly int NumRadiusCells = GenRadial.NumCellsInRadius(3.9f);
        private static readonly List<IntVec3> RadialPatternMiddleOutward = ((IEnumerable<IntVec3>)GenRadial.RadialPattern).Take<IntVec3>(NumRadiusCells).OrderBy<IntVec3, float>((Func<IntVec3, float>)(c => Mathf.Abs((c - IntVec3.Zero).LengthHorizontal - 1.95f))).ToList<IntVec3>();
        private static List<ThingDef> nurseableDrugs = new List<ThingDef>();
        private const float GatherRadius = 3.9f;

        public Job PlaceToRead(Pawn pawn, Thing t)
        {

            if (pawn.Map.gatherSpotLister.activeSpots.Count == 0)
                return (Job)null;
            workingSpots.Clear();
            for (int index = 0; index < pawn.Map.gatherSpotLister.activeSpots.Count; ++index)
                workingSpots.Add(pawn.Map.gatherSpotLister.activeSpots[index]);
            CompGatherSpot result1;
            while (workingSpots.TryRandomElement<CompGatherSpot>(out result1))
            {
                workingSpots.Remove(result1);
                if (!result1.parent.IsForbidden(pawn) && pawn.CanReach((LocalTargetInfo)((Thing)result1.parent), PathEndMode.Touch, Danger.None, false, TraverseMode.ByPawn) && (result1.parent.IsSociallyProper(pawn) && result1.parent.IsPoliticallyProper(pawn)))
                {
                    if (result1.parent.def.surfaceType == SurfaceType.Eat)
                    {
                        Messages.Message("1", MessageTypeDefOf.NegativeEvent);
                        Thing chair;
                        if (!TryFindChairBesideTable((Thing)result1.parent, pawn, out chair))
                            return (Job)null;
                        return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)chair, (LocalTargetInfo)t);
                    }
                    else
                    {
                        Thing chair;
                        if (TryFindChairNear(result1.parent.Position, pawn, out chair))
                        {
                            Messages.Message("2", MessageTypeDefOf.NegativeEvent);
                            return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)chair, (LocalTargetInfo)t);
                        }
                        else
                        {
                            IntVec3 result2;
                            if (!TryFindSitSpotOnGroundNear(result1.parent.Position, pawn, out result2))
                                return (Job)null;
                            Messages.Message("3", MessageTypeDefOf.NegativeEvent);
                            return new Job(ReligionDefOf.ReadBook, (LocalTargetInfo)((Thing)result1.parent), (LocalTargetInfo)result2, (LocalTargetInfo)t);
                        }
                    }
                }
            }
            return (Job)null;
        }

        private static bool TryFindChairBesideTable(Thing table, Pawn sitter, out Thing chair)
        {
            for (int index = 0; index < 30; ++index)
            {
                Building edifice = table.RandomAdjacentCellCardinal().GetEdifice(table.Map);
                if (edifice != null && edifice.def.building.isSittable && sitter.CanReserve((LocalTargetInfo)((Thing)edifice), 1, -1, (ReservationLayerDef)null, false))
                {
                    chair = (Thing)edifice;
                    return true;
                }
            }
            chair = (Thing)null;
            return false;
        }

        private static bool TryFindSitSpotOnGroundNear(IntVec3 center, Pawn sitter, out IntVec3 result)
        {
            for (int index = 0; index < 30; ++index)
            {
                IntVec3 intVec3 = center + GenRadial.RadialPattern[Rand.Range(1, NumRadiusCells)];
                if (sitter.CanReserveAndReach((LocalTargetInfo)intVec3, PathEndMode.OnCell, Danger.None, 1, -1, (ReservationLayerDef)null, false) && intVec3.GetEdifice(sitter.Map) == null && GenSight.LineOfSight(center, intVec3, sitter.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    result = intVec3;
                    return true;
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        private static bool TryFindChairNear(IntVec3 center, Pawn sitter, out Thing chair)
        {
            for (int index = 0; index < RadialPatternMiddleOutward.Count; ++index)
            {
                Building edifice = (center + RadialPatternMiddleOutward[index]).GetEdifice(sitter.Map);
                if (edifice != null && edifice.def.building.isSittable && (sitter.CanReserve((LocalTargetInfo)((Thing)edifice), 1, -1, (ReservationLayerDef)null, false) && !edifice.IsForbidden(sitter)) && GenSight.LineOfSight(center, edifice.Position, sitter.Map, true, (Func<IntVec3, bool>)null, 0, 0))
                {
                    chair = (Thing)edifice;
                    return true;
                }
            }
            chair = (Thing)null;
            return false;
        }
    }
}
