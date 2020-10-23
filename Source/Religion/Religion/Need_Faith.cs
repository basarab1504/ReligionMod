using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Religion
{
    class Need_Faith : Need
    {
        public Need_Faith(Pawn pawn)
        : base(pawn)
        {
            threshPercents = new List<float>
            {
                0.3f
            };
        }

        private const float ThreshDesire = 0.01f;
        private const float ThreshSatisfied = 0.3f;

        public override int GUIChangeArrow
        {
            get
            {
                return -1;
            }
        }

        public FaithCategory CurCategory
        {
            get
            {
                if (CurLevel > 0.300000011920929)
                    return FaithCategory.Satisfied;
                return CurLevel > 0.00999999977648258 ? FaithCategory.Desire : FaithCategory.Withdrawal;
            }
        }

        public override float CurLevel
        {
            get
            {
                return base.CurLevel;
            }
            set
            {
                FaithCategory curCategory = CurCategory;
                base.CurLevel = value;
                if (CurCategory == curCategory)
                    return;
            }
        }

        private float ChemicalFallPerTick
        {
            get
            {
                return def.fallPerDay / 60000f;
            }
        }

        private float ChemicalFallPerTickMult
        {
            get
            {
                return pawn.health.hediffSet.GetFirstHediffOfDef(ReligionDefOf.ReligionTolerance).Severity / 60000f;
            }
        }

        public override void SetInitialLevel()
        {
            CurLevelPercentage = Rand.Range(0.8f, 1f);
        }

        public override void NeedInterval()
        {
            if (IsFrozen)
                return;
            CurLevel -= (ChemicalFallPerTick + ChemicalFallPerTickMult) * 150f;
        }

    }
}
