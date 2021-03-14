using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    public class ThingWithComps_Book : ThingWithComps
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("TakeABook".Translate(),
                () => selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.TakeInventory, this) {count = 1}));
            //if (first == null)
            //    yield return new FloatMenuOption("ReadABook".Translate(), (Action)(() => { selPawn.jobs.TryTakeOrderedJob(ReligionUtility.PlaceToRead(selPawn, this)); /*first = selPawn; */}), MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, null);
        }
    }
}