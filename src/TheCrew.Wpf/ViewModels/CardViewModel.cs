using System.Windows.Media;
using TheCrew.Shared;
using TheCrew.Wpf.Tools;

namespace TheCrew.Wpf.ViewModels;

internal class CardViewModel : ViewModelBase
{
   private readonly ICardImageSelector _cardImageSelector;
   private bool frontface;

   public CardViewModel(ICard card, ICardImageSelector cardImageSelector, RelayCommand<CardViewModel> cardClickedCommand)
   {
      Card = card;
      _cardImageSelector = cardImageSelector;

      CardClickedCommand = cardClickedCommand;

      //CardSelected
   }

   public ICard Card { get; }

   public RelayCommand<CardViewModel> CardClickedCommand { get; }

   public bool Frontface
   {
      get { return frontface; }
      set
      {
         frontface = value;
         OnPropertyChanged();
         OnPropertyChanged(nameof(Image));
      }
   }

   public ImageSource Image => _cardImageSelector.GetCard(Card, Frontface);
}
