using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Model.Tools;
using TheCrew.Player;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;
using TheCrew.Wpf.Factories;

namespace TheCrew.Wpf;

internal class GameAwareness : IGameAwareness
{
   private readonly PlayerModel _player;

   public GameAwareness(PlayerModel player)
   {
      _player = player;
   }
   public Predicate<IPlayCard> CanPlayPredicate => _player.CanPlayCard;

   public Func<IEnumerable<IMissionTaskCard>> UnassignedMissionCards => () => _player.Game.UnassignedMissionCards;

   public Func<IEnumerable<IPlayCard>> Hand => () => _player.Hand;
}

internal class MainViewModel : ViewModelBase
{
   readonly GameEngine _engine;
   readonly GameModel _game;
   private readonly PlayerViewModelFactory _playerViewModelFactory;

   public MainViewModel(GameModel game, PlayerViewModelFactory playerViewModelFactory)
   {
      _game = game;
      _playerViewModelFactory = playerViewModelFactory;
      _engine = new(_game, PlayCard, StickCompleted);
      _engine.PlayerChanged += (_, playerId) =>
      {
         Players.Values.ForEach(pvm => pvm.IsCurrent = pvm.Id.Equals(playerId));
         OnPropertyChanged(nameof(CurrentPlayer));
      };
   }

   public void InitNewGame(int missionNumber)
   {
      new GameInitiator().InitNewGame(_game, missionNumber);

      var playerEnumerator = _game.Players.Select(_playerViewModelFactory.Create).GetEnumerator();

      playerEnumerator.MoveNext();
      BottomPlayer = playerEnumerator.Current;

      playerEnumerator.MoveNext();
      LeftPlayer = playerEnumerator.Current;

      playerEnumerator.MoveNext();
      TopPlayer = playerEnumerator.Current;

      playerEnumerator.MoveNext();
      RightPlayer = playerEnumerator.Current;

      Players = new[] { TopPlayer, LeftPlayer, RightPlayer, BottomPlayer }.ToImmutableDictionary(x => x.Id, x => x);
   }

   public void StartEngine(CancellationToken cancellationToken)
   {
      InitTrick();

      Task.Run(() => _engine.GamePlay(cancellationToken), cancellationToken);
   }


   public PlayerViewModel BottomPlayer { get; private set; } = null!;

   public PlayerViewModel CurrentPlayer
   {
      get { return Players[_game.CurrentPlayer]; }
   }
   public PlayerViewModel LeftPlayer { get; private set; } = null!;
   public PlayerViewModel RightPlayer { get; private set; } = null!;

   public PlayerViewModel TopPlayer { get; private set; } = null!;

   private IReadOnlyDictionary<Guid, PlayerViewModel> Players { get; set; } = ImmutableDictionary<Guid, PlayerViewModel>.Empty;

   private void CompleteTrick()
   {
      // Check winner

   }

   private void InitTrick()
   {
      int playOrder = 0;
      foreach (var player in _game.GetPlayerRoundFromStartPlayer())
      {
         Players[player.Id].PlayOrder = ++playOrder;
      }
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
}
