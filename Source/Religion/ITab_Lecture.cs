using RimWorld;
using UnityEngine;
using Verse;

namespace Religion //крыса
{
    public class ITab_Worship : ITab
    {
        private static readonly Vector2 WinSize = new(420f, 480f);

        public ITab_Worship()
        {
            size = WinSize;
            labelKey = "TabWorship".Translate();
            //this.tutorTag = "Worships".Translate();
        }

        private Building_Lectern SelLectern => (Building_Lectern) SelThing;

        protected override void FillTab()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
            ReligionCardUtility.DrawCard(SelLectern);
        }
    }
}