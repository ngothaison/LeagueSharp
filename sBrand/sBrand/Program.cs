using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace sBrand
{
    class Program
    {
        private static readonly Obj_AI_Hero player = ObjectManager.Player;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot Ignite;
        private static Menu menu;
        static void Main(string[] args)
        {
            
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (player.ChampionName != "Brand")
                return;

            Q = new Spell(SpellSlot.Q, 1050f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 750f);
            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.85f, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
           

            Ignite = player.GetSpellSlot("summonerdot");

            menu = new Menu("sBrand", "sBrand", true);
            //Orbwalker
            menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalker"));
            //Target selector
            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            menu.AddSubMenu(tsMenu);
            //Combo Menu
            menu.AddSubMenu(new Menu("Combo", "sBrand.Combo"));
            menu.SubMenu("sBrand.Combo").AddItem(new MenuItem("Combo.Mode", "Mode").SetValue(new StringList(new[] { "W+Q+E+R", "E+Q+W+R" })));
            menu.SubMenu("sBrand.Combo").AddItem(new MenuItem("Combo.UseIgnite", "Use Ignite").SetValue(false));
            menu.SubMenu("sBrand.Combo").AddSubMenu(new Menu("Mode W+Q+E+R delay", "Combo.Mode1Delay"));
            menu.SubMenu("sBrand.Combo").AddSubMenu(new Menu("Mode E+Q+W+R delay", "Combo.Mode2Delay"));

            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode1Delay").AddItem(new MenuItem("Combo.Mode1Delay.Q", "Q delay").SetValue(new Slider(900, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode1Delay").AddItem(new MenuItem("Combo.Mode1Delay.W", "W delay").SetValue(new Slider(100, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode1Delay").AddItem(new MenuItem("Combo.Mode1Delay.E", "E delay").SetValue(new Slider(300, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode1Delay").AddItem(new MenuItem("Combo.Mode1Delay.R", "R delay").SetValue(new Slider(500, 0, 2000)));

            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode2Delay").AddItem(new MenuItem("Combo.Mode2Delay.Q", "Q delay").SetValue(new Slider(400, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode2Delay").AddItem(new MenuItem("Combo.Mode2Delay.W", "W delay").SetValue(new Slider(700, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode2Delay").AddItem(new MenuItem("Combo.Mode2Delay.E", "E delay").SetValue(new Slider(100, 0, 2000)));
            menu.SubMenu("sBrand.Combo").SubMenu("Combo.Mode2Delay").AddItem(new MenuItem("Combo.Mode2Delay.R", "R delay").SetValue(new Slider(600, 0, 2000)));
            //Harass Menu
            menu.AddSubMenu(new Menu("Harass", "sBrand.Harass"));
            menu.SubMenu("sBrand.Harass").AddItem(new MenuItem("Harass.Mode", "Mode").SetValue(new StringList(new[] { "W+Q", "E+Q", "E+W", "W+Q+E", "E+Q+W" })));
            menu.SubMenu("sBrand.Harass").AddSubMenu(new Menu("Mode W+Q delay", "Harass.Mode1Delay"));
            menu.SubMenu("sBrand.Harass").AddSubMenu(new Menu("Mode E+Q delay", "Harass.Mode2Delay"));
            menu.SubMenu("sBrand.Harass").AddSubMenu(new Menu("Mode E+W delay", "Harass.Mode3Delay"));
            menu.SubMenu("sBrand.Harass").AddSubMenu(new Menu("Mode W+Q+E delay", "Harass.Mode4Delay"));
            menu.SubMenu("sBrand.Harass").AddSubMenu(new Menu("Mode E+Q+W delay", "Harass.Mode5Delay"));

            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode1Delay").AddItem(new MenuItem("Harass.Mode1Delay.Q","Q delay").SetValue(new Slider(900, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode1Delay").AddItem(new MenuItem("Harass.Mode1Delay.W","W delay").SetValue(new Slider(100, 0, 2000)));

            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode2Delay").AddItem(new MenuItem("Harass.Mode2Delay.Q", "Q delay").SetValue(new Slider(400, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode2Delay").AddItem(new MenuItem("Harass.Mode2Delay.E", "E delay").SetValue(new Slider(100, 0, 2000)));

            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode3Delay").AddItem(new MenuItem("Harass.Mode3Delay.W", "W delay").SetValue(new Slider(300, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode3Delay").AddItem(new MenuItem("Harass.Mode3Delay.E", "E delay").SetValue(new Slider(100, 0, 2000)));

            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode4Delay").AddItem(new MenuItem("Harass.Mode4Delay.Q", "Q delay").SetValue(new Slider(1000, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode4Delay").AddItem(new MenuItem("Harass.Mode4Delay.W", "W delay").SetValue(new Slider(100, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode4Delay").AddItem(new MenuItem("Harass.Mode4Delay.E", "E delay").SetValue(new Slider(400, 0, 2000)));

            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode5Delay").AddItem(new MenuItem("Harass.Mode5Delay.Q", "Q delay").SetValue(new Slider(300, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode5Delay").AddItem(new MenuItem("Harass.Mode5Delay.W", "W delay").SetValue(new Slider(700, 0, 2000)));
            menu.SubMenu("sBrand.Harass").SubMenu("Harass.Mode5Delay").AddItem(new MenuItem("Harass.Mode5Delay.E", "E delay").SetValue(new Slider(100, 0, 2000)));
            //KS menu
            menu.AddSubMenu(new Menu("Kill Steal", "sBrand.KillSteal"));
            menu.SubMenu("sBrand.KillSteal").AddItem(new MenuItem("KillSteal.Enable", "Enable").SetValue(true));
            menu.SubMenu("sBrand.KillSteal").AddItem(new MenuItem("KillSteal.Q", "Use Q").SetValue(true));
            menu.SubMenu("sBrand.KillSteal").AddItem(new MenuItem("KillSteal.W", "Use W").SetValue(true));
            menu.SubMenu("sBrand.KillSteal").AddItem(new MenuItem("KillSteal.E", "Use E").SetValue(true));
            menu.SubMenu("sBrand.KillSteal").AddItem(new MenuItem("KillSteal.Ignite", "Use Ignite").SetValue(false));
            //Farm menu
            menu.AddSubMenu(new Menu("Farming", "sBrand.Farm"));
            menu.SubMenu("sBrand.Farm").AddItem(new MenuItem("Farm.Q", "Use Q").SetValue(true));
            menu.SubMenu("sBrand.Farm").AddItem(new MenuItem("Farm.W", "Use W").SetValue(true));
            menu.SubMenu("sBrand.Farm").AddItem(new MenuItem("Farm.E", "Use E").SetValue(true));
            //Gap closer
            menu.AddSubMenu(new Menu("Gap Closer", "sBrand.GapCloser"));
            menu.SubMenu("sBrand.GapCloser").AddItem(new MenuItem("GapCloser.Enable", "Auto stun on gap closer (E+Q)").SetValue(true));
            //Interrupts
            menu.AddSubMenu(new Menu("Interrupts", "sBrand.Interrupts"));
            menu.SubMenu("sBrand.Interrupts").AddItem(new MenuItem("Interrupts.Enable", "Interrupts spell with E+Q").SetValue(true));
            //Drawing
            menu.AddSubMenu(new Menu("Drawing", "sBrand.Drawing"));
            menu.SubMenu("sBrand.Drawing").AddItem(new MenuItem("Drawing.Enable", "Enable").SetValue(true));
            menu.SubMenu("sBrand.Drawing").AddItem(new MenuItem("Drawing.Q", "Draw Q Range").SetValue(true));
            menu.SubMenu("sBrand.Drawing").AddItem(new MenuItem("Drawing.W", "Draw W Range").SetValue(true));
            menu.SubMenu("sBrand.Drawing").AddItem(new MenuItem("Drawing.E", "Draw E Range").SetValue(true));
            menu.SubMenu("sBrand.Drawing").AddItem(new MenuItem("Drawing.R", "Draw R Range").SetValue(true));

            menu.AddToMainMenu();

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var enable = menu.Item("Drawing.Enable").GetValue<bool>();
            if (enable)
            {
                var drawQ = menu.Item("Drawing.Q").GetValue<bool>();
                var drawW = menu.Item("Drawing.W").GetValue<bool>();
                var drawE = menu.Item("Drawing.E").GetValue<bool>();
                var drawR = menu.Item("Drawing.R").GetValue<bool>();

                if (drawQ)
                    Drawing.DrawCircle(player.Position, Q.Range, System.Drawing.Color.Blue);

                if (drawW)
                    Drawing.DrawCircle(player.Position, W.Range, System.Drawing.Color.Green);

                if (drawE)
                    Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.BlueViolet);

                if (drawR)
                    Drawing.DrawCircle(player.Position, R.Range, System.Drawing.Color.Red);
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            var enable = menu.Item("GapCloser.Enable").GetValue<bool>();
            if(enable)
            {
                var Qpre = Q.GetPrediction(gapcloser.Sender);
                if(E.IsReady() && E.IsInRange(gapcloser.Sender))
                    E.CastOnUnit(gapcloser.Sender);
                if (Q.IsReady() && Q.IsInRange(gapcloser.Sender))
                {
                    Utility.DelayAction.Add(400, () => Q.Cast(Qpre.CastPosition));
                }
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch(Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: LaneClean();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;

                var ks = menu.Item("KillSteal.Enable").GetValue<bool>();

                if (ks)
                     KillSteal();
            }
        }

        static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var enable = menu.Item("Interrupts.Enable").GetValue<bool>();
            if (enable)
            {
                var Qpre = Q.GetPrediction(sender);
                if(E.IsReady() && E.IsInRange(sender))
                    E.CastOnUnit(sender);
                if (Q.IsReady() && Q.IsInRange(sender))
                {
                    Utility.DelayAction.Add(350, () => Q.Cast(Qpre.CastPosition));
                }
            }
        }
        static void Combo()
        {
            var mode = menu.Item("Combo.Mode").GetValue<StringList>().SelectedIndex;
            var useIgnite = menu.Item("Combo.UseIgnite").GetValue<bool>();
            var target = TargetSelector.GetTarget(E.Range,TargetSelector.DamageType.Magical);

            var Qdelay1 = menu.Item("Combo.Mode1Delay.Q").GetValue<Slider>().Value;
            var Wdelay1 = menu.Item("Combo.Mode1Delay.W").GetValue<Slider>().Value;
            var Edelay1 = menu.Item("Combo.Mode1Delay.E").GetValue<Slider>().Value;
            var Rdelay1 = menu.Item("Combo.Mode1Delay.R").GetValue<Slider>().Value;

            var Qdelay2 = menu.Item("Combo.Mode2Delay.Q").GetValue<Slider>().Value;
            var Wdelay2 = menu.Item("Combo.Mode2Delay.W").GetValue<Slider>().Value;
            var Edelay2 = menu.Item("Combo.Mode2Delay.E").GetValue<Slider>().Value;
            var Rdelay2 = menu.Item("Combo.Mode2Delay.R").GetValue<Slider>().Value;

            switch(mode)
            {
                case 0:
                    {
                        if (W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            Utility.DelayAction.Add(Wdelay1, () => W.Cast(target));
                        }

                        if(Q.IsReady() && target.IsValidTarget(Q.Range))
                        {                            
                            Utility.DelayAction.Add(Qdelay1, () => Q.Cast(target));
                        }

                        if (E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            Utility.DelayAction.Add(Edelay1, () => E.CastOnUnit(target));
                        }

                        if (R.IsReady() && target.IsValidTarget(R.Range))
                        {
                            Utility.DelayAction.Add(Rdelay1, () => R.CastOnUnit(target));
                        }

                        if (useIgnite)
                        {
                            player.Spellbook.CastSpell(Ignite, target);
                        }
                    }
                    break;

                case 1:
                    {
                        if (E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            Utility.DelayAction.Add(Edelay2, () => E.CastOnUnit(target));
                        }

                        if (Q.IsReady() && target.IsValidTarget(Q.Range))
                        {                           
                            Utility.DelayAction.Add(Qdelay2, () => Q.Cast(target));
                        }

                        if (W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            Utility.DelayAction.Add(Wdelay2, () => W.Cast(target));
                        }

                        if (R.IsReady() && target.IsValidTarget(R.Range))
                        {
                            Utility.DelayAction.Add(Rdelay2, () => R.CastOnUnit(target));
                        }

                        if (useIgnite)
                        {
                            player.Spellbook.CastSpell(Ignite, target);
                        }
                    }
                    break;
            }
        }
        static void Harass()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var mode = menu.Item("Harass.Mode").GetValue<StringList>().SelectedIndex;
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                var Qdelay1 = menu.Item("Harass.Mode1Delay.Q").GetValue<Slider>().Value;
                var Wdelay1 = menu.Item("Harass.Mode1Delay.W").GetValue<Slider>().Value;

                var Edelay2 = menu.Item("Harass.Mode2Delay.E").GetValue<Slider>().Value;
                var Qdelay2 = menu.Item("Harass.Mode2Delay.Q").GetValue<Slider>().Value;

                var Edelay3 = menu.Item("Harass.Mode3Delay.E").GetValue<Slider>().Value;
                var Wdelay3 = menu.Item("Harass.Mode3Delay.W").GetValue<Slider>().Value;

                var Qdelay4 = menu.Item("Harass.Mode4Delay.Q").GetValue<Slider>().Value;
                var Wdelay4 = menu.Item("Harass.Mode4Delay.W").GetValue<Slider>().Value;
                var Edelay4 = menu.Item("Harass.Mode4Delay.E").GetValue<Slider>().Value;

                var Qdelay5 = menu.Item("Harass.Mode5Delay.Q").GetValue<Slider>().Value;
                var Wdelay5 = menu.Item("Harass.Mode5Delay.W").GetValue<Slider>().Value;
                var Edelay5 = menu.Item("Harass.Mode5Delay.E").GetValue<Slider>().Value;

                switch (mode)
                {
                    case 0:
                        {
                            if (W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                Utility.DelayAction.Add(Wdelay1, () => W.Cast(target));
                            }

                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Utility.DelayAction.Add(Qdelay1, () => Q.Cast(target));                                
                            }
                        }
                        break;

                    case 1:
                        {
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                Utility.DelayAction.Add(Edelay2, () => E.CastOnUnit(target));
                            }

                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Utility.DelayAction.Add(Qdelay2, () => Q.Cast(target));                               
                            }
                        }
                        break;

                    case 2:
                        {
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                Utility.DelayAction.Add(Edelay3, () => E.CastOnUnit(target));
                            }

                            if (W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                Utility.DelayAction.Add(Wdelay3, () => W.Cast(target));
                            }
                        }
                        break;

                    case 3:
                        {
                            if (W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                Utility.DelayAction.Add(Wdelay4, () => W.Cast(target));
                            }

                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Utility.DelayAction.Add(Qdelay4, () => Q.Cast(target));
                            }

                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                Utility.DelayAction.Add(Edelay4, () => E.CastOnUnit(target));
                            }
                        }
                        break;

                    case 4:
                        {
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                Utility.DelayAction.Add(Edelay5, () => E.CastOnUnit(target));
                            }

                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Utility.DelayAction.Add(Qdelay5, () => Q.Cast(target));                               
                            }

                            if (W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                Utility.DelayAction.Add(Wdelay5, () => W.Cast(target));
                            }
                        }
                        break;
                }
            }
            
        }
        static void KillSteal()
        {
            foreach(var target in ObjectManager.Get<Obj_AI_Hero>().Where(target=>target.IsValidTarget(E.Range) && !target.IsInvulnerable && target.IsEnemy))
            {
                var useQ = menu.Item("KillSteal.Q").GetValue<bool>();
                var useW = menu.Item("KillSteal.W").GetValue<bool>();
                var useE = menu.Item("KillSteal.E").GetValue<bool>();
                var useIg = menu.Item("KillSteal.Ignite").GetValue<bool>();

                var Qdmg = Q.GetDamage(target) * 0.9;
                var Wdmg = W.GetDamage(target) * 0.9;
                var Edmg = E.GetDamage(target) * 0.9;
                float Ignitedmg;
                if (Ignite != SpellSlot.Unknown)
                    Ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                else
                    Ignitedmg = 0f;

                if (useIg && Ignite.IsReady() && target.Health < Ignitedmg)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                }
                                                
                if (useQ && ! useW && !useE && Q.IsReady() && target.Health < Qdmg)
                {
                    Q.Cast(target);                    
                }

                if (!useQ && !useE && useW && W.IsReady() && target.Health < Wdmg)
                {
                    W.Cast(target);
                }

                if (!useQ && !useW && useE && E.IsReady() && target.Health < Edmg)
                {
                    E.CastOnUnit(target);
                }
            }                        
        }
        static void Farm()
        {
            if(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                var useQ = menu.Item("Farm.Q").GetValue<bool>();
                var useW = menu.Item("Farm.W").GetValue<bool>();
                var useE = menu.Item("Farm.E").GetValue<bool>();

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && m.IsValidTarget(E.Range) && !m.IsDead && !m.IsInvulnerable))
                {
                    var Qdmg = Q.GetDamage(minion) * 0.9;
                    var Wdmg = W.GetDamage(minion) * 0.9;
                    var Edmg = E.GetDamage(minion) * 0.9;

                    if (Q.IsReady() && minion.Health < Qdmg && useQ)
                    {
                        Q.Cast(minion);
                    }

                    if (W.IsReady() && minion.Health < Wdmg && useW)
                    {
                        W.Cast(minion);
                    }

                    if (E.IsReady() && minion.Health < Edmg && useE)
                    {
                        E.CastOnUnit(minion);
                    }                   
                }
            }            
        }
        static void LaneClean()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {                
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && m.IsValidTarget(E.Range) && !m.IsDead && !m.IsInvulnerable))
                {
                    var Qdmg = Q.GetDamage(minion) * 0.9;
                    var Wdmg = W.GetDamage(minion) * 0.9;
                    var Edmg = E.GetDamage(minion) * 0.9;

                    if (Q.IsReady() && minion.Health < Qdmg)
                    {
                        Q.Cast(minion);
                    }

                    if (W.IsReady() && minion.Health < Wdmg)
                    {
                        W.Cast(minion);
                    }

                    if (E.IsReady() && minion.Health < Edmg)
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
        }
    }
}
