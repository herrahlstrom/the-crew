using System.Diagnostics;
using TheCrew.Engine;
using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

var gameEngine = new GameInitiator();

int numberOfPLayers = 0;
while (numberOfPLayers < 3 || numberOfPLayers > 5)
{
   Console.Write("Select number of players (3-5): ");
   try
   {
      numberOfPLayers = int.Parse(Console.ReadLine() ?? "0");
   }
   catch
   {
      Console.Write("Invalid number");
   }
}

GameModel game = await gameEngine.InitiateNewGame(1, numberOfPLayers);
var gameState = new GameState(game);
var players = gameEngine.GetPlayers(gameState, game.Players);



var playerEnumeratorFromCaptain = new LoopEnumerator<IPlayer>(game.GetPlayerRoundFromStartPlayer().Select(x=> players[x.Id]));
while (game.UnassignedMissionCards.Count > 0 && playerEnumeratorFromCaptain.MoveNext())
{
   var card = await playerEnumeratorFromCaptain.Current.SelectMissionCard();

   game.UnassignedMissionCards.Remove(card);
   game.Players.Where(x => x.Id == playerEnumeratorFromCaptain.Current.ID).First().Missions.Add(card);
}

while (game.Players.All(p => p.Hand.Count > 0))
{
   PrintPlayer(game);

   foreach (IPlayer player in game.GetPlayerRoundFromStartPlayer().Select(x=> players[x.Id]))
   {
      await player.PlayCard();

      if (!(player.PlayedCard is { } playedCard))
      {
         throw new UnreachableException();
      }

      game.CurrentSuit ??= playedCard.Suit;

      Console.WriteLine("{0} plays {1}", player.Name, playedCard);
   }

   PlayerModel winner = GetWinner(game);
   Console.WriteLine("{0} winns the tick", winner.Name);

   foreach (var p in game.Players)
   {
      var card = p.PlayedCard ?? throw new UnreachableException();
      winner.TakenCards.Add(card);

      foreach (var missionCard in winner.Missions.Where(x => !x.Completed).OfType<ValueMissionCardTask>().Where(x => x.SameCard(card)))
      {
         missionCard.Completed = true;
      }

      p.PlayedCard = null;
   }

   game.LastWinnerPlayerId = winner.Id;
   game.CurrentSuit = null;

   Console.WriteLine();
   Console.WriteLine();

   Console.WriteLine(new string('-', Console.WindowWidth));
   Console.WriteLine();

}

Console.Clear();
Console.WriteLine("Result");
foreach (var mission in game.Players.SelectMany(x => x.Missions))
{
   Console.WriteLine("{0,-20} {1}", mission, mission.Completed ? "Completed" : "Fail");
}

static PlayerModel GetWinner(GameModel game)
{
   PlayerModel? winnerByRocket = game.Players
      .Where(x => x.PlayedCard?.Suit == ValueCardSuit.Rocket)
      .OrderByDescending(x => x.PlayedCard!.Value)
      .FirstOrDefault();
   if (winnerByRocket != null)
   {
      return winnerByRocket;
   }

   PlayerModel? winnerBySuit = game.Players
      .Where(x => x.PlayedCard?.Suit == game.CurrentSuit)
      .OrderByDescending(x => x.PlayedCard!.Value)
      .FirstOrDefault();
   if (winnerBySuit != null)
   {
      return winnerBySuit;
   }

   throw new UnreachableException();
}

static void PrintPlayer(GameModel game)
{
   foreach (var player in game.Players)
   {
      Console.WriteLine("-= {0} =- {1}", player.Name, player.IsCommander ? "COMMANDER" : "");
      if (player.Type == PlayerType.Console)
      {
         Console.Write("  Hand:");
         foreach (var perSuit in player.Hand.GroupBy(x => x.Suit))
         {
            Console.Write("  {0}: {{{1}}}", perSuit.Key, string.Join(", ", perSuit.OrderBy(x => x.Value).Select(x => x.Value)));
         }
         Console.WriteLine();
      }

      if (player.Missions.Any())
      {
         Console.WriteLine("  Missions: {0}", string.Join(", ", player.Missions));
      }

      Console.WriteLine();
   }
}

