using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InfluenceMaximization
{
    class ProcessData4Visual
    {
        public static void Fig3(string path, string outPath)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                StreamWriter writer = new StreamWriter(outPath);

                writer.WriteLine("B M INFLU STD TIME");
                string line;
                line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split('=');
                    int B = int.Parse(strs[1]);
                    writer.Write(B);
                    line = reader.ReadLine();
                    string[] words = line.Split();
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + words[1] + " " + words[2] + " " + words[3]);

                    writer.Write(B);
                    line = reader.ReadLine();
                    words = line.Split();
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + words[1] + " " + words[2] + " " + words[3]);

                    writer.Write(B);
                    line = reader.ReadLine();
                    words = line.Split();
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + words[1] + " " + words[2] + " " + words[3]);
                }

                reader.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Fig4(string path, string outPath)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                StreamWriter writer = new StreamWriter(outPath);

                writer.WriteLine("B AccuracyLB");
                string line;
                line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split('=');
                    int B = int.Parse(strs[1]);
                    line = reader.ReadLine();
                    string[] words = line.Split();
                    writer.WriteLine(B + " " + words[4]);

                    reader.ReadLine();
                    reader.ReadLine();
                }

                reader.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Fig5(string path, string outPath)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                StreamWriter writer = new StreamWriter(outPath);

                writer.WriteLine("C S");
                string line;
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split();
                    double c = double.Parse(strs[0]);
                    double s = double.Parse(strs[1]);
                    if (c - 1.0 < 1e-4)
                        writer.WriteLine(c + " " + s);
                }

                reader.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Fig6(string path, string outPath)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                StreamWriter writer = new StreamWriter(outPath);

                writer.WriteLine("B M TIME");
                string line;
                line = reader.ReadLine();
                string[] info = line.Split('\t');
                double gbt = double.Parse(info[1]);
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split('=');
                    int B = int.Parse(strs[1]);
                    writer.Write(B);
                    line = reader.ReadLine();
                    string[] words = line.Split();
                    double t = double.Parse(words[3]);
                    t += gbt;
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + t);

                    writer.Write(B);
                    line = reader.ReadLine();
                    words = line.Split();
                    t = double.Parse(words[3]);
                    t += gbt;
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + t);

                    writer.Write(B);
                    line = reader.ReadLine();
                    words = line.Split();
                    t = double.Parse(words[3]);
                    t += gbt;
                    writer.WriteLine(" " + words[0].Substring(0, words[0].Length - 1) + " " + t);

                    writer.WriteLine(B + " GBT " + gbt);
                }

                reader.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
