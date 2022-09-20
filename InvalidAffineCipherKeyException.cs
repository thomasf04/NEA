using System;
namespace Technical_Solution
{
    public class InvalidAffineCipherKeyException : Exception
    {
        public InvalidAffineCipherKeyException(int a) : base($"The 'a' coefficient, {a}, is not coprime with 26.")
        {
        }
    }
}
