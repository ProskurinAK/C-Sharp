using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// ************************************************************************************************************

namespace TestProject
{
    class Program
    {
        static public int[,] DataSet =
            { { 17, 64, 18, 20, 38, 49, 55, 25, 29, 31, 33 },
            { 25, 80, 22, 36, 37, 59, 74, 70, 33, 102, 88 },
            { 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 1 } };
        // ------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            List<List<int>> TrainingSample = new List<List<int>>();

            MakeTrainingSample(TrainingSample);

            List<List<int>> NewTrainingSample = new List<List<int>>();
            List<List<int>> OnePredicate = new List<List<int>>();
            /*
             * 1 столбец - номер фичи с наилучшим предикатом
             * 2 столбец - значение по которому будет происходить разбиение (предикат)
             * 3 столбец - условие для значения по которому будет происходить разбиение
             * если 1, то >=
             * если 0, то <=
             * 4 столбец - значение которому равен целевой признак объекта по данному предикату
            */
            List<List<int>> AllPredicates = new List<List<int>>();

            // Создание первого узла дерева
            (NewTrainingSample, OnePredicate) = Training(TrainingSample);

            for (int i = 0; i < OnePredicate.Count; i++)
            {
                List<int> RowInAllPredicates = new List<int>();

                for (int j = 0; j < OnePredicate[i].Count; j++)
                {
                    RowInAllPredicates.Add(OnePredicate[i][j]);
                }
                AllPredicates.Add(RowInAllPredicates);
            }

            Console.WriteLine("#######################################");
            for (int i = 0; i < NewTrainingSample.Count; i++)
            {
                for (int j = 0; j < NewTrainingSample[i].Count; j++)
                {
                    Console.Write(NewTrainingSample[i][j] + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            for (int i = 0; i < AllPredicates.Count; i++)
            {
                for (int j = 0; j < AllPredicates[i].Count; j++)
                {
                    Console.Write(AllPredicates[i][j] + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("#######################################");

            // Все последующие узлы
            while (NewTrainingSample.Count > 1) // Условие создания новых узлов дерева
            {
                (NewTrainingSample, OnePredicate) = Training(NewTrainingSample);

                for (int i = 0; i < OnePredicate.Count; i++)
                {
                    List<int> RowInAllPredicates = new List<int>();

                    for (int j = 0; j < OnePredicate[i].Count; j++)
                    {
                        RowInAllPredicates.Add(OnePredicate[i][j]);
                    }
                    AllPredicates.Add(RowInAllPredicates);
                }

                Console.WriteLine("#######################################");
                for (int i = 0; i < NewTrainingSample.Count; i++)
                {
                    for (int j = 0; j < NewTrainingSample[i].Count; j++)
                    {
                        Console.Write(NewTrainingSample[i][j] + "\t");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();

                for (int i = 0; i < AllPredicates.Count; i++)
                {
                    for (int j = 0; j < AllPredicates[i].Count; j++)
                    {
                        Console.Write(AllPredicates[i][j] + "\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("#######################################");
            }
            Console.WriteLine("End Algorithm");
        }
        // ------------------------------------------------------------------------------------------------------------
        static public void MakeTrainingSample(List<List<int>> TrainingSample)
        {
            for (int i = 0; i < DataSet.GetLength(1); i++)
            {
                List<int> RowInTrainingSample = new List<int>();

                for (int j = 0; j < DataSet.GetLength(0); j++)
                {
                    RowInTrainingSample.Add(DataSet[j, i]);
                }
                TrainingSample.Add(RowInTrainingSample);
            }

            //for (int i = 0; i < TrainingSample.Count; i++)
            //{
            //    for (int j = 0; j < TrainingSample[i].Count; j++)
            //    {
            //        Console.Write(TrainingSample[i][j] + "\t");
            //    }
            //    Console.WriteLine();
            //}
        }
        // ------------------------------------------------------------------------------------------------------------
        static public (List<List<int>>, List<List<int>>) Training(List<List<int>> TrainingSample)
        {
            for (int i = 0; i < TrainingSample.Count; i++)
            {
                for (int j = 0; j < TrainingSample[i].Count; j++)
                {
                    Console.Write(TrainingSample[i][j] + "\t");
                }
                Console.WriteLine();
            }
            // Console.WriteLine("--------------------------");

            List<List<double>> AllDeltaOfGiniIndex = new List<List<double>>();  // Массив всех дельт индекса джини для разбиений на две части по каждой из фич
            List<List<int>> NewTrainingSample = new List<List<int>>();  // Новая обучающая выборка, которую вернёт функция
            List<List<int>> Predicate = new List<List<int>>();  // Новый предикат, который вернёт функция

            // Цикл итерирования по фичам обучающей выборки
            for (int NumbOfFeature = 0; NumbOfFeature < TrainingSample[1].Count - 1; NumbOfFeature++)
            {
                // Алгоритм сортировки одной фичи обучающей выборки по возростанию методом сортировки выбором
                for (int i = 0; i < TrainingSample.Count; i++)
                {
                    int MinIndex = i;
                    List<int> TmpList = new List<int>();

                    for (int j = i + 1; j < TrainingSample.Count; j++)
                    {
                        if (TrainingSample[j][NumbOfFeature] < TrainingSample[MinIndex][NumbOfFeature])
                        {
                            MinIndex = j;
                        }
                    }
                    if (MinIndex != i)
                    {
                        for (int j = 0; j < TrainingSample[i].Count; j++)
                        {
                            TmpList.Add(TrainingSample[i][j]);
                        }

                        for (int j = 0; j < TrainingSample[i].Count; j++)
                        {
                            TrainingSample[i][j] = TrainingSample[MinIndex][j];
                        }

                        for (int j = 0; j < TrainingSample[i].Count; j++)
                        {
                            TrainingSample[MinIndex][j] = TmpList[j];
                        }
                    }
                }

                Console.WriteLine("----------------------------------------------------");
                for (int i = 0; i < TrainingSample.Count; i++)
                {
                    for (int j = 0; j < TrainingSample[i].Count; j++)
                    {
                        Console.Write(TrainingSample[i][j] + "\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("----------------------------------------------------");

                // Выделение вектора целевого признака из обучающей выборки
                List<int> VectorOfTarget = new List<int>();

                for (int i = 0; i < TrainingSample.Count; i++)
                {
                    VectorOfTarget.Add(TrainingSample[i][TrainingSample[i].Count - 1]);
                }

                for (int i = 0; i < VectorOfTarget.Count; i++)
                {
                    Console.WriteLine(VectorOfTarget[i]);
                }
                Console.WriteLine("--------------------------");

                // Подсчёт кол-ва нулей и единиц в таргет векторе и нахождения индекса джини для всей выборки
                int NumbOf0 = 0;
                int NumbOf1 = 0;

                for (int i = 0; i < VectorOfTarget.Count; i++)
                {
                    if (VectorOfTarget[i] == 0)
                    {
                        NumbOf0++;
                    }
                    else
                    {
                        NumbOf1++;
                    }
                }

                double GiniIndex = Math.Round((1 - Math.Pow((NumbOf0 / Convert.ToDouble(VectorOfTarget.Count)), 2) - Math.Pow((NumbOf1 / Convert.ToDouble(VectorOfTarget.Count)), 2)), 4);
                Console.WriteLine("---" + GiniIndex);

                List<double> RowInAllDeltaOfGiniIndex = new List<double>(); // Список дельт индекса джини для каждой фичи
                int SampleSplitIndex = 0;   // Индекс на котором находится лучший предикат для разбиения
                bool FlagSampleSplitIndex = false;  // Переключается в true если наилучший предикат для разбиения находится не в первой фиче
                bool IsLeftSample = false;  // Переключается в true если индекс джини для левой части больше чем для правой
                bool IsLastNode = false;    // Переключается в true если индекс джини для левой и правой части равны

                for (int i = 0; i < VectorOfTarget.Count - 1; i++)
                {
                    if (VectorOfTarget[i] != VectorOfTarget[i + 1])
                    {
                        Console.WriteLine("------------------------------------- New Iteration " + i + "-------------------------------------");

                        List<int> LeftSample = new List<int>();
                        List<int> RightSample = new List<int>();

                        for (int j = 0; j < VectorOfTarget.Count; j++)
                        {
                            if (j <= i)
                            {
                                LeftSample.Add(VectorOfTarget[j]);
                            }
                            else
                            {
                                RightSample.Add(VectorOfTarget[j]);
                            }
                        }

                        // Подсчёт кол-ва нулей и единиц в левой и правой выборке и нахождения индекса джини для них
                        int NumbOfLeft0 = 0;
                        int NumbOfLeft1 = 0;
                        int NumbOfRight0 = 0;
                        int NumbOfRight1 = 0;

                        for (int j = 0; j < LeftSample.Count; j++)
                        {
                            if (LeftSample[j] == 0)
                            {
                                NumbOfLeft0++;
                            }
                            else
                            {
                                NumbOfLeft1++;
                            }
                        }

                        for (int j = 0; j < RightSample.Count; j++)
                        {
                            if (RightSample[j] == 0)
                            {
                                NumbOfRight0++;
                            }
                            else
                            {
                                NumbOfRight1++;
                            }
                        }

                        double GiniIndexForLeftSample = Math.Round((1 - Math.Pow((NumbOfLeft0 / Convert.ToDouble(LeftSample.Count)), 2) - Math.Pow((NumbOfLeft1 / Convert.ToDouble(LeftSample.Count)), 2)), 4);
                        double GiniIndexForRightSample = Math.Round((1 - Math.Pow((NumbOfRight0 / Convert.ToDouble(RightSample.Count)), 2) - Math.Pow((NumbOfRight1 / Convert.ToDouble(RightSample.Count)), 2)), 4);
                        double DeltaOfGiniIndex = (GiniIndexForLeftSample + GiniIndexForRightSample) / 2;

                        RowInAllDeltaOfGiniIndex.Add(DeltaOfGiniIndex);

                        // Поиск наилучшего предиката для всего набора объектов по наименьшей дельте индексов джини
                        if(AllDeltaOfGiniIndex.Count == 0)
                        {
                            double MinOfAllDeltaOfGiniIndex = RowInAllDeltaOfGiniIndex.Min();

                            if (DeltaOfGiniIndex == MinOfAllDeltaOfGiniIndex)
                            {
                                SampleSplitIndex = i;

                                if (GiniIndexForLeftSample > GiniIndexForRightSample)
                                {
                                    IsLeftSample = true;

                                    List<int> RowInPredicate = new List<int>();

                                    Predicate.Clear();
                                    RowInPredicate.Add(NumbOfFeature);
                                    RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                    RowInPredicate.Add(1);
                                    RowInPredicate.Add(TrainingSample[i + 1][TrainingSample[i].Count - 1]);

                                    Predicate.Add(RowInPredicate);
                                }
                                else if (GiniIndexForLeftSample < GiniIndexForRightSample)
                                {
                                    List<int> RowInPredicate = new List<int>();

                                    Predicate.Clear();
                                    RowInPredicate.Add(NumbOfFeature);
                                    RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                    RowInPredicate.Add(0);
                                    RowInPredicate.Add(TrainingSample[i][TrainingSample[i].Count - 1]);

                                    Predicate.Add(RowInPredicate);
                                }
                                else if (GiniIndexForLeftSample == GiniIndexForRightSample)
                                {
                                    IsLastNode = true;
                                    Predicate.Clear();

                                    for (int k = 0; k < 2; k++)
                                    {
                                        List<int> RowInPredicate = new List<int>();

                                        if (k == 0)
                                        {
                                            RowInPredicate.Add(NumbOfFeature);
                                            RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                            RowInPredicate.Add(1);
                                            RowInPredicate.Add(TrainingSample[i + 1][TrainingSample[i].Count - 1]);

                                            Predicate.Add(RowInPredicate);
                                        }
                                        else
                                        {
                                            RowInPredicate.Add(NumbOfFeature);
                                            RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                            RowInPredicate.Add(0);
                                            RowInPredicate.Add(TrainingSample[i][TrainingSample[i].Count - 1]);

                                            Predicate.Add(RowInPredicate);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            double MinOfAllDeltaOfGiniIndex = RowInAllDeltaOfGiniIndex.Min();

                            for (int j = 0; j < AllDeltaOfGiniIndex.Count; j++)
                            {
                                if (MinOfAllDeltaOfGiniIndex < AllDeltaOfGiniIndex[j].Min())
                                {
                                    SampleSplitIndex = i;
                                    FlagSampleSplitIndex = true;

                                    if (GiniIndexForLeftSample > GiniIndexForRightSample)
                                    {
                                        IsLeftSample = true;

                                        List<int> RowInPredicate = new List<int>();

                                        Predicate.Clear();
                                        RowInPredicate.Add(NumbOfFeature);
                                        RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                        RowInPredicate.Add(1);
                                        RowInPredicate.Add(TrainingSample[i + 1][TrainingSample[i].Count - 1]);

                                        Predicate.Add(RowInPredicate);
                                    }
                                    else if (GiniIndexForLeftSample < GiniIndexForRightSample)
                                    {
                                        List<int> RowInPredicate = new List<int>();

                                        Predicate.Clear();
                                        RowInPredicate.Add(NumbOfFeature);
                                        RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                        RowInPredicate.Add(0);
                                        RowInPredicate.Add(TrainingSample[i][TrainingSample[i].Count - 1]);

                                        Predicate.Add(RowInPredicate);
                                    }
                                    else if(GiniIndexForLeftSample == GiniIndexForRightSample)
                                    {
                                        IsLastNode = true;
                                        Predicate.Clear();

                                        for (int k = 0; k < 2; k++)
                                        {
                                            List<int> RowInPredicate = new List<int>();

                                            if (k == 0)
                                            {
                                                RowInPredicate.Add(NumbOfFeature);
                                                RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                                RowInPredicate.Add(1);
                                                RowInPredicate.Add(TrainingSample[i + 1][TrainingSample[i].Count - 1]);

                                                Predicate.Add(RowInPredicate);
                                            }
                                            else
                                            {
                                                RowInPredicate.Add(NumbOfFeature);
                                                RowInPredicate.Add((TrainingSample[i][NumbOfFeature] + TrainingSample[i + 1][NumbOfFeature]) / 2);
                                                RowInPredicate.Add(0);
                                                RowInPredicate.Add(TrainingSample[i][TrainingSample[i].Count - 1]);

                                                Predicate.Add(RowInPredicate);
                                            }
                                        }
                                    }
                                }
                            }
                        }



                        for (int j = 0; j < LeftSample.Count; j++)
                        {
                            Console.Write(LeftSample[j] + " ");
                        }
                        Console.WriteLine();
                        for (int j = 0; j < RightSample.Count; j++)
                        {
                            Console.Write(RightSample[j] + " ");
                        }
                        Console.WriteLine();
                        //Console.WriteLine("NumbOfLeft0 = " + NumbOfLeft0);
                        //Console.WriteLine("NumbOfLeft1 = " + NumbOfLeft1);
                        //Console.WriteLine("NumbORight0 = " + NumbOfRight0);
                        //Console.WriteLine("NumbOfRight1 = " + NumbOfRight1);
                        Console.WriteLine("GiniIndexForLeftSample = " + GiniIndexForLeftSample);
                        Console.WriteLine("GiniIndexForRightSample = " + GiniIndexForRightSample);
                        Console.WriteLine("DeltaOfGiniIndex = " + DeltaOfGiniIndex);
                        Console.WriteLine("Value in this index = " + TrainingSample[i][NumbOfFeature]);
                        //Console.WriteLine();
                    }
                }

                AllDeltaOfGiniIndex.Add(RowInAllDeltaOfGiniIndex);

                // Создание новой обучающей выборки если это не последний узел в дереве
                if (IsLastNode == false)
                {
                    if (AllDeltaOfGiniIndex.Count == 1 || FlagSampleSplitIndex == true)
                    {
                        NewTrainingSample.Clear();

                        if (IsLeftSample == true)
                        {
                            for (int i = 0; i <= SampleSplitIndex; i++)
                            {
                                List<int> RowInNewTrainingSample = new List<int>();

                                for (int j = 0; j < TrainingSample[i].Count; j++)
                                {
                                    RowInNewTrainingSample.Add(TrainingSample[i][j]);
                                }
                                NewTrainingSample.Add(RowInNewTrainingSample);
                            }
                        }
                        else
                        {
                            for (int i = SampleSplitIndex + 1; i <= TrainingSample.Count - SampleSplitIndex; i++)
                            {
                                List<int> RowInNewTrainingSample = new List<int>();

                                for (int j = 0; j < TrainingSample[i].Count; j++)
                                {
                                    RowInNewTrainingSample.Add(TrainingSample[i][j]);
                                }
                                NewTrainingSample.Add(RowInNewTrainingSample);
                            }
                        }
                    }
                }



                Console.WriteLine("Sample split index [" + NumbOfFeature + "] = " + SampleSplitIndex);
                Console.WriteLine();

                Console.WriteLine("All delta of Gini index");
                for (int i = 0; i < AllDeltaOfGiniIndex.Count; i++)
                {
                    for (int j = 0; j < AllDeltaOfGiniIndex[i].Count; j++)
                    {
                        Console.WriteLine(AllDeltaOfGiniIndex[i][j] + "\t");
                    }
                    Console.WriteLine();
                }

                //Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                //for (int i = 0; i < NewTrainingSample.Count; i++)
                //{
                //    for (int j = 0; j < NewTrainingSample[i].Count; j++)
                //    {
                //        Console.Write(NewTrainingSample[i][j] + "\t");
                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine("----------------------------");
                //for (int i = 0; i < Predicate.Count; i++)
                //{
                //    for (int j = 0; j < Predicate[i].Count; j++)
                //    {
                //        Console.Write(Predicate[i][j] + " ");
                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine();
                //Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            }
            return (NewTrainingSample, Predicate);
        }
	}
}