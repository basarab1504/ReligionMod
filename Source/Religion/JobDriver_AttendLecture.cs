using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace Religion
{
    internal class JobDriver_AttendWorship : JobDriver
    {
        private readonly string report = "";

        private Building_Lectern Lectern => (Building_Lectern) job.GetTarget(TargetIndex.A).Thing;
        private Pawn Preacher => Lectern.CompAssignableToPawn.AssignedPawnsForReading[0];

        public override string GetReport()
        {
            if (report != "")
            {
                return base.ReportStringProcessed(report);
            }

            return base.GetReport();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn = this.pawn;
            var target = this.job.targetA;
            var job = this.job;
            var num = this.job.def.joyMaxParticipants;
            var num2 = 0;
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

            if (!TargetC.HasThing)
            {
                return true;
            }

            if (TargetC.Thing is Building_Bed bed)
            {
                pawn = this.pawn;
                var targetC = this.job.targetC;
                job = this.job;
                num2 = bed.SleepingSlotsCount;
                num = 0;
                if (!pawn.Reserve(targetC, job, num2, num, null, errorOnFailed))
                {
                    return false;
                }
            }
            else
            {
                pawn = this.pawn;
                var targetC = this.job.targetC;
                job = this.job;
                if (!pawn.Reserve(targetC, job, 1, -1, null, errorOnFailed))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            return TargetC.HasThing && TargetC.Thing is Building_Bed &&
                   JobInBedUtility.InBedOrRestSpotNow(pawn, TargetC);
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.EndOnDespawnedOrNull(TargetIndex.A);

            var goToChurch = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            yield return goToChurch;

            var watch = new Toil();
            watch.AddPreTickAction(() =>
            {
                pawn.rotationTracker.FaceCell(TargetA.Thing.OccupiedRect().ClosestCellTo(pawn.Position));
                pawn.GainComfortFromCellIfPossible();
                pawn.rotationTracker.FaceCell(TargetB.Cell);
                if (Preacher.CurJob.def != ReligionDefOf.HoldWorship)
                {
                    ReadyForNextToil();
                }
            });
            watch.defaultCompleteMode = ToilCompleteMode.Delay;
            watch.defaultDuration = job.def.joyDuration;
            watch.handlingFacing = true;
            yield return watch;

            yield return Toils_Jump.JumpIf(watch, () => Preacher.CurJob.def == ReligionDefOf.HoldWorship);

            AddFinishAction(() =>
            {
                if (pawn.Position != TargetC.Cell)
                {
                    return;
                }

                ReligionUtility.TryGainTempleRoomThought(pawn);
                ReligionUtility.AttendedWorshipThought(pawn, Preacher);
                ReligionUtility.TryAddAddiction(pawn, Preacher);
            });
        }
    }
}