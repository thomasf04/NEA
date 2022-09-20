using System;
namespace Technical_Solution
{
    public class ZeroDeterminantException : Exception
    {
        public ZeroDeterminantException() : base("The determinant of the matrix is zero")
        {
        }
    }
}
