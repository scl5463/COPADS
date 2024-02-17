﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers1
{
	class Program
	{
		static void Main(string[] args)
		{
			var philosophers = new Philosophers().InitializePhilosophers();
			var phEatTasks = new List<Task>();

			using (var stopDiningTokenSource = new CancellationTokenSource())
			{
				var stopDiningToken = stopDiningTokenSource.Token;
				foreach (var ph in philosophers)
					phEatTasks.Add(
						Task.Factory.StartNew(() => ph.EatingHabit(stopDiningToken), stopDiningToken)
							.ContinueWith(_ => {
								Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} ERROR...    Ph{ph.Name} FALTED AND LOST DINING PRIVILEGES ...");
							}, TaskContinuationOptions.OnlyOnFaulted)
							.ContinueWith(_ => {
								Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff}             Ph{ph.Name} HAS LEFT THE TABLE ...");
							}, TaskContinuationOptions.OnlyOnCanceled)
					);

				// Allow philosophers to dine for DurationAllowPhilosophersToEat
				// milliseconds.  Original problem have philosophers dine 
				// forever, but we are not patient enough to wait until 
				// forever...
				Task.Delay(ConfigValue.Inst.DurationPhilosophersEat).Wait();

				try
				{
					// After a duration of DurationAllowPhilosophersToEat we
					// ask the philosophers to stop dining.
					stopDiningTokenSource.Cancel();
					Task.WaitAll(phEatTasks.ToArray());
				}
				catch (AggregateException ae)
				{
					foreach (var ex in ae.Flatten().InnerExceptions)
						Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} {ex.GetType().Name}:  {ex.Message}");
				}
			}

			// Done--so say so
			Console.WriteLine("Done.");

			// Write some statistics down
			Console.WriteLine();
			var totalEatCount = philosophers.Sum(p => p.EatCount);
			var totalEatingTime = philosophers.Sum(p => p.TotalEatingTime);
			var totalEatingConflicts = philosophers.Sum(p => p.EatingConflictCount);
			foreach (var ph in philosophers)
				Console.WriteLine($"Philosopher Ph{ph.Name} ate {ph.EatCount,3} times.  " +
					$"For a total of {ph.TotalEatingTime:#,##0} milliseconds.  " +
					$"Eating conflicts: {ph.EatingConflictCount}.");
			Console.WriteLine($"Collectively philosophers ate {totalEatCount} times.  For a total of {totalEatingTime:#,##0} milliseconds.  Eating conflicts: {totalEatingConflicts}");

			Console.WriteLine();
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
	}
}


