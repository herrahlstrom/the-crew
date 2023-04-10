using System;
using TheCrew.Model;
using TheCrew.Shared;

namespace TheCrew.Player.Human;

public class ConsolePlayer : PlayerBase, IPlayer
{
   public ConsolePlayer(PlayerModel playerModel, IGameState gameState)
      : base(playerModel, gameState)
   {
   }

   public bool IsCommander => PlayerModel.IsCommander;

   protected override IPlayCard SelectCardToPlay()
   {
      var cardsBySuit = PlayerModel.Hand.Where(IsFollowingSuit).OrderBy(x=> x.Suit).ThenBy(x=> x.Value).ToList();

      if (cardsBySuit.Count > 0)
      {
         return SelectCard(cardsBySuit, "Select card to play");
      }
      else
      {  
         return SelectCard(PlayerModel.Hand.OrderBy(x=> x.Suit).ThenBy(x=> x.Value), "Select card to play");
      }
   }

   public Task<IMissionCardTask> SelectMissionCard()
   {
      return Task.FromResult(SelectCard(GameState.UnassignedMissionCards, "Select mission"));
   }

   private static T SelectCard<T>(IEnumerable<T> cards, string requestText) where T : ICard
   {
      List<T> cardList = new();

      foreach (var card in cards)
      {
         cardList.Add(card);

         Console.WriteLine("{0,2}: {1}", cardList.Count, card.ToString());
      }

      while (true)
      {
         try
         {
            Console.Write("{0}: ", requestText);
            int index = int.Parse(Console.ReadLine()!);
            return cardList[index - 1];
         }
         catch
         {
            Console.WriteLine("Invalid input");
         }

      }
   }
}
