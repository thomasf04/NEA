using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Technical_Solution
{

    public class Program
    {
        public static double[] frequencies = new double[0];
        public static Cipher[] ciphers;
        public static string[] cipherNames;
        public static bool walkthrough;

        public static void UpdateWalkthrough()
        {
            int choice = Menu("Would you like to activate walkthrough?","Yes","No");
            if (choice == 0)
            {
                walkthrough = true;
            }
            else
            {
                walkthrough = false;
            }
        }

        //Not my own code
        public static T GetKey<T>(object candidate)
        {
            if (!(candidate is T))
            {
                throw new Exception("Invalid Key");
            }
            return (T)candidate;
        }

        public static void PrintArray<T>(T[] array)
        {
            Console.Write("[ ");
            foreach (T item in array)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine("\b\b ]");
        }

        public static void PrintList<T>(List<T> list)
        {
            Console.Write("[ ");
            foreach (T item in list)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine("\b\b ]");
        }

        //Letter is assumed to be lower case, a-0, z-25
        public static int Integer(char letter)
        {
            return (int)letter - (int)'a';
        }

        //Message is assumed to be in lower case, a-0, z-25
        public static int[] ConvertStringToIntegerArray(string message)
        {
            int[] messageArray = new int[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                messageArray[i] = Integer(message[i]);
            }
            return messageArray;
        }

        public static string ConvertIntegerArrayToString(int[] messageArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in messageArray)
            {
                sb.Append((char)(i + (int)'a'));
            }
            return sb.ToString();
        }



        public static int[] CreateBadMatchTable(int[] crib)
        {
            int[] table = new int[26];
            int cribLength = crib.Length;

            for (int i = 0; i < 26; i++)
            {
                table[i] = cribLength;
            }

            for (int i = 0; i < cribLength - 1; i++)
            {
                table[crib[i]] = cribLength - i - 1;
            }

            return table;
        }


        //Requires crib to be at least 2 characters
        public static List<int> BoyerMooreHorspool(int[] text, int[] crib)
        {
            int cribLength = crib.Length;
            if (cribLength < 2)
            {
                throw new ArgumentOutOfRangeException("crib");
            }

            int[] table = CreateBadMatchTable(crib);
            int i = 0;
            int x;
            List<int> occurences = new List<int>();

            while (i <= text.Length - cribLength)
            {
                x = cribLength - 1;
                while (text[i + x] == crib[x] && x > 0)
                {
                    x -= 1;
                }
                if (x == 0)
                {
                    occurences.Add(i);
                }

                i += table[text[i + x]];
            }

            return occurences;
        }

        public static int factorial(int n)
        {
            int x = 1;
            for (int i = 1; i <= n; i++)
            {
                x *= i;
            }
            return x;
        }

        public static int[][] HeapsAlgorithm(int[] array)
        {
            int n = array.Length;
            int[][] permutations = new int[factorial(n)][];
            permutations[0] = (int[])array.Clone();
            int counter = 1;
            int[] c = new int[n];
            int temp;
            for (int j = 0; j < n; j++)
            {
                c[j] = 0;
            }

            int i = 0;
            while (i < n)
            {
                if (c[i] < i)
                {
                    if (i % 2 == 0)
                    {
                        temp = array[0];
                        array[0] = array[i];
                        array[i] = temp;
                    }
                    else
                    {
                        temp = array[c[i]];
                        array[c[i]] = array[i];
                        array[i] = temp;

                    }
                    permutations[counter] = (int[])array.Clone();
                    counter += 1;
                    c[i]++;
                    i = 0;
                }
                if (c[i] == i)
                {
                    c[i] = 0;
                    i++;
                }
            }
            return permutations;

        }

        public static int[][] HeapsAlgorithm(int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = i;
            }
            return HeapsAlgorithm(array);
        }

        public static List<string> WordDictionary(string filename)
        {
            List<string> words = new List<string>();
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    words.Add(sr.ReadLine());
                }
            }

            return words;
        }

        public static int mod(int a)
        {
            if (a < 0)
            {
                return a - 26 * (int)(a / 26 - 1);
            }
            return a % 26;
        }

        public static bool Coprime26(int x)
        {
            if (x % 2 == 0 || x % 13 == 0)
            {
                return false;
            }
            return true;
        }

        public static int GreatestCommonDivisor(int m, int n)
        {
            while (n != 0)
            {
                m = m % n;
                (m, n) = (n, m);
            }
            return m;
        }

        public static int Menu(string prompt, params string[] options)
        {
            Console.WriteLine(prompt);
            int offset = Console.CursorTop - 1;
            foreach (string option in options)
            {
                Console.WriteLine($"  {option}");
            }
            int optionNumber = 1;
            bool optionSelected = false;

            while (!optionSelected)
            {
                Console.CursorTop = offset + optionNumber;
                Console.CursorLeft = 0;
                Console.Write(">");

                ConsoleKeyInfo choice = Console.ReadKey(true);
                Console.CursorTop = offset + optionNumber;
                Console.CursorLeft = 0;
                Console.Write(" ");
                if (choice.Key == ConsoleKey.DownArrow && optionNumber < options.Length)
                {
                    optionNumber++;
                }
                else if (choice.Key == ConsoleKey.UpArrow && optionNumber > 1)
                {
                    optionNumber--;
                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    Console.CursorTop = offset + options.Length + 1;
                    optionSelected = true;
                }
            }
            Console.Clear();
            return optionNumber - 1;

        }

        public static T[][] GenerateCombinations<T>(T[] array, int l)
        {
            int cLength = (int)Math.Pow(array.Length, l);
            T[][] combinations = new T[cLength][];
            T[][] subCombinations;
            int counter = 0;


            if (l == 1)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    combinations[i] = new T[] { array[i] };
                }
                return combinations;
            }

            subCombinations = GenerateCombinations(array, l - 1);
            foreach (T[] combination in subCombinations)
            {
                foreach (T element in array)
                {
                    combinations[counter] = combination.Concat(new T[] { element }).ToArray();
                    counter++;
                }
            }

            return combinations;
        }

        public static int[] ConvertBaseN(int x, int n, int length)
        {
            int[] array = new int[length];

            for (int i = 0; i < length; i++)
            {
                array[length - i - 1] = x % n;
                x = x / n;
            }

            return array;
        }

        public static int[] ConvertBaseN(int x, int n)
        {
            int length;
            if (x == 0)
            {
                length = 1;
            }
            else
            {
                length = (int)Math.Log(x, n) + 1;

            }
            return ConvertBaseN(x, n, length);
        }

        public static int[,] Convert1DArrayToSquareArray(int[] array, int size)
        {
            if (array.Length != size * size)
            {
                throw new ArgumentException();
            }

            int[,] squareArray = new int[size, size];

            for (int i = 0; i < array.Length; i++)
            {
                squareArray[i / size, i % size] = array[i];
            }

            return squareArray;

        }

        public static string CleanString(string input)
        {
            string output = Regex.Replace(input.ToLower(), @"[^a-z]", "");
            return output;
        }

        public static T[][] SlicePeriodically<T>(T[] array, int period)
        {
            T[][] slices = new T[period][];
            int i, index;

            for (int j = 0; j < period; j++)
            {
                index = 0;
                i = j;
                slices[j] = new T[(array.Length - j - 1) / period + 1];
                while (i < array.Length)
                {
                    slices[j][index] = array[i];
                    i += period;
                    index += 1;
                }
            }
            return slices;
        }

        static int[] PermutationInverse(int[] permutation)
        {
            int[] inverse = new int[permutation.Length];
            for (int i = 0; i < permutation.Length; i++)
            {
                inverse[permutation[i]] = i;
            }
            return inverse;
        }

        static int[] MonoDecrypt(int[] ciphertext, int[] keyAlphabet)
        {
            int[] plaintext = new int[ciphertext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = keyAlphabet[ciphertext[i]];
            }
            return plaintext;
        }

        static SolKey<string> MonoalphabeticHillClimbing(int[] ciphertext)
        {
            SolKey<string> sk;
            int[] parentPlaintext, childPlaintext;
            int[] parentKeyAlphabet = new int[26];
            int[] childKeyAlphabet;
            Random rnd = new Random();
            int x, y, temp;
            double parentFitness, childFitness;
            int counter = 0;

            for (int i = 0; i < 26; i++)
            {
                parentKeyAlphabet[i] = i;
            }

            parentPlaintext = MonoDecrypt(ciphertext, parentKeyAlphabet);
            parentFitness = TetragramFitness(ciphertext);

            while (counter < 10000)
            {
                childKeyAlphabet = (int[])parentKeyAlphabet.Clone();
                x = rnd.Next(26);
                y = rnd.Next(26);
                temp = childKeyAlphabet[x];
                childKeyAlphabet[x] = childKeyAlphabet[y];
                childKeyAlphabet[y] = temp;

                childPlaintext = MonoDecrypt(ciphertext, childKeyAlphabet);
                childFitness = TetragramFitness(childPlaintext);

                if (childFitness > parentFitness)
                {
                    Console.WriteLine(childFitness);
                    parentKeyAlphabet = (int[])childKeyAlphabet.Clone();
                    parentPlaintext = (int[])childPlaintext.Clone();
                    parentFitness = childFitness;
                    counter = 0;
                }
                counter += 1;
            }
            sk.solution = parentPlaintext;
            sk.key = ConvertIntegerArrayToString(PermutationInverse(parentKeyAlphabet));
            return sk;
        }

        public static int[] ConvertWordToPermutation(string wordString)
        {
            int[] word = ConvertStringToIntegerArray(wordString).Distinct().ToArray();
            Console.WriteLine(ConvertIntegerArrayToString(word));
            int[] wordSorted = (int[])word.Clone();
            Array.Sort(wordSorted);
            int[] permutation = new int[word.Length];

            for (int i = 0; i < permutation.Length; i++)
            {
                permutation[i] = Array.IndexOf(wordSorted, word[i]);
            }

            return permutation;
        }



        //--------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------

        public static double IndexOfCoincidence(int[] ciphertext)
        {
            double ioc = 0;
            int count;
            for (int i = 0; i <= 26; i++)
            {
                count = ciphertext.Count(x => x == i);
                ioc += count * (count - 1);
            }
            ioc /= (ciphertext.Length * (ciphertext.Length - 1));
            ioc *= 26;
            return ioc;
        }

        public static int NormaliseIndexOfCoincidence(double fitness)
        {
            if (fitness < 0.9021)
            {
                return 0;
            }
            if (fitness > 1.7342)
            {
                return 100;
            }
            else
            {
                return (int)(120.178 * fitness - 108.412);

            }
        }

        static double[] LogFrequencies()
        {
            double[] frequencies = new double[26 * 26 * 26 * 26];

            using (BinaryReader br = new BinaryReader(File.Open("LogFrequencies.file", FileMode.OpenOrCreate)))
            {
                for (int i = 0; i < 26 * 26 * 26 * 26; i++)
                {
                    frequencies[i] = br.ReadDouble();
                }
            }

            return frequencies;
        }

        static int TetragramIndex(int[] subarray)
        {
            int i = subarray[3] + 26 * subarray[2] + 676 * subarray[1] + 17576 * subarray[0];
            return i;
        }

        public static double TetragramFitness(int[] text)
        {
            double fitness = 0;
            int[] subarray = new int[4];
            for (int i = 0; i <= text.Length - 4; i++)
            {
                Array.Copy(text, i, subarray, 0, 4);
                fitness += frequencies[TetragramIndex(subarray)];
            }
            return fitness / (text.Length - 3);
        }

        public static int NormaliseTetragramFitness(double fitness)
        {
            if (fitness <= -4.75)
            {
                return (int)(19.048 * fitness + 190.48);
            }
            else
            {
                return 100;
            }
        }

        public static double MonogramFitness(int[] ciphertext)
        {
            double[] englishFreqsArray = { 8.55, 1.60, 3.16, 3.87, 12.10, 2.18, 2.09, 4.96, 7.33, 0.22, 0.81, 4.21, 2.53, 7.17, 7.47, 2.07, 0.10, 6.33, 6.73, 8.94, 2.68, 1.06, 1.83, 0.19, 1.72, 0.11 };
            double englishFreqsArrayMagnitude = 25.510137200728654;
            Vector englishFreqs = new Vector(englishFreqsArray);
            double[] messageFreqsArray = CreateFrequencyArray(ciphertext);
            Vector messageFreqs = new Vector(messageFreqsArray);
            double fitness = Vector.CosineAngle(messageFreqs, englishFreqs, englishFreqsArrayMagnitude);
            return fitness;
        }

        public static double[] CreateFrequencyArray(int[] input)
        {
            double[] frequencies = new double[26];

            for (int i = 0; i < 26; i++)
            {
                frequencies[i] = input.Count(c => c == i);
            }

            return frequencies;
        }

        public static int NormaliseMonogramFitness(double fitness)
        {
            return (int)(100 * fitness);
        }

        //For use in cipher identifier
        public enum Ciphers
        {
            English,
            Transpostion,
            Caesar,
            Affine,
            KeywordSubstitution,
            Vigenere,
            Hill
        }


        static Cipher CipherIdentifier(int[] ciphertext)
        {
            if (NormaliseMonogramFitness(MonogramFitness(ciphertext)) == 100)
            {
                if (NormaliseTetragramFitness(TetragramFitness(ciphertext)) == 100)
                {
                    //English
                    return null;
                }
                else
                {
                    //Transposition
                    return null;                       
                }
            }
            else
            {
                if (NormaliseIndexOfCoincidence(IndexOfCoincidence(ciphertext)) == 100)
                {
                    (double, bool) bestWithAffine = BestFitnessWithAffine(ciphertext);
                    if (NormaliseTetragramFitness(bestWithAffine.Item1) == 100)
                    {
                        if (bestWithAffine.Item2 == true)
                        {
                            return CaesarCipher.Instance;
                        }
                        else
                        {
                            return AffineCipher.Instance;
                        }
                    }
                    else
                    {
                        //Or MONO!
                        return Keyword_Substitution_Cipher.Instance;
                    }
                }
                else
                {
                    double bestWithVigenere = BestFitnessWithVigenere(ciphertext);
                    if (NormaliseTetragramFitness(bestWithVigenere) == 100)
                    {
                        return VigenereCipher.Instance;
                    }
                    else
                    {
                        return HillCipher.Instance;
                    }
                    
                }
            }


        }

        static (double, bool) BestFitnessWithAffine(int[] ciphertext)
        {
            AffineCipher cipher = AffineCipher.Instance;
            cipher.SetBreakMethod(new AC_BruteForce());
            SolKey<(int,int)> sk = cipher.Break_sk();
            int[] plaintext = sk.solution;
            double fitness = TetragramFitness(plaintext);
            int a = sk.key.Item1;
            bool equivalentToCaesar = (a == 1);
            return (fitness, equivalentToCaesar);
               
        }

        static double BestFitnessWithVigenere(int[] ciphertext)
        {
            VigenereCipher cipher = VigenereCipher.Instance;
            cipher.SetBreakMethod(new VC_BruteForce());
            SolKey<string> sk = cipher.Break_sk();
            int[] plaintext = sk.solution;
            double fitness = TetragramFitness(plaintext);
            return fitness;
        }
     

        static int[] GetText(int entryChoice, string textType)
        {
            string text= "";
            if (entryChoice == 0)
            {
                Console.WriteLine($"Enter the {textType} below:");
                text = Console.ReadLine();
            }
            if (entryChoice == 1)
            {
                Console.WriteLine("Enter the filename (include extension):");
                string filename = Console.ReadLine();
                text = File.ReadAllText(filename);
            }
            return ConvertStringToIntegerArray(CleanString(text));
        }

        static int[] Text(string textType)
        {
            int[] text = new int[0];
            bool validText = false;
            while (!validText)
            {
                int entryChoice = Menu($"Enter {textType} into console or load from file?", "Enter into console", "Load from file");
                try
                {
                    text = GetText(entryChoice, textType);
                    validText = true;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("No file with that name was found");
                }
            }
            return text;
        }



        static void Encipher()
        {
            int[] plaintext = Text("plaintext");
            int[] ciphertext;
            int cipherChoice = Menu("Which cipher would you like to encrypt the message with?", cipherNames);
            UpdateWalkthrough();
            Cipher cipher = ciphers[cipherChoice];
            ciphertext = cipher.Encrypt(plaintext);
            string ciphertextString = ConvertIntegerArrayToString(ciphertext);
            Console.WriteLine(ciphertextString);
        }

        static void Decipher()
        {
            int[] ciphertext = Text("ciphertext");
            int[] plaintext;
            string plaintextString;
            string keyString;
            int cipherKnown = Menu("Do you know which cipher has been used to encrypt this?", "Yes","No");

            Cipher cipher;
            if (cipherKnown == 0)
            {
                cipher = SelectCipher();
            }
            else
            {
                UpdateWalkthrough();
                cipher = CipherIdentifier(ciphertext);
            }

            int keyKnown = Menu("Do you know the key that has been used to encrypt this message?", "Yes", "No");
            if (keyKnown == 0)
            {
                plaintext = cipher.Decrypt(ciphertext);
                plaintextString = ConvertIntegerArrayToString(plaintext);
            }
            else
            {
                BreakMethod.SetCiphertext(ciphertext);
                UpdateWalkthrough();
                (plaintext, keyString) = cipher.Break();
                plaintextString = ConvertIntegerArrayToString(plaintext);
                Console.WriteLine($"The key used was : {keyString}");
            }

            Console.WriteLine("The plaintext is:");
            Console.WriteLine(plaintextString);

        }

        static Cipher SelectCipher()
        {
            int cipherChoice = Menu("Which cipher would you like to decrypt the message with?", cipherNames);
            UpdateWalkthrough();
            Cipher cipher = ciphers[cipherChoice];
            return cipher;
        }

        static void Main(string[] args)
        {
            double[] f = LogFrequencies();
            frequencies = f;

            //Initialise ALL ciphers
            AffineCipher AC = AffineCipher.Instance;
            CaesarCipher CC = CaesarCipher.Instance;
            Keyword_Substitution_Cipher KS = Keyword_Substitution_Cipher.Instance;
            VigenereCipher VC = VigenereCipher.Instance;
            HillCipher HC = HillCipher.Instance;
            ScytaleCipher SC = ScytaleCipher.Instance;
            RailfenceCipher RF = RailfenceCipher.Instance;
            PermutationCipher PC = PermutationCipher.Instance;
            ColumnarTranspositionCipher CT = ColumnarTranspositionCipher.Instance;

            ciphers = new Cipher[] {AC, CC, KS, VC, HC, SC, RF, PC, CT }.OrderBy(c => c.Id).ToArray();
           
            cipherNames = new string[ciphers.Length];
            for (int i = 0; i < ciphers.Length; i++)
            {
                cipherNames[i] = ciphers[i].Name;
            }


            //Initialise ALL breakmethods
            CC_BruteForce ccBruteForce = new CC_BruteForce();
            CC_Cribs ccCribs = new CC_Cribs();
            KS_Dictionary ksDictionary = new KS_Dictionary();
            AC_BruteForce acBruteForce = new AC_BruteForce();
            AC_Cribs acCribs = new AC_Cribs();
            VC_Cribs1 vcCribs1 = new VC_Cribs1();
            VC_Cribs2 vcCribs2 = new VC_Cribs2();
            VC_BruteForce vcBruteForce = new VC_BruteForce();
            HC_BruteForce hcBruteForce = new HC_BruteForce();
            SC_BruteForce scBruteForce = new SC_BruteForce();
            RF_BruteForce rfBruteForce = new RF_BruteForce();
            PC_BruteForce pcBruteForce = new PC_BruteForce();
            CT_BruteForce ctBruteForce = new CT_BruteForce();


            int choice;
            while (true)
            {
                Console.WriteLine("Welcome to CrytpanalAssistor!");
                choice = Menu("Would you like to encipher or decipher a message?","Encipher","Decipher");
                if (choice == 0)
                {
                    Encipher();
                }
                if (choice == 1)
                {
                    Decipher();
                }
                Console.ReadKey();
            }
        }
    }

}
