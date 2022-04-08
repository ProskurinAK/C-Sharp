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
// В качестве параметра принимает объект делегата ParameterizedThreadStart, этот делегат представляет действие, при котором возможна передача параметров в поток и возврат из него
// При создании нового потока используется делегат и лямбда-выражение

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread MyThread = new Thread((object Value) =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("--- " + i * (int)Value);
                    Thread.Sleep(500);
                }
            });
            MyThread.Start(2);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Main thread - " + i);
                Thread.Sleep(500);
            }
        }
    }
}