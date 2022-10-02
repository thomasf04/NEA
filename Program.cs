using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Technical_Solution
{

    internal class Program
    {
        struct SolKey<T>
        {
            public int[] solution;
            public T key;
        }

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

        //Increases plaintext length until it is a multiple of 'x'
        static int LengthOfPaddedPlaintext(int[] plaintext, int x)
        {
            int lengthOfPaddedPlaintext = plaintext.Length;
            if (plaintext.Length % x != 0)
            {
                lengthOfPaddedPlaintext += x - (plaintext.Length % x);
            }
            return lengthOfPaddedPlaintext;
        }

        static int[] PadPlaintext(int[] plaintext, int x, char nullCharacter)
        {
            int lengthOfPaddedPlaintext = LengthOfPaddedPlaintext(plaintext, x);
            int[] paddedPlaintext = new int[lengthOfPaddedPlaintext];
            plaintext.CopyTo(paddedPlaintext, 0);

            for (int i = plaintext.Length; i < lengthOfPaddedPlaintext; i++)
            {
                paddedPlaintext[i] = Integer(nullCharacter);
            }

            return paddedPlaintext;
        }

        static int[] CreateBadMatchTable(int[] crib)
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
        static List<int> BoyerMooreHorspool(int[] text, int[] crib)
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
                    Console.WriteLine(i);
                }

                i += table[text[i + x]];
            }

            return occurences;
        }

        static List<string> WordDictionary(string filename)
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

        static int mod(int a)
        {
            if (a < 0)
            {
                return a - 26 * (int)(a / 26 - 1);
            }
            return a % 26;
        }

        static bool Coprime26(int x)
        {
            if (x % 2 == 0 || x % 13 == 0)
            {
                return false;
            }
            return true;
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

        static SolKey<int> CaesarCipherBreak_BruteForce(string ciphertextString, double[] f)
        {
            SolKey<int> sk;
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            int bestShift = 0;

            bestFitness = TetragramFitness(ciphertext, f);
  
            for (int i = 1; i < 26; i++)
            {
                solution = CaesarCipherDecrypt(ciphertext, i);
                fitness = TetragramFitness(solution, f);
                if (fitness > bestFitness)
                {
                    bestSolution = solution;
                    bestFitness = fitness;
                    bestShift = i;
                }
            }

            sk.solution = bestSolution;
            sk.key = bestShift;
            return sk;
        }

        static SolKey<int> CaesarCipherBreak_Cribs(string ciphertextString, string cribString, double[] f)
        {
            SolKey<int> sk;
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);
            int[] crib = ConvertStringToIntegerArray(cribString);
            List<int> possibleShifts = new List<int>();
            bool validPosition;
            int currentShift;
            int i = 0, j = 0;
            int[] solution, bestSolution;
            double fitness, bestFitness;
            int bestShift;

            while (i <= ciphertext.Length - crib.Length)
            {
                j = 0;
                validPosition = true;
                currentShift = mod(ciphertext[i] - crib[j]);
                while (validPosition && j < crib.Length - 1)
                {
                    j++;
                    if (mod(ciphertext[i + j] - crib[j]) != currentShift)
                    {
                        validPosition = false;
                    }
                }
                if (validPosition)
                {
                    possibleShifts.Add(currentShift);
                }
                i++;
            }

            if (possibleShifts.Count == 0)
            {
                throw new CribNotFoundException();
            }


            bestSolution = CaesarCipherDecrypt(ciphertext, possibleShifts[0]);
            bestFitness = TetragramFitness(bestSolution, f);
            bestShift = 1;


            for (int k = 1; k < possibleShifts.Count; k++)
            {
                solution = CaesarCipherDecrypt(ciphertext, possibleShifts[k]);
                fitness = TetragramFitness(solution, f);
                Console.WriteLine(fitness);
                if (fitness > bestFitness)
                {
                    bestSolution = solution;
                    bestFitness = fitness;
                    bestShift = possibleShifts[k];
                }
            }

            sk.solution = bestSolution;
            sk.key = bestShift;
            return sk;
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
                    keyAlphabet[i] = lettersAvailable[i - keywordNoDuplicates.Length];
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

        static SolKey<string> KeywordSubstitutionBreak_Dictionary(string ciphertextString, double[] f)
        {
            SolKey<string> sk;
            List<string> words = WordDictionary("words.txt");
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            string bestWord = "";


            bestFitness = TetragramFitness(ciphertext, f);
            for (int mode = 1; mode <= 2; mode++)
            {
                foreach (string word in words)
                {

                    solution = KeywordSubstitutionCipherDecrypt(ciphertext, word, mode);
                    fitness = TetragramFitness(solution, f);
          
                    if (fitness > bestFitness)
                    {
                        bestSolution = solution;
                        bestFitness = fitness;
                        bestWord = word;
                    }
                }
            }


            sk.solution = bestSolution;
            sk.key = bestWord;
            return sk;
        }



        static int[] AffineCipherEncrypt(int[] plaintext, int a, int b)
        {
            if (!Coprime26(a))
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
            if (!Coprime26(a))
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


        static SolKey<(int,int)> AffineCipherBreak_BruteForce(string ciphertextString, double[] f)
        {
            SolKey<(int,int)> sk;
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            (int, int) bestKey = (0, 0);
            int[] validA = { 1, 3, 5, 7, 9, 11, 15, 17, 19, 21, 23, 25 };


            bestFitness = TetragramFitness(ciphertext, f);

            foreach (int a in validA)
            {
                for (int b = 0; b < 25; b++)
                {
                    solution = AffineCipherDecrypt(ciphertext, a, b);
                    fitness = TetragramFitness(solution, f);
                    if (fitness > bestFitness)
                    {
                        bestSolution = solution;
                        bestFitness = fitness;
                        bestKey = (a,b);
                    }
                }
            }

            sk.solution = bestSolution;
            sk.key = bestKey;
            return sk;
        }

        static (bool,int,int) CheckValidAffineCribPosition(int[] ciphertext, int[] crib, int i)
        {
            int j = 0, k = 0, x = 0, a = 0, b;
            bool validSubtraction = false;
            int[] ModularInverses = new int[] { 0, 1, 0, 9, 0, 21, 0, 15, 0, 3, 0, 19, 0, 0, 0, 7, 0, 23, 0, 11, 0, 5, 0, 17, 0, 25 };
            int[] ciphertextSplit = new int[crib.Length];
            Array.Copy(ciphertext, i, ciphertextSplit, 0, crib.Length);


            //Coefficient under substraction must be coprime with 26
            while (j < crib.Length - 1 && !validSubtraction)
            {
                k = j + 1;
                while (k < crib.Length && !validSubtraction)
                {
                    if (crib[k] > crib[j])
                    {
                        x = crib[k] - crib[j];
                        a = ciphertextSplit[k] - ciphertextSplit[j];
                    }
                    else
                    {
                        x = crib[j] - crib[k];
                        a = ciphertextSplit[j] - ciphertextSplit[k];
                    }
                    if (Coprime26(x) && Coprime26(a))
                    {
                        validSubtraction = true;
                    }
                    k++;
                }
                j++;
            }

            if (!validSubtraction)
            {
                return (false,0,0);
            }

            a = mod(ModularInverses[x] * a);
            b = mod(ciphertext[i] - crib[0] * a);

            if (Enumerable.SequenceEqual(AffineCipherEncrypt(crib, a, b), ciphertextSplit))
            {
                return (true,a,b);
            }
            return (false,0,0);
        }


        static SolKey<(int,int)> AffineCipherBreak_Cribs(string ciphertextString, string cribString, double[] f)
        {
            SolKey<(int,int)> sk;
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);
            int[] crib = ConvertStringToIntegerArray(cribString);
            List<(int,int)> possibleKeys = new List<(int,int)>();
            (bool,int,int) validPosition;
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            (int,int) bestKey = (0,0);

            for (int i = 0; i < ciphertext.Length - crib.Length; i++)
            {

                validPosition = CheckValidAffineCribPosition(ciphertext, crib, i);
                if (validPosition.Item1 == true)
                {
                    possibleKeys.Add((validPosition.Item2,validPosition.Item3));
                }
       
            }

            if (possibleKeys.Count == 0)
            {
                throw new CribNotFoundException();
            }


            bestFitness = TetragramFitness(ciphertext, f);
      

            foreach ((int,int) key in possibleKeys)
            {
                solution = AffineCipherDecrypt(ciphertext, key.Item1, key.Item2);
                fitness = TetragramFitness(solution, f);
                if (fitness > bestFitness)
                {
                    bestSolution = solution;
                    bestFitness = fitness;
                    bestKey = key;
                }
            }

            sk.solution = bestSolution;
            sk.key = bestKey;
            return sk;
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
            int[] plaintextPadded = PadPlaintext(plaintext, matrix.size, nullCharacter);
            int[] ciphertext = new int[plaintextPadded.Length];
            int[] subarray = new int[matrix.size];


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
            int[] ciphertext = new int[LengthOfPaddedPlaintext(plaintext, key)];
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

        static (int, bool) UpdateRailFenceRowNumber(int row, int key, bool increasing)
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
                (row, increasing) = UpdateRailFenceRowNumber(row, key, increasing);
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

        static int[] PermutationCipherEncrypt(int[] plaintext, int[] permutation, char nullCharacter)
        {
            int pLength = permutation.Length;
            int[] paddedPlaintext = PadPlaintext(plaintext, pLength, nullCharacter);
            int[] ciphertext = new int[paddedPlaintext.Length];

            for (int p = 0; p < pLength; p++)
            {
                for (int i = 0; i < paddedPlaintext.Length / pLength; i++)
                {
                    ciphertext[i * pLength + permutation[p]] = paddedPlaintext[i * pLength + p];
                }
            }
            return ciphertext;
        }

        static int[] PermutationCipherDecrypt(int[] ciphertext, int[] permutation)
        {
            int pLength = permutation.Length;
            int[] plaintext = new int[ciphertext.Length];

            for (int p = 0; p < pLength; p++)
            {
                for (int i = 0; i < ciphertext.Length / pLength; i++)
                {
                    plaintext[i * pLength + p] = ciphertext[i * pLength + permutation[p]];
                }
            }
            return plaintext;
        }

        static int[] ColumnarTranspositionCipherEncrypt(int[] plaintext, int[] permutation, char nullCharacter)
        {
            int pLength = permutation.Length;
            int[] paddedPlaintext = PadPlaintext(plaintext, pLength, nullCharacter);
            int[] ciphertext = new int[paddedPlaintext.Length];
            for (int p = 0; p < pLength; p++)
            {
                for (int i = 0; i < paddedPlaintext.Length / pLength; i++)
                {
                    ciphertext[permutation[p] * paddedPlaintext.Length / pLength + i] = paddedPlaintext[i * pLength + p];
                }
            }
            return ciphertext;

        }

        static int[] ColumnarTranspositionCipherDecrypt(int[] ciphertext, int[] permutation)
        {
            int pLength = permutation.Length;
            int[] plaintext = new int[ciphertext.Length];

            for (int p = 0; p < pLength; p++)
            {
                for (int i = 0; i < ciphertext.Length / pLength; i++)
                {
                    plaintext[i * pLength + p] = ciphertext[permutation[p] * ciphertext.Length / pLength + i];
                }
            }
            return plaintext;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------

        static double IndexOfCoincidence(int[] ciphertext)
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

        static double TetragramFitness(int[] ciphertext, double[] frequencies)
        {
            double fitness = 0;
            int[] subarray = new int[4];
            for (int i = 0; i <= ciphertext.Length - 4; i++)
            {
                Array.Copy(ciphertext, i, subarray, 0, 4);
                fitness += frequencies[TetragramIndex(subarray)];
            }
            return fitness / (ciphertext.Length - 3);
        }

        static double MonogramFitness(int[] ciphertext)
        {
            double[] englishFreqsArray = { 8.55, 1.60, 3.16, 3.87, 12.10, 2.18, 2.09, 4.96, 7.33, 0.22, 0.81, 4.21, 2.53, 7.17, 7.47, 2.07, 0.10, 6.33, 6.73, 8.94, 2.68, 1.06, 1.83, 0.19, 1.72, 0.11 };
            Vector englishFreqs = new Vector(englishFreqsArray);
            double[] messageFreqsArray = CreateFrequencyArray(ciphertext);
            Vector messageFreqs = new Vector(messageFreqsArray);
            double fitness = Vector.CosineAngle(messageFreqs, englishFreqs);
            return fitness;
        }

        static double[] CreateFrequencyArray(int[] input)
        {
            double[] frequencies = new double[26];

            for (int i = 0; i < 26; i++)
            {
                frequencies[i] = input.Count(c => c == i);
            }

            return frequencies;
        }

        static void CipherIdentifier(int[] ciphertext)
        {
            double[] logFrequencies = LogFrequencies();

            if (MonogramFitness(ciphertext) > 0)
            {
                if (TetragramFitness(ciphertext, logFrequencies) > 0)
                {
                    //English
                }
                else
                {
                    //Transposition
                }
            }
            else
            {
                if (IndexOfCoincidence(ciphertext) > 0)
                {
                    (double, bool) bestWithAffine = BestFitnessWithAffine(ciphertext);
                    if (bestWithAffine.Item1 > 0)
                    {
                        if (bestWithAffine.Item2)
                        {
                            //Caesar
                        }
                        else
                        {
                            //Affine
                        }
                    }
                    else
                    {
                        //keyword substitution
                    }
                }
                else
                {
                    if (IsCharacteristicOfPlayfairCipher(ciphertext))
                    {
                        //Playfair
                    }
                    else
                    {
                        double bestWithVigenere = BestFitnessWithVigenere(ciphertext);
                        if (bestWithVigenere > 0)
                        {
                            //vigenere
                        }
                        else
                        {
                            //hill
                        }
                    }
                }
            }


        }

        static (double, bool) BestFitnessWithAffine(int[] ciphertext)
        {
            return (0, true);
        }

        static double BestFitnessWithVigenere(int[] ciphertext)
        {
            return 0;
        }

        static bool IsCharacteristicOfPlayfairCipher(int[] ciphertext)
        {
            return true;
        }

        static void CreatingConfidenceIntervals()
        {
            Random rnd = new Random();
            int start;
            int tests = 100000;
            int length = 250;
            string corpus = File.ReadAllText("brown.txt");
            string text;
            double[] scores = new double[tests];

            for (int i = 0; i < tests; i++)
            {
                start = rnd.Next(0, corpus.Length - length);
                text = corpus.Substring(start, length);
                //text = ShuffleString(text);
                //text = RandomString(length);

                scores[i] = MonogramFitness(ConvertStringToIntegerArray(text));

            }
            Array.Sort(scores);
            Array.Reverse(scores);

            int p = 0;
            for (int i = 0; i < tests; i += tests / 200)
            {
                Console.WriteLine($"{p}% {scores[i]}");
                p += 1;
            }
            Console.WriteLine(scores[0]);
            Console.WriteLine(scores[tests - 1]);
        }


        static void Main(string[] args)
        {
            double[] f = LogFrequencies();


            string ciphertextString = "bzsynrefsaqybzsnkwanbzkngydbyjyqbjynsbzssansxsnbrkgsbzkqiymrhlswyxkhbzs".ToLower();
            string cribString = "the";
            int[] ciphertext = ConvertStringToIntegerArray(ciphertextString);

            int[] crib = ConvertStringToIntegerArray(cribString);
            SolKey<(int, int)> sk = AffineCipherBreak_Cribs(ciphertextString, cribString, f);
            Console.WriteLine(ConvertIntegerArrayToString(sk.solution));
     
            Console.ReadKey();

        }
    }
}
