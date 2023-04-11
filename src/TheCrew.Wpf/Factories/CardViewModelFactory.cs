using TheCrew.Shared;
using TheCrew.Wpf.ViewModels;

namespace TheCrew.Wpf.Factories;

internal class CardViewModelFactory
{
    readonly ICardImageSelector _cardImageSelector;

    public CardViewModelFactory(ICardImageSelector cardImageSelector)
    {
        _cardImageSelector = cardImageSelector;
    }

   public CardViewModel<IPlayCard> CreateViewModel(IPlayCard playCard)
   {
      return new CardViewModel<IPlayCard>(playCard, _cardImageSelector);
   }
}
