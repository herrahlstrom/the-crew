using TheCrew.Shared;

namespace TheCrew.Model;

public class GameModel
{
   public GameModel()
   {
      Players = new List<PlayerModel>();
      UnassignedMissionCards = new List<IMissionTaskCard>();
      GenericMissions = new List<IGenericMissionTask>();
   }
   public List<PlayerModel> Players { get; }
   public List<IMissionTaskCard> UnassignedMissionCards { get; }
   public List<IGenericMissionTask> GenericMissions { get; }
   public Guid? LastWinnerPlayerId { get; set; } = null;
   public Guid CurrentPlayer { get; set; } = Guid.Empty;
   public ValueCardSuit? CurrentSuit { get; set; } = null;

   public void Clear()
   {
      UnassignedMissionCards.Clear();
      GenericMissions.Clear();
      LastWinnerPlayerId = null;
      CurrentSuit = null;
      CurrentPlayer = Guid.Empty;

      Players.Clear();
   }
}

public record CommunicationCard(IPlayCard ValueCard, CommunicationToken Token);
