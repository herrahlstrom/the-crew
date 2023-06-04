using TheCrew.Shared;

namespace TheCrew.Model;

public class PlayerModel
{
   public PlayerModel(GameModel game)
   {
      Game = game;

      Id = Guid.NewGuid();
      Hand = new List<IPlayCard>();
      Missions = new List<IMissionTaskCard>();
      TakenCards = new List<IPlayCard>();
   }
   public GameModel Game { get; }
   public Guid Id { get; }
   required public PlayerType Type { get; init; }
   required public string Name { get; set; }
   public List<IPlayCard> Hand { get; }
   public List<IMissionTaskCard> Missions { get; }
   public List<IPlayCard> TakenCards { get; }
   public IPlayCard? PlayedCard { get; set; }
   public CommunicationCard? CommunicationCard { get; set; }
   public bool IsCommander { get; set; }
}

public enum PlayerType { Human, Ai };
