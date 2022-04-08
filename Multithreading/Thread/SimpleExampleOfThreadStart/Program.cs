using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
// ************************************************************************************************************

// Пример по созданию нового потока MyThread который отробатывает параллельно с главным потоком
// В качестве параметра принимает объект делегата ThreadStart, этот делегат представляет действие, которое не принимает никаких параметров
// и не возвращает никакого значения
// При создании нового потока используется делегат и лямбда-выражение

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread MyThread = new Thread(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("My thread - " + i);
                }
            });
            MyThread.Start();

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("Main thread - " + i);
            }
        }
    }
}