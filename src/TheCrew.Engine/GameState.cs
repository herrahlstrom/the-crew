using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Shared;

namespace TheCrew.Engine;

public class GameState : IGameState
{
   private readonly GameModel _gameModel;

   public GameState(GameModel gameModel)
   {
      _gameModel = gameModel;
   }

   public ValueCardSuit? CurrentSuit => _gameModel.CurrentSuit;

   public IReadOnlyCollection<IMissionCardTask> UnassignedMissionCards => _gameModel.UnassignedMissionCards;
}