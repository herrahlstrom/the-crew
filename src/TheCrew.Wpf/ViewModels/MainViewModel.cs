using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Player.AI;
using TheCrew.Player.Human;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;
using TheCrew.Wpf.Tools;
using TheCrew.Wpf.ViewModels;

namespace TheCrew.Wpf;
internal class MainViewModel : ViewModelBase
{
   readonly GameModel _game;
   readonly ICardImageSelector _cardImageSelector;
   readonly GameEngine _engine;
   readonly Task _engineTask;

   public MainViewModel(GameModel game, ICardImageSelector cardImageSelector, CancellationToken cancellationToken)
   {
      _cardImageSelector = cardImageSelector;
      _game = game;

      var humanPlayer = (from player in _game.Players
                         where player.Type == PlayerType.Human
                         select player).Single();
      BottomPlayer = PlayerViewModel.CreateHuman(_game, humanPlayer, CanPlay);
      BottomPlayer.CardSelected += HumanPlayer_CardSelected;

      var otherPlayerEnumerator = (from player in _game.Players
                                   where player.Type != PlayerType.Human
                                   select player).GetEnumerator();
      otherPlayerEnumerator.MoveNext();
      LeftPlayer = PlayerViewModel.CreateAi(_game, otherPlayerEnumerator.Current, CanPlay);

      otherPlayerEnumerator.MoveNext();
      TopPlayer = PlayerViewModel.CreateAi(_game, otherPlayerEnumerator.Current, CanPlay);

      otherPlayerEnumerator.MoveNext();
      RightPlayer = PlayerViewModel.CreateAi(_game, otherPlayerEnumerator.Current, CanPlay);

      Players = new[] { TopPlayer, LeftPlayer, RightPlayer, BottomPlayer }
         .ToImmutableDictionary(x => x.Id, x => x);

      _engine = new(_game, PlayCard, StickCompleted);
      _engine.PlayerChanged += (_, player) => { CurrentPlayer = Players[player.Id]; };

      InitGame();
      InitTrick();

      //      CurrentPlayer = Players[turnPlayerIdQueue.Dequeue()];

      _engineTask = Task.Run(() => _engine.GamePlay(cancellationToken), cancellationToken);
   }

   private async Task PlayCard(Guid playerId)
   {
      var player = Players[playerId];
      await player.StartPlayCard();
   }

   private void StickCompleted(Guid winnerPLayerId)
   {
      var winnerPlayer = Players[winnerPLayerId];
      winnerPlayer.TakeStick(Players.Values.Select(x => x.TakePlayedCard()));

      InitTrick();
   }

   private void HumanPlayer_CardSelected(object? sender, CardSelectedEventArgs e)
   {
      if (CurrentPlayer.Equals(sender))
      {
         CurrentPlayer.PlayCard(e.Card);
      }
   }

   bool CanPlay(CardViewModel card, IReadOnlyCollection<CardViewModel> hand)
   {
      if (card.Card is IPlayCard playCard)
      {
         if (_game.CurrentSuit is null)
         {
            return true;
         }

         if (playCard.Suit == ValueCardSuit.Rocket)
         {
            return true;
         }

         if (playCard.Suit == _game.CurrentSuit)
         {
            return true;
         }

         if (!hand.Select(x => x.Card).OfType<IPlayCard>().Any(x => x.Suit == _game.CurrentSuit))
         {
            return true;
         }
      }

      return false;
   }

   private void InitGame()
   {
      foreach (var player in _game.Players)
      {
         Players[player.Id]
            .AddCardsToHand(player.Hand
               .OrderBy(x => x.Suit == ValueCardSuit.Rocket ? 1 : 0)
               .ThenBy(x => x.Suit)
               .ThenBy(x => x.Value), _cardImageSelector, player.Type.Equals(PlayerType.Human));
      }
   }

   private PlayerViewModel _currentPlayer = null!;

   public PlayerViewModel CurrentPlayer
   {
      get { return _currentPlayer; }
      set
      {
         _currentPlayer = value;
         OnPropertyChanged();

         foreach (var player in Players.Values)
         {
            player.IsCurrent = player.Equals(_currentPlayer);
         }
      }
   }


   private void InitTrick()
   {
      int playOrder = 0;
      foreach (var player in _game.GetPlayerRoundFromStartPlayer())
      {
         Players[player.Id].PlayOrder = ++playOrder;
      }
   }

   private void CompleteTrick()
   {
      // Check winner

   }

   private IReadOnlyDictionary<Guid, PlayerViewModel> Players { get; }

   public PlayerViewModel TopPlayer { get; }
   public PlayerViewModel LeftPlayer { get; }
   public PlayerViewModel RightPlayer { get; }
   public PlayerViewModel BottomPlayer { get; }
}
