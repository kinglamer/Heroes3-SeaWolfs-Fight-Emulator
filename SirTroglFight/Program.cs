using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SirTroglFight
{
    class Program
    {  

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            List<int[]> armyVariants = new List<int[]>();
            armyVariants.Add(new int[] { 80 });
            armyVariants.Add(new int[] { 74, 1, 1,1,1,1,1 });
            armyVariants.Add(new int[] { 34, 34, 2, 2, 2, 3, 3 });
            armyVariants.Add(new int[] { 12, 12, 12, 11, 11, 11, 11 });

            int _enemyHP = 40;
            Stack earthElemantal = new Stack();
            int repeats = 1000000;
            foreach (var army in armyVariants)
            {
                Log.Information("Армия {@army}", army);

                List<int> deathStrikes = new List<int>();
                List<int> rounds = new List<int>();
                List<int> morals = new List<int>();
                List<int> lucks = new List<int>();

                for (int i = 0; i < repeats; i++)
                {
                    earthElemantal.Push(17 * _enemyHP);
                    earthElemantal.Push(17 * _enemyHP);
                    earthElemantal.Push(17 * _enemyHP);
                    earthElemantal.Push(17 * _enemyHP);
                    earthElemantal.Push(16 * _enemyHP);
                    earthElemantal.Push(16 * _enemyHP);

                    Log.Debug("Битва №{i}", i);
                    FightEmulator emulator = new FightEmulator();
                    emulator.Fight(army, earthElemantal, _enemyHP);

                    deathStrikes.Add(emulator.DeathStrikeCounts);
                    rounds.Add(emulator.Rounds);
                    morals.Add(emulator.Moral);
                    lucks.Add(emulator.Luck);
                }

                PrintStats("Итого убито смертельными ударами ", deathStrikes, repeats);
                PrintStats("Итого раундов ", rounds, repeats);
                PrintStats("Итого удачи ", lucks, repeats);
                PrintStats("Итого морали ", morals, repeats);
            }
            Console.ReadLine();
        }

        static void PrintStats(string description, List<int> values, int repeats)
        {
            double avg = values.Average();
            decimal median = GetMedian(values.ToArray());
            int min = values.Min();
            int max = values.Max();    
            Log.Information("{@description} avg - {@avg}; median - {@median}; min - {@min}; max - {@max}.", description, avg, median, min, max);
        

            var q = from x in values
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };
            foreach (var x in q)
            {
                string persents = ((float)x.Count / repeats).ToString("0.00000%");
                Log.Information("Value: {@Value} Count: {@Count} Persentage: {@persents}", x.Value, x.Count, persents);
            }
        }

        static decimal GetMedian(int[] array)
        {
            int[] tempArray = array;
            int count = tempArray.Length;

            Array.Sort(tempArray);

            decimal medianValue = 0;

            if (count % 2 == 0)
            {               
                int middleElement1 = tempArray[(count / 2) - 1];
                int middleElement2 = tempArray[(count / 2)];
                medianValue = (middleElement1 + middleElement2) / 2;
            }
            else
            {               
                medianValue = tempArray[(count / 2)];
            }

            return medianValue;
        }
        
    }
}
