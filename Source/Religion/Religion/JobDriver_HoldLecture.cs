// ----------------------------------------------------------------------
// These are basic usings. Always let them be here.
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// ----------------------------------------------------------------------
// These are RimWorld-specific usings. Activate/Deactivate what you need:
// ----------------------------------------------------------------------
using UnityEngine;         // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.AI;          // Needed when you do something with the AI
using Verse.AI.Group;
using Verse.Sound;       // Needed when you do something with Sound
using Verse.Noise;       // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;   // RimWorld specific functions for world creation
namespace Religion
{
    class JobDriver_HoldLecture : JobDriver
    {
        protected Building_Lectern lectern => (Building_Lectern)base.job.GetTarget(TargetIndex.A).Thing;
        Thing WorshipCaller = null;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, this.job.def.joyMaxParticipants, 0, (ReservationLayerDef)null, true);
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
            //this.FailOnDestroyedOrNull(TargetIndex.A);

            Toil goToAltar = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            yield return new Toil
            {
                initAction = delegate
                {
                    Predicate<Thing> validator = (x => x.TryGetComp<ThingComp_ReligionComp>() != null);
                    Thing worshipCaller = GenClosest.ClosestThingReachable(lectern.Position, lectern.Map,
                        ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.ClosestTouch,
                        TraverseParms.For(this.pawn, Danger.None, TraverseMode.ByPawn), 9999, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                    if (worshipCaller != null)
                    {
                        WorshipCaller = worshipCaller;
                        this.job.SetTarget(TargetIndex.B, worshipCaller);
                    }
                    else
                        base.JumpToToil(goToAltar);
                }
            };

            Toil call = new Toil();
            call.initAction = delegate
            {
                lectern.TryGetComp<ThingComp_ReligionComp>().Use();
            };
            yield return call;

            yield return goToAltar;

            Toil waitingTime = new Toil();
            waitingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            waitingTime.defaultDuration = 740;
            waitingTime.initAction = delegate
            {
                report = "Waiting for congregation".Translate();
            };
            yield return waitingTime;

            Toil preachingTime = new Toil();
            preachingTime.initAction = delegate
            {
                report = "Read a prayer".Translate();
            };
            preachingTime.defaultCompleteMode = ToilCompleteMode.Delay;
            preachingTime.defaultDuration = 600;
            preachingTime.tickAction = delegate
            {
                Pawn actor = this.pawn;
                actor.skills.Learn(SkillDefOf.Social, 0.25f);
            };
            yield return preachingTime;

            this.AddFinishAction(() =>
            {
                //When the ritual is finished -- then let's give the thoughts
                ReligionUtility.isMorningLectureDone = true;
                pawn.ClearAllReservations();
            });
            yield break;
        }
    }
}
