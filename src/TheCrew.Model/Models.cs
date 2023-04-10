using System.Collections.ObjectModel;
using TheCrew.Shared;

namespace TheCrew.Model;

public interface ReadOnlyGameModel
{
   ValueCardSuit? CurrentSuit { get; }
   IReadOnlyList<IMissionCardTask> UnassignedMissionCards { get; }
}
public class GameModel : ReadOnlyGameModel
{
   public GameModel()
   {
      Id = Guid.NewGuid();
      Players = new List<PlayerModel>();
      UnassignedMissionCards = new List<IMissionCardTask>();
      GenericMissions = new List<IGenericMissionTask>();
   }
   public Guid Id { get; }
   public List<PlayerModel> Players { get; }
   public List<IMissionCardTask> UnassignedMissionCards { get; }
   public List<IGenericMissionTask> GenericMissions { get; }
   public Guid? LastWinnerPlayerId { get; set; } = null;
   public ValueCardSuit? CurrentSuit { get; set; } = null;

   IReadOnlyList<IMissionCardTask> ReadOnlyGameModel.UnassignedMissionCards => UnassignedMissionCards;
}

public class PlayerModel
{
   public PlayerModel()
   {
      Id = Guid.NewGuid();
      Hand = new List<IPlayCard>();
      Missions = new List<IMissionCardTask>();
      TakenCards = new List<IPlayCard>();
   }
   public Guid Id { get; }
   required public PlayerType Type { get; init; }
   required public string Name { get; set; }
   public List<IPlayCard> Hand { get; }
   public List<IMissionCardTask> Missions { get; }
   public List<IPlayCard> TakenCards { get; }
   public IPlayCard? PlayedCard { get; set; }
   public CommunicationCard? CommunicationCard { get; set; }
   public bool IsCommander { get; set; }
}

public record CommunicationCard(ValueCard ValueCard, CommunicationToken Token);

public enum PlayerType { Human, Ai };