using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.Sound;
using Verse;

namespace Religion
{
    class ThingComp_ReligionComp : ThingComp
    {
        public CompProperties_RelProp Props => this.props as CompProperties_RelProp;
        public Building_Lectern Altar
        {
            get
            {
                return (Building_Lectern)GenClosest.ClosestThingReachable(this.parent.PositionHeld, this.parent.Map,
                    ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), Verse.AI.PathEndMode.ClosestTouch,
                    TraverseMode.ByPawn, 9999, x => x is Building_Lectern, null, 0, -1, false, RegionType.Set_Passable, false);
            }
        }
        public IEnumerable<IntVec3> CellsInRange
        {
            get
            {
                return GenRadial.RadialCellsAround(parent.Position, Props.rangeRadius, true);
            }
        }


        public virtual void Use()
        {
            Props.hitSound.PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map));
            Altar.Listeners();
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            GenDraw.DrawRadiusRing(this.parent.Position, Props.rangeRadius);
        }
    }
}
