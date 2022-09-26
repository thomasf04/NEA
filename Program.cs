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
            int[] ModularInverses = new int[] { 0, 1, 0, 9, 0, 21, 0, 15, 0, 3, 0, 19, 0, 0, 0, 7, 0, 23, 0, 11, 0, 5, 0, 17, 0, 25 };
            int aPrime, bPrime;

            aPrime = ModularInverses[a];
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

        static int[] HillCipherEncrypt(int[] plaintext, SquareMatrix matrix, char nullCharacter)
        {
            int lengthOfPaddedPlaintext = plaintext.Length;
            if (plaintext.Length % matrix.size != 0)
            {
                lengthOfPaddedPlaintext += matrix.size - (plaintext.Length % matrix.size);
            }

            int[] plaintextPadded = new int[lengthOfPaddedPlaintext];
            int[] ciphertext = new int[plaintextPadded.Length];
            int[] subarray = new int[matrix.size];

            for (int i = 0; i < plaintextPadded.Length; i++)
            {
                if (i < plaintext.Length)
                {
                    plaintextPadded[i] = plaintext[i];
                }
                else
                {
                    plaintextPadded[i] = Integer(nullCharacter);
                }
            }

            for (int i = 0; i < plaintextPadded.Length; i += matrix.size)
            {
                Array.Copy(plaintextPadded, i, subarray, 0, matrix.size);
                Array.Copy(matrix.ModMultitplyVector(subarray), 0, ciphertext, i, matrix.size);
            }

            return ciphertext;

        }

        static int[] HillCipherDecrypt(int[] ciphertext, SquareMatrix matrix)
        { 
            int[] plaintext = HillCipherEncrypt(ciphertext, matrix.Inverse(), 'x');
            return plaintext;
        }

        //Key is taken to be number of columns of matrix
        static int[] ScytaleCipherEncrypt(int[] plaintext, int key, char nullCharacter)
        {
            int lengthOfPaddedPlaintext = plaintext.Length;
            if (plaintext.Length % key != 0)
            {
                lengthOfPaddedPlaintext += key - (plaintext.Length % key);
            }

            int[] ciphertext = new int[lengthOfPaddedPlaintext];
            int counter = 0;

            //Integer division in outer for loop
            for (int i = 0; i < ciphertext.Length / key; i++)
            {
                for (int j = 0; j < key; j++)
                {
                    if (counter < plaintext.Length)
                    {
                        ciphertext[ciphertext.Length / key * j + i] = plaintext[counter];

                    }
                    else
                    {
                        ciphertext[ciphertext.Length / key * j + i] = Integer(nullCharacter);
                    }
                    counter += 1;
                }
            }

            return ciphertext;
        }

        //Key is taken to be number of columns of matrix
        static int[] ScytaleCipherDecrypt(int[] ciphertext, int key)
        {
            int[] plaintext = ScytaleCipherEncrypt(ciphertext, ciphertext.Length / key, 'x');
            return plaintext;
        }

        static (int,bool) UpdateRailFenceRowNumber(int row, int key, bool increasing)
        {
            if (increasing)
            {
                row += 1;
                if (row == key)
                {
                    increasing = false;
                    row = key - 2;
                }
            }
            else
            {
                row -= 1;
                if (row == -1)
                {
                    increasing = true;
                    row = 1;
                }
            }
            return (row, increasing);
        }

        static int[] RailFenceCipherEncrypt(int[] plaintext, int key, int offset)
        {
            int[] ciphertext = new int[plaintext.Length];
            List<int>[] rows = new List<int>[key];
            int row = 0, counter = 0;
            bool increasing = true;

            for (int i = 0; i < offset; i++)
            {
                (row,increasing) = UpdateRailFenceRowNumber(row, key, increasing);
            }

            for (int i = 0; i < key; i++)
            {
                rows[i] = new List<int>();
            }

            foreach (int i in plaintext)
            {
                rows[row].Add(i);
                (row, increasing) = UpdateRailFenceRowNumber(row, key, increasing);
            }

            foreach (List<int> r in rows)
            {
                foreach (int i in r)
                {
                    ciphertext[counter] = i;
                    counter += 1;
                }
            }

            return ciphertext;
        }

        static int[] RailFenceCipherDecrypt(int[] ciphertext, int key, int offset)
        {
            int[] plaintext = new int[ciphertext.Length];
            int[] numbers = new int[ciphertext.Length];
            int[] order = new int[ciphertext.Length];

            for (int i = 0; i < ciphertext.Length; i++)
            {
                numbers[i] = i;
            }

            order = RailFenceCipherEncrypt(numbers, key, offset);


            for (int i = 0; i < order.Length; i++)
            {
                plaintext[order[i]] = ciphertext[i];
            }

            return plaintext;
        }

        static int[] PermutationCipherEncrypt(int[] plaintext, )

        static void Main(string[] args)
        {
            Console.WriteLine("Enter message:");
            string plaintext = Console.ReadLine().ToLower();
            int[] plaintextArray = ConvertStringToIntegerArray(plaintext);
            int[] ciphertextArray = RailFenceCipherDecrypt(plaintextArray,3,3);
            string ciphertext = ConvertIntegerArrayToString(ciphertextArray);
            Console.WriteLine(ciphertext);

            Console.ReadKey();

        }
    }
}
