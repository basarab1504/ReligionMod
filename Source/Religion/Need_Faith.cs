using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Religion
{
    internal class Need_Faith : Need
    {
        private const float ThreshDesire = 0.01f;
        private const float ThreshSatisfied = 0.3f;

        public Need_Faith(Pawn pawn)
            : base(pawn)
        {
            threshPercents = new List<float>
            {
                0.3f
            };
        }

        public override int GUIChangeArrow => -1;

        private FaithCategory CurCategory
        {
            get
            {
                if (CurLevel > 0.300000011920929)
                {
                    return FaithCategory.Satisfied;
                }

                return CurLevel > 0.00999999977648258 ? FaithCategory.Desire : FaithCategory.Withdrawal;
            }
        }

        public override float CurLevel
        {
            get => base.CurLevel;
            set
            {
                var curCategory = CurCategory;
                base.CurLevel = value;
            }
        }

        private float ChemicalFallPerTick => def.fallPerDay / 60000f;

        private float ChemicalFallPerTickMult =>
            pawn.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionTolerance).Severity / 60000f;

        public override void SetInitialLevel()
        {
            CurLevelPercentage = Rand.Range(0.8f, 1f);
        }

        public override void NeedInterval()
        {
            if (IsFrozen)
            {
                return;
            }

            CurLevel -= (ChemicalFallPerTick + ChemicalFallPerTickMult) * 150f;
        }
    }
}