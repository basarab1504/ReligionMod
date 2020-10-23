using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace Religion //крыса
{
    public class ITab_Worship : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        protected Building_Lectern SelLectern
        {
            get
            {
                return (Building_Lectern)SelThing;
            }
        }

        public ITab_Worship()
        {
            size = WinSize;
            labelKey = "TabWorship".Translate();
            //this.tutorTag = "Worships".Translate();
        }

        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            float num = ReligionCardUtility.DrawCard(SelLectern);
        }
    }
}
