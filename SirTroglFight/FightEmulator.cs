using Serilog;
using System;
using System.Collections;

namespace SirTroglFight
{
    public class FightEmulator
    {
        private double _currentHP = 0.0;
        private Random _random = new Random();

        /// <summary>
        /// сколько убили с помощью смертельного удара
        /// </summary>
        public int DeathStrikeCounts = 0;

        /// <summary>
        /// за сколько раундов завершился бой
        /// </summary>
        public int Rounds = 0;

        /// <summary>
        /// сколько раз выпала мораль
        /// </summary>
        public int Moral = 0;

        /// <summary>
        /// сколько раз выпала удача
        /// </summary>
        public int Luck = 0; 

        public void Fight(int[] myArmy, Stack enemy, int enemyHP)
        { 
            while (enemy.Count > 0)
            {
                _currentHP = Convert.ToDouble(enemy.Pop());
                Log.Debug($"Выбор нового существа - его хп {_currentHP}");
                while (_currentHP > 0)
                {
                    Rounds++;
                    Log.Debug($"Раунд №{Rounds}");
                    foreach (var squad in myArmy)
                    {
                        DeathStrikeCounts += Attack(squad, enemyHP);
                        if (_currentHP <= 0)
                        {
                            if (enemy.Count > 0)
                                _currentHP = Convert.ToDouble(enemy.Pop());
                            else
                                break;
                        }

                        if (IsMoral())
                        {
                            Moral++;
                            Log.Debug($"Морские волки почувствовали мораль");
                            DeathStrikeCounts += Attack(squad, enemyHP);

                            if (_currentHP <= 0)
                            {
                                if (enemy.Count > 0)
                                    _currentHP = Convert.ToDouble(enemy.Pop());
                                else
                                    break;
                            }
                        }
                        Log.Debug($"У существа осталось {_currentHP} хп");
                    }

                }
            }
            Log.Debug($"Морские волки убили {DeathStrikeCounts} смертельными ударами за {Rounds} раундов - {(float)DeathStrikeCounts / Rounds}");         
        }

        private int Attack(int sqad, int enemyHP)
        {
            double dmg = CalculateDamage(sqad);
            _currentHP -= dmg;

            if (_currentHP <= 0)
                return 0;

            if (IsLucky())
            {
                Luck++;
                Log.Debug($"Морские волки поймали удачу и нанесли еще {dmg}");
                _currentHP -= dmg;
            }

            if (_currentHP <= 0)
                return 0;

            double death = DeathStrike(sqad, enemyHP);
            if (death > 0)
            {
                _currentHP -= death;
                Log.Debug($"Морские волки нанесли смертельный удар {death / enemyHP} элементалям");
                return Convert.ToInt32(death / enemyHP);
            }

            return 0;
        }

        private double DeathStrike(int cnt, int enemyHP)
        {
            double chanse = (double)(cnt * 3) / (double)(100);
            int maxKills = Convert.ToInt32(Math.Ceiling(chanse));
            int killed = 0;
            for (int i = 1; i <= cnt; i++)
            {
                if (_random.Next(1, 101) <= 3)
                    killed++;
            }
            if (killed > maxKills)
                killed = maxKills;
            return enemyHP * killed;
        }

        private double CalculateDamage(int cnt)
        {
            var val = GetRandomNumber(6.4, 12.5) * cnt;
            Log.Debug($"Морские волки ({cnt}) наносят {val} урона");
            return val;
        }

        private bool IsLucky()
        {
            var val = GetRandomNumber(1.0, 101.0);
            return val <= 4.2 ? true : false; //1 lucks
        }

        private bool IsMoral()
        {
            var val = GetRandomNumber(1.0, 101.0);
            return val <= 8.3 ? true : false; //2 moral
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
