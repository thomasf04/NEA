using System;
using System.Collections.Generic;
using System.Linq;

namespace Technical_Solution
{
    public class AffineCipher : Cipher<(int,int)>
    {
        public override int[] Encrypt(int[] plaintext, (int,int) key)
        {
            int a = key.Item1;
            int b = key.Item2;

            if (!Program.Coprime26(a))
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

        public override int[] Decrypt(int[] ciphertext, (int, int) key)
        {
            int a = key.Item1;
            int b = key.Item2;

            if (!Program.Coprime26(a))
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

    }
    //----------------------------------------------------------------------------------------------------

    public class AC_BruteForce : BreakMethod<(int, int)>
    {
        public AC_BruteForce(Cipher<(int, int)> cipher) : base(cipher) { }

        public override SolKey<(int, int)> Break(int[] ciphertext)
        {
            SolKey<(int, int)> sk;
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            (int, int) bestKey = (0, 0);
            int[] validA = { 1, 3, 5, 7, 9, 11, 15, 17, 19, 21, 23, 25 };


            bestFitness = Program.TetragramFitness(ciphertext);

            foreach (int a in validA)
            {
                for (int b = 0; b < 25; b++)
                {
                    solution = cipher.Decrypt(ciphertext, (a, b));
                    fitness = Program.TetragramFitness(solution);
                    if (fitness > bestFitness)
                    {
                        bestSolution = solution;
                        bestFitness = fitness;
                        bestKey = (a, b);
                    }
                }
            }

            sk.solution = bestSolution;
            sk.key = bestKey;
            return sk;
        }
    }
    //----------------------------------------------------------------------------------------------------

    public class AC_Cribs : BreakMethod<(int, int)>
    {
        private string cribString;

        public AC_Cribs(Cipher<(int, int)> cipher) : base(cipher) { }

        public AC_Cribs(Cipher<(int, int)> cipher, string cribString) : base(cipher)
        {
            this.cribString = cribString;
        }


        public void SetCrib(string cribString)
        {
            this.cribString = cribString;
        }


        public override SolKey<(int,int)> Break(int[] ciphertext)
        {
            SolKey<(int, int)> sk;

            if (cribString is null)
            {
                throw new Exception("cribString has not been set");
            }

            int[] crib = Program.ConvertStringToIntegerArray(cribString);
            List<(int, int)> possibleKeys = new List<(int, int)>();
            (bool, int, int) validPosition;
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            (int, int) bestKey = (0, 0);

            for (int i = 0; i < ciphertext.Length - crib.Length; i++)
            {

                validPosition = CheckValidAffineCribPosition(ciphertext, crib, i);
                if (validPosition.Item1 == true)
                {
                    possibleKeys.Add((validPosition.Item2, validPosition.Item3));
                }

            }

            if (possibleKeys.Count == 0)
            {
                Console.WriteLine(cribString);
                throw new CribNotFoundException();
            }


            bestFitness = Program.TetragramFitness(ciphertext);


            foreach ((int, int) key in possibleKeys)
            {
                solution = cipher.Decrypt(ciphertext, (key.Item1, key.Item2));
                fitness = Program.TetragramFitness(solution);
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

        public (bool, int, int) CheckValidAffineCribPosition(int[] ciphertext, int[] crib, int i)
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
                    if (Program.Coprime26(x) && Program.Coprime26(a))
                    {
                        validSubtraction = true;
                    }
                    k++;
                }
                j++;
            }

            if (!validSubtraction)
            {
                return (false, 0, 0);
            }

            a = Program.mod(ModularInverses[x] * a);
            b = Program.mod(ciphertext[i] - crib[0] * a);

            if (Enumerable.SequenceEqual(cipher.Encrypt(crib, (a, b)), ciphertextSplit))
            {
                return (true, a, b);
            }
            return (false, 0, 0);
        }
    }
}
