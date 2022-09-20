using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class Program
    {

        static void PrintArray<T>(T[] array)
        {
            Console.Write("[ ");
            foreach (T item in array)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine("\b\b ]");
        }

        static void PrintList<T>(List<T> list)
        {
            Console.Write("[ ");
            foreach (T item in list)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine("\b\b ]");
        }

        //Letter is assumed to be lower case, a-0, z-25
        static int Integer(char letter)
        {
            return (int)letter - (int)'a';
        }

        //Message is assumed to be in lower case, a-0, z-25
        static int[] ConvertStringToIntegerArray(string message)
        {
            int[] messageArray = new int[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                messageArray[i] = Integer(message[i]);
            }
            return messageArray;
        }

        static string ConvertIntegerArrayToString(int[] messageArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in messageArray)
            {
                sb.Append((char)(i + (int)'a'));
            }
            return sb.ToString();
        }

        static int[] CaesarCipherEncrypt(int[] plaintext, int shift)
        {
            int[] ciphertext = new int[plaintext.Length];

            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext[i] = (plaintext[i] + shift) % 26;
            }

            return ciphertext;

        }

        static int[] CaesarCipherDecrypt(int[] ciphertext, int shift)
        {
            return CaesarCipherEncrypt(ciphertext, 26 - shift);
        }


        //Mode 1 adds letters alphabetically after last letter of keyword e.g. keywordabcfg...
        //Mode 2 adds letters alphabetically starting with last letter of key word e.g. keywordefghi...
        static int[] KeywordSubstitutionCipherEncrypt(int[] plaintext, string keyword, int mode)
        {
            int[] ciphertext = new int[plaintext.Length];
            int[] keyAlphabet = new int[26];
            keyAlphabet = KeywordSubstitutionAlphabet(keyword, mode);

            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext[i] = keyAlphabet[plaintext[i]];
            }

            return ciphertext;
        }

        static int[] KeywordSubstitutionCipherDecrypt(int[] ciphertext, string keyword, int mode)
        {
            int[] plaintext = new int[ciphertext.Length];
            int[] keyAlphabetInverse = new int[26];
            int[] keyAlphabet = new int[26];

            keyAlphabetInverse = KeywordSubstitutionAlphabet(keyword, mode);
            for (int i = 0; i < 26; i++)
            {
                keyAlphabet[keyAlphabetInverse[i]] = i;
            }

            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = keyAlphabet[ciphertext[i]];
            }

            return plaintext;
        }

        static int[] KeywordSubstitutionAlphabet(string keyword, int mode)
        {
            List<int> lettersAvailable = new List<int>();
            int[] keyAlphabet = new int[26];
            int lastLetter, currentLetter;
          

            for (int i = 0; i < 26; i++)
            {
                lettersAvailable.Add(i);
            }

            int[] keywordNoDuplicates = ConvertStringToIntegerArray(keyword).Distinct().ToArray();

            for (int i = 0; i < keywordNoDuplicates.Length; i++)
            {
                keyAlphabet[i] = keywordNoDuplicates[i];
                lettersAvailable.Remove(keywordNoDuplicates[i]);
            }


            if (mode == 1)
            {
                for (int i = keywordNoDuplicates.Length; i < 26; i++)
                {
                    keyAlphabet[i] = lettersAvailable[i- keywordNoDuplicates.Length];  
                }
            }

            if (mode == 2)
            {
                lastLetter = keywordNoDuplicates[keywordNoDuplicates.Length - 1];
                currentLetter = (lastLetter + 1) % 26;
                for (int i = keywordNoDuplicates.Length; i < 26; i++)
                {

                    while (!lettersAvailable.Contains(currentLetter))
                    {
                        currentLetter = (currentLetter + 1) % 26;
                    }
                    keyAlphabet[i] = currentLetter;
                    lettersAvailable.Remove(currentLetter);
                }
            }

            return keyAlphabet;
        }

        static int[] AffineCipherEncrypt(int[] plaintext, int a, int b)
        {
            if (a % 2 == 0 || a % 13 == 0)
            {
                throw new InvalidAffineCipherKeyException(a);
            }

            int[] ciphertext = new int[plaintext.Length];

            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext[i] = (a * plaintext[i] + b) % 26; 
            }
            return ciphertext;
        }

        static int[] AffineCipherDecrypt(int[] ciphertext, int a, int b)
        {
            if (a % 2 == 0 || a % 13 == 0)
            {
                throw new InvalidAffineCipherKeyException(a);
            }

            int[] plaintext = new int[ciphertext.Length];
            int[] ModularInverses = new int[] { 1, 0, 9, 0, 21, 0, 15, 0, 3, 0, 19, 0, 0, 0, 7, 0, 23, 0, 11, 0, 5, 0, 17, 0, 25 };
            int aPrime, bPrime;

            aPrime = ModularInverses[a - 1];
            bPrime = 26 - b;

            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = (aPrime * (ciphertext[i] + bPrime)) % 26;
            }

            return plaintext;
        }

        static int[] VigenereCipherEncrypt(int[] plaintext, string key)
        {
            int[] ciphertext = new int[plaintext.Length];
            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext[i] = (plaintext[i] + Integer(key[i % key.Length])) % 26;
            }
            return ciphertext;
        }

        static int[] VigenereCipherDecrypt(int[] ciphertext, string key)
        {
            int[] plaintext = new int[ciphertext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = (ciphertext[i] + 26 - Integer(key[i % key.Length])) % 26;
            }
            return plaintext;
        }

        //static int[] HillCipherEncrypt(int[] plaintext, Matrix matrix, char nullCharacter)
        //{
        //    int[] plaintextPadded = new int[plaintext.Length + matrix.size - (plaintext.Length % matrix.size)];
        //    int[] ciphertext = new int[plaintextPadded.Length];
        //    double[] subarray = new double[matrix.size];

        //    for (int i = 0; i < plaintextPadded.Length; i++)
        //    {
        //        if (i < plaintext.Length)
        //        {
        //            plaintextPadded[i] = plaintext[i];
        //        }
        //        else
        //        {
        //            plaintextPadded[i] = Integer(nullCharacter);
        //        }
        //    }

        //    for (int i = 0; i < plaintextPadded.Length; i+=matrix.size)
        //    {
        //        Array.Copy(plaintextPadded, i, subarray, 0, matrix.size);
        //        Array.Copy(matrix.ModMultitplyVector(subarray), 0, ciphertext, i, matrix.size);
        //    }

        //    return ciphertext;

        //}


        static void MatrixTest()
        {
            double[,] array = new double[,]{ { 4,2,1 },{ -3,1,0},{ 1,3,1} }; //{ { 2,1,2}, { 1,3,3 }, { 5,6,7 } };
            SquareMatrix matrix = new SquareMatrix(array);
            //matrix.PrintMatrix();
            Matrix inverse = matrix.Inverse();
            inverse.PrintMatrix();
  
        }

        static void Main(string[] args)
        {
            MatrixTest();
            Console.WriteLine("Enter message:");
            string plaintext = Console.ReadLine().ToLower();

          
            int[] plaintextArray = ConvertStringToIntegerArray(plaintext);
            //string ciphertext = ConvertIntegerArrayToString(HillCipherEncrypt(plaintextArray, new Matrix(new double[,] { { 7,8,11},{ 11,2,8},{ 15,7,4} }),'x'));
            //Console.WriteLine(ciphertext);

            Console.ReadKey();

        }
    }
}
