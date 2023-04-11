using System.Diagnostics;
using TheCrew.Model;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

namespace TheCrew.Player.AI;
public class DummyAiPlayer : PlayerBase, IPlayer, IAiPlayer
{
   public DummyAiPlayer(PlayerModel playerModel, ReadOnlyGameModel gameModel)
      : base(playerModel, gameModel)
   {
   }

   public bool IsCommander => PlayerModel.IsCommander;

   public Task<IMissionCardTask> SelectMissionCard()
   {
      var highValueOnHandMissions = GameModel.UnassignedMissionCards
         .OfType<ValueMissionCardTask>()
         .Where(x => x.Value > 6)
         .Where(x => PlayerModel.Hand.Any(y => y.Suit == x.Suit && y.Value == x.Value))
         .OrderByDescending(x => x.Value)
         .FirstOrDefault();
      if (highValueOnHandMissions != null)
      {
         return Task.FromResult((IMissionCardTask)highValueOnHandMissions);
      }

      var lowValueNotInHandMissions = GameModel.UnassignedMissionCards
         .OfType<ValueMissionCardTask>()
         .Where(x => x.Value < 4)
         .Where(x => !PlayerModel.Hand.Any(y => y.Suit == x.Suit && y.Value == x.Value))
         .OrderBy(x => x.Value)
         .FirstOrDefault();
      if (lowValueNotInHandMissions != null)
      {
         return Task.FromResult((IMissionCardTask)lowValueNotInHandMissions);
      }

      // Select at random
      var randomEnumerator = new RandomEnumerator<IMissionCardTask>(GameModel.UnassignedMissionCards);
      randomEnumerator.MoveNext();
      return Task.FromResult(randomEnumerator.Current);
   }

   protected override IPlayCard SelectCardToPlay()
   {
      // Todo: gör lite smartare

      var cardEnumerator = PlayerModel.Hand.Where(IsFollowingSuit).GetRandomEnumerator();
      if (cardEnumerator.MoveNext())
      {
         return cardEnumerator.Current;
      }

      cardEnumerator = PlayerModel.Hand.GetRandomEnumerator();
      return cardEnumerator.MoveNext()
               ? cardEnumerator.Current
               : throw new UnreachableException();
   }

   Task<ICard> IAiPlayer.PlayCard()
   {
      ICard result;

      var cardEnumerator = PlayerModel.Hand.Where(IsFollowingSuit).GetRandomEnumerator();
      if (cardEnumerator.MoveNext())
      {
         result = cardEnumerator.Current;
      }
      else
      {
         cardEnumerator = PlayerModel.Hand.GetRandomEnumerator();
         result = cardEnumerator.MoveNext()
                  ? cardEnumerator.Current
                  : throw new UnreachableException();
      }

      return Task.FromResult(result);
   }
}
