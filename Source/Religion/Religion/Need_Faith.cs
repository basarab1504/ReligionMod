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
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.3f);
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
                if ((double)this.CurLevel > 0.300000011920929)
                    return FaithCategory.Satisfied;
                return (double)this.CurLevel > 0.00999999977648258 ? FaithCategory.Desire : FaithCategory.Withdrawal;
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
                FaithCategory curCategory = this.CurCategory;
                base.CurLevel = value;
                if (this.CurCategory == curCategory)
                    return;
            }
        }

        private float ChemicalFallPerTick
        {
            get
            {
                return this.def.fallPerDay / 60000f;
            }
        }

        public override void SetInitialLevel()
        {
            this.CurLevelPercentage = Rand.Range(0.8f, 1f);
        }

        public override void NeedInterval()
        {
            if (this.IsFrozen)
                return;
            this.CurLevel -= this.ChemicalFallPerTick * 150f;
        }

    }
}
