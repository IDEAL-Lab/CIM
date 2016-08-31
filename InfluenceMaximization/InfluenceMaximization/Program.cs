using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace InfluenceMaximization
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
                Console.WriteLine(args[i]);
            if (args.Length > -1)
            {
                string ConfigPath = args[0];

                StreamReader ConfigReader = new StreamReader(ConfigPath);
                string GraphPath, FunPath, RsltDir;
                string line;
                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GraphPath = line;

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);


                FunPath = line;

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                RsltDir = line;

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.MC = int.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.mH = int.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.batch_num = int.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.Alpha = double.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.St = int.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.End = int.Parse(line);

                while ((line = ConfigReader.ReadLine()) != null)
                {
                    if (line.ElementAt(0) != '#' && line != "")
                        break;
                }
                Console.WriteLine(line);
                GlobalVar.b = double.Parse(line);
                ConfigReader.Close();

                Graph graph = new Graph(GraphPath);

                List<int> Type = new List<int>();
                StreamReader reader = new StreamReader(FunPath);
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strs = line.Split();
                    //int id = int.Parse (strs [0]);
                    int type = int.Parse(strs[1]);
                    if (type == 4)
                        type = 3;
                    Type.Add(type);
                }

                CoordinateDescentAlgCommonHyperGraphOneAlpha(graph, Type, RsltDir);
            }

        }

        public static void CoordinateDescentAlgCommonHyperGraphOneAlpha(Graph graph, List<int> Type, string RsltDir)
        {
            double b = GlobalVar.b; // Step of c of searching the best discount in th Unified Discount Algorithm
            double alpha = GlobalVar.Alpha;
            string Dir = RsltDir + "/Alpha=" + alpha;
            string Path = Dir + "/AllResults.txt";
            StreamWriter writer = new StreamWriter(Path);

            // Build a random hyper graph with mH random hyper edges.
            DateTime Hyper_start = DateTime.Now;
            ICModel icm = new ICModel(alpha);
            int mH = GlobalVar.mH;
            Console.WriteLine(mH);
            List<List<int>> RR = new List<List<int>>();
            for (int r = 0; r < mH; ++r)
            {
                List<int> rSet = icm.RR(graph).ToList();
                RR.Add(rSet);
                if (r > 0 && r % 100000 == 0)
                    Console.WriteLine(r + " samples");
            }
            Bipartite bg = new Bipartite(RR, graph.numV);
            DateTime Hyper_end = DateTime.Now;
            double Hyper_time = (Hyper_end - Hyper_start).TotalMilliseconds;
            Console.WriteLine("Hyper-graph has been built");

            writer.WriteLine("Hyper-graph time:\t" + Hyper_time);
            writer.Flush();

            for (int ind = GlobalVar.St; ind <= GlobalVar.End; ++ind)
            {
                double B = ind * 10.0;

                bg.Greedy((int)B);


                // Unified Discount Algorithm
                DateTime startTime = DateTime.Now;
                List<Tuple<List<int>, double>> Res = new List<Tuple<List<int>, double>>();
                for (int i = 1; i <= 20; ++i)
                {
                    double c = i * b;
                    Tuple<List<int>, double> tup = UnifiedCGreedy(graph, bg, Type, c, B, alpha);
                    Res.Add(tup);
                    Console.WriteLine("Alpha=" + alpha + "\tc=" + c + "\t" + tup.Item2);
                }


                // Discrete Influence Maximization
                double IM_greedy = 0;
                DateTime IM_greedy_start = DateTime.Now;
                Tuple<List<int>, double> tup_IM = UnifiedCGreedy(graph, bg, Type, 20 * b, B, alpha);
                Res.Add(tup_IM);
                Console.WriteLine("Alpha=" + alpha + "\tc=1\t" + tup_IM.Item2);
                DateTime IM_greedy_end = DateTime.Now;
                IM_greedy = (IM_greedy_end - IM_greedy_start).TotalMilliseconds;

                //Influence Maximization results
                double IM_sp = Res[Res.Count - 1].Item2;
                List<int> IM_seeds = Res[Res.Count - 1].Item1;
                double IM_ts = Hyper_time + IM_greedy;


                // Unified Discount results (except standard deviation)
                int max_i = 0;
                for (int i = 0; i < Res.Count - 1; ++i)
                {
                    if (Res[i].Item2 > Res[max_i].Item2)
                        max_i = i;
                }
                DateTime UC_endTime = DateTime.Now;
                Console.WriteLine("Best c=" + (max_i + 1) * b + "\t" + Res[max_i].Item2);
                double UC_sp = Res[max_i].Item2;
                List<int> UC_seeds = Res[max_i].Item1;
                double UC_ts = (UC_endTime - startTime).TotalMilliseconds + Hyper_time;

                // Proceed to Coordinate Descent. Use the best result of Unified Discount as initial value
                double init_c = (max_i + 1) * b;
                List<int> initNodes = UC_seeds;
                CoordinateDescent cd = new CoordinateDescent(graph, bg, initNodes, init_c, Type, GlobalVar.batch_num, alpha, GlobalVar.mH);
                List<double> C = cd.IterativeMinimize();
                DateTime CD_endTime = DateTime.Now;
                double CD_ts = (CD_endTime - startTime).TotalMilliseconds + Hyper_time;


                // Evaluation using Monte Carlo Simulations
                List<Pair> IM_P = new List<Pair>();
                foreach (int u in IM_seeds)
                {
                    Pair pair = new Pair(u, 1.0);
                    IM_P.Add(pair);
                }

                List<Pair> UC_P = new List<Pair>();
                foreach (int u in UC_seeds)
                {
                    double p = 0;
                    if (Type[u] == 1)
                        p = init_c * init_c;
                    else if (Type[u] == 2)
                        p = init_c;
                    else
                        p = (2 - init_c) * init_c;
                    Pair pair = new Pair(u, p);
                    UC_P.Add(pair);
                }

                List<Pair> CD_P = new List<Pair>();
                for (int i = 0; i < graph.numV; ++i)
                {
                    double p = 0;
                    if (Type[i] == 1)
                        p = C[i] * C[i];
                    else if (Type[i] == 2)
                        p = C[i];
                    else
                        p = (2 - C[i]) * C[i];
                    Pair pair = new Pair(i, p);

                    if (p > GlobalVar.epsilon)
                        CD_P.Add(pair);
                }

                // //ICModel icm = new ICModel(alpha);
                Tuple<double, double> IM_tup = icm.InfluenceSpread(graph, IM_P, GlobalVar.MC);
                Tuple<double, double> UC_tup = icm.InfluenceSpread(graph, UC_P, GlobalVar.MC);
                Tuple<double, double> CD_tup = icm.InfluenceSpread(graph, CD_P, GlobalVar.MC);

                double Numerator = 2 * graph.numV * (1 - 1.0 / Math.E) * (Math.Log(Cnk(graph.numV, (int)B)) + Math.Log(graph.numV) + Math.Log(2.0));
                double appro = 1 - 1.0 / Math.E - Math.Sqrt(Numerator / (IM_tup.Item1*(double)GlobalVar.mH));
                writer.WriteLine("B=" + B);
                writer.WriteLine("IM:\t" + IM_tup.Item1 + "\t" + IM_tup.Item2 + "\t" + IM_ts + "\t" + appro);
                writer.WriteLine("UC:\t" + UC_tup.Item1 + "\t" + UC_tup.Item2 + "\t" + UC_ts);
                writer.WriteLine("CD:\t" + CD_tup.Item1 + "\t" + CD_tup.Item2 + "\t" + CD_ts);
                writer.Flush();

                string outPath = Dir + "/B=" + B + ".txt";
                StreamWriter outWriter = new StreamWriter(outPath);
                outWriter.WriteLine("IM\t" + IM_ts + "\t" + IM_seeds.Count);
                foreach (int u in IM_seeds)
                    outWriter.WriteLine(u + "\t1\t1");

                outWriter.WriteLine("UC\t" + UC_ts + "\t" + UC_seeds.Count);
                foreach (Pair pair in UC_P)
                    outWriter.WriteLine(pair.id + "\t" + init_c + "\t" + pair.prob);

                outWriter.WriteLine("CD\t" + CD_ts + "\t" + CD_P.Count);
                foreach (Pair pair in CD_P)
                    outWriter.WriteLine(pair.id + "\t" + C[pair.id] + "\t" + pair.prob);
                outWriter.Close();

                string ccurvePath = Dir + "/curve_c(" + B + ").txt";
                StreamWriter cWriter = new StreamWriter(ccurvePath);
                for (int i = 0; i < Res.Count; ++i)
                {
                    double c = (i + 1) * b;
                    cWriter.WriteLine(c + "\t" + Res[i].Item2);
                }
                cWriter.Close();
            }
            writer.Close();
        }

        public static Tuple<List<int>, double> UnifiedCGreedy(Graph graph, List<int> Type, double c, double B, double alpha)
        {
            List<double> P = new List<double>();
            for (int id = 0; id < Type.Count; ++id)
            {
                int type = Type[id];
                double p;
                if (type == 1)
                    p = c * c;
                else if (type == 2)
                    p = c;
                else
                    p = (2 - c) * c;
                P.Add(p);
            }

            ICModel icm = new ICModel(alpha);
            int mH = GlobalVar.mH;
            List<List<int>> RR = new List<List<int>>();
            for (int r = 0; r < mH; ++r)
            {
                List<int> rSet = icm.RR(graph).ToList();
                RR.Add(rSet);
            }
            Bipartite bg = new Bipartite(RR, graph.numV);
            Console.WriteLine("Hyper-graph has been built");
            int k = (int)(B / c);
            Tuple<List<int>, double> tup = bg.Greedy(k, P);
            Console.WriteLine("seeds have been selected");
            return tup;
        }

        public static Tuple<List<int>, double> UnifiedCGreedy(Graph graph, Bipartite bg, List<int> Type, double c, double B, double alpha)
        {
            List<double> P = new List<double>();
            for (int id = 0; id < Type.Count; ++id)
            {
                int type = Type[id];
                double p;
                if (type == 1)
                    p = c * c;
                else if (type == 2)
                    p = c;
                else
                    p = (2 - c) * c;
                P.Add(p);
            }

            int k = (int)(B / c);
            Tuple<List<int>, double> tup = bg.Greedy(k, P);
            Console.WriteLine("seeds have been selected");
            return tup;
        }

        public static double Cnk(int n, int k)
        {
            double ans = 1.0;
            for (int i = 0; i < k; ++i)
            {
                ans *= (double)(n - i) / (double)(i + 1);
            }
            return ans;
        }
    }

}
