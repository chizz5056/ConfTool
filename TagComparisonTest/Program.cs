using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;


using FixSchema;

namespace TagComparisonTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Task t;
            var tasks = new ConcurrentBag<Task>();

            Console.WriteLine("Press any key to begin tasks...");
            Console.WriteLine("To terminate the example, press 'c' to cancel and exit...");
            Console.ReadKey();
            Console.WriteLine();

            // Request cancellation of a single task when the token source is canceled.  
            // Pass the token to the user delegate, and also to the task so it can   
            // handle the exception correctly.
            t = Task.Factory.StartNew(() => runTask(1, token), token);
            Console.WriteLine("Task {0} executing", t.Id);

            if (Console.ReadKey().KeyChar == 'c')
            {
                tokenSource.Cancel();
                Console.WriteLine("\nTask cancellation requested.");

                // Optional: Observe the change in the Status property on the task.  
                // It is not necessary to wait on tasks that have canceled. However,  
                // if you do wait, you must enclose the call in a try-catch block to  
                // catch the TaskCanceledExceptions that are thrown. If you do   
                // not wait, no exception is thrown if the token that was passed to the   
                // StartNew method is the same token that requested the cancellation. 
            }

            Console.WriteLine("ENDED");
            Console.ReadKey();


            //Console.WriteLine("Press any key to stop");
            //var tokenSource2 = new CancellationTokenSource();
            //CancellationToken ct = tokenSource2.Token;
            //var task = Task.Factory.StartNew(() =>
            //{
            //    ct.ThrowIfCancellationRequested();

            //    runTask();

            //}, tokenSource2.Token);
            
            //do
            //{
            //    while (!Console.KeyAvailable)
            //    {               
            //    }
            //} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            //tokenSource2.Cancel();

            //Console.WriteLine("ENDED");
            //Console.ReadKey();

            //var tokenSource2 = new CancellationTokenSource();
            //CancellationToken ct = tokenSource2.Token;

            //var task = Task.Factory.StartNew(() =>
            //{
            //    ct.ThrowIfCancellationRequested();

            //    runTask();

            //}, tokenSource2.Token);

            //Console.WriteLine("Press ESC to stop");
            //do
            //{
            //    while (!Console.KeyAvailable)
            //    {
            //        Console.WriteLine("Detected ESC press");
            //        tokenSource2.Cancel();
            //        try
            //        {
            //            task.Wait();
            //        }
            //        catch (AggregateException e)
            //        {
            //            foreach (var v in e.InnerExceptions)
            //                Console.WriteLine(e.Message + " " + v.Message);
            //        }

            //        Console.ReadKey();
            //    }
            //} while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            //Task task1 = new Task(new Action(runTask));
            //task1.Start();
            //Console.ReadKey();
        }

        private static void runTask(int taskNum, CancellationToken ct)
        {

            // Was cancellation already requested?  
            if (ct.IsCancellationRequested == true)
            {
                Console.WriteLine("Task {0} was cancelled before it got started.",
                                  taskNum);
                ct.ThrowIfCancellationRequested();
            }

            for (int i = 1; i < 60; i++)
            {
                
                Console.WriteLine("TaskNum {0}: {1}", taskNum.ToString(), i.ToString());
                Thread.Sleep(1000);

                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Task {0} cancelled", taskNum);
                    ct.ThrowIfCancellationRequested();
                } 
            }  


            //Schema schema = new Schema();
            //Field f = Fields.Instance.GetField(31);
            //string left = "0.0000";
            //string right = "0.000000";

            //bool test = FixDataTypes.Compare(f, left, right);
            //Console.WriteLine(test.ToString());

          
        }
    }
}
