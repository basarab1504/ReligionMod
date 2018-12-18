﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;

namespace Religion
{
    public interface IAssignableTrait
    {
        IEnumerable<TraitDef> AssigningTrait { get; }

        IEnumerable<TraitDef> AssignedTraits { get; }

        int MaxAssignedTraitsCount { get; }

        void TryAssignTrait(TraitDef trait);

        void TryUnassignTrait(TraitDef trait);

        bool AssignedAnything(TraitDef trait);
    }
}