using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

namespace TheCrew.Wpf.Tools;
internal class GameInitiator
{
   public GameModel CreateNewGame(int missionNumber)
   {
      var game = new GameModel();
      game.Players.AddRange(CreatePlayerModels());

      DistributeCardsToPlayer(game.Players);

      foreach (var mission in GetMissions(missionNumber))
      {
         switch (mission)
         {
            case IMissionCardTask missionCard:
               game.UnassignedMissionCards.Add(missionCard);
               break;

            case IGenericMissionTask genericMission:
               game.GenericMissions.Add(genericMission);
               break;

            default:
               throw new NotSupportedException("Not supported type of mission: " + mission.GetType());
         }
      }

      return game;
   }

   private void DistributeCardsToPlayer(IEnumerable<PlayerModel> players)
   {
      IEnumerator<IPlayCard> cardEnumerator = CardFactory.GetAllPlayCards().GetRandomEnumerator();
      LoopEnumerator<PlayerModel> playerEnumerator = players.GetLoopEnumerator();
      while (cardEnumerator.MoveNext() && playerEnumerator.MoveNext())
      {
         playerEnumerator.Current.Hand.Add(cardEnumerator.Current);

         if(cardEnumerator.Current.Suit == ValueCardSuit.Rocket && cardEnumerator.Current.Value == 4)
         {
            playerEnumerator.Current.IsCommander = true;
         }
      }
   }

   private IEnumerable<PlayerModel> CreatePlayerModels()
   {
      yield return new PlayerModel
      {
         Type = PlayerType.Human,
         Name = Environment.UserName,
      };

      const int numberOfAi = 3;
      var aiNames = new[] { "Konrad", "Madelene", "Kjell", "Elisabeth", "Ove", "Annika" }.GetRandomEnumerator();
      for (int i = 0; i < numberOfAi; i++)
      {
         aiNames.MoveNext();
         yield return new PlayerModel()
         {
            Type = PlayerType.Ai,
            Name = aiNames.MoveNext() ? aiNames.Current : throw new UnreachableException(),
         };
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

      IEnumerable<IMissionCardTask> GetRandomMissionCards(int numberOfCards, params MissionCardToken[] tokens)
      {
         var valueCards = CardFactory
            .GetAllValueCards()
            .AtRandomOrder()
            .Take(numberOfCards);

         var tokenEnumerator = tokens.OfType<MissionCardToken>().GetEnumerator();

         foreach (var valueCard in valueCards)
         {
            MissionCardToken token = tokenEnumerator.MoveNext()
               ? tokenEnumerator.Current
               : MissionCardToken.None;
            IMissionCardTask card = new ValueMissionCardTask(valueCard.Suit, valueCard.Value, token);
            yield return card;
         }
      }
   }
}
