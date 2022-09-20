using System;
namespace Technical_Solution
{
    public class Matrix
    {
        protected internal double[,] elements;
        public int rows, columns;

        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            elements = new double[rows, columns];
        }

        public Matrix(double[,] elements)
        {
            this.elements = elements;
            rows = elements.GetLength(0);
            columns = elements.GetLength(1);
        }

        public double[] ModMultitplyVector(double[] vector)
        {
            double[] vectorOutput = new double[columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    vectorOutput[i] = (vectorOutput[i] + elements[i, j] * vector[j]) % 26;
                }
            }
            return vectorOutput;
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write($"{elements[i, j]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }


    }
}

