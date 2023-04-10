using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Player.AI;
using TheCrew.Player.Human;
using TheCrew.Shared;
using TheCrew.Wpf.Tools;
using TheCrew.Wpf.ViewModels;

namespace TheCrew.Wpf;

class CardSelectedEventArgs : EventArgs { public CardViewModel Card { get; set; } }

delegate bool CanPlayDelegate(CardViewModel card, IReadOnlyCollection<CardViewModel> hand);

internal class PlayerViewModel : ViewModelBase
{
   public SemaphoreSlim? CardPlayedSemaphore { get; init; }
   
   public static PlayerViewModel CreateAi(GameModel gameModel, PlayerModel model, CanPlayDelegate canPlayPredicate)
   {
      return new PlayerViewModel(new DummyAiPlayer(model, gameModel), gameModel, model, canPlayPredicate);
   }
   public static PlayerViewModel CreateHuman(GameModel gameModel, PlayerModel model, CanPlayDelegate canPlayPredicate)
   {
      return new PlayerViewModel(new WpfPlayer(model, gameModel), gameModel, model, canPlayPredicate)
      {
         CardPlayedSemaphore = new(0, 1)
      };
   }

   public PlayerViewModel(IPlayer player, GameModel gameModel, PlayerModel model, CanPlayDelegate canPlayPredicate)
   {
      Id = player.ID;
      _gameModel = gameModel;
      _player = player;
      _model = model;

      SelectCardCommand = new RelayCommand<CardViewModel>(
         card => canPlayPredicate.Invoke(card, Hand),
         card => CardSelected?.Invoke(this, new CardSelectedEventArgs() { Card = card }));
   }

   public event EventHandler<CardSelectedEventArgs> CardSelected = null!;

   public Guid Id { get; }
   private List<CardViewModel> _takenCards = new();

   private int _playOrder;

   public int PlayOrder
   {
      get => _playOrder;
      set
      {
         _playOrder = value;
         OnPropertyChanged(nameof(PlayOrder));
      }
   }

   private CardViewModel? _communicatedCard;
   public CardViewModel? CommunicatedCard
   {
      get { return _communicatedCard; }
      set
      {
         _communicatedCard = value;
         OnPropertyChanged();
      }
   }
   public RelayCommand<CardViewModel> SelectCardCommand { get; }
   private CommunicationToken _communicationToken = CommunicationToken.None;

   public CommunicationToken CommunicationToken
   {
      get { return _communicationToken; }
      set
      {
         _communicationToken = value;
         OnPropertyChanged();
      }
   }

   public ObservableCollection<CardViewModel> Hand { get; } = new();

   private CardViewModel? _playedCard;
   public CardViewModel? PlayedCard
   {
      get { return _playedCard; }
      private set
      {
         _playedCard = value;
         OnPropertyChanged();
      }
   }

   public async Task StartPlayCard()
   {
      if (_player is IAiPlayer iAiPlayer)
      {
         var card = await iAiPlayer.PlayCard();
         await Task.Delay(200);
         var cardViewModel = Hand.Where(x => x.Card.Equals(card)).First();
         PlayCard(cardViewModel);
      }
      else if (CardPlayedSemaphore is not null)
      {
         SelectCardCommand.Revalidate();
         await CardPlayedSemaphore.WaitAsync();
      }
      else
      {
         throw new NotSupportedException();
      }
   }
   public void TakeStick(IEnumerable<CardViewModel> cards)
   {
      foreach (var card in cards)
      {
         _takenCards.Add(card);

         // todo: flagga uppdrag om slutförda
      }
   }
   public void PlayCard(CardViewModel cardViewModel)
   {
      var cardFromHand = Hand.Where(x => x.Card.Equals(cardViewModel.Card)).First();

      App.Current.Dispatcher.Invoke(() =>
      {
         if (Hand.Remove(cardFromHand))
         {
            cardViewModel.Frontface = true;
            PlayedCard = cardViewModel;
         }
      });

      _model.PlayCard(cardViewModel.Card);

      if(_gameModel.CurrentSuit is null && cardViewModel.Card is IPlayCard playCard)
      {
         _gameModel.CurrentSuit = playCard.Suit;
      }

      CardPlayedSemaphore?.Release();
   }

   public CardViewModel TakePlayedCard()
   {
      try
      {
         return PlayedCard ?? throw new UnreachableException();
      }
      finally
      {
         PlayedCard = null;
      }
   }

   private bool _isCurrent;
   private readonly IPlayer _player;
   private readonly PlayerModel _model;
   private readonly GameModel _gameModel;

   public bool IsCurrent
   {
      get { return _isCurrent; }
      set
      {
         _isCurrent = value;
         OnPropertyChanged();
      }
   }

   public void AddCardsToHand(IEnumerable<ICard> cards, ICardImageSelector cardImageSelector, bool frontface)
   {
      foreach (var card in cards)
      {
         Hand.Add(new CardViewModel(card, cardImageSelector, SelectCardCommand)
         {
            Frontface = frontface,
         });
      }
   }
}
