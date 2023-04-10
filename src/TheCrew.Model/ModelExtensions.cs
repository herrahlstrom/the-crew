using TheCrew.Shared;

namespace TheCrew.Model;

public static class ModelExtensions
{
   public static IEnumerable<PlayerModel> GetPlayerRoundFromStartPlayer(this GameModel model)
   {
      Predicate<PlayerModel> isStartPlayer = model.LastWinnerPlayerId is null
         ? player => player.IsCommander
         : player => player.Id.Equals(model.LastWinnerPlayerId);

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
}