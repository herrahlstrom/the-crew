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

public record CommunicationCard(ValueCard ValueCard, CommunicationToken Token);
