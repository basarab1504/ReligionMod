using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_ReadBook : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.job.targetA;
            Job job = this.job;
            int num = this.job.def.joyMaxParticipants;
            int num2 = 0;
            if (!pawn.Reserve(target, job, num, num2, null, errorOnFailed))
            {
                return false;
            }
            pawn = this.pawn;
            target = this.job.targetB;
            job = this.job;
            if (!pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            if (base.TargetC.HasThing)
            {
                if (base.TargetC.Thing is Building_Bed)
                {
                    pawn = this.pawn;
                    LocalTargetInfo targetC = this.job.targetC;
                    job = this.job;
                    num2 = ((Building_Bed)base.TargetC.Thing).SleepingSlotsCount;
                    num = 0;
                    if (!pawn.Reserve(targetC, job, num2, num, null, errorOnFailed))
                    {
                        return false;
                    }
                }
                else
                {
                    pawn = this.pawn;
                    LocalTargetInfo targetC = this.job.targetC;
                    job = this.job;
                    if (!pawn.Reserve(targetC, job, 1, -1, null, errorOnFailed))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        ReligionBook_ThingDef book
        {
            get
            {
                return TargetC.Thing.def as ReligionBook_ThingDef;
            }
        }

        private string report = "";
        public override string GetReport()
        {
            if (report != "")
            {
                return base.ReportStringProcessed(report);
            }
            return base.GetReport();
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.C);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            Toil reading = new Toil();
            reading.defaultCompleteMode = ToilCompleteMode.Delay;
            reading.defaultDuration = 1100;
            reading.WithProgressBarToilDelay(TargetIndex.B, false, -0.5f);
            reading.initAction = delegate
            {
                report = "Reading".Translate();
            };
            reading.AddFinishAction(() =>
            {
                if(!pawn.story.traits.allTraits.Any(x => x.def is TraitDef_ReligionTrait) && !pawn.story.traits.HasTrait(ReligionDefOf.Atheist))
                {
                    pawn.story.traits.GainTrait(new Trait(book.religion));
                    Find.LetterStack.ReceiveLetter(pawn.ToString() + " in faith".Translate(), "Is now religious".Translate(), LetterDefOf.PositiveEvent, (LookTargets)((Thing)this.pawn), (Faction)null, (string)null);
                }
            });
            yield return reading;

            Toil toStoreToil = new Toil();
            toStoreToil.initAction = delegate
            {
                Pawn actor = toStoreToil.actor;
                Job curJob = actor.jobs.curJob;
                IntVec3 foundCell = IntVec3.Invalid;
                StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map, StoragePriority.Important, Faction.OfPlayer, out foundCell, true);
                if (!foundCell.IsValid)
                    StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map, StoragePriority.Unstored, Faction.OfPlayer, out foundCell, true);
                actor.carryTracker.TryStartCarry(TargetB.Thing);
                if (foundCell.IsValid)
                    curJob.targetC = (LocalTargetInfo)foundCell;
            };
            yield return toStoreToil;

            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, Toils_Haul.TakeToInventory(TargetIndex.C, 1), false);
            yield break;
        }
    }
}
