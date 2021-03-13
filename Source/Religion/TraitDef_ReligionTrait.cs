using System.Collections.Generic;
using RimWorld;

namespace Religion
{
    internal class TraitDef_ReligionTrait : TraitDef
    {
        public readonly List<TraitDef> foreignReligions = new();
        public bool isAgressive;
    }
}