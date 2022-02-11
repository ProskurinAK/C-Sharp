using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
// ************************************************************************************************************

namespace RandomForest
{
    class RandomForestClassifier
    {
        // Датасет
        private static List<List<double>> DataSetFeatures = new List<List<double>>();
        private static List<double> DataSetTargets = new List<double>();

        // Первая половина датасета используемая как обучающая выборка
        public static List<List<double>> DataSetFeaturesForTraining = new List<List<double>>();
        public static List<double> DataSetTargetsForTraining = new List<double>();

        // Вторая половина датасета используемая как тестовая выборка
        private static List<List<double>> DataSetFeaturesForTest = new List<List<double>>();
        private static List<double> DataSetTargetsForTest = new List<double>();

        private static List<List<double>> PredictionResult = new List<List<double>>();  // Список ответов для тестовой выборки на основании предикатов для каждой модели дерева
        private static List<double> FinalPredict = new List<double>();  // Финальное предсказание для каждого объекта на основе всех простых моделей дерева
        // ------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            ReadDataSet();
            MakeTrainingDataSet();
            // ShowInfo();

            int AmountOfSamplesAndTrees = 9;    // Количество созданных бутстрэп выборок и деревьев

            //for (int i = 0; i < AmountOfSamplesAndTrees; i++)
            //{
            //    Console.WriteLine("--------------------------------------- Create training sample ---------------------------------------");
            //    TrainingSample NewSample = new TrainingSample(i);
            //    Console.WriteLine("--------------------------------------- Training sample made ---------------------------------------");
            //    DecisionTree FirstTree = new DecisionTree(i);
            //    Console.WriteLine("//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
            //}

            MakeTestDataSet();
            Compute();
            ShowInfo();
            PredictionAccuarcy();
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void ShowInfo()
        {
            for (int i = 0; i < PredictionResult.Count; i++)
            {
                for (int j = 0; j < PredictionResult[i].Count; j++)
                {
                    Console.Write(PredictionResult[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------------------------");
            for (int i = 0; i < 100; i++)
            {
                Console.Write(DataSetTargetsForTest[i]);
            }
            Console.WriteLine();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //for (int i = 0; i < DataSetFeaturesForTest.Count; i++)
            //{
            //    for (int j = 0; j < DataSetFeaturesForTest[i].Count; j++)
            //    {
            //        Console.Write(DataSetFeaturesForTest[i][j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            //for (int i = 0; i < DataSetTargetsForTest.Count; i++)
            //{
            //    Console.Write(DataSetTargetsForTest[i] + " ");
            //}
            //Console.WriteLine();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //for (int i = 0; i < DataSetFeaturesForTraining.Count; i++)
            //{
            //    for (int j = 0; j < DataSetFeaturesForTraining[i].Count; j++)
            //    {
            //        Console.Write(DataSetFeaturesForTraining[i][j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            //for (int i = 0; i < DataSetTargetsForTraining.Count; i++)
            //{
            //    Console.Write(DataSetTargetsForTraining[i] + " ");
            //}
            //Console.WriteLine();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //for (int i = 0; i < DataSetFeatures.Count; i++)
            //{
            //    for (int j = 0; j < DataSetFeatures[i].Count; j++)
            //    {
            //        Console.Write(DataSetFeatures[i][j] + "    ");
            //    }
            //    Console.WriteLine();
            //}

            //for (int i = 0; i < DataSetTargets.Count; i++)
            //{
            //    Console.Write(DataSetTargets[i] + " ");
            //}
            //Console.WriteLine();
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void ReadDataSet()
        {
            // Функция считывания матрицы объектов-признаков и вектора ответов из файла

            StreamReader Sr = new StreamReader(@"D:\Progi\C#\ML\EnsembleOfModels\Bagging\RandomForest\DataSet2.txt");

            int LinesOfObject = 200;    // Количество строк с объектами в файле
            int LinesOfTarget = 6;  // Количесвто строк с ответами в файле

            // Цикл считывания матрицы объектов-признаков и добавление их в двумерный список DataSetFeatures
            for (int i = 0; i < LinesOfObject; i++)
            {
                string Line = Sr.ReadLine();

                string[] Values = Line.Split(new char[] { ' ', '[', ']', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                List<double> RowInDataSetFeatures = new List<double>();
                for (int j = 0; j < Values.GetLength(0); j++)
                {
                    RowInDataSetFeatures.Add(Convert.ToDouble(Values[j], CultureInfo.InvariantCulture));
                }

                DataSetFeatures.Add(RowInDataSetFeatures);
            }

            // Цикл считывания вектора таргетов для объектов и добавление их в вектор DataSetTargets
            for (int i = 0; i < LinesOfTarget; i++)
            {
                string Line = Sr.ReadLine();

                string[] Values = Line.Split(new char[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < Values.GetLength(0); j++)
                {
                    DataSetTargets.Add(Convert.ToDouble(Values[j]));
                }
            }

            Sr.Close();
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void MakeTrainingDataSet()
        {
            // Функция создания обучающей выборки из исходного датасета

            int NumbOfFeaturesAndTargets = 100; // Количество объектов записываемых в обучающую выборку из датасета

            for (int i = 0; i < NumbOfFeaturesAndTargets; i++)
            {
                List<double> RowInDataSetFeaturesForTraining = new List<double>();

                for (int j = 0; j < DataSetFeatures[i].Count; j++)
                {
                    RowInDataSetFeaturesForTraining.Add(DataSetFeatures[i][j]);
                }
                DataSetFeaturesForTraining.Add(RowInDataSetFeaturesForTraining);
            }

            for (int i = 0; i < NumbOfFeaturesAndTargets; i++)
            {
                DataSetTargetsForTraining.Add(DataSetTargets[i]);
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void MakeTestDataSet()
        {
            // Функция создания тестовой выборки

            int IndexOfFirstObject = 100; // Номер индекса объекта из датасета, с которого начинается тестовая выборка

            for (int i = IndexOfFirstObject; i < DataSetFeatures.Count; i++)
            {
                List<double> RowInDataSetFeaturesForTest = new List<double>();

                for (int j = 0; j < DataSetFeatures[i].Count; j++)
                {
                    RowInDataSetFeaturesForTest.Add(DataSetFeatures[i][j]);
                }
                DataSetFeaturesForTest.Add(RowInDataSetFeaturesForTest);
            }

            for (int i = IndexOfFirstObject; i < DataSetFeatures.Count; i++)
            {
                DataSetTargetsForTest.Add(DataSetTargets[i]);
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void Compute()
        {
            // Функция предсказания ответа на основании полученных предикатов

            int AmountOfAllPredicates = 9;

            for (int NumbOfPredicate = 0; NumbOfPredicate < AmountOfAllPredicates; NumbOfPredicate++)
            {
                List<List<double>> Predicates = new List<List<double>>();   // Матрица предикатов
                List<int> NumbOfFeature = new List<int>();  // Номера фич из исходного датасета

                List<double> ListOfPrediction = new List<double>();

                StreamReader Sr = new StreamReader($@"D:\Progi\C#\ML\EnsembleOfModels\Bagging\RandomForest\Predicates\Predicate{NumbOfPredicate}.txt");

                int AmountOfRows = System.IO.File.ReadAllLines($@"D:\Progi\C#\ML\EnsembleOfModels\Bagging\RandomForest\Predicates\Predicate{NumbOfPredicate}.txt").Length;

                // Чтение предикатов и номеров признаков из файла
                for (int i = 0; i < AmountOfRows; i++)
                {
                    if (i == 0)
                    {
                        string Line = Sr.ReadLine();

                        string[] Values = Line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < Values.GetLength(0); j++)
                        {
                            NumbOfFeature.Add(Convert.ToInt32(Values[j]));
                        }
                    }
                    else
                    {
                        string Line = Sr.ReadLine();

                        string[] Values = Line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        List<double> RowInPredicates = new List<double>();

                        for (int j = 0; j < Values.GetLength(0); j++)
                        {
                            RowInPredicates.Add(Convert.ToDouble(Values[j]));
                        }
                        Predicates.Add(RowInPredicates);
                    }
                }

                //for (int i = 0; i < NumbOfFeature.Count; i++)
                //{
                //    Console.Write(NumbOfFeature[i] + " ");
                //}
                //Console.WriteLine();

                //for (int i = 0; i < Predicates.Count; i++)
                //{
                //    for (int j = 0; j < Predicates[i].Count; j++)
                //    {
                //        Console.Write(Predicates[i][j] + "\t");
                //    }
                //    Console.WriteLine();
                //}

                // Замена индексов в первом столбце на индексы соответсвтующие реальному датасету
                for (int i = 0; i < Predicates.Count; i++)
                {
                    for (int j = 0; j < 1; j++)
                    {
                        for (int k = 0; k < NumbOfFeature.Count; k++)
                        {
                            if (Predicates[i][j] == k)
                            {
                                Predicates[i][j] = NumbOfFeature[k];
                                break;
                            }
                        }
                    }
                }

                Console.WriteLine("-------------------------------");
                for (int i = 0; i < Predicates.Count; i++)
                {
                    for (int j = 0; j < Predicates[i].Count; j++)
                    {
                        Console.Write(Predicates[i][j] + "\t");
                    }
                    Console.WriteLine();
                }

                int AmountOfAllObjects = 100;   // Количество всех объектов в тестовой выборке

                // Алгоритм предсказания ответа на основании полученных предикатов

                for (int NumbOfObject = 0; NumbOfObject < AmountOfAllObjects; NumbOfObject++)
                {
                    double Prediction = -1;  // Полученное предсказание

                    for (int i = 0; i < Predicates.Count; i++)
                    {
                        for (int k = 0; k < DataSetFeaturesForTraining[NumbOfObject].Count; k++)
                        {
                            if (Predicates[i][0] == k)
                            {
                                if (Predicates[i][2] == 1)
                                {
                                    if (DataSetFeaturesForTraining[NumbOfObject][k] >= Predicates[i][1])
                                    {
                                        Prediction = Predicates[i][3];
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (Predicates[i][2] == 0)
                                {
                                    if (DataSetFeaturesForTraining[NumbOfObject][k] <= Predicates[i][1])
                                    {
                                        Prediction = Predicates[i][3];
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (Prediction >= 0)
                        {
                            break;
                        }
                    }
                    ListOfPrediction.Add(Prediction);
                }
                //Console.Write("List of prediction - ");
                //for (int i = 0; i < ListOfPrediction.Count; i++)
                //{
                //    Console.Write(ListOfPrediction[i] + " ");
                //}
                //Console.WriteLine();

                PredictionResult.Add(ListOfPrediction);
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        private static void PredictionAccuarcy()
        {
            // подсчёт количества совпадений и общий процент правильных предсказаний для каждой из моделей

            for (int i = 0; i < PredictionResult.Count; i++)
            {
                int Counter = 0;
                double Percent = 0;

                for (int j = 0; j < PredictionResult[i].Count; j++)
                {
                    if (PredictionResult[i][j] == DataSetTargetsForTest[j])
                    {
                        Counter++;
                    }
                }
                Percent = (double)Counter / PredictionResult[i].Count * 100;

                Console.WriteLine($"Counter for {i} model = " + Counter);
                Console.WriteLine($"Percent of {i} model = " + Percent + "%");
            }
            Console.WriteLine("-----------------------------------------------------------");

            // выбор лучшего предсказания из всех моделей
            for (int j = 0; j < PredictionResult[0].Count; j++)
            {
                int NumbOf0 = 0;
                int NumbOf1 = 0;

                for (int i = 0; i < PredictionResult.Count; i++)
                {
                    if (PredictionResult[i][j] == 0)
                    {
                        NumbOf0++;
                    }
                    else if (PredictionResult[i][j] == 1)
                    {
                        NumbOf1++;
                    }
                }

                if (NumbOf0 > NumbOf1)
                {
                    FinalPredict.Add(0);
                }
                else if (NumbOf0 < NumbOf1)
                {
                    FinalPredict.Add(1);
                }
            }

            for (int i = 0; i < FinalPredict.Count; i++)
            {
                Console.Write(FinalPredict[i]);
            }
            Console.WriteLine();

            // подсчёт количества совпадений и общий процент правильных предсказаний для всех моделей
            int FinalCounter = 0;
            double FinalPercent = 0;

            for (int i = 0; i < FinalPredict.Count; i++)
            {
                if (FinalPredict[i] == DataSetTargetsForTest[i])
                {
                    FinalCounter++;
                }
            }

            FinalPercent = (double)FinalCounter / FinalPredict.Count * 100;

            Console.WriteLine("Final Counter = " + FinalCounter);
            Console.WriteLine("Final Percent = " + FinalPercent + "%");

        }
    }
}
