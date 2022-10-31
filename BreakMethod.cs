using System;
namespace Technical_Solution
{
    public abstract class BreakMethod<T>
    {
        protected Cipher<T> cipher;

        public BreakMethod(Cipher<T> cipher)
        {
            this.cipher = cipher;
        }

        public abstract SolKey<T> Break(int[] ciphertext);

    }
}
