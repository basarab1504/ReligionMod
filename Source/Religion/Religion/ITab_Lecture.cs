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
            Rect rect2 = new Rect(ITab_Lecture.WinSize.x - ITab_Lecture.PasteX, ITab_Lecture.PasteY + 30, 34f, 34f);
            Widgets.TextFieldNumeric<int>(rect2, ref SelLectern.timeOfLecture, ref this.buffer);

            Rect rect4 = new Rect(0f, ITab_Lecture.PasteY + 60, WinSize.x, 24f);
            Widgets.CheckboxLabeled(rect4, "Monday".Translate(), ref SelLectern.one, false, null, null, false);
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
