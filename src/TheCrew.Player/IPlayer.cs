using TheCrew.Shared;

namespace TheCrew.Player;

public interface IAiPlayer
{
   Task<ICard> PlayCard();
}

public interface IPlayer
{
   Guid ID { get; }
   bool IsCommander { get; }
   string Name { get; }
   IPlayCard? PlayedCard { get; }

   Task PlayCard();
   Task<IMissionCardTask> SelectMissionCard();
}
