using System.Diagnostics;
using TheCrew.Model;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

namespace TheCrew.Player.AI;

public abstract class AiPlayer
{
   protected IGameAwareness Game { get; }

   public AiPlayer(IGameAwareness game)
   {
      Game = game;
   }
}

public class DummyAiPlayer : AiPlayer, IAiPlayer
{
   public DummyAiPlayer(IGameAwareness gameAwareness) : base(gameAwareness)
   {
   }

   public IMissionTaskCard SelectMissionCard()
   {
      IReadOnlyCollection<IPlayCard> hand = Game.Hand.Invoke().AsReadOnlyCollection();
      IReadOnlyCollection<IMissionTaskCard> unassignedMissionCards = Game.UnassignedMissionCards.Invoke().AsReadOnlyCollection();

      var highValueOnHandMissions = unassignedMissionCards
         .OfType<ValueMissionCardTask>()
         .Where(x => x.Value > 6)
         .Where(x => hand.Any(y => y.Suit == x.Suit && y.Value == x.Value))
         .OrderByDescending(x => x.Value)
         .FirstOrDefault();
      if (highValueOnHandMissions != null)
      {
         return highValueOnHandMissions;
      }

      var lowValueNotInHandMissions = unassignedMissionCards
         .OfType<ValueMissionCardTask>()
         .Where(x => x.Value < 4)
         .Where(x => !hand.Any(y => y.Suit == x.Suit && y.Value == x.Value))
         .OrderBy(x => x.Value)
         .FirstOrDefault();
      if (lowValueNotInHandMissions != null)
      {
         return lowValueNotInHandMissions;
      }

      // Select at random
      var randomEnumerator = new RandomEnumerator<IMissionTaskCard>(unassignedMissionCards);
      randomEnumerator.MoveNext();
      return randomEnumerator.Current;
   }

   public IPlayCard SelectCardToPlay()
   {
      // Todo: gör lite smartare

      IReadOnlyCollection<IPlayCard> hand = Game.Hand.Invoke().AsReadOnlyCollection();

      var cardEnumerator = hand.Where(x => Game.CanPlayPredicate(x)).GetRandomEnumerator();
      if (cardEnumerator.MoveNext())
      {
         return cardEnumerator.Current;
      }

      cardEnumerator = hand.GetRandomEnumerator();
      return cardEnumerator.MoveNext()
               ? cardEnumerator.Current
               : throw new UnreachableException();
   }

}
