namespace TheCrew.Shared;

public static class CardFactory
{
   public static IEnumerable<ValueCard> GetAllValueCards()
   {
      for (int value = 1; value <= 9; value++)
      {
         yield return new ValueCard(ValueCardSuit.Green, value);
         yield return new ValueCard(ValueCardSuit.Pink, value);
         yield return new ValueCard(ValueCardSuit.Blue, value);
         yield return new ValueCard(ValueCardSuit.Yellow, value);
      }
   }

   public static IEnumerable<IPlayCard> GetAllPlayCards()
   {
      for (int value = 1; value <= 9; value++)
      {
         yield return new PlayCard(ValueCardSuit.Green, value);
         yield return new PlayCard(ValueCardSuit.Pink, value);
         yield return new PlayCard(ValueCardSuit.Blue, value);
         yield return new PlayCard(ValueCardSuit.Yellow, value);
      }

      for (int value = 1; value <= 4; value++)
      {
         yield return new PlayCard(ValueCardSuit.Rocket, value);
      }
   }
}
