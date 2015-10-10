using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using sAIO.Core;
using Color = System.Drawing.Color;

namespace sAIO.Champions
{
    class Talon : Helper
    {
        private static int lastQ = 0;
        public Talon()
        {
            Talon_OnGameLoad();
        }
        private static void Talon_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 500f);
            W.SetSkillshot(0.25f, 75, 2300, false, SkillshotType.SkillshotLine);


            Tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 420);
            Hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 420);


            menu.AddSubMenu(new Menu("Combo", "Combo"));
            
            CreateMenuBool("Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("Combo", "Combo.W", "Use W", true);
            CreateMenuBool("Combo", "Combo.E", "Use E", true);
            CreateMenuBool("Combo", "Combo.R", "Use R", true);
            //menu.SubMenu("Combo").AddItem(new MenuItem("Combo.EDelay", "E delay").SetValue(new Slider(0, 0, 1000)));


            menu.AddSubMenu(new Menu("Harass", "Harass"));
            
            CreateMenuBool("Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("Harass", "Harass.W", "Use E", true);
            CreateMenuBool("Harass", "Harass.E", "Use E", true);
            //menu.SubMenu("Harass").AddItem(new MenuItem("Harass.EDelay", "E delay").SetValue(new Slider(0, 0, 1000)));


            menu.AddSubMenu(new Menu("Gap closer", "GC"));
            CreateMenuBool("GC", "GC.W", "Use W", true);

            menu.AddSubMenu(new Menu("Kill Steal", "KS"));
            CreateMenuBool("KC", "KS.Q", "Use Q", true);
            CreateMenuBool("KC", "KS.W", "Use E", true);
            CreateMenuBool("KC", "KS.E", "Use E", false);

            menu.AddSubMenu(new Menu("Farm", "Farm"));
            CreateMenuBool("Farm", "Farm.Q", "Use Q", true);
            CreateMenuBool("Farm", "Farm.W", "Use W", true);
            


            menu.AddSubMenu(new Menu("Lane Clean", "LC"));
            CreateMenuBool("LC", "LC.Q", "Use Q", true);
            CreateMenuBool("LC", "LC.W", "Use W", true);
            


            menu.AddSubMenu(new Menu("Drawing", "Draw"));
            CreateMenuBool("Draw", "Draw.W", "Draw W", true);
            CreateMenuBool("Draw", "Draw.E", "Draw E", true);
            CreateMenuBool("Draw", "Draw.R", "Draw R", true);
            CreateMenuBool("Draw", "Draw.CBDamage", "Draw Combo Damage", true);
            menu.SubMenu("Draw").AddItem(new MenuItem("Draw.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = GetValueMenuBool("Draw.CBDamage");
            DrawDamage.Fill = menu.Item("Draw.DrawColor").GetValue<Circle>().Active;
            DrawDamage.FillColor = menu.Item("Draw.DrawColor").GetValue<Circle>().Color;

            menu.Item("Draw.CBDamage").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            menu.Item("Draw.DrawColor").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            new AssassinManager();
            Game.PrintChat("sAIO: " + player.ChampionName + " loaded");
        }

        static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(sender.IsMe)
            {
                if(args.SData.Name == Q.Instance.SData.Name)
                {
                    lastQ = Utils.TickCount;
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;

            if (Q.IsReady() && (GetValueMenuBool("Combo.Q") || GetValueMenuBool("Harass.Q")))
                Q.Cast();

            if (Tiamat.IsOwned() && Tiamat.IsReady() && Utils.TickCount - lastQ >= 2000)
                Tiamat.Cast();

            if (Hydra.IsOwned() && Hydra.IsReady() && Utils.TickCount - lastQ >= 2300)
                Hydra.Cast();
        }

        static void Game_OnUpdate(EventArgs args)
        {

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: LaneClear();
                    break;
            }                         
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            
            if (GetValueMenuBool("Draw.W"))
                Drawing.DrawCircle(player.Position, W.Range, Color.YellowGreen);

            if (GetValueMenuBool("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.Green);

            if (GetValueMenuBool("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target == null) return;

            if (R.IsReady() && GetValueMenuBool("Combo.R"))
                R.Cast();

            if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Combo.E"))
                E.CastOnUnit(target);

            if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Combo.W"))
                W.Cast(target);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target == null) return;

            if (E.IsReady() && E.IsInRange(target) && GetValueMenuBool("Harass.E"))
                E.CastOnUnit(target);

            if (W.IsReady() && W.IsInRange(target) && GetValueMenuBool("Harass.W"))
                W.Cast(target);
        }

        private static void Farm()
        {
            var minions = MinionManager.GetMinions(W.Range);

            var wFarmLocation = W.GetCircularFarmLocation(minions);

            if (wFarmLocation.MinionsHit > 1 && W.IsReady() && GetValueMenuBool("Farm.W"))
                W.Cast(wFarmLocation.Position);

            foreach(var minion in minions.Where(m => m.Health < (Q.GetDamage(m) * 0.9)))
            {
                if(Q.IsReady() && GetValueMenuBool("Farm.Q"))
                {
                    Q.Cast();
                    player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                }
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(W.Range);

            var wFarmLocation = W.GetCircularFarmLocation(minions);

            if (wFarmLocation.MinionsHit > 1 && W.IsReady() && GetValueMenuBool("Farm.W"))
                W.Cast(wFarmLocation.Position);

            foreach (var minion in minions)
            {
                if (Q.IsReady() && GetValueMenuBool("LC.Q"))
                {
                    Q.Cast();
                   
                }
            }
        }
        static float GetComboDamage(Obj_AI_Hero enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady() && R.Level > 0)
                damage += player.GetSpellDamage(enemy, SpellSlot.R);
                      
            return (float)damage;
        }
    }
}
