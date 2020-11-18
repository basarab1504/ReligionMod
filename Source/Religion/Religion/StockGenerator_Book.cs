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
        public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
        {
            foreach (Thing th in StockGeneratorUtility.TryMakeForStock(thingDef, RandomCountOf(thingDef)))
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
            if (!thingDef.tradeability.TraderCanSell())
            {
                yield return thingDef + " tradeability doesn't allow traders to sell this thing";
            }
        }
    }
}
