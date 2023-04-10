using TheCrew.Model;
using TheCrew.Shared;

namespace TheCrew.Player;

public abstract class PlayerBase
{
   protected PlayerBase(PlayerModel playerModel, IGameState gameState)
   {
      PlayerModel = playerModel;
      GameState = gameState;
   }

   protected IGameState GameState;
   public Guid ID => PlayerModel.Id;
   public string Name => PlayerModel.Name;
   public IPlayCard? PlayedCard => PlayerModel.PlayedCard;
   protected PlayerModel PlayerModel { get; }

   public Task PlayCard()
   {
      var card = SelectCardToPlay();
      PlayerModel.PlayedCard = card;
      PlayerModel.Hand.Remove(card);
      return Task.CompletedTask;
   }

   protected bool IsFollowingSuit(IPlayCard card)
   {
      return GameState.CurrentSuit is null ||
         card.Suit == ValueCardSuit.Rocket ||
         card.Suit == GameState.CurrentSuit;
   }

   protected abstract IPlayCard SelectCardToPlay();
}
