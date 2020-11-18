using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace Religion
{
    class JobDriver_HoldWorship : JobDriver
    {
        protected Building_Lectern Lectern
        {
            get
            {
                return (Building_Lectern)job.GetTarget(TargetIndex.A).Thing;
            }
        }

        protected ThingWithComps_Book Book
        {
            get
            {
                return (ThingWithComps_Book)job.GetTarget(TargetIndex.B).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
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
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);

            if(!pawn.inventory.Contains(TargetThingB))
            {
                yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
                yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            }
            else
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.B);

            Toil goToAltar = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return goToAltar;

            Toil waitingTime = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 740
            };
            waitingTime.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            waitingTime.initAction = delegate
            {
                report = "WaitingForCongregation".Translate();
                ReligionUtility.Listeners(Lectern, Lectern.listeners);
                foreach (Pawn p in Lectern.listeners)
                    ReligionUtility.GiveAttendJob(Lectern, p);
            };
            yield return waitingTime;

            Toil preachingTime = new Toil
            {
                initAction = delegate
                {
                    report = "ReadAPrayer".Translate();
                //MoteMaker.MakeInteractionBubble(this.pawn, null, ThingDefOf.Mote_Speech, ReligionUtility.faith);
                InteractionDef intDef = ReligionDefOf.WorshipInteraction;
                    foreach (Pawn p in Lectern.listeners)
                        ReligionUtility.TryWorshipInteraction(pawn, p, intDef);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 1800
            };
            preachingTime.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            preachingTime.tickAction = delegate
            {
                Pawn actor = pawn;
                actor.skills.Learn(SkillDefOf.Social, 0.25f);
            };
            yield return preachingTime;
            

            Toil toStoreToil = new Toil();
            toStoreToil.initAction = delegate
            {
                Pawn actor = toStoreToil.actor;
                Job curJob = actor.jobs.curJob;
                IntVec3 foundCell = IntVec3.Invalid;
                StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map, StoragePriority.Important, Faction.OfPlayer, out foundCell, true);
                //if(!foundCell.IsValid)
                //    StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map, StoragePriority.Unstored, Faction.OfPlayer, out foundCell, true);
                //actor.carryTracker.TryStartCarry(TargetB.Thing);
                if (foundCell.IsValid)
                    curJob.targetC = foundCell;
                else
                    curJob.targetC = Lectern.Position;
                foreach (Pawn p in Lectern.listeners)
                    p.jobs.EndCurrentJob(JobCondition.Succeeded);
            };
            yield return toStoreToil;

            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, Toils_Haul.TakeToInventory(TargetIndex.C,1), false);

            AddFinishAction(() =>
            {
                ReligionUtility.HeldWorshipThought(pawn);
                ReligionUtility.TryAddAddictionForPreacher(pawn);
            });
            yield break;
        }
    }
}
