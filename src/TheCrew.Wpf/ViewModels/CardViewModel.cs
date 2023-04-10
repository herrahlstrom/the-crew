using System.Windows.Media;
using TheCrew.Shared;
using TheCrew.Wpf.Tools;

namespace TheCrew.Wpf.ViewModels;

internal class CardViewModel : ViewModelBase
{
    private bool frontface;
    private readonly ICardImageSelector _cardImageSelector;

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

    public ICard Card { get; }

    public ImageSource Image => _cardImageSelector.GetCard(Card, Frontface);

    public CardViewModel(ICard card, ICardImageSelector cardImageSelector, RelayCommand<CardViewModel> cardClickedCommand)
    {
        Card = card;
        _cardImageSelector = cardImageSelector;

        CardClickedCommand = cardClickedCommand;

        //CardSelected
    }

    public RelayCommand<CardViewModel> CardClickedCommand { get; }
}
