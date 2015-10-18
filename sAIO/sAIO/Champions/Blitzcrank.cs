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
    class Blitzcrank : Helper
    {
        private static int lastE;
      
        public Blitzcrank()
        {
            Blitzcrank_OnGameLoad();
        }

        private static void Blitzcrank_OnGameLoad()
        {

            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            CreateMenuSlider("Combo", "Combo.RDelay", "R Delay", 0, 1000, 2000);

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);

            menu.AddSubMenu(new Menu("Interrupter", "Interrupter"));
            CreateMenuBool("Interrupter", "Interrupter.E", "Use E", true);
            CreateMenuBool("Interrupter", "Interrupter.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("GC", "GC"));
            CreateMenuBool("GC", "GC.E", "Use E", true);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q ", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
        }
    }
}
