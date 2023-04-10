using System.Collections.Immutable;
using System.Diagnostics;
using TheCrew.Model;
using TheCrew.Player;
using TheCrew.Player.AI;
using TheCrew.Player.Human;
using TheCrew.Shared;
using TheCrew.Shared.Extensions;

namespace TheCrew.Engine;

public class GameInitiator
{
   readonly ModelRepository _repository;

   public GameInitiator()
   {
      _repository = new ModelRepository();
   }

   public async Task<GameModel> InitiateNewGame(int missionNumber, int numberOfPlayers)
   {
      var model = await _repository.CreateNewAsync();

      model.Players.Add(new PlayerModel
      {
         Id = Guid.NewGuid(),
         Type = PlayerType.Console,
         Name = Environment.UserName,
         Hand = new List<IPlayCard>(),
         Missions = new List<IMissionCardTask>(),
         TakenCards = new List<IPlayCard>()
      });

      var aiNames = new[] { "Dilberg", "Daisy", "Mike", "Bob" }.GetRandomEnumerator();
      model.Players.AddRange(Enumerable.Range(0, numberOfPlayers - 1).Select(i => new TheCrew.Model.PlayerModel()
      {
         Id = Guid.NewGuid(),
         Type = PlayerType.Ai,
         Name = aiNames.MoveNext() ? aiNames.Current : throw new UnreachableException(),
         Hand = new List<IPlayCard>(),
         Missions = new List<IMissionCardTask>(),
         TakenCards = new List<IPlayCard>()
      }));

      // Distribute play cards to the players
      LoopEnumerator<PlayerModel> playerEnumerator = model.Players.GetLoopEnumerator();
      IEnumerator<IPlayCard> cardEnumerator = CardFactory.GetAllPlayCards().GetRandomEnumerator();
      while (cardEnumerator.MoveNext() && playerEnumerator.MoveNext())
      {
         playerEnumerator.Current.Hand.Add(cardEnumerator.Current);
         if (cardEnumerator.Current is { Suit: ValueCardSuit.Rocket, Value: 4 })
         {
            playerEnumerator.Current.IsCommander = true;
         }
      }

      // Get the mission tasks for the current mission number
      foreach (var mission in GetMissions(missionNumber))
      {
         switch (mission)
         {
            case IMissionCardTask missionCard:
               model.UnassignedMissionCards.Add(missionCard);
               break;

            case IGenericMissionTask genericMission:
               model.GenericMissions.Add(genericMission);
               break;

            default:
               throw new NotSupportedException("Not supported type of mission: " + mission.GetType());
         }
      }

      await _repository.SaveAsync(model);

      return model;
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

   public IReadOnlyDictionary<Guid, IPlayer> GetPlayers(IGameState gameState, IEnumerable<PlayerModel> models)
   {
      return models.Select<PlayerModel, IPlayer>(player => player.Type switch
         {
            PlayerType.Console => new ConsolePlayer(player, gameState),
            PlayerType.Ai => new DummyAiPlayer(player, gameState),
            _ => throw new NotImplementedException()
         }).ToImmutableDictionary(x => x.ID, x => x);
   }
}
