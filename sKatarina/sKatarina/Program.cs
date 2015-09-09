using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

namespace sKatarina
{
    class Program
    {
        private static Obj_AI_Hero player;
        private static Spell Q, W, E, R;
        
        private static SpellSlot IgniteSlot;
        private const int AutoAttackRange = 125;
        private const float overkill = 0.8f;
        private static Menu menu;
        public static Orbwalking.Orbwalker Orbwalker;
        
        private static int _lastPlaced;
        private static Vector3 _lastWardPos;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            player = ObjectManager.Player;

            if (player.ChampionName != "Katarina")
                return;

            Q = new Spell(SpellSlot.Q, 675f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 550f);

            

            IgniteSlot = player.GetSpellSlot("summonerdot");

            menu = new Menu("sKatarina", "sKatarina", true);

            menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalking"));

            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            menu.AddSubMenu(tsMenu);

            menu.AddSubMenu(new Menu("Combo", "Combo"));            
            menu.SubMenu("Combo").AddItem(new MenuItem("cbMode", "Mode").SetValue(new StringList(new[] { "Q+E+W+R", "E+Q+W+R" })));
            menu.SubMenu("Combo").AddItem(new MenuItem("Use Ignite in combo", "Use Ignite in combo").SetValue(false));
            menu.SubMenu("Combo").AddSubMenu(new Menu("Mode Q+E+W+R delay", "modecb1delay"));
            menu.SubMenu("Combo").AddSubMenu(new Menu("Mode E+Q+W+R delay", "modecb2delay"));
            menu.SubMenu("Combo").SubMenu("modecb1delay").AddItem(new MenuItem("Qdelaycb1", "Q delay").SetValue(new Slider(120, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb1delay").AddItem(new MenuItem("Wdelaycb1", "W delay").SetValue(new Slider(600, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb1delay").AddItem(new MenuItem("Edelaycb1", "E delay").SetValue(new Slider(670, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb1delay").AddItem(new MenuItem("Rdelaycb1", "R delay").SetValue(new Slider(500, 0, 2000)));
            
            menu.SubMenu("Combo").SubMenu("modecb2delay").AddItem(new MenuItem("Qdelaycb2", "Q delay").SetValue(new Slider(200, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb2delay").AddItem(new MenuItem("Wdelaycb2", "W delay").SetValue(new Slider(600, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb2delay").AddItem(new MenuItem("Edelaycb2", "E delay").SetValue(new Slider(120, 0, 2000)));
            menu.SubMenu("Combo").SubMenu("modecb2delay").AddItem(new MenuItem("Rdelaycb2", "R delay").SetValue(new Slider(500, 0, 2000)));

            menu.AddSubMenu(new Menu("Harass", "Harass"));
            menu.SubMenu("Harass").AddItem(new MenuItem("hMode", "Mode").SetValue(new StringList(new[] { "Q", "Q+W", "Q+E+W", "E+Q+W" })));
            menu.SubMenu("Harass").AddSubMenu(new Menu("Mode Q+E+W delay", "modeh1delay"));
            menu.SubMenu("Harass").AddSubMenu(new Menu("Mode E+Q+W delay", "modeh2delay"));

            menu.SubMenu("Harass").SubMenu("modeh1delay").AddItem(new MenuItem("Qdelayh1", "Delay Q").SetValue(new Slider(120, 0, 2000)));
            menu.SubMenu("Harass").SubMenu("modeh1delay").AddItem(new MenuItem("Wdelayh1", "Delay W").SetValue(new Slider(600, 0, 2000)));
            menu.SubMenu("Harass").SubMenu("modeh1delay").AddItem(new MenuItem("Edelayh1", "Delay E").SetValue(new Slider(670, 0, 2000)));

            menu.SubMenu("Harass").SubMenu("modeh2delay").AddItem(new MenuItem("Qdelayh2", "Delay Q").SetValue(new Slider(200, 0, 2000)));
            menu.SubMenu("Harass").SubMenu("modeh2delay").AddItem(new MenuItem("Wdelayh2", "Delay W").SetValue(new Slider(600, 0, 2000)));
            menu.SubMenu("Harass").SubMenu("modeh2delay").AddItem(new MenuItem("Edelayh2", "Delay E").SetValue(new Slider(120, 0, 2000)));

            menu.AddSubMenu(new Menu("Farming", "farm"));
            menu.SubMenu("farm").AddItem(new MenuItem("useQf", "Use Q").SetValue(true));
            menu.SubMenu("farm").AddItem(new MenuItem("useWf", "Use W").SetValue(true));
            menu.SubMenu("farm").AddItem(new MenuItem("useEf", "Use E").SetValue(true));

            menu.AddSubMenu(new Menu("Kill steal", "ks"));
            menu.SubMenu("ks").AddItem(new MenuItem("usQks", "Use Q").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("usWks", "Use W").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("usEks", "Use E").SetValue(true));
            menu.SubMenu("ks").AddItem(new MenuItem("useIgks", "Use Ignite").SetValue(false));

            menu.AddSubMenu(new Menu("Drawing", "draw"));
            menu.SubMenu("draw").AddItem(new MenuItem("enabledraw", "Enable drawing").SetValue(true));
            menu.SubMenu("draw").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            menu.SubMenu("draw").AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            menu.SubMenu("draw").AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            menu.SubMenu("draw").AddItem(new MenuItem("drawR", "Draw R").SetValue(true));

            menu.AddSubMenu(new Menu("Ward Jump", "wardjump"));
            menu.SubMenu("wardjump").AddItem(new MenuItem("wjkey", "Ward jump key").SetValue(new KeyBind("Z".ToCharArray()[0],KeyBindType.Press)));
            

            menu.AddToMainMenu();

            Game.OnUpdate+=Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            
            GameObject.OnCreate += GameObject_OnCreate;     
        }

        
        

        static void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            var isenable = menu.Item("enabledraw").GetValue<bool>();
            var drawQ = menu.Item("drawQ").GetValue<bool>();
            var drawW = menu.Item("drawW").GetValue<bool>();
            var drawE = menu.Item("drawE").GetValue<bool>();
            var drawR = menu.Item("drawR").GetValue<bool>();

            if (isenable)
            {
                if (drawQ)
                    Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);
                if (drawW)
                    Drawing.DrawCircle(player.Position, W.Range, Color.Green);
                if (drawE)
                    Drawing.DrawCircle(player.Position, E.Range, Color.White);
                if (drawR)
                    Drawing.DrawCircle(player.Position, R.Range, Color.Red);
            }
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(target=>target.IsEnemy&&target.IsValidTarget(20000)&&!target.IsInvulnerable))
            {
                float cbdmg = GetComboDamage(target);
                int percent = (int)((cbdmg / target.Health) * 100);
                var pos = Drawing.WorldToScreen(target.Position);
                if (percent <= 50)
                    Drawing.DrawText(pos.X, pos.Y, Color.Red, percent.ToString() + "%");
                if (percent >= 50 && percent <= 80)
                    Drawing.DrawText(pos.X, pos.Y, Color.Yellow, percent.ToString() + "%");
                if (percent >= 80 && percent <= 99)
                    Drawing.DrawText(pos.X, pos.Y, Color.Yellow, percent.ToString() + "%");
                if (percent >= 100)
                {
                    Drawing.DrawText(pos.X, pos.Y, Color.White, percent.ToString() + "%");
                    Drawing.DrawText(pos.X, pos.Y + 20, Color.White, "Killable");
                }
            }
            
                
        }
        
        static void Game_OnUpdate(EventArgs args)
        {
            if (player.IsChannelingImportantSpell() || player.HasBuff("KatarinaR") || player.HasBuff("katarinarsound", true))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);                                
            }
            else
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }
                      
            switch(Orbwalker.ActiveMode)
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

            KillSteal();
            var wardjump = menu.Item("wjkey").GetValue<KeyBind>().Active;

            if (wardjump)
                WardJump();
        }
        private static InventorySlot GetBestWardSlot()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }
        static void WardJump()
        {
            if (Environment.TickCount <= _lastPlaced + 3000 || !E.IsReady())
                return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = player.ServerPosition;
            Vector3 delta = cursorPos - myPos;

            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);
            InventorySlot invSlot = GetBestWardSlot();

            if (invSlot == null)
                return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            _lastWardPos = wardPosition;
            _lastPlaced = Environment.TickCount;

            E.Cast();
        }
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!E.IsReady() || !(sender is Obj_AI_Minion) || Environment.TickCount >= _lastPlaced + 300)
                return;

            if (Environment.TickCount >= _lastPlaced + 300) return;
            var ward = (Obj_AI_Minion)sender;

            if (ward.Name.ToLower().Contains("ward") && ward.Distance(_lastWardPos) < 500)
            {
                E.Cast(ward);
            }
        }
        static void Harass()
        {            
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var mode = menu.Item("hMode").GetValue<StringList>().SelectedIndex;
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                var Qdelay1 = menu.Item("Qdelayh1").GetValue<Slider>().Value;
                var Wdelay1 = menu.Item("Wdelayh1").GetValue<Slider>().Value;
                var Edelay1 = menu.Item("Edelayh1").GetValue<Slider>().Value;
                
                switch (mode)
                {
                    case 0:
                        {

                            if (Q.IsReady())
                                Utility.DelayAction.Add(Qdelay1, () => Q.CastOnUnit(target));
                                
                        }
                        break;
                    case 1:
                        {
                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                Utility.DelayAction.Add(Qdelay1, () => Q.CastOnUnit(target));
                                
                            if (W.IsReady() && W.IsInRange(target) && target.IsValidTarget(W.Range) && !Q.IsReady())
                                Utility.DelayAction.Add(Wdelay1, () => W.Cast());
                                                         
                           
                        }
                        break;
                    case 2:
                        {
                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                Utility.DelayAction.Add(Qdelay1, () => Q.CastOnUnit(target));
                                
                            if (E.IsReady() && target.IsValidTarget(E.Range) && !Q.IsReady())
                                Utility.DelayAction.Add(Edelay1, () => E.Cast(target));

                            if (W.IsReady() && !E.IsReady())
                                Utility.DelayAction.Add(Wdelay1, () => W.Cast());
                               
                           

                        }
                        break;
                    case 3:
                        {

                            var Qdelay2 = menu.Item("Qdelayh2").GetValue<Slider>().Value;
                            var Wdelay2 = menu.Item("Wdelayh2").GetValue<Slider>().Value;
                            var Edelay2 = menu.Item("Edelayh2").GetValue<Slider>().Value;
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                                Utility.DelayAction.Add(Edelay2, () => E.Cast(target));
                                
                            if (Q.IsReady() && target.IsValidTarget(Q.Range) && !E.IsReady())
                                Utility.DelayAction.Add(Qdelay2, () => Q.CastOnUnit(target));
                                                           
                            if (W.IsReady() && !Q.IsReady())
                                Utility.DelayAction.Add(Wdelay2, () => W.Cast());
                                
                           
                        }
                        break;
                }
            }

            
        }

        static void Combo()
        {
            
            if (Orbwalker.ActiveMode==Orbwalking.OrbwalkingMode.Combo)
            {
                var mode = menu.Item("cbMode").GetValue<StringList>().SelectedIndex;
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                var useIg = menu.Item("Use Ignite in combo").GetValue<bool>();
                
                
                switch(mode)
                {
                    case 0:
                        {
                            var Qdelay1 = menu.Item("Qdelaycb1").GetValue<Slider>().Value;
                            var Wdelay1 = menu.Item("Wdelaycb1").GetValue<Slider>().Value;
                            var Edelay1 = menu.Item("Edelaycb1").GetValue<Slider>().Value;
                            var Rdelay1 = menu.Item("Rdelaycb1").GetValue<Slider>().Value;

                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                Utility.DelayAction.Add(Qdelay1, () => Q.CastOnUnit(target));
                                
                            if (E.IsReady() && target.IsValidTarget(E.Range) && !Q.IsReady())
                                Utility.DelayAction.Add(Edelay1, () => E.Cast(target));

                            if (W.IsReady() && !E.IsReady())
                                Utility.DelayAction.Add(Wdelay1, () => W.Cast());
                                
                            if (R.IsReady() && !W.IsReady())
                            {
                                Orbwalker.SetAttack(false);
                                Orbwalker.SetMovement(false);
                                Utility.DelayAction.Add(Rdelay1, () => R.Cast());
                            }
                            if (useIg && IgniteSlot.IsReady() && !R.IsReady())
                                player.Spellbook.CastSpell(IgniteSlot, target);                           
                            
                        }
                        break;
                    case 1:
                        {
                            var Qdelay2 = menu.Item("Qdelaycb2").GetValue<Slider>().Value;
                            var Wdelay2 = menu.Item("Wdelaycb2").GetValue<Slider>().Value;
                            var Edelay2 = menu.Item("Edelaycb2").GetValue<Slider>().Value;
                            var Rdelay2 = menu.Item("Rdelaycb2").GetValue<Slider>().Value;

                            if (E.IsReady() && target.IsValidTarget(E.Range))
                                Utility.DelayAction.Add(Edelay2, () => E.Cast(target));

                            if (Q.IsReady() && !E.IsReady())
                                Utility.DelayAction.Add(Qdelay2, () => Q.CastOnUnit(target));
                                
                            if (W.IsReady() && !Q.IsReady())
                                Utility.DelayAction.Add(Wdelay2, () => W.Cast());
                               
                            if (R.IsReady() && !W.IsReady())
                            {
                                Orbwalker.SetAttack(false);
                                Orbwalker.SetMovement(false);
                                Utility.DelayAction.Add(Rdelay2, () => R.Cast());
                            }
                               
                            if (useIg && IgniteSlot.IsReady() && !R.IsReady())
                                player.Spellbook.CastSpell(IgniteSlot, target);
                            
                            
                        }
                        break;

                }     
            }
            
        }

        private static float GetComboDamage(Obj_AI_Hero enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q) + player.GetSpellDamage(enemy, SpellSlot.Q, 1);

            if (W.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady() || (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Surpressed && R.Level > 0))
                damage += player.GetSpellDamage(enemy, SpellSlot.R) * 8;


            return (float)damage;
        }
        
        static void KillSteal()
        {
            
            foreach(Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(target=>target.IsValidTarget(E.Range)
                                                                                                &&target.IsEnemy&&!target.IsInvulnerable))
            {
                var useQ = menu.Item("usQks").GetValue<bool>();
                var useW = menu.Item("usWks").GetValue<bool>();
                var useE = menu.Item("usEks").GetValue<bool>();
                var useIg = menu.Item("useIgks").GetValue<bool>();

                var Qdmg = (Q.GetDamage(target)) * overkill;
                var Wdmg = (W.GetDamage(target)) * overkill;
                var Edmg = (E.GetDamage(target)) * overkill;
                var Markdmg = (player.CalcDamage(target, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15)) * overkill;

                float ignitedmg;

                
                if (IgniteSlot != SpellSlot.Unknown)
                {
                    ignitedmg = (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                }
                else
                {
                    ignitedmg = 0f;
                }

                if (target.Health<ignitedmg&&useIg&&IgniteSlot.IsReady())
                {
                    player.Spellbook.CastSpell(IgniteSlot,target);
                }

                if (target.HasBuff("katarinaqmark") && target.Health < (Wdmg + Markdmg) && W.IsInRange(target) && W.IsReady() && useW)
                {
                    W.Cast();
                }

                if ( target.Health < Wdmg  && W.IsInRange(target) && W.IsReady() && useW)
                {
                    W.Cast();
                }

                if (target.Health<Qdmg&&Q.IsReady()&&useQ&&Q.IsInRange(target))
                {
                    Q.CastOnUnit(target);
                }

                if (target.Health < Edmg && E.IsReady() && useE && E.IsInRange(target))
                {
                    E.Cast(target);
                }

                if (target.HasBuff("katarinaqmark") && target.Health < (Edmg + Markdmg) && E.IsReady() && useE && E.IsInRange(target))
                {
                    E.Cast(target);
                }

                if (target.Health<(Edmg+Wdmg)&&E.IsReady()&&W.IsReady()&&E.IsInRange(target)&&useE&&useW)
                {
                    E.Cast(target);
                    W.Cast();
                }

                if (target.Health < (Edmg + Qdmg) && E.IsReady() && Q.IsReady() && E.IsInRange(target) && useE && useQ)
                {
                    E.Cast(target);
                    Q.CastOnUnit(target);
                }

                if ( target.Health < (Qdmg + Edmg + Wdmg + Markdmg) && Q.IsReady() && W.IsReady() && E.IsReady() && E.IsInRange(target))
                {
                    Q.CastOnUnit(target);
                    E.Cast(target);
                    W.Cast();
                }

                if (target.Health < (Qdmg + Edmg + Wdmg + Markdmg+ignitedmg) && Q.IsReady() && W.IsReady() && E.IsReady()&&IgniteSlot.IsReady() && E.IsInRange(target) && useIg)
                {
                    Q.CastOnUnit(target);
                    E.Cast(target);
                    W.Cast();
                    player.Spellbook.CastSpell(IgniteSlot, target);
                }
            }
        }

        static void Farm()
        {
            if (Orbwalker.ActiveMode==Orbwalking.OrbwalkingMode.LastHit)
            {
                foreach(var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion=>minion.IsEnemy&&minion.IsValidTarget(W.Range)&&!minion.IsInvulnerable))
                {
                    var Qdmg = Q.GetDamage(minion);
                    var Wdmg = W.GetDamage(minion);
                    var Edmg = E.GetDamage(minion);
                    var Markdmg = player.CalcDamage(minion, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15);

                    var useQ = menu.Item("useQf").GetValue<bool>();
                    var useW = menu.Item("useWf").GetValue<bool>();
                    var useE = menu.Item("useEf").GetValue<bool>();
                  
                    if (minion.Health<Qdmg&&Q.IsReady()&&Q.IsInRange(minion)&&useQ)
                    {
                        Q.CastOnUnit(minion);
                    }

                    if (minion.Health<Wdmg&&W.IsReady()&&W.IsInRange(minion)&&useW)
                    {
                        W.Cast();
                    }

                    if (minion.Health<Edmg&&E.IsReady()&&E.IsInRange(minion)&&useE)
                    {
                        E.Cast(minion);
                    }

                    if (minion.Health<Qdmg+Wdmg+Markdmg&&Q.IsReady()&&W.IsReady()&&W.IsInRange(minion)&&useQ&&useW)
                    {
                        Q.CastOnUnit(minion);
                        W.Cast();
                    }

                    if (minion.Health < Qdmg + Edmg + Markdmg && Q.IsReady() && E.IsReady() && Q.IsInRange(minion)&&useQ&&useE)
                    {
                        Q.CastOnUnit(minion);
                        E.Cast(minion);
                    }

                    if (minion.Health<Qdmg+Edmg+Wdmg+Markdmg&&Q.IsReady()&&W.IsReady()&&E.IsReady()&&Q.IsInRange(minion)&&useQ&&useW&&useE)
                    {
                        Q.CastOnUnit(minion);
                        E.Cast(minion);
                        W.Cast();
                    }
                }
            }            
        }

        static void LaneClear()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsEnemy && minion.IsValidTarget(W.Range) && !minion.IsInvulnerable))
                {
                    var Qdmg = Q.GetDamage(minion);
                    var Wdmg = W.GetDamage(minion);
                    var Edmg = E.GetDamage(minion);
                    var Markdmg = player.CalcDamage(minion, Damage.DamageType.Magical, player.FlatMagicDamageMod * 0.15 + Q.Level * 15);
                    
                    if (minion.Health < Qdmg && Q.IsReady() && Q.IsInRange(minion))
                    {
                        Q.CastOnUnit(minion);
                    }

                    if (minion.Health < Wdmg && W.IsReady() && W.IsInRange(minion))
                    {
                        W.Cast();
                    }

                    if (minion.Health < Edmg && E.IsReady() && E.IsInRange(minion))
                    {
                        E.Cast(minion);
                    }

                    if (minion.Health < Qdmg + Wdmg + Markdmg && Q.IsReady() && W.IsReady() && W.IsInRange(minion))
                    {
                        Q.CastOnUnit(minion);
                        W.Cast();
                    }

                    if (minion.Health < Qdmg + Edmg + Markdmg && Q.IsReady() && E.IsReady() && Q.IsInRange(minion))
                    {
                        Q.CastOnUnit(minion);
                        E.Cast(minion);
                    }

                    if (minion.Health < Qdmg + Edmg + Wdmg + Markdmg && Q.IsReady() && W.IsReady() && E.IsReady() && Q.IsInRange(minion))
                    {
                        Q.CastOnUnit(minion);
                        E.Cast(minion);
                        W.Cast();
                    }
                }
            }   
        }
    }
}
