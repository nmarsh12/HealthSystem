using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection.Emit;

namespace HealthSystem
{
    internal class Program
    {
        // Had to absolutely massacre my code to fit the test code, class based code should be in an older commit
        static int maxHealth;
        static int maxShield;
        static int health;
        static int shield;
        static int lives;
        static string healthStatus;
        static int xp = 0;
        static int level = 1;

        static void Main(string[] args)
        {
            shield = 0;
            health = 50;
            lives = 3;
            TakeDamage(10);
            Console.WriteLine(shield);
            Console.WriteLine(health);
            UnitTestHealthSystem();
            UnitTestXPSystem();
            Console.ReadKey();
        }

        static void ShowHUD()
        {
            Console.WriteLine(health);
            Console.WriteLine(shield);
            Console.WriteLine(healthStatus);
            Console.WriteLine(lives);
            Console.WriteLine();
        }

        static void CheckForRevives()
        {
            if (health <= 0)
            {
                health = ClampInteger(health, 0, maxHealth);
                if (lives > 0)
                {
                    Revive();
                }
            }
        }
        static void IncreaseXP(int exp)
        {
            xp += exp;
            RecursiveXPCheck();
        }

        static void RecursiveXPCheck()
        {
            if (xp >= level * 100)
            {
                xp -= level * 100;
                level += 1;
                RecursiveXPCheck();
            }
        }

        static void Heal(int amountHealed)
        {
            health += amountHealed;
            health = ClampInteger(health, 0, maxHealth);
        }

        static void RegenerateShield(int shieldAmountHealed)
        {
            shield += shieldAmountHealed;
            shield = ClampInteger(shield, 0, maxShield);
        }

        static int ClampInteger(int value, int minimum, int maximum)
        {
            if (value > maximum)
            {
                return maximum;
            }
            else if (value < minimum)
            {
                return minimum;
            }
            else
            {
                 return value;
            }
        }
        static void TakeDamage(int damageTaken)
        {
            int shieldSpillDamage;
            if (shield > 0)
            {
                shield -= damageTaken;
                if (shield < 0)
                {
                    shieldSpillDamage = shield * -1;
                    health -= shieldSpillDamage;
                    shield = ClampInteger(shield, 0, maxShield);
                }
            }
            else if (health > 0)
            {
                health -= damageTaken;
                health = ClampInteger(health, 0, maxHealth);
            }
        }
        static void Revive()
        {
            health = maxHealth;
            shield = maxShield;
            lives -= 1;
        }

        static void Refresh()
        {
            CheckForRevives();
        }

        static void UnitTestHealthSystem()
        {
            Debug.WriteLine("Unit testing Health System started..."); // None of these print anything, idk why

            // TakeDamage()

            // TakeDamage() - only shield
            shield = 100;
            health = 100;
            lives = 3;
            TakeDamage(10);
            Debug.Assert(shield == 90);
            Debug.Assert(health == 100);
            Debug.Assert(lives == 3);

            // TakeDamage() - shield and health
            shield = 10;
            health = 100;
            lives = 3;
            TakeDamage(50);
            Debug.Assert(shield == 0);
            Debug.Assert(health == 60);
            Debug.Assert(lives == 3);

            // TakeDamage() - only health
            shield = 0;
            health = 50;
            lives = 3;
            TakeDamage(10);
            Debug.Assert(shield == 0);
            Debug.Assert(health == 40);
            Debug.Assert(lives == 3);

            // TakeDamage() - health and lives
            shield = 0;
            health = 10;
            lives = 3;
            TakeDamage(25);
            Debug.Assert(shield == 0);
            Debug.Assert(health == 0);
            Debug.Assert(lives == 3);

            // TakeDamage() - shield, health, and lives
            shield = 5;
            health = 100;
            lives = 3;
            TakeDamage(110);
            Debug.Assert(shield == 0);
            Debug.Assert(health == 0);
            Debug.Assert(lives == 3);

            // TakeDamage() - negative input
            shield = 50;
            health = 50;
            lives = 3;
            TakeDamage(-10);
            Debug.Assert(shield == 50);
            Debug.Assert(health == 50);
            Debug.Assert(lives == 3);

            // Heal()

            // Heal() - normal
            shield = 0;
            health = 90;
            lives = 3;
            Heal(5);
            Debug.Assert(shield == 0);
            Debug.Assert(health == 95);
            Debug.Assert(lives == 3);

            // Heal() - already max health
            shield = 90;
            health = 100;
            lives = 3;
            Heal(5);
            Debug.Assert(shield == 90);
            Debug.Assert(health == 100);
            Debug.Assert(lives == 3);

            // Heal() - negative input
            shield = 50;
            health = 50;
            lives = 3;
            Heal(-10);
            Debug.Assert(shield == 50);
            Debug.Assert(health == 50);
            Debug.Assert(lives == 3);

            // RegenerateShield()

            // RegenerateShield() - normal
            shield = 50;
            health = 100;
            lives = 3;
            RegenerateShield(10);
            Debug.Assert(shield == 60);
            Debug.Assert(health == 100);
            Debug.Assert(lives == 3);

            // RegenerateShield() - already max shield
            shield = 100;
            health = 100;
            lives = 3;
            RegenerateShield(10);
            Debug.Assert(shield == 100);
            Debug.Assert(health == 100);
            Debug.Assert(lives == 3);

            // RegenerateShield() - negative input
            shield = 50;
            health = 50;
            lives = 3;
            RegenerateShield(-10);
            Debug.Assert(shield == 50);
            Debug.Assert(health == 50);
            Debug.Assert(lives == 3);

            // Revive()

            // Revive()
            shield = 0;
            health = 0;
            lives = 2;
            Revive();
            Debug.Assert(shield == 100);
            Debug.Assert(health == 100);
            Debug.Assert(lives == 1);

            Debug.WriteLine("Unit testing Health System completed.");
            Console.Clear();
        }
        static void UnitTestXPSystem()
        {
            Debug.WriteLine("Unit testing XP / Level Up System started...");

            // IncreaseXP()

            // IncreaseXP() - no level up; remain at level 1
            xp = 0;
            level = 1;
            IncreaseXP(10);
            Debug.Assert(xp == 10);
            Debug.Assert(level == 1);

            // IncreaseXP() - level up to level 2 (costs 100 xp)
            xp = 0;
            level = 1;
            IncreaseXP(105);
            Debug.Assert(xp == 5);
            Debug.Assert(level == 2);

            // IncreaseXP() - level up to level 3 (costs 200 xp)
            xp = 0;
            level = 2;
            IncreaseXP(210);
            Debug.Assert(xp == 10);
            Debug.Assert(level == 3);

            // IncreaseXP() - level up to level 4 (costs 300 xp)
            xp = 0;
            level = 3;
            IncreaseXP(315);
            Debug.Assert(xp == 15);
            Debug.Assert(level == 4);

            // IncreaseXP() - level up to level 5 (costs 400 xp)
            xp = 0;
            level = 4;
            IncreaseXP(499);
            Debug.Assert(xp == 99);
            Debug.Assert(level == 5);

            Debug.WriteLine("Unit testing XP / Level Up System completed.");
            Console.Clear();
        }
    }
}



