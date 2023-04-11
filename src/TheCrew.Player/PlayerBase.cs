using TheCrew.Model;
using TheCrew.Shared;

namespace TheCrew.Player;

public abstract class PlayerBase
{

   protected ReadOnlyGameModel GameModel;

   protected PlayerBase(PlayerModel playerModel, ReadOnlyGameModel gameModel)
   {
      PlayerModel = playerModel;
      GameModel = gameModel;
   }

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
      return GameModel.CurrentSuit is null ||
         card.Suit == ValueCardSuit.Rocket ||
         card.Suit == GameModel.CurrentSuit;
   }

   protected abstract IPlayCard SelectCardToPlay();
}
