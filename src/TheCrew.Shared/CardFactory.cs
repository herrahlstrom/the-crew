namespace TheCrew.Shared;

public static class CardFactory
{
   public static IEnumerable<ValueMissionCardTask> GetAllValueMissionCards()
   {
      for (int value = 1; value <= 9; value++)
      {
         yield return new ValueMissionCardTask(ValueCardSuit.Green, value, MissionCardToken.None);
         yield return new ValueMissionCardTask(ValueCardSuit.Pink, value, MissionCardToken.None);
         yield return new ValueMissionCardTask(ValueCardSuit.Blue, value, MissionCardToken.None);
         yield return new ValueMissionCardTask(ValueCardSuit.Yellow, value, MissionCardToken.None);
      }
   }

   public static IEnumerable<PlayCard> GetAllPlayCards()
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
