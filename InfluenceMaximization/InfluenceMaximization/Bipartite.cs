using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public class Bipartite
	{
		public int numV;
		public int numS;

		public List<List<int>> V2S;
		public List<List<int>> S2V;

		public Bipartite (List<List<int>> RRs, int numV)
		{
			this.numV = numV;
			this.numS = RRs.Count;

			V2S = new List<List<int>> ();
			S2V = new List<List<int>> ();
			for (int i = 0; i < numV; ++i) 
			{
				List<int> vsets = new List<int> ();
				V2S.Add (vsets);
			}
			for (int i = 0; i < numS; ++i) 
			{
				List<int> snodes = new List<int> ();
				S2V.Add (snodes);
			}

			for (int i = 0; i < RRs.Count; ++i) 
			{
				foreach (int u in RRs[i]) 
				{
					V2S [u].Add (i);
					S2V [i].Add (u);
				}
			}
		}

        //Greedy for influence maximization
		public Tuple<List<int>, double> Greedy(int k)
		{
			HashSet<int> seeds = new HashSet<int> ();
			HashSet<int> hyperEdges = new HashSet<int> ();
			List<int> seedSet = new List<int> ();
			List<int> degree = new List<int> ();
			for (int u = 0; u < numV; ++u) 
			{
				degree.Add (V2S [u].Count);
			}
			double sum = 0;
			for (int i = 0; i < k; ++i) 
			{
				int v = 0;
				for (int iter = 0; iter < numV; ++iter)
					if (degree [iter] > degree [v])
						v = iter;
				if (seeds.Contains (v))
					throw new Exception ("degree error");
				seeds.Add (v);
				seedSet.Add (v);
				sum += degree [v];
				foreach (int sid in V2S[v]) 
				{
					if (!hyperEdges.Contains(sid))
		                    	{
		                        	foreach (int uid in S2V[sid])
		                        	{
		                            		if (seeds.Contains(uid) == false)
		                                		degree[uid]--;
		                        	}
		                        	hyperEdges.Add(sid);
		                    	}
				}
				degree [v] = 0;
			}
			Console.WriteLine ("sum=" + sum);
			double sp = (double)sum*numV / (double)numS;
			return new Tuple<List<int>, double> (seedSet, sp);
		}

        //Given seed probability P, find the best k nodes that can maximize influence spread
		public Tuple<List<int>, double> Greedy(int k, List<double> P)
		{
			HashSet<int> seeds = new HashSet<int> ();
			List<int> seedSet = new List<int> ();
			List<double> edgeW = new List<double> ();
			for (int h = 0; h < numS; ++h)
				edgeW.Add (1.0);
            

            //CELF Algorithm
			PriorityQueue<VNode> pq = new PriorityQueue<VNode> (numV+1, new VNodeComparer ());
			List<bool> update = new List<bool> ();
			for (int u = 0; u < numV; ++u) 
			{
				VNode node = new VNode (u, numS);
				pq.Push (node);
				update.Add (false);
			}

			double total_gain = 0.0;
			for (int i = 0; i < k; ++i) 
			{
				for (int u = 0; u < numV; ++u)
					update [u] = false;
				int next = 0;
				double gain = 0;
				while (true) 
				{
					VNode node = pq.Pop ();
					int max_u = node.id;

					if (update [max_u]) 
					{
						next = max_u;
						gain = node.val;
						break;
					} 
					else 
					{
						double sum = 0;
                        if (i == 0)
                            sum = V2S[max_u].Count * P[max_u];
                        else
                        {
                            foreach (int sid in V2S[max_u])
                                sum += edgeW[sid] * P[max_u];
                        }
                        VNode n1 = new VNode (max_u, sum);
						pq.Push (n1);
						update [max_u] = true;
					}
				}
				total_gain += gain;
				foreach (int sid in V2S[next])
					edgeW [sid] = edgeW [sid] * (1 - P [next]);
				seeds.Add (next);
				seedSet.Add (next);
			}

			return new Tuple<List<int>, double> (seedSet, total_gain*numV/numS);
		}
	}
}

