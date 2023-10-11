using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Entity Player = new Entity(100, 100, 3);
            HUDManager HUD = new HUDManager(Player); 
            // HUDManager doesn't work with test code I bet, probably scrap for something simpler since there's no real input
            ReviveChecker reviveChecker = new ReviveChecker(Player);
            bool running = true;

            void OnStart()
            {
                Console.CursorVisible = false;
            }

            OnStart();

            while (running)
            {
                HUD.ClearHUD();
                HUD.ShowHUD();
                HUD.WaitForInput();
                Player.TakeDamage(5);
                reviveChecker.CheckForRevivesEveryUpdateForSomeReason();
            }
        }
    }

    class Entity
    {
        public int maxHealth;
        public int maxShield;
        public int health;
        public int shield;
        public int lives;
        public string healthStatus;

        public Entity(int _MaxHealth, int _MaxShield, int _Lives = 0)
        {
            this.maxHealth = _MaxHealth;
            this.maxShield = _MaxShield;
            this.lives = _Lives;

            health = maxHealth;
            shield = maxShield;
            healthStatus = "Alive";
        }

        public void TakeDamage(int damageTaken)
        {
            int shieldSpillDamage; // TODO make damage spill
            if (shield > 0)
            {
                shield -= damageTaken;
            }
            else
            {
                health -= damageTaken;
            }
            // Don't call revive?????????
            // Checker system seems inefficient as hell 
            // Calling revive manually just doesn't work in reality so it's really just bad practice
            // Checker system it is
        }

        void Heal(int amountHealed)
        {
            health += amountHealed;
        }

        void RegenerateShield(int shieldAmountHealed)
        {
            shield += shieldAmountHealed;
        }

        public void Revive()
        {
            health = maxHealth;
            shield = maxShield;
            lives -= 1;
        }
    }

    class HUDManager
    {
        Entity Player;
        public HUDManager(Entity _Player)
        {
            this.Player = _Player;
        }

        public void ClearHUD() // This is a nasty solution but works
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < 10; i++) // TODO replace 10 with screen height
            {
                Console.WriteLine("                                        ");
            }

            Console.SetCursorPosition(0, 0);
        }

        public void ShowHUD() 
        {
            // don't use past testing
            Console.WriteLine(Player.health);
            Console.WriteLine(Player.shield);
            Console.WriteLine(Player.healthStatus);
            Console.WriteLine(Player.lives);
            Console.WriteLine();
        }

        public ConsoleKeyInfo WaitForInput()
        {
            return Console.ReadKey(true);
        }
    }

    class ReviveChecker
    {
        Entity targetEntity;
        public ReviveChecker(Entity _TargetEntity)
        {
            this.targetEntity = _TargetEntity;
        }
        public void CheckForRevivesEveryUpdateForSomeReason()
        {
            if (targetEntity.health <= 0) { 
                targetEntity.health = 0;
                if (targetEntity.lives > 0) {
                    targetEntity.Revive();
                }
            }
        }
    }
}

