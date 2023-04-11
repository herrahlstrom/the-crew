using System;
using System.Linq;
using System.Threading.Tasks;
using TheCrew.Model;
using TheCrew.Shared;

namespace TheCrew.Player.Human;
public class WpfPlayer : PlayerBase, IPlayer
{
   public WpfPlayer(PlayerModel playerModel, ReadOnlyGameModel gameModel) : base(playerModel, gameModel)
   {
   }

   public bool IsCommander => throw new NotImplementedException();

   public Task<IMissionCardTask> SelectMissionCard()
   {
      throw new NotImplementedException();
   }

   protected override IPlayCard SelectCardToPlay()
   {
      throw new NotImplementedException();
   }
}
