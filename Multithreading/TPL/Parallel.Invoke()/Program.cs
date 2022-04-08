using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
// ************************************************************************************************************

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch StopW = new Stopwatch();	// Объект для подсчёта времени выполнения программы
            StopW.Start();

            Console.WriteLine("Main thread start");

            //FirstFunction();
            //SecondFunction();

            // --------------------------------------------------------------------
			// выполнение метода Main() приостанавливается до тех пор, пока не произойдет возврат из метода Invoke(). Следовательно, метод Main(),
			// в отличие от методов FirstFunction() и SecondFunction(), не выполняется параллельно. Поэтому применять метод Invoke() 
			// нельзя в том случае, если требуется, чтобы исполнение вызывающего потока продолжалось.

            Parallel.Invoke(FirstFunction, SecondFunction);	// Методы First и Second выполняются параллельно 

            Console.WriteLine("Main thread stop");

            StopW.Stop();
            Console.WriteLine("Time = " + StopW.Elapsed);
        }

        static void FirstFunction()
        {
            for (int i = 0; i < 7000; i++)
            {
                Console.WriteLine("First function - " + i);
            }
        }

        static void SecondFunction()
        {
            for (int i = 0; i < 7000; i++)
            {
                Console.WriteLine("Second function - " + i);
            }
        }
    }
}