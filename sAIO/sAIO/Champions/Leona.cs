using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using sAIO.Core;

namespace sAIO.Champions
{
    class Leona : Helper
    {
        public Leona()
        {
            Leona_OnGameLoad();
        }
        static void Leona_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, player.AttackRange + 20);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 1200f);
            E.SetSkillshot(0.25f, 120f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            menu.AddSubMenu(new Menu("Combo", "Combo"));
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
          

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use W", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            CreateMenuSlider("Harass", "Harass.MinManaPercent", "Min Mana Percent To Use", 0, 15, 100);


            menu.AddSubMenu(new Menu("Interrupter", "Interrupter"));
            CreateMenuBool("Interrupter", "Interrupter.E", "Use E", true);
            CreateMenuBool("Interrupter", "Interrupter.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("GC", "GC"));
            CreateMenuBool("GC", "GC.Q", "Use Q", true);

            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.Q", "Draw Q ", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);

            menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            throw new NotImplementedException();
        }

        static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            throw new NotImplementedException();
        }

        static void Game_OnUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }
        static void Combo()
        {

        }
        static void Harass()
        {

        }
    }

}
