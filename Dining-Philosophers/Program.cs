using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dining_Philosophers
{
    class Program
    {
        static void Main()
        {
            //int temp;
            //temp = Console.Read();
            Console.Write("Please enter the amount of philosopers:");
            DinnerTable.num_philosophers = int.Parse(Console.ReadLine()); //A little help from http://forum.codecall.net/topic/40488-question-about-c-user-int-input/
            Console.Write("Please enter the minimum time delay for eating and thinking (in seconds): ");
            DinnerTable.rand_min = int.Parse(Console.ReadLine());
            Console.Write("Please enter the maximum time delay for eating and thinking (in seconds): ");
            DinnerTable.rand_max = int.Parse(Console.ReadLine());
            //DinnerTable.num_philosophers = num_philosophers;
            //User input space ends

            DinnerTable.phil_states = new DinnerTable.states[DinnerTable.num_philosophers];
            DinnerTable.forks = new object[DinnerTable.num_philosophers];
            //DinnerTable[] problem = new DinnerTable[num_philosophers];
            Thread[] philosophers = new Thread[DinnerTable.num_philosophers];
            for (int i = 0; i < DinnerTable.num_philosophers; i++)
            {
                // Spawning threads http://msdn.microsoft.com/en-us/library/aa645740(v=vs.71).aspx
                philosophers[i] = new Thread(new ThreadStart(DinnerTable.run));
                philosophers[i].Start();
            }
        }

        class DinnerTable
        {
            /*
             * http://msdn.microsoft.com/en-us/library/c5kehkcz.aspx (Locks for critical region)
             * http://msdn.microsoft.com/en-us/library/system.threading.monitor.aspx (Entering the object using Monitor)
             */
            public static int num_philosophers;
            public enum states { t, h, e };
            public static object[] forks;// = new object [5];// = new object[num_philosophers]; // Locks the different forks
            private static Object fork_lock1 = new Object(); //locks when putting fork putting forks down
            private static Object fork_lock2 = new Object();  // locks when taking forks
            public static states[] phil_states = new states[num_philosophers]; //keeps states of philosophers
            private static Object num_lock = new Object();// = new Object();  // locks when taking a number
            //public static int phil_num; //Philosophers number
            private static int next_phil_num = 0;// = 0;
            public static int rand_max, rand_min;
            private static Random rand = new Random(); //Generating random numbers http://stackoverflow.com/questions/13539974/random-number-generator-c-sharp
            private static Object print_lock = new Object();
            /*public DinnerTable()
            {
                
                rand = new Random();
                //next_phil_num = 0;
                phil_states = new states[num_philosophers];
                //fork_lock1 = new Object();
                //fork_lock2 = new Object();
                forks = new object[num_philosophers];
            }*/


            public static void run()
            {
                for (int i = 0; i < forks.Length; i++)
                {
                    forks[i] = new object();
                }
                //Console.WriteLine("I am running");
                int phil_num = assign_num();
                while (true)
                {
                    //Thanks Dr. Tanenbaum MOS 
                    think();
                    take_forks(phil_num);
                    eat();
                    put_forks(phil_num);
                    //print();
                }
            }

            private static int assign_num()
            {
                int temp;
                lock (num_lock)
                {
                    temp = next_phil_num;
                    next_phil_num++;
                }
                // Console.WriteLine("{0} philosopher is running", temp);
                return temp;
            }

            private static void put_forks(int i)
            {
                lock (fork_lock1)
                {
                    // Console.WriteLine("{0} is putting forks down", i);
                    Monitor.Exit(forks[i]);
                    Monitor.Exit(forks[(i + 1) % num_philosophers]);
                    phil_states[i] = states.t;
                }
                print();
            }

            private static void take_forks(int i)
            {
                phil_states[i] = states.h;
                print();
                //Console.WriteLine("{0}", num_philosophers);
                lock (fork_lock2)
                {
                    Monitor.Enter(forks[i]);
                    Monitor.Enter(forks[(i + 1) % num_philosophers]);
                    //Console.WriteLine("{0} has taken a fork", i);
                    phil_states[i] = states.e;
                }
                print();
            }
            private static void print()
            {
                lock (print_lock)
                {
                    for (int i = 0; i < num_philosophers; i++)
                    {
                        Console.Write("{0}    ", phil_states[i]);
                    }
                    Console.WriteLine("   ");
                }
            }

            private static void eat()
            {
                Thread.Sleep(rand.Next(rand_min, rand_max) * 1000);
            }

            private static void think()
            {
                Thread.Sleep(rand.Next(rand_min, rand_max) * 1000);
            }
        }
    }
}
