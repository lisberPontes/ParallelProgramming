using System;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreSlim
{
	class Program
	{
		private static System.Threading.SemaphoreSlim semaphore;
		// A padding interval to make the output more orderly.
		private static int padding;

		static async Task Main(string[] args)
		{
			// Create the semaphore.
			semaphore = new System.Threading.SemaphoreSlim(0, 3);
			Console.WriteLine("{0} tasks can enter the semaphore.",
				semaphore.CurrentCount);
			Task[] tasks = new Task[5];

			// Create and start five numbered tasks.
			for (int i = 0; i <= 4; i++)
			{
				tasks[i] = Task.Run(() => {
					// Each task begins by requesting the semaphore.
					Console.WriteLine("Task {0} begins and waits for the semaphore.",
						Task.CurrentId);
					semaphore.Wait();

					Interlocked.Add(ref padding, 100);

					Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);

					// The task just sleeps for 1+ seconds.
					Thread.Sleep(1000 + padding);

					Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.",
						Task.CurrentId, semaphore.Release());
				});
			}

			// Wait for half a second, to allow all the tasks to start and block.
			Thread.Sleep(500);

			// Restore the semaphore count to its maximum value.
			Console.WriteLine("Main thread calls Release(3) --> ");
			semaphore.Release(3);
			Console.WriteLine("   {0} tasks can enter the semaphore.",semaphore.CurrentCount);
			// Main thread waits for the tasks to complete.
			await Task.WhenAll(tasks);
			Console.WriteLine("Main thread exits.");

			Console.ReadLine();
		}
	}
}