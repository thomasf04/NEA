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
            Console.Write("\b\b ]");
        }

        //Message is assumed to be in lower case, a-0, z-25
        static int[] ConvertStringToIntegerArray(string message)
        {
            int[] messageArray = new int[message.Length]; 
            for (int i = 0; i < message.Length; i++)
            {
                messageArray[i] = (int)message[i] - (int)'a';
            }
            return messageArray;
        }

        static string ConvertIntegerArrayToString(int[] messageArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach(int i in messageArray)
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
            return CaesarCipherEncrypt(ciphertext, 26-shift);
        }


        //Mode 1 adds letters alphabetically after last letter of keyword e.g. keywordabcfg...
        //Mode 2 adds letters alphabetically starting with last letter of key word e.g. keywordefghi...
        static void KeywordSubstitutionCipherEncrypt(int[] plaintext, string keyword, int mode)
        {

        }




        static void Main(string[] args)
        {
   

        }
    }
}
