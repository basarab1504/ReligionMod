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
        [TweakValue("Interface", 0.0f, 128f)]
        private static float PasteX = 400f;
        [TweakValue("Interface", 0.0f, 128f)]
        private static float PasteY = 20f;
        [TweakValue("Interface", 0.0f, 32f)]
        private static float PasteSizeY = 24f;
        private static float PasteSizeX = 200f;
        private static float SizeY = 42f;
        string buffer;

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
            Rect rect1 = new Rect(ITab_Lecture.WinSize.x - ITab_Lecture.PasteX, ITab_Lecture.PasteY, WinSize.x, ITab_Lecture.PasteSizeY);
            string label = "TimeOfLecture".Translate();
            Widgets.Label(rect1, label);
            Rect rect2 = new Rect(ITab_Lecture.WinSize.x - 160, ITab_Lecture.PasteY, 24f, 24f);
            Widgets.TextFieldNumeric<int>(rect2, ref SelLectern.timeOfLecture, ref this.buffer, 1f, 23f);
            for (int i = 0; i < 15; ++i)
            {
                Rect rect4 = new Rect(ITab_Lecture.WinSize.x - ITab_Lecture.PasteX, ITab_Lecture.SizeY + (i * 22), WinSize.x/2, 24f);
                ReligionUtility.CheckboxLabeled(rect4, SelLectern, i, ("Day " + (i + 1)).Translate(), SelLectern.daysOfLectures[i], false, null, null, false);            
            }
        }

        //public override void TabUpdate()
        //{
        //    if (this.mouseoverBill == null)
        //        return;
        //    this.mouseoverBill.TryDrawIngredientSearchRadiusOnMap(this.SelLectern.Position);
        //    this.mouseoverBill = (Bill)null;
        //}
    }
}
