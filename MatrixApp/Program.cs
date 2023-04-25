using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace MatrixApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create Matrix with n-element side
            int Cols = 9, Rows = 9;
            Range NumberRange = 0..3;
            int EqualCount = 3;
            MatrixClass<byte> Matrix = new MatrixClass<byte>(Cols, Rows, NumberRange, EqualCount);
            Matrix.Do();
        }

        class MatrixClass<T> where T : struct
        {
            internal MatrixClass(int matrixcols, int matrixrows, Range matrixnumberrange, int equalcount)
            {
                MatrixType = typeof(T);
                if (ValidTypes.Contains(MatrixType))
                {
                    MatrixCols = matrixcols;
                    MatrixRows = matrixrows;
                    MatrixNumberRange = matrixnumberrange;
                    EqualCount = equalcount;
                    CreateMatrix();
                }
                else
                   throw new InvalidTypeException($"{typeof(T)} is invalid type of Matrix cell");
            }

            Type MatrixType { get; set; }
            int MatrixRows { get; set; }
            int MatrixCols { get; set; }
            int EqualCount { get; set; }
            Range MatrixNumberRange { get; set; }
            T[ , ] Matrix { get; set; }
            //List<T[ , ]> CoincidenceList { get; set; } = new List<T[ , ]>();

            static List<Type> ValidTypes = new List<Type>() { typeof(Byte), typeof(Int32) };

            #region ClassMethods
            private void CreateMatrix()
            {
                Matrix = new T[MatrixRows, MatrixCols];
                for (int i = 0; i < MatrixRows; i++)
                {
                    for (int j = 0; j < MatrixCols; j++)
                    {
                        Matrix[i , j] = (T)Convert.ChangeType(RandomValue(), typeof(T));
                    }
                }
            }

            private int RandomValue()
            {
                var Random = new Random();
                return Random.Next(MatrixNumberRange.Start.Value, MatrixNumberRange.End.Value + 1);
            }

            public void Do()
            {
                PrintMatrix();
                var Coincidence = FindCoincidence();
                if (Coincidence?.Count > 0)
                {
                    PrintMatrix(Coincidence);
                    ReNewMatrix(Coincidence);
                    Do();
                }
            }

            private void ReNewMatrix(List<(int col, int row)> ListOfCoincidence)
            {
                ListOfCoincidence.Distinct().OrderBy(x => x.row).ToList().ForEach(x =>
                {
                    if (x.row > 0)
                    {
                        for (int i = x.row; i > 0; i--)
                        {
                            T OverItem = Matrix[i - 1, x.col];
                            Matrix[i, x.col] = OverItem;
                        }
                    }
                    Matrix[0, x.col] = (T)Convert.ChangeType(RandomValue(), typeof(T));
                });
            }

            private List<(int col, int row)> FindCoincidence()
            {
                var Result = new List<(int col, int row)>();
                var EqualItems = new Queue<(T value, int col, int row)>();

                //Find by horisontal 
                if (MatrixCols >= EqualCount)
                {
                    for (int i = MatrixRows - 1; i >= 0; i--)
                    {
                        for (int j = MatrixCols - 1; j >= 0; j--)
                        {
                            (T value, int col, int row) CurrentItem = (Matrix[i, j], j, i);
                            EqualItems.Enqueue(CurrentItem);
                            if(EqualItems.Count() >= EqualCount)
                            {
                                if (EqualItems.All(x => x.value.Equals(CurrentItem.value)))
                                {
                                    var CoincidenceItems = EqualItems.AsEnumerable().Select(x => (x.col, x.row));
                                    Result.AddRange(CoincidenceItems);
                                }

                                EqualItems.Dequeue();
                            }
                        }
                        EqualItems.Clear();
                    }
                }
                
                //Find by vertical 
                if (MatrixCols >= EqualCount)
                {
                    for (int i = MatrixCols - 1; i > 0; i--)
                    {
                        for (int j = MatrixRows - 1; j > 0; j--)
                        {
                            (T value, int col, int row) CurrentItem = (Matrix[j, i], i, j);
                            EqualItems.Enqueue(CurrentItem);
                            if(EqualItems.Count() >= EqualCount)
                            {
                                if (EqualItems.All(x => x.value.Equals(CurrentItem.value)))
                                {
                                    var CoincidenceItems = EqualItems.AsEnumerable().Select(x => (x.col, x.row));
                                    Result.AddRange(CoincidenceItems);
                                }

                                EqualItems.Dequeue();
                            }
                        }
                        EqualItems.Clear();
                    }
                }


                return Result;
            }

            public void PrintMatrix(List<(int col, int row)> coincidencelist = null)
            {
                Action LinePrint = () =>
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < MatrixCols + 2; i++)
                    {
                        sb.Append("* ");
                    }
                    Console.Write($"{sb}\n");
                };

                Console.WriteLine("Matrix:");
                LinePrint();
                for (int i = 0; i < MatrixRows; i++)
                {
                    Console.Write("*");
                    for (int j = 0; j < MatrixCols; j++)
                    {
                        if (coincidencelist?.Count > 0 && coincidencelist.Contains((j,i)))
                            Console.ForegroundColor = ConsoleColor.Red;
                        else
                            Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" {Matrix[i, j]}");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" *\n");
                }
                LinePrint();
            }
            #endregion ClassMethods
        }

        class InvalidTypeException : Exception
        {
            internal InvalidTypeException(string message) : base(message) { }
        }
    }
}
