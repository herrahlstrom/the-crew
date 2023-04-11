using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheCrew.Shared;

namespace TheCrew.Wpf;

internal interface ICardImageSelector
{
   ImageSource GetCard(ICard card, bool frontface);
}

internal class CardImageSelector : ICardImageSelector
{
   private readonly Dictionary<string, BitmapImage> _bitmaps = new();

   public ImageSource GetCard(ICard card, bool frontface)
   {
      string path = GetCardImagePath(card, frontface);

      if (_bitmaps.TryGetValue(path, out BitmapImage? bitmapImage))
      {
         return bitmapImage;
      }

      bitmapImage = GetBitmapImage(path);
      _bitmaps.Add(path, bitmapImage);

      return bitmapImage;
   }


   private BitmapImage GetBitmapImage(string path)
   {
      BitmapImage bmp = new BitmapImage();
      bmp.BeginInit();
      bmp.UriSource = new Uri(path);
      bmp.EndInit();

      return bmp;
   }

   private string GetCardImagePath(ICard card, bool frontface)
   {
      if (card is IValueCard valueCard)
      {
         string root = "pack://application:,,,/TheCrew.Wpf;component/Resources/Cards/";
         if (!frontface)
         {
            return root + "back_of_playcard.png";
         }

         string value = valueCard.Value == 1 ? "ace" : $"{valueCard.Value}";

         return root + valueCard.Suit switch
         {
            ValueCardSuit.Green => $"{value}_of_spades.png",
            ValueCardSuit.Blue => $"{value}_of_clubs.png",
            ValueCardSuit.Yellow => $"{value}_of_diamonds.png",
            ValueCardSuit.Pink => $"{value}_of_hearts.png",
            ValueCardSuit.Rocket => $"{value}_of_hearts.png",
            _ => throw new NotImplementedException()
         };
      }
      throw new NotImplementedException();
   }
}
