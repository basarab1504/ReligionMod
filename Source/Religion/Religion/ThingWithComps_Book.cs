using RimWorld.Planet;
using System;
using System.Collections.Generic;
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
            yield return new FloatMenuOption("Take a book".Translate(), (Action)(() => selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.TakeInventory, (LocalTargetInfo)this) { count = 1 })), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null);
        }
    }
}
