using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using RimWorld;
using Verse;
using Verse.AI;


namespace Religion
{
    class JoyGiver_HoldLecture : JoyGiver_InteractBuilding
    {
        protected override bool CanInteractWith(Pawn pawn, Thing t, bool inBed)
        {
            Building_Lectern lectern = t as Building_Lectern;
            if (!(t is Building_Lectern))
                return false;
            if (lectern.owners.Count == 0)
                return false;
            if (pawn == lectern.owners[0])
                return true;
            if (!base.CanInteractWith(pawn, t, inBed))
                return false;
            if (inBed == false)
                return false;
            return false;
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            return new Job(this.def.jobDef, (LocalTargetInfo)t);
        }
    }
}
