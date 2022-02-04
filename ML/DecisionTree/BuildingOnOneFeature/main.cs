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
        static void Main(string[] args)
        {
            // Обучающая выборка
            int[] VectorOfFeatures = { 17, 18, 20, 25, 29, 31, 33, 38, 49, 55, 64 };
            int[] VectorOfTargets = { 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0 };

            // Выходные данные алгоритма
            int OnePredicate = 0;
            List<int> AllPredicates = new List<int>();
            List<int> NewVectorOfFeatures = new List<int>();
            List<int> NewVectorOfTargets = new List<int>();

            // Создание первого узла дерева
            (NewVectorOfFeatures, NewVectorOfTargets, OnePredicate) = CreateNode(VectorOfFeatures, VectorOfTargets);
            AllPredicates.Add(OnePredicate);

            Console.Write("All Predicates - ");
            for (int i = 0; i < AllPredicates.Count; i++)
            {
                Console.Write(AllPredicates[i] + " ");
            }
            Console.WriteLine();
            Console.Write("New Vector Of Features - ");
            for (int i = 0; i < NewVectorOfFeatures.Count; i++)
            {
                Console.Write(NewVectorOfFeatures[i] + " ");
            }
            Console.WriteLine();
            Console.Write("New Vector Of Targets - ");
            for (int i = 0; i < NewVectorOfTargets.Count; i++)
            {
                Console.Write(NewVectorOfTargets[i] + " ");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------");

            // Все последующие узлы
            while (NewVectorOfTargets.Count > 1)
            {
                (NewVectorOfFeatures, NewVectorOfTargets, OnePredicate) = CreateNode(NewVectorOfFeatures.ToArray(), NewVectorOfTargets.ToArray());

                AllPredicates.Add(OnePredicate);

                Console.Write("All Predicates - ");
                for (int i = 0; i < AllPredicates.Count; i++)
                {
                    Console.Write(AllPredicates[i] + " ");
                }
                Console.WriteLine();
                Console.Write("New Vector Of Features - ");
                for (int i = 0; i < NewVectorOfFeatures.Count; i++)
                {
                    Console.Write(NewVectorOfFeatures[i] + " ");
                }
                Console.WriteLine();
                Console.Write("New Vector Of Targets - ");
                for (int i = 0; i < NewVectorOfTargets.Count; i++)
                {
                    Console.Write(NewVectorOfTargets[i] + " ");
                }
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------");
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        static public (List<int>, List<int>, int) CreateNode(int[] VectorOfFeatures, int[] VectorOfTargets)
        {
            int[] VectorOfData = VectorOfFeatures;
            int[] VectorOfAnswer = VectorOfTargets;
            int NumbOf0 = 0;
            int NumbOf1 = 0;

            // Подсчёт кол-ва нулей и единиц в таргет векторе и нахождения индекса джини для всей выборки

            for (int i = 0; i < VectorOfAnswer.GetLength(0); i++)
            {
                if (VectorOfAnswer[i] == 0)
                {
                    NumbOf0++;
                }
                else
                {
                    NumbOf1++;
                }
            }

            double GiniIndex = Math.Round((1 - Math.Pow((NumbOf0 / Convert.ToDouble(VectorOfAnswer.GetLength(0))), 2) - Math.Pow((NumbOf1 / Convert.ToDouble(VectorOfAnswer.GetLength(0))), 2)), 4);
            Console.WriteLine("---" + GiniIndex);

            // Инициализация переменных которые необходимо найти в функции
            List<double> AllDeltaOfGiniIndex = new List<double>();
            int Predicate = 0;  // Предикат узла
            List<int> NewVectorOfData = new List<int>();    // Новый вектор фичей
            List<int> NewVectorOfAnswer = new List<int>();  // Новый вектор таргетов

            // Алгоритм создание узла для дерева
            for (int i = 0; i < VectorOfAnswer.GetLength(0) - 1; i++)
            {
                //Console.WriteLine("-------------------------------------" + i + " - Iteration" + "-------------------------------------");

                // Условие при котором подсчёт будет вестись только для таргет переменных поменявших значение
                if (VectorOfAnswer[i] != VectorOfAnswer[i + 1])
                {
                    List<int> LeftSample = new List<int>();
                    List<int> RightSample = new List<int>();

                    for (int j = 0; j < VectorOfAnswer.GetLength(0); j++)
                    {
                        if (j <= i)
                        {
                            LeftSample.Add(VectorOfAnswer[j]);
                        }
                        else
                        {
                            RightSample.Add(VectorOfAnswer[j]);
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

                    AllDeltaOfGiniIndex.Add(DeltaOfGiniIndex);

                    // Поиск наилучшего предиката для всего набора объектов по наименьшей дельте индексов джини
                    double MinOfAllDeltaOfGiniIndex = AllDeltaOfGiniIndex.Min();

                    if (DeltaOfGiniIndex == MinOfAllDeltaOfGiniIndex)
                    {
                        if (GiniIndexForLeftSample > GiniIndexForRightSample)
                        {
                            NewVectorOfData.Clear();
                            NewVectorOfAnswer.Clear();

                            Predicate = VectorOfData[i];

                            for (int k = 0; k < LeftSample.Count; k++)
                            {
                                NewVectorOfData.Add(VectorOfData[k]);
                                NewVectorOfAnswer.Add(VectorOfAnswer[k]);
                            }
                        }
                        else
                        {
                            NewVectorOfData.Clear();
                            NewVectorOfAnswer.Clear();

                            Predicate = VectorOfData[i];

                            for (int k = 0; k < RightSample.Count; k++)
                            {
                                NewVectorOfData.Add(VectorOfData[i + k]);
                                NewVectorOfAnswer.Add(VectorOfAnswer[i + k]);
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
                    Console.WriteLine("NumbOfLeft0 = " + NumbOfLeft0);
                    Console.WriteLine("NumbOfLeft1 = " + NumbOfLeft1);
                    Console.WriteLine("NumbORight0 = " + NumbOfRight0);
                    Console.WriteLine("NumbOfRight1 = " + NumbOfRight1);
                    Console.WriteLine("GiniIndexForLeftSample = " + GiniIndexForLeftSample);
                    Console.WriteLine("GiniIndexForRightSample = " + GiniIndexForRightSample);
                    Console.WriteLine("DeltaOfGiniIndex = " + DeltaOfGiniIndex);
                }
            }

            Console.WriteLine("////////////////////////////////////////////////////////////////////////////////////////////");

            for (int i = 0; i < AllDeltaOfGiniIndex.Count; i++)
            {
                Console.WriteLine(AllDeltaOfGiniIndex[i]);
            }

            //Console.WriteLine(Predicate);

            //for (int i = 0; i < NewVectorOfData.Count; i++)
            //{
            //    Console.Write(NewVectorOfData[i] + " ");
            //}
            //Console.WriteLine();
            //for (int i = 0; i < NewVectorOfAnswer.Count; i++)
            //{
            //    Console.Write(NewVectorOfAnswer[i] + " ");
            //}
            //Console.WriteLine();

            return (NewVectorOfData, NewVectorOfAnswer, Predicate);
        }
    }
}