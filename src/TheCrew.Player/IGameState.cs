﻿using TheCrew.Shared;

namespace TheCrew.Player;

public interface IGameState
{
   ValueCardSuit? CurrentSuit { get; }
   IReadOnlyCollection<IMissionTaskCard> UnassignedMissionCards { get; }
}