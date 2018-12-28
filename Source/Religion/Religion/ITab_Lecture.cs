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
    public class ITab_Lecture : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(420f, 480f);

        protected Building_Lectern SelLectern
        {
            get
            {
                return (Building_Lectern)this.SelThing;
            }
        }

        public ITab_Lecture()
        {
            this.size = ITab_Lecture.WinSize;
            this.labelKey = "TabLecture";
            this.tutorTag = "Lectures";
        }

        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            float num = ReligionCardUtility.DrawCard(SelLectern);
        }
    }
}
