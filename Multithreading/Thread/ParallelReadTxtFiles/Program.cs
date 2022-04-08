using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
// ************************************************************************************************************

// Пример программы считывающей текстовые файлы в многопоточном режиме
// Функция CreateDataSet() вызывает в многопоточном формате функцию ReadDataset(object Obj) и производит запись данных в список
// Функция ReadDataset(object Obj) принимает параметром структуру с заданными границами считываемых файлов и списком в который эти файлы запишутся
// СОЗДАНИЕ(Thread.Start()) И ОЖИДАНИЕ(Thread.Join()) ПОТОКОВ ДОЛЖНО НАХОДИТЬСЯ В РАЗНЫХ ЦИКЛАХ

namespace TestProject
{
    class Program
    {
        static List<List<double>> DataSetFeatures = new List<List<double>>();   // Массив для записи данных

        struct ThreadRange
        {
            public int Start;
            public int Stop;
            public List<List<double>> DataInRange;
        }
        // ------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            //Stopwatch StopW = new Stopwatch();
            //StopW.Start();

            //ThreadRange FirstData = new ThreadRange();
            //FirstData.Start = 0;
            //FirstData.Stop = 35;
            //FirstData.DataInRange = DataSetFeatures;

            //ReadDataset(FirstData);

            //ShowInfo();

            //StopW.Stop();
            //Console.WriteLine("Time = " + StopW.Elapsed);

            // -------------------------------------------------------------------------------------------------

            Stopwatch StopW = new Stopwatch();
            StopW.Start();

            CreateDataSet();
            ShowInfo();

            StopW.Stop();
            Console.WriteLine("Time = " + StopW.Elapsed);
        }
        // ------------------------------------------------------------------------------------------------------------
        static void CreateDataSet()
        {
            int AmountOfThread = 7; // Количество создаваемых потоков
            int[,] Borders = { { 0, 5 }, { 5, 10 }, { 10, 15 }, { 15, 20 }, { 20, 25 }, { 25, 30 }, { 30, 35 } };   // Границы считываемых файлов с данными, в нутри каталога
            List<ThreadRange> ListThreadRange = new List<ThreadRange>();    // Список структур с данными в заданных границах

            // Цикл создания структур с данными в заданных гнраницах и добавление их в список
            for (int i = 0; i < AmountOfThread; i++)
            {
                List<List<double>> Data = new List<List<double>>();

                ThreadRange NewTreadRange = new ThreadRange();
                NewTreadRange.Start = Borders[i, 0];
                NewTreadRange.Stop = Borders[i, 1];
                NewTreadRange.DataInRange = Data;

                ListThreadRange.Add(NewTreadRange);
            }

            Thread[] Threads = new Thread[AmountOfThread];  // Массив создаваемых потоков

            // Цикл создания новых потоков и добавления их в массив
            for (int i = 0; i < AmountOfThread; i++)
            {
                Thread NewThread = new Thread(ReadDataset);
                Threads[i] = NewThread;
                NewThread.Start(ListThreadRange[i]);
            }

            // Цикл ожидания отрабатывания всех потоков
            for (int i = 0; i < AmountOfThread; i++)
            {
                Threads[i].Join();
            }

            // Цикл добавления данных их всех структур в одну
            for (int i = 0; i < ListThreadRange.Count - 1; i++)
            {
                ListThreadRange[0].DataInRange.AddRange(ListThreadRange[i + 1].DataInRange);
            }

            // Добавление всех считанных параллельно данных в один список
            DataSetFeatures.AddRange(ListThreadRange[0].DataInRange);
        }
        // ------------------------------------------------------------------------------------------------------------
        static void ReadDataset(object Obj)
        {
            if (Obj is ThreadRange NewThreadRange)
            {
                string[] FilesName = Directory.GetFiles(@"D:\Работа\EnsembleOfModels\Bagging\RandomForest\Data\2020");

                //for (int i = 0; i < FilesName.GetLength(0); i++)
                //{
                //    Console.WriteLine(FilesName[i]);
                //}
                //Console.WriteLine(FilesName.Length);

                int FirstRow = 157; // номер первой строки с данными

                for (int i = NewThreadRange.Start; i < NewThreadRange.Stop; i++)
                {
                    StreamReader Sr = new StreamReader(FilesName[i]);

                    int AmountOfLines = File.ReadAllLines(FilesName[i]).Length; // количество строк в файле

                    for (int j = 0; j < AmountOfLines; j++)
                    {
                        string Line = Sr.ReadLine();

                        if (j >= FirstRow)
                        {
                            string[] Values = Line.Split(new char[] { ' ', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (Values.GetLength(0) == 0)   // пропуск пыстых строк в файле
                            {
                                continue;
                            }

                            List<double> RowInDataSetFeatures = new List<double>();
                            for (int k = 3; k < Values.GetLength(0); k++)   // k = 3 чтобы отбросить время из файла(первые 3 значения)
                            {
                                try    // проверка на то есть ли значение в ячейке признака, если нет то заполняется нулём
                                {
                                    RowInDataSetFeatures.Add(Convert.ToDouble(Values[k]));
                                }
                                catch
                                {
                                    RowInDataSetFeatures.Add(0);
                                }
                            }

                            NewThreadRange.DataInRange.Add(RowInDataSetFeatures);
                        }
                    }
                }
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        static void ShowInfo()
        {
            for (int i = 0; i < DataSetFeatures.Count; i++)
            {
                if (i == 0 || i == DataSetFeatures.Count - 1)
                {
                    for (int j = 0; j < DataSetFeatures[i].Count; j++)
                    {
                        Console.Write(DataSetFeatures[i][j] + "\t");
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("DataSetFeatures row count = " + DataSetFeatures.Count);
            Console.WriteLine("DataSetFeature column count = " + DataSetFeatures[0].Count);
        }

    }
}