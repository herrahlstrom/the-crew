using TheCrew.Shared;

namespace TheCrew.Player;

public interface IAiPlayer
{
   Task<ICard> PlayCard();
}

public interface IPlayer
{
   Task<IMissionCardTask> SelectMissionCard();
   Task PlayCard();
   IPlayCard? PlayedCard { get; }
   Guid ID { get; }
   string Name { get; }
   bool IsCommander { get; }
}
