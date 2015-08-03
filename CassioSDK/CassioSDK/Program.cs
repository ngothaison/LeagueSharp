/*CassioSDK by ngothaison*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.SDK.Core;
using LeagueSharp.SDK.Core.Wrappers;
using LeagueSharp.SDK.Core.Enumerations;
using LeagueSharp.SDK.Core.Extensions;
using LeagueSharp.SDK.Core.UI.IMenu;
using LeagueSharp.SDK.Core.UI.IMenu.Values;
using SharpDX;
using LeagueSharp.SDK.Core.Events;

using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;



namespace Cassiopeia
{
    class Program
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static Menu config;
        private static Spell Q, W, E, R;
        private const int AutoAttackRange = 550;
        private static SpellSlot Ignite;
             

        static void Main(string[] args)
        {
            Load.OnLoad += OnLoad;
        }

        static void OnLoad(object sender, EventArgs args)
        {
            if (Player.ChampionName != "Cassiopeia")
                return;

            Bootstrap.Init(null);
            Game.PrintChat("CassioSDK by ngothaison Loaded!");
            Game.PrintChat(Player.FlatMagicDamageMod.ToString());
            Game.PrintChat(Player.TotalMagicalDamage.ToString());
            Ignite = Player.GetSpellSlot("summonerdot");
            config = new Menu("cassiopeia", "Cassiopeia", true);

            var drawmenu = config.Add(new Menu("drawing", "Drawing"));

            drawmenu.Add(new MenuBool("drawQ", "Draw Q, W range"));

            drawmenu.Add(new MenuBool("drawE", "Draw E range"));
            drawmenu.Add(new MenuBool("drawR", "Draw R range"));

            var farm = config.Add(new Menu("farm", "Farm"));
            farm.Add(new MenuBool("useqlh", "Use Q Last Hit",true));
            farm.Add(new MenuBool("useelh", "Use E Last Hit",true));

            var combo = config.Add(new Menu("combo", "Combo Settings"));
            combo.Add(new MenuBool("useQ", "Use Q",true));
            combo.Add(new MenuBool("useW", "Use W",true));
            combo.Add(new MenuBool("useR", "Use R",true));
            combo.Add(new MenuBool("useIgnite", "Use Ignite In Combo", true));

            var harass = config.Add(new Menu("harass", "Harass Settings"));

            harass.Add(new MenuBool("useq", "Use Q",true));
            harass.Add(new MenuBool("usew", "Use W",true));

            var killsteal = config.Add(new Menu("ks", "Kill Steal With E"));
            killsteal.Add(new MenuBool("kswE", "Kill Steal With E",true));


            Q = new Spell(SpellSlot.Q, 850f);
            W = new Spell(SpellSlot.W, 850f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(false, SkillshotType.SkillshotCone);
            

            config.Attach();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }

        static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Orbwalk:
                    Combo();
                    break;
                case OrbwalkerMode.LastHit:
                    Lasthit();
                    break;
                case OrbwalkerMode.LaneClear:
                    Laneclear();
                    break;
                case OrbwalkerMode.Hybrid:
                    Harass();
                    break;
            }

            var useEks = config["ks"]["kswE"].GetValue<MenuBool>().Value;

            if (useEks)
                Killsteal();

            
        }

  

        private static double Qdmg(Obj_AI_Base target)
        {
            return
              Player.CalculateDamage(target, DamageType.Magical,
                    new[] { 75, 115, 155, 195, 235 }[Q.Level - 1] + 0.45 * Player.FlatMagicDamageMod);
        }
        private static double Wdmg(Obj_AI_Base target)
        {
            return
              Player.CalculateDamage(target, DamageType.Magical,
                    new[] { 90, 135, 180, 225, 270 }[W.Level - 1] + 0.9 * Player.FlatMagicDamageMod);
        }

        private static double Edmg(Obj_AI_Base target)
        {
            return
              Player.CalculateDamage(target, DamageType.Magical,
                    new[] { 55, 80, 105, 130, 155 }[E.Level - 1] + 0.55 * Player.FlatMagicDamageMod);
        }

        private static double Rdmg(Obj_AI_Hero target)
        {
            return
              Player.CalculateDamage(target, DamageType.Magical,
                    new[] { 150, 250, 350 }[R.Level - 1] + 0.50 * Player.FlatMagicDamageMod);
        }

        private static double Ignitedmg(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            if (Ignite == SpellSlot.Summoner1)
                return (float)Player.GetSpellDamage(target, SpellSlot.Summoner1);
            if (Ignite == SpellSlot.Summoner2)
                return (float)Player.GetSpellDamage(target, SpellSlot.Summoner2);
            return 0;
        }

        private static double FullCombodmg(Obj_AI_Hero target)
        {
            return Qdmg(target) + Wdmg(target) + (Edmg(target) * 3) + Rdmg(target) + Ignitedmg(target);
        }

        private static double Combodmg(Obj_AI_Hero target)
        {
            return Qdmg(target) + Wdmg(target) + (Edmg(target) * 3) + Rdmg(target);
        }

        private static double Harassdmg(Obj_AI_Hero target)
        {
            return Qdmg(target) + Wdmg(target) + (Edmg(target) * 3) ;
        }

        static void Killsteal()
        {
              foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(
                target =>
                
                target.IsEnemy
                && target.IsValidTarget(E.Range)

                && !target.IsInvulnerable))
              {
                  if (target.Health < Edmg(target) && E.IsReady())
                      E.CastOnUnit(target);
              }
        }
        static void Harass()
        {


            var useQhr = config["harass"]["useq"].GetValue<MenuBool>().Value;
            var useWhr = config["harass"]["usew"].GetValue<MenuBool>().Value;

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null)
                return;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Hybrid)
            {
                /*foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(
                target =>
                Player.Distance(target.ServerPosition) <= E.Range
                && target.IsEnemy
                && target.IsValidTarget()

                && !target.IsInvulnerable))
                { }*/
                    if (target.HasBuffOfType(BuffType.Poison)&&E.IsReady())
                    {
                        E.CastOnUnit(target);
                    }

                    //Q+E
                    if (E.IsInRange(target) && !useWhr)
                    {
                        if (Q.IsReady() && useQhr)
                            Q.Cast(target);
                        if (E.IsReady())
                            E.CastOnUnit(target);
                    }

                    //Q+W+E
                    if (E.IsInRange(target))
                    {
                        if (Q.IsReady() && useQhr)
                            Q.Cast(target);
                        if (W.IsReady() && useWhr)
                            W.CastOnUnit(target);
                        if (E.IsReady())
                            E.CastOnUnit(target);
                    }

                
            }

        }

        static void Combo()
        {
            var useQcb = config["combo"]["useQ"].GetValue<MenuBool>().Value;
            var useWcb = config["combo"]["useW"].GetValue<MenuBool>().Value;
            var useRcb = config["combo"]["useR"].GetValue<MenuBool>().Value;
            var useIgnitecb = config["combo"]["useIgnite"].GetValue<MenuBool>().Value;

            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null)
                return;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Orbwalk && target.IsValidTarget() && !target.IsInvulnerable)
            {
                if (E.IsInRange(target))
                {
                    if (Q.IsReady() && useQcb)
                        Q.Cast(target);

                    if (W.IsReady() && useWcb)
                        W.CastOnUnit(target);

                    if (E.IsReady())
                        E.CastOnUnit(target);

                    if (R.IsReady() && useRcb)
                        R.Cast(target);

                    if (Ignite.IsReady() && useIgnitecb)
                        Player.Spellbook.CastSpell(Ignite, target);
                }

            }
        }

        static void OnDraw(EventArgs args)
        {
            var drawQ=config["drawing"]["drawQ"].GetValue<MenuBool>().Value;
            var drawE=config["drawing"]["drawE"].GetValue<MenuBool>().Value;
            var drawR=config["drawing"]["drawR"].GetValue<MenuBool>().Value;

            if (drawQ)
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Green);

            if (drawE)
                Drawing.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Violet);
            if (drawR)
                Drawing.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Red);
          
            var useIgnitecb0 = config["combo"]["useIgnite"].GetValue<MenuBool>().Value;
            var target = TargetSelector.GetTarget(E.Range);
            var pos = Drawing.WorldToScreen(target.Position);

            if (target.Health < Harassdmg(target))
                Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.White, "Killable with Q+W+Ex3");

            if (target.Health < FullCombodmg(target) && useIgnitecb0)
                Drawing.DrawText(pos.X , pos.Y , System.Drawing.Color.White, "Killable with Q+W+Ex3+R+Ignite");

            if (target.Health < Combodmg(target))
                Drawing.DrawText(pos.X , pos.Y , System.Drawing.Color.White, "Killable with Q+W+Ex3+R");
        }

        static void Lasthit()
        {
            var useQlh = config["farm"]["useqlh"].GetValue<MenuBool>().Value;
            var useElh = config["farm"]["useelh"].GetValue<MenuBool>().Value;

            if (Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(E.Range) && minion.IsEnemy ))
                {
                    if (Player.Distance(minion.ServerPosition) <= AutoAttackRange && Player.GetAutoAttackDamage(minion) > minion.Health)
                        return;

                    if (minion.Health < Qdmg(minion) && Q.IsReady() && useQlh)
                        Q.Cast(minion);

                    if (minion.Health < Edmg(minion) && E.IsReady() && useElh)
                        E.CastOnUnit(minion);

                }
            }
        }

        static void Laneclear()
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(E.Range) && minion.IsEnemy ))
                {
                    if (Player.Distance(minion.ServerPosition) <= AutoAttackRange && Player.GetAutoAttackDamage(minion) > minion.Health)
                        return;

                    if (minion.Health < Qdmg(minion) && Q.IsReady())
                        Q.Cast(minion);

                    if (minion.Health < Wdmg(minion) && W.IsReady())
                        W.CastOnUnit(minion);

                    if (minion.Health < Edmg(minion) && E.IsReady())
                        E.CastOnUnit(minion);

                }
            }
        }
    }
}

