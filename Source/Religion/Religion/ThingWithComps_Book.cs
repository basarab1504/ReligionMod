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
        public Pawn first;

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("Take a book".Translate(), (Action)(() => selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.TakeInventory, (LocalTargetInfo)this) { count = 1 })), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, null);
            if(first == null)
            yield return new FloatMenuOption("ReadABook".Translate(), (Action)(() => { selPawn.jobs.TryTakeOrderedJob(ReligionUtility.PlaceToRead(selPawn, this)); /*first = selPawn; */}), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, null);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.first, "first");
        }
    }
}
