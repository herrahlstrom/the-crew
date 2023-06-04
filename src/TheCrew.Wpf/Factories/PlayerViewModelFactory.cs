using System;
using System.Collections.Generic;
using System.Linq;
using TheCrew.Model;
using TheCrew.Player.AI;

namespace TheCrew.Wpf.Factories;

internal class PlayerViewModelFactory
{
   private readonly GameModel _game;
   readonly CardViewModelFactory _cardViewModelFactory;

   public PlayerViewModelFactory(GameModel game, CardViewModelFactory cardViewModelFactory)
   {
      _game = game;
      _cardViewModelFactory = cardViewModelFactory;
   }

   public PlayerViewModel Create(PlayerModel playerModel)
   {
      return playerModel.Type switch
      {
         PlayerType.Human => CreateHuman(playerModel),
         PlayerType.Ai => CreateAi(playerModel),
         _ => throw new NotImplementedException()
      };
   }

   private PlayerViewModel CreateHuman(PlayerModel playerModel)
   {
      return new HumanPlayerViewModel(_game, playerModel, _cardViewModelFactory);
   }

   private PlayerViewModel CreateAi(PlayerModel playerModel)
   {
      var ai = new DummyAiPlayer(new GameAwareness(playerModel));
      return new AiPlayerViewModel(ai, _game, playerModel, _cardViewModelFactory);
   }
}
