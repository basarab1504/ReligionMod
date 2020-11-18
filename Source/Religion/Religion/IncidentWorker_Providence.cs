using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace Religion
{
    class IncidentWorker_Providence : IncidentWorker
    {
        readonly Trait Religion = new Trait(DefDatabase<TraitDef>.AllDefsListForReading.FindAll(x => x is TraitDef_ReligionTrait).RandomElement());

        public IEnumerable<Pawn> PotentialVictimCandidates(IIncidentTarget target)
        {
            if (target is Map map)
                return map.mapPawns.FreeColonistsAndPrisoners.Where<Pawn>(x =>
                {
                    if (x.ParentHolder is Building_CryptosleepCasket)
                        return false;
                    if (!x.IsFreeColonist)
                        return x.IsPrisonerOfColony;
                    if (x.story.traits.allTraits.Any(a => a.def is TraitDef_ReligionTrait || a.def is TraitDef_NonReligion))
                        return false;
                    if (!ReligionUtility.CanBeReligious(x, Religion))
                        return false;
                    return x.RaceProps.IsFlesh;
                });
            return ((Caravan)target).PawnsListForReading.Where<Pawn>(x =>
            {
                if (x.ParentHolder is Building_CryptosleepCasket)
                    return false;
                if (!x.IsFreeColonist)
                    return x.IsPrisonerOfColony;
                if (x.story.traits.allTraits.Any(a => a.def is TraitDef_ReligionTrait || a.def is TraitDef_NonReligion))
                    return false;
                if (!ReligionUtility.CanBeReligious(x, Religion))
                    return false;
                return x.RaceProps.IsFlesh;
            });
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Pawn pawn = PotentialVictimCandidates(parms.target).RandomElement();
            if (pawn == null)
                return false;
            pawn.story.traits.GainTrait(Religion);
            Find.LetterStack.ReceiveLetter(def.letterLabel, pawn.LabelCap + " " + def.letterText + " " + Religion.LabelCap, def.letterDef, (LookTargets)pawn, (Faction)null);
            return true;
        }
    }
}
