using Verse;

namespace Religion
{
    internal class RoomRoleWorker_Church : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            var num = 0;
            var andAdjacentThings = room.ContainedAndAdjacentThings;
            foreach (var thing in andAdjacentThings)
            {
                if (thing is Building_Altar)
                {
                    ++num;
                }

                if (thing is Building_Lectern)
                {
                    ++num;
                }
            }

            return num * 7.6f;
        }
    }
}