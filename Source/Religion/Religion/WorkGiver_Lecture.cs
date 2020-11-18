using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    class WorkGiver_Worship : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                if (def.fixedBillGiverDefs != null && def.fixedBillGiverDefs.Count == 1)
                    return ThingRequest.ForDef(def.fixedBillGiverDefs[0]);
                return ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
            }
        }

        public override PathEndMode PathEndMode
        {
            get { return PathEndMode.OnCell; }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_Lectern lectern))
                return false;
            if (lectern.CompAssignableToPawn.AssignedPawnsForReading.Count == 0)
                return false;
            if (lectern.CompAssignableToPawn.AssignedPawnsForReading[0] == pawn && pawn.CanReserve(t, 1, -1, null, forced))
            {
                return base.HasJobOnThing(lectern.CompAssignableToPawn.AssignedPawnsForReading[0], t, forced);
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(ReligionDefOf.HoldWorship, t);
        }
    }
}
