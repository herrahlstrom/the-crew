using System.Linq;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Wpf.Factories;

namespace TheCrew.Wpf;

internal class AiPlayerViewModel : PlayerViewModel
{
   private IAiPlayer _aiPlayer;

   public AiPlayerViewModel(IAiPlayer aiPlayer, GameModel gameModel, PlayerModel player, CardViewModelFactory cardViewModelFactory)
     : base(gameModel, player)
   {
      _aiPlayer = aiPlayer;
      
      Hand.AddRange(player.Hand.Select(cardViewModelFactory.CreateViewModel));
      HandView.Refresh();
   }

   public override Task StartPlayCard()
   {
      var card = _aiPlayer.SelectCardToPlay();
      var cardViewModel = Hand.Where(x => x.Card.Equals(card)).First();
      PlayCard(cardViewModel);

      return Task.CompletedTask;
   }
}

