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
            elements = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                elements[i, i] = 1;
            }
        }

        public SquareMatrix(int[,] elements) : base(elements)
        {
            this.elements = elements;
            size = elements.GetLength(0);
        }

        public SquareMatrix Inverse()
        {
            Matrix augMatrix = new Matrix(size, 2 * size);
            SquareMatrix inverseMatrix = new SquareMatrix(size);
            int[] ModularInverses = new int[] { 0, 1, 0, 9, 0, 21, 0, 15, 0, 3, 0, 19, 0, 0, 0, 7, 0, 23, 0, 11, 0, 5, 0, 17, 0, 25 };
            int x;

            //C# '%' operator returns negative values
            int mod(int a)
            {
                if (a < 0)
                {
                    return a - 26 * (int)(a / 26 - 1);
                }
                return a % 26;
            }

            void SwapRowsUntilLegalX(int k)
            {
                int x;
                int newK = k;
                do
                {
                    x = augMatrix.elements[newK, k];
                    newK += 1;
                } while (x % 2 == 0 || x % 13 == 0);

                augMatrix.SwapRows(k, newK-1);
            }
            

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

            //Into echelon form
            for (int k = 0; k < size; k++)
            {
                x = mod(augMatrix.elements[k, k]);
                if (x % 2 == 0 || x % 13 == 0)
                {
                    SwapRowsUntilLegalX(k);
                }
                x = mod(augMatrix.elements[k, k]);


                if (x == 0)
                {
                    throw new ZeroDeterminantException();
                }
                for (int j = 0; j < 2 * size; j++)
                {
                    augMatrix.elements[k, j] *=  ModularInverses[x];
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
            //Back substitution
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

            //Fill inverse matrix with right-hand side of augMatrix
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    inverseMatrix.elements[i, j] = mod(augMatrix.elements[i, j + size]);
                }
            }


            return inverseMatrix;
        }


    }
}
