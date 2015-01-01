using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace HowManyAutos
{
    class Program
    {
        static Dictionary<int, double> counts;
        static Obj_AI_Hero attacking;
        static Menu noobmenu;
        static double killme;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad(EventArgs args)
        {
            counts = new Dictionary<int,double>();
            LoadMenu();
            Game.PrintChat("HowManyAutos Loaded!");
            Game.OnGameUpdate += OnGameUpdate;
            Orbwalking.OnAttack += OnAttack;
            
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && !hero.IsDead))
            {
                counts[enemy.NetworkId] = 0;
                var enemyText = new Render.Text(0, 0, String.Empty, 20, SharpDX.Color.PeachPuff)
                {
                    OutLined = true,
                    PositionUpdate = () => enemy.HPBarPosition + new SharpDX.Vector2(-28, 17),
                    TextUpdate = delegate
                    {
                        return String.Format("{0:0}" + " AA", counts[enemy.NetworkId]);
                    },

                    VisibleCondition = sender => noobmenu.Item("showonenemies").GetValue<bool>() && !enemy.IsDead //&& enemy.IsVisible
                };
                enemyText.Add();
            }

            var selfText = new Render.Text(0, 0, String.Empty, 20, SharpDX.Color.RosyBrown)
            {
                OutLined = true,
                PositionUpdate = () => ObjectManager.Player.HPBarPosition + new SharpDX.Vector2(-15, 7),
                TextUpdate = delegate
                {
                    return String.Format("{0:0}" + " AA", killme);
                },

                VisibleCondition = sender => noobmenu.Item("showonme").GetValue<bool>() && !ObjectManager.Player.IsDead
            };
            selfText.Add();
        }

        static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (target is Obj_AI_Hero)
                {
                    attacking = (Obj_AI_Hero)target;
                } else{
                    attacking = null;
                }
            }
        }

        static void OnGameUpdate(EventArgs args)
        {
            double AA = 1;
            double AA2 = 1;
            var player = ObjectManager.Player;

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && !hero.IsDead))
            {
                if (noobmenu.Item("calccrit").GetValue<bool>())
                {
                    if (Items.HasItem(3031, player))
                    {
                        AA = player.GetAutoAttackDamage(enemy, true) * (1 + player.Crit);
                    }
                    else
                    {
                        AA = player.GetAutoAttackDamage(enemy, true) * (1 + 1.5 * player.Crit);
                    }
                }
                else
                {
                    AA = player.GetAutoAttackDamage(enemy, true);
                }
                counts[enemy.NetworkId] = Math.Ceiling(enemy.Health / AA);
            }

            if (attacking != null)
            {
                if (noobmenu.Item("calccrit").GetValue<bool>())
                {
                    if (Items.HasItem(3031, attacking))
                    {
                        AA2 = attacking.GetAutoAttackDamage(player, true) * (1 + attacking.Crit);
                    }
                    else
                    {
                        AA2 = attacking.GetAutoAttackDamage(player, true) * (1 + 1.5 * attacking.Crit);
                    }
                }
                else
                {
                    AA2 = attacking.GetAutoAttackDamage(player, true);
                }

                killme = Math.Ceiling(player.Health / AA2);
            }
            else
            {
                killme = 0;
            }
  


        }

        static void LoadMenu()
        {
            noobmenu = new Menu("How Many Autos", "How Many Autos", true);
            noobmenu.AddItem(new MenuItem("showonenemies", "Show Autos to kill Enemy").SetValue(true));
            noobmenu.AddItem(new MenuItem("showonme", "Show Autos to kill Me").SetValue(true));
            noobmenu.AddItem(new MenuItem("calccrit", "Include critical damage").SetValue(false));
            noobmenu.AddToMainMenu();
        }
    }
}
