using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class JobDriver_HoldWorship : JobDriver
    {
        private string report = "";
        private Building_Lectern Lectern => (Building_Lectern) job.GetTarget(TargetIndex.A).Thing;

        protected ThingWithComps_Book Book => (ThingWithComps_Book) job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
        }

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

            if (!pawn.inventory.Contains(TargetThingB))
            {
                yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
                yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            }
            else
            {
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.B);
            }

            var goToAltar = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return goToAltar;

            var waitingTime = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 740
            };
            waitingTime.WithProgressBarToilDelay(TargetIndex.A);
            waitingTime.initAction = delegate
            {
                report = "WaitingForCongregation".Translate();
                ReligionUtility.Listeners(Lectern, Lectern.listeners);
                foreach (var p in Lectern.listeners)
                {
                    ReligionUtility.GiveAttendJob(Lectern, p);
                }
            };
            yield return waitingTime;

            var preachingTime = new Toil
            {
                initAction = delegate
                {
                    report = "ReadAPrayer".Translate();
                    //MoteMaker.MakeInteractionBubble(this.pawn, null, ThingDefOf.Mote_Speech, ReligionUtility.faith);
                    var intDef = ReligionDefOf.WorshipInteraction;
                    foreach (var p in Lectern.listeners)
                    {
                        ReligionUtility.TryWorshipInteraction(pawn, p, intDef);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 1800
            };
            preachingTime.WithProgressBarToilDelay(TargetIndex.A);
            preachingTime.tickAction = delegate
            {
                var actor = pawn;
                actor.skills.Learn(SkillDefOf.Social, 0.25f);
            };
            yield return preachingTime;


            var toStoreToil = new Toil();
            toStoreToil.initAction = delegate
            {
                var actor = toStoreToil.actor;
                var curJob = actor.jobs.curJob;
                StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map,
                    StoragePriority.Important, Faction.OfPlayer, out var foundCell);
                //if(!foundCell.IsValid)
                //    StoreUtility.TryFindBestBetterStoreCellFor(curJob.targetB.Thing, actor, actor.Map, StoragePriority.Unstored, Faction.OfPlayer, out foundCell, true);
                //actor.carryTracker.TryStartCarry(TargetB.Thing);
                curJob.targetC = foundCell.IsValid ? foundCell : Lectern.Position;
                foreach (var p in Lectern.listeners)
                {
                    p.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
            };
            yield return toStoreToil;

            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, Toils_Haul.TakeToInventory(TargetIndex.C, 1),
                false);

            AddFinishAction(() =>
            {
                ReligionUtility.HeldWorshipThought(pawn);
                ReligionUtility.TryAddAddictionForPreacher(pawn);
            });
        }
    }
}