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
                if (this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Count == 1)
                    return ThingRequest.ForDef(this.def.fixedBillGiverDefs[0]);
                return ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
            }
        }

        public override PathEndMode PathEndMode
        {
            get { return PathEndMode.OnCell; }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_Lectern lectern = t as Building_Lectern;
            if (lectern == null)
                return false;
            if (lectern.owners.Count == 0)
                return false;
            if (lectern.owners[0] == pawn && pawn.CanReserve((LocalTargetInfo)t, 1, -1, (ReservationLayerDef)null, forced))
            {
                return base.HasJobOnThing(lectern.owners[0], t, forced);
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(ReligionDefOf.HoldWorship, (LocalTargetInfo)t);
        }
    }
}
