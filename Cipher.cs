using System;
namespace Technical_Solution
{
    public abstract class Cipher<T>
    {
        private BreakMethod<T> breakMethod;
        protected SolKey<T> sk;

        public Cipher()
        {

        }

        public void SetBreakMethod(BreakMethod<T> breakMethod)
        {
            this.breakMethod = breakMethod;
        }

        public SolKey<T> Break(int[] ciphertext)
        {
            return breakMethod.Break(ciphertext);
        }

        public abstract int[] Encrypt(int[] plaintext, T key);

        public abstract int[] Decrypt(int[] ciphertext, T key);

    }

    public struct SolKey<T>
    {
        public int[] solution;
        public T key;
    }
        
}
