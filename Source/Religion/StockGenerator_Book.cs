using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;

namespace Religion
{
    internal class StockGenerator_Book : StockGenerator
    {
        public ThingDef thingDef;

        [DebuggerHidden]
        public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
        {
            foreach (var th in StockGeneratorUtility.TryMakeForStock(thingDef, RandomCountOf(thingDef)))
            {
                yield return th;
            }
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef == this.thingDef;
        }

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
        {
            foreach (var e in base.ConfigErrors(parentDef))
            {
                yield return e;
            }

            if (!thingDef.tradeability.TraderCanSell())
            {
                yield return thingDef + " tradeability doesn't allow traders to sell this thing";
            }
        }
    }
}