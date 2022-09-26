using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prototype
{
    class Program
    {

        static double IndexOfCoincidence(string input)
        {
            char[] inputArray = input.ToCharArray();
            double ioc = 0;
            int count;
            for (int i = 97; i <= 122; i++)
            {
                count = inputArray.Count(x => x == (char)i);
                ioc += count * (count - 1);
            }
            ioc /= (inputArray.Length * (inputArray.Length - 1));
            ioc *= 26;
            return ioc;
        }


        static double[] CreateFrequencyArray(string input)
        {
            double[] frequencies = new double[26];

            for (int i = 0; i < 26; i++)
            {
                frequencies[i] = input.Count(c => c == (char)i + 97);
            }

            return frequencies;
        }


        static Dictionary<string, double> CreateTetragramDictionary()
        {
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            string tetragram;
            for (int i = 97; i <= 122; i++)
            {
                for (int j = 97; j <= 122; j++)
                {
                    for (int k = 97; k <= 122; k++)
                    {
                        for (int l = 97; l <= 122; l++)
                        {
                            tetragram = new string(new char[] { (char)i, (char)j, (char)k, (char)l });
                            dictionary.Add(tetragram, 0);
                        }
                    }
                }
            }
            return dictionary;
        }

        static void FillTetragramDictionary(Dictionary<string, double> dictionary)
        {
            string[] line;
            using (StreamReader sr = new StreamReader("tetragramsNumericOrder.txt"))
            {
                while (sr.EndOfStream == false)
                {
                    line = sr.ReadLine().Split(' ');
                    dictionary[line[0].ToLower()] = int.Parse(line[1]);
                }
            }
        }

        static void WriteNewFile(Dictionary<string, double> dictionary)
        {
            using (StreamWriter sr = new StreamWriter("tetragramFrequencies.txt", true))
            {
                foreach (KeyValuePair<string, double> entry in dictionary)
                {
                    sr.WriteLine($"{entry.Key} {entry.Value}");
                }
            }
        }

        static void WriteLogFile(Dictionary<string, double> dictionary)
        {
            double total = 0;
            foreach(KeyValuePair<string, double> entry in dictionary)
            {
                total += entry.Value;
            }


            using (StreamWriter sr = new StreamWriter("tetragramLogFrequencies.txt", false))
            {
                foreach (KeyValuePair<string, double> entry in dictionary)
                {
                    if (entry.Value == 0)
                    {
                        sr.WriteLine($"{entry.Key} -10");
                    }
                    else
                    {
                        sr.WriteLine($"{entry.Key} {Math.Log10(entry.Value/total)}");
                    }
                }
            }
        }

        static Dictionary<string, double> LogDictionary()
        {
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            string[] line;
            using (StreamReader sr = new StreamReader("tetragramLogFrequencies.txt"))
            {
                while (sr.EndOfStream == false)
                {
         
                    line = sr.ReadLine().Split(' ');
                    dictionary[line[0]] = double.Parse(line[1]);
                }
            }

            return dictionary;
        }


        static double TetragramFitness(string message, Dictionary<string, double> dictionary)
        {
            message = message.Replace(" ", "");
            double fitness = 0;
            string substring;

            for (int i = 0; i <= message.Length-4; i++)
            {
                substring = message.Substring(i, 4); 
                fitness += dictionary[substring];
            }
            return fitness/(message.Length-3);
        }



        static void PrintArray(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i] + " ");
            }
            Console.WriteLine();
        }

        static double MonogramFrequency(string message)
        {
            double[] englishFreqsArray = { 8.55, 1.60, 3.16, 3.87, 12.10, 2.18, 2.09, 4.96, 7.33, 0.22, 0.81, 4.21, 2.53, 7.17, 7.47, 2.07, 0.10, 6.33, 6.73, 8.94, 2.68, 1.06, 1.83, 0.19, 1.72, 0.11 };
            Vector englishFreqs = new Vector(englishFreqsArray);
            double[] messageFreqsArray = CreateFrequencyArray(message);
            Vector messageFreqs = new Vector(messageFreqsArray);
            double fitness = Vector.CosineAngle(englishFreqs, messageFreqs);
            return fitness;
        }


        static int Menu(string prompt, string[] options)
        {
            int offset = Console.CursorTop;
            Console.WriteLine(prompt);
            foreach (string option in options)
            {
                Console.WriteLine($"  {option}");
            }
            int optionNumber = 1;

            while (true)
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
                    return optionNumber - 1;
                }
            }
        }

        static string CleanString(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                    sb.Append(c);
            }
            input =  sb.ToString();
            input = String.Concat(input.Where(c => !Char.IsWhiteSpace(c)));
            return input;
        }

        static string ShuffleString(string message)
        {
            Random rnd = new Random();
            char[] messageArray = message.ToCharArray();
            for (int i = messageArray.Length - 1; i > 0; i--)
            {
                int randomIndex = rnd.Next(0, i + 1);

                char temp = messageArray[i];
                messageArray[i] = messageArray[randomIndex];
                messageArray[randomIndex] = temp;
            }
            return new string(messageArray);
        }


        static void CreatingConfidenceIntervals(Dictionary<string, double> dictionary)
        {
            Random rnd = new Random();
            int start;
            int tests = 100000;
            int length = 250;
            string corpus = File.ReadAllText("brown4.txt");
            string text;
            double[] scores = new double[tests];

            for (int i = 0; i < tests; i++)
            {
                start = rnd.Next(0, corpus.Length - length);
                text = corpus.Substring(start, length);
                //text = ShuffleString(text);
                //text = RandomString(length);

                scores[i] = TetragramFitness(text,dictionary);

            }
            Array.Sort(scores);
            Array.Reverse(scores);

            int p = 0;
            for (int i = 0; i < tests; i+=tests/200)
            {
                Console.WriteLine($"{p}% {scores[i]}");
                p += 1;
            }
            Console.WriteLine(scores[0]);
            Console.WriteLine(scores[tests-1]);
        }

        static string RandomString(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)rnd.Next(97, 123));
            }
            return sb.ToString();
        }



        static void Main(string[] args)
        {

            Dictionary<string, double> TetragramDictionary = LogDictionary();
            //CreatingConfidenceIntervals(TetragramDictionary);




            string message;
            int choice;
            while (true)
            {
                Console.WriteLine("Enter a message to analyse:");
                message = Console.ReadLine().ToLower();
                message = CleanString(message);
                Console.WriteLine(message);

                choice = Menu("", new string[] { "Monogram fitness", "Tetragram fitness", "Index of coincidence" });

                switch (choice)
                {
                    case 0:
                        Console.WriteLine($"\nMonogram fitness is {MonogramFrequency(message)}");
                        break;
                    case 1:
                        Console.WriteLine($"\nTetragram fitness is {TetragramFitness(message, TetragramDictionary)}");
                        break;
                    case 2:
                        Console.WriteLine($"\nIndex of coincidence is {IndexOfCoincidence(message)}");
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }

        }
    }
}
