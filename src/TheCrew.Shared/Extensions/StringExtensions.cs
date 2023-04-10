using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheCrew.Shared.Extensions;
public static class StringExtensions
{
   public static string? NullIfEmpty(this string? str)
   {
      if (string.IsNullOrEmpty(str))
      {
         return null;
      }
      return str;
   }
}
