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
        private static float PasteX = 48f;
        [TweakValue("Interface", 0.0f, 128f)]
        private static float PasteY = 3f;
        [TweakValue("Interface", 0.0f, 32f)]
        private static float PasteSize = 24f;
        private float viewHeight = 1000f;
        private Vector2 scrollPosition = new Vector2();
        private Bill mouseoverBill;

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
            Rect rect1 = new Rect(ITab_Lecture.WinSize.x - ITab_Lecture.PasteX, ITab_Lecture.PasteY, ITab_Lecture.PasteSize, ITab_Lecture.PasteSize);
            if (BillUtility.Clipboard == null)
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect1, (Texture)ReligionUtility.Paste, 1f);
                GUI.color = Color.white;
                TooltipHandler.TipRegion(rect1, (TipSignal)"PasteBillTip".Translate());
            }
            else if (!this.SelLectern.def.AllRecipes.Contains(BillUtility.Clipboard.recipe) || !BillUtility.Clipboard.recipe.AvailableNow)
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect1, (Texture)ReligionUtility.Paste, 1f);
                GUI.color = Color.white;
                TooltipHandler.TipRegion(rect1, (TipSignal)"ClipboardBillNotAvailableHere".Translate());
            }
            else if (this.SelLectern.billStack.Count >= 15)
            {
                GUI.color = Color.gray;
                Widgets.DrawTextureFitted(rect1, (Texture)ReligionUtility.Paste, 1f);
                GUI.color = Color.white;
                TooltipHandler.TipRegion(rect1, (TipSignal)("PasteBillTip".Translate() + " (" + "PasteBillTip_LimitReached".Translate() + ")"));
            }
            else
            {
                if (Widgets.ButtonImageFitted(rect1, ReligionUtility.Paste, Color.white))
                {
                    Bill bill = BillUtility.Clipboard.Clone();
                    bill.InitializeAfterClone();
                    this.SelLectern.billStack.AddBill(bill);
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera((Map)null);
                }
                TooltipHandler.TipRegion(rect1, (TipSignal)"PasteBillTip".Translate());
            }
            this.mouseoverBill = this.SelLectern.billStack.DoListing(new Rect(0.0f, 0.0f, ITab_Lecture.WinSize.x, ITab_Lecture.WinSize.y).ContractedBy(10f), (Func<List<FloatMenuOption>>)(() =>
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                for (int index = 0; index < this.SelLectern.def.AllRecipes.Count; ++index)
                {
                    if (this.SelLectern.def.AllRecipes[index].AvailableNow)
                    {
                        RecipeDef recipe = this.SelLectern.def.AllRecipes[index];
                        list.Add(new FloatMenuOption(recipe.LabelCap, (Action)(() =>
                        {
                            if (!this.SelLectern.Map.mapPawns.FreeColonists.Any<Pawn>((Func<Pawn, bool>)(col => recipe.PawnSatisfiesSkillRequirements(col))))
                                Bill.CreateNoPawnsWithSkillDialog(recipe);
                            this.SelLectern.billStack.AddBill(recipe.MakeNewBill());
                            if (recipe.conceptLearned != null)
                                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
                            if (!TutorSystem.TutorialMode)
                                return;
                            TutorSystem.Notify_Event((EventPack)("AddBill-" + recipe.LabelCap));
                        }), MenuOptionPriority.Default, (Action)null, (Thing)null, 29f, (Func<Rect, bool>)(rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + (float)(((double)rect.height - 24.0) / 2.0), (Def)recipe)), (WorldObject)null));
                    }
                }
                if (!list.Any<FloatMenuOption>())
                    list.Add(new FloatMenuOption("NoneBrackets".Translate(), (Action)null, MenuOptionPriority.Default, (Action)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null));
                return list;
            }), ref this.scrollPosition, ref this.viewHeight);
        }

        public override void TabUpdate()
        {
            if (this.mouseoverBill == null)
                return;
            this.mouseoverBill.TryDrawIngredientSearchRadiusOnMap(this.SelLectern.Position);
            this.mouseoverBill = (Bill)null;
        }
    }
}
