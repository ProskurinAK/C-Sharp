using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// ************************************************************************************************************

namespace RandomForest
{
    class TrainingSample
    {
        private List<List<double>> TrainingSampleFeatures = new List<List<double>>();   // Матрица созданных объектов ДО ОБЪЕДИНЕНИЯ В МЕТОДЕ UNIT
        private List<double> TrainingSampleTargets = new List<double>();    // Вектор ответов ДО ОБЪЕДИНЕНИЯ В МЕТОДЕ UNIT

        private List<int> NumbOfFeature = new List<int>();  // Номера индексов признаков выбранных для обучающей выборки методом случайных подпространств
        private List<int> IndicesOfObject = new List<int>();    // Номера индексов объектов взятых из датасета в обучающую выборку
        private int NumbOfSample;   // Номер обучающей(бутстрэп) выборки

        public List<List<double>> Training_Sample = new List<List<double>>();   // Результат работы класса - Матрица объектов-признаков вместе с ответами для каждого объекта
        // ------------------------------------------------------------------------------------------------------------
        public TrainingSample(int NumbOfSample)
        {
            this.NumbOfSample = NumbOfSample;

            BootstrapSample();
            Unit();
            WriteToFile();

            ShowInfo();
        }
        // ------------------------------------------------------------------------------------------------------------
        private void ShowInfo()
        {
            Console.Write("Indices of features - ");
            for (int i = 0; i < NumbOfFeature.Count; i++)
            {
                Console.Write(NumbOfFeature[i] + " ");
            }
            Console.WriteLine();

            Console.Write("Indices of object - ");
            for (int i = 0; i < IndicesOfObject.Count; i++)
            {
                Console.Write(IndicesOfObject[i] + " ");
            }
            Console.WriteLine();

            for (int i = 0; i < Training_Sample.Count; i++)
            {
                for (int j = 0; j < Training_Sample[i].Count; j++)
                {
                    Console.Write(Training_Sample[i][j] + "\t");
                }
                Console.WriteLine();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //for (int i = 0; i < TrainingSampleFeatures.Count; i++)
            //{
            //    for (int j = 0; j < TrainingSampleFeatures[i].Count; j++)
            //    {
            //        Console.Write(TrainingSampleFeatures[i][j] + "\t");
            //    }
            //    Console.Write(TrainingSampleTargets[i] + " ");
            //    Console.WriteLine();
            //}
        }
        // ------------------------------------------------------------------------------------------------------------
        private void BootstrapSample()
        {
            // Функция создания обучающей(бутстрэп) выборки

            Random Rnd = new Random();

            int FeaturesAmount = 3; // Количество признаков в обучающей выборке

            // Генерация номеров признаков для обучающей выборки
            for (int i = 0; i < FeaturesAmount; )
            {
                bool AlreadyThere = false;
                int Value = Rnd.Next(9);    // 9 - количество признаков в исходном датасете

                for (int j = 0; j < NumbOfFeature.Count; j++)
                {
                    if (Value == NumbOfFeature[j])
                    {
                        AlreadyThere = true;
                        break;
                    }
                }
                if (!AlreadyThere)
                {
                    NumbOfFeature.Add(Value);
                    i++;
                }
            }
            NumbOfFeature.Sort();

            int TrainingSampleSize = 80;    // Количество объектов в обучающей выборке

            // Создание матрицы объектов-признаков на основе бутстрепа и метода случайных подпространств для обучающей выборки
            for (int k = 0; k < TrainingSampleSize; k++)
            {
                int NewObjectIndex = Rnd.Next(100);
                IndicesOfObject.Add(NewObjectIndex);

                List<double> TrainingSampleFeaturesRow = new List<double>();

                for (int i = 0; i < RandomForestClassifier.DataSetFeaturesForTraining.Count; i++)
                {
                    if (NewObjectIndex == i)
                    {
                        for (int n = 0; n < NumbOfFeature.Count; n++)
                        {
                            for (int j = 0; j < RandomForestClassifier.DataSetFeaturesForTraining[i].Count; j++)
                            {
                                if (j == NumbOfFeature[n])
                                {
                                    TrainingSampleFeaturesRow.Add(RandomForestClassifier.DataSetFeaturesForTraining[i][j]);
                                }
                            }
                        }
                    }
                }

                TrainingSampleFeatures.Add(TrainingSampleFeaturesRow);

                // Создание вектора ответов на основе бутстрепа и метода случайных подпространств для обучающей выборки
                for (int i = 0; i < RandomForestClassifier.DataSetTargetsForTraining.Count; i++)
                {
                    if (i == NewObjectIndex)
                    {
                        TrainingSampleTargets.Add(RandomForestClassifier.DataSetTargetsForTraining[i]);
                    }
                }
            }
        }
        // ------------------------------------------------------------------------------------------------------------
        private void Unit()
        {
            // Функция объединения матрицы объектов-признаков и вектора ответов в одну матрицу

            for (int i = 0; i < TrainingSampleFeatures.Count; i++)
            {
                List<double> RowInTrainingSample = new List<double>();

                for (int j = 0; j < TrainingSampleFeatures[i].Count; j++)
                {
                    RowInTrainingSample.Add(TrainingSampleFeatures[i][j]);
                }

                RowInTrainingSample.Add(TrainingSampleTargets[i]);

                Training_Sample.Add(RowInTrainingSample);
            }

            TrainingSampleFeatures.Clear();
            TrainingSampleTargets.Clear();
        }
        // ------------------------------------------------------------------------------------------------------------
        private void WriteToFile()
        {
            // Функция записи обучающей(бутстрэп) выборки в файл

            StreamWriter Sw = new StreamWriter($@"D:\Progi\C#\ML\EnsembleOfModels\Bagging\RandomForest\TrainingSamples\TrainingSample{NumbOfSample}.txt", false);

            // Запись индексов параметров из датасета по которым построилась обучающая выборка с помощью метода случайных подпространств
            for (int i = 0; i < NumbOfFeature.Count; i++)
            {
                Sw.Write(NumbOfFeature[i] + " ");
            }
            Sw.WriteLine();

            // Запись индексов объектов из датасета в файл
            for (int i = 0; i < IndicesOfObject.Count; i++)
            {
                Sw.Write(IndicesOfObject[i] + " ");
            }
            Sw.WriteLine();

            // Запись обучающей выборки в файл
            for (int i = 0; i < Training_Sample.Count; i++)
            {
                for (int j = 0; j < Training_Sample[i].Count; j++)
                {
                    Sw.Write(Training_Sample[i][j] + "\t");
                }
                Sw.WriteLine();
            }

            Sw.Close();
        }
    }
}
