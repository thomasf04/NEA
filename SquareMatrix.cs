using System;
namespace Technical_Solution
{
    public class SquareMatrix : Matrix
    {
        public int size;

        //Identity matrix of dimension equal to 'size'
        public SquareMatrix(int size) : base(size, size)
        {
            this.size = size;
            rows = size;
            columns = size;
            elements = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                elements[i, i] = 1;
            }
        }

        public SquareMatrix(double[,] elements) : base(elements)
        {
            this.elements = elements;
            size = elements.GetLength(0);
        }

        public Matrix Inverse()
        {
            Matrix augMatrix = new Matrix(size, 2 * size);
            double x;

            //Fill augMatrix with original matrix
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    augMatrix.elements[i, j] = elements[i, j];
                }
            }

            //Fill matrix with identity matrix
            for (int i = 0; i < size; i++)
            {
                augMatrix.elements[i, i + size] = 1;
            }
            Console.WriteLine(true);
            augMatrix.PrintMatrix();


            for (int k = 0; k < size; k++)
            {
                x = augMatrix.elements[k, k];
                if (x == 0)
                {
                    throw new ZeroDeterminantException();
                }
                for (int j = 0; j < 2 * size; j++)
                {
                    augMatrix.elements[k, j] /= x;
                }

                for (int i = k+1; i < size; i++)
                {
                    x = augMatrix.elements[i, k];
                    for (int j = 0; j < 2 * size; j++)
                    {
                        augMatrix.elements[i, j] -= x * augMatrix.elements[k, j];
                    }
                }
            }
            augMatrix.PrintMatrix();
            for (int k = 0; k < size; k++)
            {
                for (int j = k+1; j < size; j++)
                {
                    x = augMatrix.elements[k, j];
                    for (int j2 = j; j2 < 2 * size; j2++)
                    {
                        augMatrix.elements[k, j2] -= x * augMatrix.elements[j, j2];
                    }
                }
            }


            return augMatrix;
        }


    }
}
