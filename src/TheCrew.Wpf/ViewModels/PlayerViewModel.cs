using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using TheCrew.Model;
using TheCrew.Shared;
using TheCrew.Wpf.Factories;
using TheCrew.Wpf.ViewModels;

namespace TheCrew.Wpf;

internal abstract class PlayerViewModel : ViewModelBase
{
   protected readonly GameModel _gameModel;
   protected readonly PlayerModel _playerModel;
   protected readonly List<CardViewModel<IPlayCard>> Hand = new();

   private CardViewModel<IPlayCard>? _communicatedCard;
   private CommunicationToken _communicationToken = CommunicationToken.None;

   private bool _isCurrent;

   private CardViewModel<IPlayCard>? _playedCard;

   private int _playOrder;
   private List<CardViewModel<IPlayCard>> _takenCards = new();

   protected PlayerViewModel(GameModel gameModel, PlayerModel playerModel)
   {
      _gameModel = gameModel;
      _playerModel = playerModel;

      HandView = new ListCollectionView(Hand);
      HandView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Card.Suit", System.ComponentModel.ListSortDirection.Ascending));
      HandView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Card.Value", System.ComponentModel.ListSortDirection.Ascending));
   }

   public CardViewModel<IPlayCard>? CommunicatedCard
   {
      get { return _communicatedCard; }
      set
      {
         _communicatedCard = value;
         OnPropertyChanged();
      }
   }

   public CommunicationToken CommunicationToken
   {
      get { return _communicationToken; }
      set
      {
         _communicationToken = value;
         OnPropertyChanged();
      }
   }

   public ListCollectionView HandView { get; }

   public Guid Id => _playerModel.Id;

   public bool IsCurrent
   {
      get { return _isCurrent; }
      set
      {
         _isCurrent = value;
         OnPropertyChanged();
      }
   }

   private bool _isCurrentWinner;

   public bool IsCurrentWinner
   {
      get { return _isCurrentWinner; }
      set
      {
         _isCurrentWinner = value;
         OnPropertyChanged();
      }
   }

   public CardViewModel<IPlayCard>? PlayedCard
   {
      get { return _playedCard; }
      private set
      {
         _playedCard = value;
         OnPropertyChanged();
      }
   }

   public int PlayOrder
   {
      get => _playOrder;
      set
      {
         _playOrder = value;
         OnPropertyChanged(nameof(PlayOrder));
      }
   }

   public virtual void PlayCard(CardViewModel<IPlayCard> cardViewModel)
   {
      var cardFromHand = Hand.Where(x => x.Card.Equals(cardViewModel.Card)).First();

      App.Current.Dispatcher.Invoke(() =>
      {
         if (Hand.Remove(cardFromHand))
         {
            cardViewModel.Frontface = true;
            PlayedCard = cardViewModel;
         }

         HandView.Refresh();
      });

      _playerModel.PlayCard(cardViewModel.Card);

      if (_gameModel.CurrentSuit is null && cardViewModel.Card is IPlayCard playCard)
      {
         _gameModel.CurrentSuit = playCard.Suit;
      }
   }

   public abstract Task StartPlayCard();

   public CardViewModel<IPlayCard> TakePlayedCard()
   {
      try
      {
         return PlayedCard ?? throw new UnreachableException();
      }
      finally
      {
         PlayedCard = null;
         _playerModel.PlayedCard = null;
      }
   }
   public void TakeStick(IEnumerable<CardViewModel<IPlayCard>> cards)
   {
      foreach (var card in cards)
      {
         _takenCards.Add(card);

         // todo: flagga uppdrag om slutförda
      }
   }

   //protected void OnHandUpdate()
   //{
   //   _hand.Sort(new HandSorter());

   //   OnPropertyChanged(nameof(Hand));
   //}

   //private class HandSorter : IComparer<CardViewModel<IPlayCard>>
   //{
   //   public int Compare(CardViewModel<IPlayCard>? x, CardViewModel<IPlayCard>? y)
   //   {
   //      return Compare(x!.Card, y!.Card);
   //   }

   //   public static int Compare(IPlayCard x, IPlayCard y)
   //   {
   //      if(x.Suit == y.Suit)
   //      {
   //         return x.Value.CompareTo(y.Value);
   //      }

   //      if (x.Suit == ValueCardSuit.Rocket)
   //      {
   //         return -1;
   //      }
         
   //      if (y.Suit == ValueCardSuit.Rocket)
   //      {
   //         return 1;
   //      }

   //      return x.Suit.CompareTo(y.Suit);
   //   }
   //}
}

