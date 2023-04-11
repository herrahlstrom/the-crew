using TheCrew.Shared;

namespace TheCrew.Player;

public interface IGameAwareness
{
    Predicate<IPlayCard> CanPlayPredicate { get; }
    Func<IEnumerable<IMissionTaskCard>> UnassignedMissionCards { get; }
    Func<IEnumerable<IPlayCard>> Hand { get; }
}
