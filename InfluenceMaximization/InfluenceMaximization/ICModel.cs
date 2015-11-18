using System;
using System.Collections.Generic;

namespace InfluenceMaximization
{
	public class ICModel
	{
		public double alpha;
		public Random random;
		public ICModel (double alpha)
		{
			this.alpha = alpha;
			random = new Random ();
		}

        // Compute UI(C), and return the estimated standard deviation
		public Tuple<double, double> InfluenceSpread(Graph graph, List<Pair> P, int R)
		{
			double sum = 0;
			List<double> spreads = new List<double>();
			for (int rnd = 0; rnd < R; ++rnd) 
			{
				List<int> seeds = new List<int> ();
				foreach (Pair pair in P) 
				{
					int u = pair.id;
					double p = pair.prob;
					if (random.NextDouble () <= p)
						seeds.Add (u);
				}
				double sp = Propagation(graph, seeds);
				sum += sp;
				spreads.Add(sp);
			}
			double ave = sum / R;
			double sd = 0;
			foreach (double sp in spreads)
				sd += (ave - sp) * (ave - sp);
			sd = sd / R;
			sd = Math.Sqrt(sd);
			return new Tuple<double, double>(ave, sd);
		}

        // Monte Carlo simlutation
		public int Propagation(Graph graph, List<int> seeds)
		{
			int num = seeds.Count;
            bool[] vis = new bool[graph.numV];
            for (int u = 0; u < graph.numV; ++u)
                vis[u] = false;
			Queue<int> que = new Queue<int> ();
			foreach (int u in seeds) 
			{
                vis[u] = true;
				que.Enqueue (u);
			}
			while (que.Count > 0) 
			{
				int u = que.Dequeue ();
				foreach (Node node in graph.adj[u]) 
				{
					int v = node.id;
					double pp = node.pp * alpha;
					if (!vis[v]) 
					{
						double rp = random.NextDouble ();
						if (rp <= pp) 
						{
                            vis[v] = true;
							que.Enqueue (v);
							num++;
						}
					}
				}
			}

			return num;
		}

        // Generate a random RR set
		public HashSet<int> RR(Graph graph)
		{
			HashSet<int> rr = new HashSet<int> ();
			int V = graph.numV;
			int u = random.Next (V);

			Queue<int> que = new Queue<int> ();
			que.Enqueue(u);
			rr.Add(u);

			while (que.Count > 0) 
			{
				int v = que.Dequeue ();
				foreach (Node node in graph.rAdj[v]) 
				{
					int w = node.id;
					double pp = node.pp * alpha;
					if (!rr.Contains(w)) 
					{
						double rp = random.NextDouble ();
						if (rp <= pp) 
						{
							rr.Add (w);
							que.Enqueue (w);
						}
					}
				}
			}

			return rr;
		}
	}
}

