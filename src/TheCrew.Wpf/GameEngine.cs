using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Player;

namespace TheCrew.Wpf
{
   public class GameEngine
   {
      private readonly GameModel _game;
      private readonly Func<Guid, Task> _playCardAction;
      private readonly Action<Guid> _stickCompleted;

      public GameEngine(GameModel game, Func<Guid, Task> playCardAction, Action<Guid> stickCompleted)
      {
         _game = game;
         _playCardAction = playCardAction;
         _stickCompleted = stickCompleted;
      }

      public event EventHandler<PlayerModel>? PlayerChanged;

      public async Task GamePlay(CancellationToken cancellationToken)
      {
         try
         {
            while (_game.Players.All(x => x.Hand.Count > 0))
            {
               cancellationToken.ThrowIfCancellationRequested();

               _game.CurrentSuit = null;

               foreach (var player in _game.GetPlayerRoundFromStartPlayer())
               {
                  await HandlePlayer(player, cancellationToken);
               }

               var winnerId = GetWinner();
               _game.LastWinnerPlayerId = winnerId;
               _stickCompleted.Invoke(winnerId);
            }
         }
         catch (OperationCanceledException) { }
      }

      private Guid GetWinner()
      {
         var winnerByRocket = _game.Players
            .Where(x => x.PlayedCard!.Suit == Shared.ValueCardSuit.Rocket)
            .OrderByDescending(x => x.PlayedCard?.Value)
            .FirstOrDefault();
         if (winnerByRocket != null)
         {
            return winnerByRocket.Id;
         }

         return _game.Players
                     .Where(x => x.PlayedCard!.Suit == _game.CurrentSuit)
                     .OrderByDescending(x => x.PlayedCard?.Value)
                     .Select(x => x.Id)
                     .First();
      }

      private async Task HandlePlayer(PlayerModel player, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();

         PlayerChanged?.Invoke(this, player);

         await _playCardAction.Invoke(player.Id);
         //if (player.Type == PlayerType.Ai)
         //{
         //   await _playCardAction.Invoke(player.Id);
         //}
         //else
         //{
         //   await HumanCompleted.WaitAsync(cancellationToken);
         //}
      }
   }
}
