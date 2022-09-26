using System;
namespace Technical_Solution
{
    public class Matrix
    {
        protected internal int[,] elements;
        public int rows, columns;

        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            elements = new int[rows, columns];
        }

        public Matrix(int[,] elements)
        {
            this.elements = elements;
            rows = elements.GetLength(0);
            columns = elements.GetLength(1);
        }

        public int[] ModMultitplyVector(int[] vector)
        {
            int[] vectorOutput = new int[columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    vectorOutput[i] = (vectorOutput[i] + elements[i, j] * vector[j]) % 26;
                }
            }
            return vectorOutput;
        }

        public void SwapRows(int rowA, int rowB)
        {
            int temp;

            for (int j = 0; j < columns; j++)
            {
                temp = elements[rowA, j];
                elements[rowA, j] = elements[rowB, j];
                elements[rowB, j] = temp;
            }
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

