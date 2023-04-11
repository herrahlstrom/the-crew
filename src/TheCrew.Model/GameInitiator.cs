using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheCrew.Model;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

namespace TheCrew.Model.Tools;

public class GameInitiator
{
   public void InitNewGame(GameModel game, int missionNumber)
   {
      game.Clear();

      game.Players.AddRange(CreatePlayerModels(game));

      DistributeCardsToPlayer(game.Players);

      foreach (var mission in GetMissions(missionNumber))
      {
         switch (mission)
         {
            case IMissionTaskCard missionCard:
               game.UnassignedMissionCards.Add(missionCard);
               break;

            case IGenericMissionTask genericMission:
               game.GenericMissions.Add(genericMission);
               break;

            default:
               throw new NotSupportedException("Not supported type of mission: " + mission.GetType());
         }
      }
   }

   private IEnumerable<PlayerModel> CreatePlayerModels(GameModel game)
   {
      yield return new PlayerModel(game)
      {
         Type = PlayerType.Human,
         Name = Environment.UserName,
      };

      const int numberOfAi = 3;
      var aiNames = new[] { "Konrad", "Madelene", "Kjell", "Elisabeth", "Ove", "Annika" }.GetRandomEnumerator();
      for (int i = 0; i < numberOfAi; i++)
      {
         aiNames.MoveNext();
         yield return new PlayerModel(game)
         {
            Type = PlayerType.Ai,
            Name = aiNames.MoveNext() ? aiNames.Current : throw new UnreachableException(),
         };
      }
   }

   private void DistributeCardsToPlayer(IEnumerable<PlayerModel> players)
   {
      IEnumerator<IPlayCard> cardEnumerator = CardFactory.GetAllPlayCards().GetRandomEnumerator();
      LoopEnumerator<PlayerModel> playerEnumerator = players.GetLoopEnumerator();
      while (cardEnumerator.MoveNext() && playerEnumerator.MoveNext())
      {
         playerEnumerator.Current.Hand.Add(cardEnumerator.Current);

         if (cardEnumerator.Current.Suit == ValueCardSuit.Rocket && cardEnumerator.Current.Value == 4)
         {
            playerEnumerator.Current.IsCommander = true;
         }
      }
   }

   private IEnumerable<IMissionTask> GetMissions(int missionNumber)
   {
      return missionNumber switch
      {
         1 => GetRandomMissionCards(1),
         2 => GetRandomMissionCards(2),
         3 => GetRandomMissionCards(2, MissionCardToken.First, MissionCardToken.Second),
         4 => GetRandomMissionCards(3),
         //5 => ,
         6 => GetRandomMissionCards(3, MissionCardToken.FirstFlex, MissionCardToken.SecondFlex),
         7 => GetRandomMissionCards(3, MissionCardToken.Last),
         8 => GetRandomMissionCards(3, MissionCardToken.First, MissionCardToken.Second, MissionCardToken.Third),
         9 => new[] { new WinTrickWithValueOfOneMission() },
         10 => GetRandomMissionCards(4),

         _ => throw new NotImplementedException($"Mission {missionNumber} is not implemented"),
      };

        static IEnumerable<IMissionTaskCard> GetRandomMissionCards(int numberOfCards, params MissionCardToken[] tokens)
      {
         var missionCards = CardFactory
            .GetAllValueMissionCards()
            .AtRandomOrder()
            .Take(numberOfCards);

         var tokenEnumerator = tokens.OfType<MissionCardToken>().GetEnumerator();

         foreach (var missionCard in missionCards)
         {
            yield return missionCard with
            {
               Token = tokenEnumerator.MoveNext()
                  ? tokenEnumerator.Current
                  : MissionCardToken.None
            };
         }
      }
   }
}
