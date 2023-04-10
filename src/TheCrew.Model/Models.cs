using TheCrew.Shared;

namespace TheCrew.Model;



public class GameModel
{
   required public Guid Id { get; init; }
   required public List<PlayerModel> Players { get; set; }
   required public List<IMissionCardTask> UnassignedMissionCards { get; set; }
   required public List<IGenericMissionTask> GenericMissions { get; set; }
   public Guid? LastWinnerPlayerId { get; set; } = null;
   public ValueCardSuit? CurrentSuit { get; set; } = null;
}

public class PlayerModel
{
   required public Guid Id { get; init; }
   required public PlayerType Type { get; init; }
   required public string Name { get; set; }
   required public List<IPlayCard> Hand { get; set; }
   required public List<IMissionCardTask> Missions { get; set; }
   required public List<IPlayCard> TakenCards { get; set; }
   public IPlayCard? PlayedCard { get; set; }
   public CommunicationCard? CommunicationCard { get; set; }
   public bool IsCommander { get; set; }
}

public record CommunicationCard(ValueCard ValueCard, CommunicationToken Token);

public enum PlayerType { Console, Ai };