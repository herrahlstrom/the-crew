using System.Numerics;
using System.Reflection;
using TheCrew.Shared;

namespace TheCrew.Model;

public static class ModelExtensions
{
   public static IEnumerable<PlayerModel> GetPlayerRoundFromStartPlayer(this GameModel model)
   {
      if (model.LastWinnerPlayerId is null)
      {
         return GetPlayersFromPivot(model, player => player.IsCommander);
      }

      return GetPlayersFromPivot(model, player => player.Id.Equals(model.LastWinnerPlayerId));
   }

   private static IEnumerable<PlayerModel> GetPlayersFromPivot(GameModel model, Predicate<PlayerModel> isStartPlayer)
   {
      List<PlayerModel> wrapped = new();

      bool startPlayerFound = false;
      for (int i = 0; i < model.Players.Count; i++)
      {
         if (startPlayerFound)
         {
            yield return model.Players[i];
         }
         else if (isStartPlayer(model.Players[i]))
         {
            yield return model.Players[i];
            startPlayerFound = true;
         }
         else
         {
            wrapped.Add(model.Players[i]);
         }
      }

      foreach (var wrappedPLayer in wrapped)
      {
         yield return wrappedPLayer;
      }
   }

   public static void PlayCard(this PlayerModel model, ICard card)
   {
      var cardFromHand = model.Hand.Where(x => x.Equals(card)).First();
      if (model.Hand.Remove(cardFromHand))
      {
         model.PlayedCard = cardFromHand;
      }
   }

   public static bool CanPlayCard(this PlayerModel player, IPlayCard card)
   {
      if (player.PlayedCard is not null)
      {
         return false;
      }

      ValueCardSuit? currentSuit = player.Game.CurrentSuit;

      if (currentSuit is null)
      {
         return true;
      }

      if (card.Suit == ValueCardSuit.Rocket)
      {
         return true;
      }

      if (card.Suit == currentSuit)
      {
         return true;
      }

      if (!player.Hand.Any(x => x.Suit == currentSuit))
      {
         return true;
      }

      return false;
   }
}