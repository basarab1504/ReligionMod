using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using RimWorld;

namespace Religion
{
    class StockGenerator_Book : StockGenerator
    {
        public ThingDef thingDef;

        [DebuggerHidden]
        public override IEnumerable<Thing> GenerateThings(int forTile)
        {
            foreach (Thing th in StockGeneratorUtility.TryMakeForStock(this.thingDef, base.RandomCountOf(this.thingDef)))
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
            foreach (string e in base.ConfigErrors(parentDef))
            {
                yield return e;
            }
            if (!this.thingDef.tradeability.TraderCanSell())
            {
                yield return this.thingDef + " tradeability doesn't allow traders to sell this thing";
            }
        }
    }
}
