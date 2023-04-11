using System;
using System.Windows.Media;
using TheCrew.Shared;
using TheCrew.Wpf.Tools;

namespace TheCrew.Wpf.ViewModels;

internal class CardViewModel<TCard> : ViewModelBase where TCard : ICard
{
   private readonly ICardImageSelector _cardImageSelector;
   private bool frontface = false;

   public CardViewModel(TCard card, ICardImageSelector cardImageSelector)
   {
      Card = card;
      _cardImageSelector = cardImageSelector;

      CardOnHandClickedCommand = new RelayCommand(
         predicate: () => CanPlay.Invoke(Card),
         execute: () => CardSelected.Invoke(Card));
   }

   public Predicate<TCard> CanPlay { get; set; } = (_) => false;
   public Action<TCard> CardSelected { get; set; } = (_) => { };

   public TCard Card { get; }

   public RelayCommand CardOnHandClickedCommand { get; }

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
