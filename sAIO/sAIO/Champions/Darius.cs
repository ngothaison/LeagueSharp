using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using sAIO.Core;

namespace sAIO.Champions
{
    class Darius
    {
         public Darius()
        {
            Darius_OnGameLoad();
        }
        static void Darius_OnGameLoad()
        {
            MenuLoader.LoadBaseMenu();
        }
    }
}
