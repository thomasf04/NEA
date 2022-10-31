using System;
using System.Collections.Generic;
namespace Technical_Solution
{
    public class CaesarCipher : Cipher<int>
    {
        public override int[] Encrypt(int[] plaintext, int oShift)
        {
            int shift = Program.GetKey<int>(oShift);

            int[] ciphertext = new int[plaintext.Length];

            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext[i] = (plaintext[i] + shift) % 26;
            }
            return ciphertext;
        }

        public override int[] Decrypt(int[] ciphertext, int oShift)
        {
            int shift = Program.GetKey<int>(oShift);
            return Encrypt(ciphertext, 26 - shift);
        }

    }
    //----------------------------------------------------------------------------------------------------
    public class CC_BruteForce : BreakMethod<int>
    {
        public CC_BruteForce(Cipher<int> cipher) : base(cipher)
        {

        }
        public override SolKey<int> Break(int[] ciphertext)
        {
            SolKey<int> sk;
            int[] solution, bestSolution = new int[ciphertext.Length];
            double fitness, bestFitness;
            int bestShift = 0;

            bestFitness = Program.TetragramFitness(ciphertext);

            for (int i = 1; i < 26; i++)
            {
                solution = cipher.Decrypt(ciphertext, i);
                fitness = Program.TetragramFitness(solution);
                if (fitness > bestFitness)
                {
                    bestSolution = solution;
                    bestFitness = fitness;
                    bestShift = i;
                }
            }

            Console.WriteLine(bestShift);
            sk.solution = bestSolution;
            sk.key = bestShift;
            return sk;
        }

    }
    //----------------------------------------------------------------------------------------------------
    public class CC_Cribs : BreakMethod<int>
    {
        private string cribString;

        internal CC_Cribs(Cipher<int> cipher) : base(cipher)
        {

        }

        internal CC_Cribs(Cipher<int> cipher, string cribString) : base(cipher)
        {
            this.cribString = cribString;
        }

        public void SetCrib(string cribString)
        {
            this.cribString = cribString;
        }


        public override SolKey<int> Break(int[] ciphertext)
        {
            SolKey<int> sk;
            int[] crib = Program.ConvertStringToIntegerArray(cribString);
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
                currentShift = Program.mod(ciphertext[i] - crib[j]);
                while (validPosition && j < crib.Length - 1)
                {
                    j++;
                    if (Program.mod(ciphertext[i + j] - crib[j]) != currentShift)
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


            bestSolution = cipher.Decrypt(ciphertext, possibleShifts[0]);
            bestFitness = Program.TetragramFitness(bestSolution);
            bestShift = 1;


            for (int k = 1; k < possibleShifts.Count; k++)
            {
                solution = cipher.Decrypt(ciphertext, possibleShifts[k]);
                fitness = Program.TetragramFitness(solution);
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

    }
}
