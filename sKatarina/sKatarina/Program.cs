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

    public class Program
    {
        public static Obj_AI_Hero player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker orbwalker;
        public static Spell Q, Q2, W, E, E2, R;
        public static Items.Item Tiamat, Hydra;
        public static Menu menu;
        public static SpellSlot Ignite;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Katarina.Katarina_OnGameLoad;
        }

    }     
}
