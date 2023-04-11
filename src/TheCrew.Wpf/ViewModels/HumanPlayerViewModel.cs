using System.Threading;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Shared;
using TheCrew.Wpf.Factories;
using TheCrew.Wpf.ViewModels;

namespace TheCrew.Wpf;

internal class HumanPlayerViewModel : PlayerViewModel
{
   public SemaphoreSlim CardPlayedSemaphore { get; init; }

   public HumanPlayerViewModel(GameModel gameModel, PlayerModel player, CardViewModelFactory cardViewModelFactory)
     : base(gameModel, player)
   {
      CardPlayedSemaphore = new(0, 1);

      foreach (var card in player.Hand)
      {
         var cardViewModel = cardViewModelFactory.CreateViewModel(card);
         cardViewModel.CanPlay = _playerModel.CanPlayCard;
         cardViewModel.CardSelected = (_) => PlayCard(cardViewModel);

         cardViewModel.Frontface = true;
         Hand.Add(cardViewModel);
      }
      HandView.Refresh();
   }

   public override async Task StartPlayCard()
   {
      Hand.ForEach(card => card.CardOnHandClickedCommand.Revalidate());
      await CardPlayedSemaphore.WaitAsync();
      Hand.ForEach(card => card.CardOnHandClickedCommand.Revalidate());
   }

   public override void PlayCard(CardViewModel<IPlayCard> cardViewModel)
   {
      base.PlayCard(cardViewModel);

      CardPlayedSemaphore?.Release();
   }
}

