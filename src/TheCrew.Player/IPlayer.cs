using TheCrew.Shared;

namespace TheCrew.Player;

public interface IAiPlayer
{
   IMissionTaskCard SelectMissionCard();
   IPlayCard SelectCardToPlay();
}

public interface IPlayer
{
   Guid ID { get; }

}
