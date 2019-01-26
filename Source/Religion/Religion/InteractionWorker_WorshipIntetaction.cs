using Verse;
using RimWorld;

namespace Religion
{
    class InteractionWorker_WorshipIntetaction : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return 0f;
        }
    }
}
