using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public class CoordinateDescent
	{
		public Graph graph;
		public List<int> initNodes;
		public double init_c;
		public List<int> type;
		public int batch_num;
		public double alpha;
		public int mH;
		public Bipartite bg;
		public List<double> C;
		public List<double> prob_edge;

		public CoordinateDescent(Graph graph, List<int> initNodes, double init_c, List<int> type, int batch_num, double alpha, int mH)
		{
			this.graph = graph;
			this.initNodes = initNodes;
			this.init_c = init_c;
			this.type = type;
			this.batch_num = batch_num;
			this.alpha = alpha;
			this.mH = mH;

			C = new List<double>();
			for (int i = 0; i < graph.numV; ++i)
				C.Add(0.0);
			foreach (int u in initNodes)
				C[u] = init_c;

			ICModel icm = new ICModel(alpha);
			List<List<int>> RR = new List<List<int>>();
			for (int r = 0; r < mH; ++r)
			{
				HashSet<int> rawSet = icm.RR(graph);
				List<int> rSet = rawSet.ToList();
				RR.Add(rSet);
			}
			bg = new Bipartite(RR, graph.numV);
			prob_edge = new List<double>();
			for (int h = 0; h < bg.numS; ++h)
			{
				double res = 1.0;
				foreach (int u in bg.S2V[h])
					res *= (1 - SeedProb(u, C[u]));
				//res = 1.0 - res;
				prob_edge.Add(res);
			}
		}

        public CoordinateDescent(Graph graph, Bipartite bg, List<int> initNodes, double init_c, List<int> type, int batch_num, double alpha, int mH)
        {
            this.graph = graph;
            this.initNodes = initNodes;
            this.init_c = init_c;
            this.type = type;
            this.batch_num = batch_num;
            this.alpha = alpha;
            this.mH = mH;
            this.bg = bg;

            C = new List<double>();
            for (int i = 0; i < graph.numV; ++i)
                C.Add(0.0);
            foreach (int u in initNodes)
                C[u] = init_c;

            prob_edge = new List<double>();
            for (int h = 0; h < bg.numS; ++h)
            {
                double res = 1.0;
                foreach (int u in bg.S2V[h])
                    res *= (1 - SeedProb(u, C[u]));
                //res = 1.0 - res;
                prob_edge.Add(res);
            }
        }

		public double SeedProb(int u, double c)
		{
			double p = 0;
			if (type[u] == 1)
				p = c * c;
			else if (type[u] == 2)
				p = c;
			else
				p = (2 - c) * c;
			return p;
		}

		public List<double> Parameters(int i, int j, double ci, double cj)
		{
			double Bp = ci + cj;
			double left = 0.0, right = 1.0;
			if (Bp - 1 > left)
				left = Bp - 1;
			if (Bp < right)
				right = Bp;
			if (right <= left)
				return null;
			List<double> parameters = new List<double>();
			parameters.Add(Bp);
			parameters.Add(left);
			parameters.Add(right);

			HashSet<int> Ei = new HashSet<int>();
			HashSet<int> Ej = new HashSet<int>();
			HashSet<int> Eij = new HashSet<int>();
			foreach (int e in bg.V2S[i])
				Ei.Add(e);
			foreach (int e in bg.V2S[j])
			{
				Ej.Add(e);
				if (Ei.Contains(e))
					Eij.Add(e);
			}

			double A1 = 0, A2 = 0, A3 = 0;
			foreach (int e in bg.V2S[i])
			{
				double tmp = prob_edge[e] / (1 - SeedProb(i, ci));
				if (Eij.Contains(e))
					tmp = tmp / (1 - SeedProb(j, cj));
				A1 -= tmp;
			}
			foreach (int e in bg.V2S[j])
			{
				double tmp = prob_edge[e] / (1 - SeedProb(j, cj));
				if (Eij.Contains(e))
					tmp = tmp / (1 - SeedProb(i, ci));
				A2 -= tmp;
			}
			foreach (int e in Eij)
			{
				double tmp = prob_edge[e];
				tmp = tmp / (1 - SeedProb(i, ci));
				tmp = tmp / (1 - SeedProb(j, cj));
				A3 += tmp;
			}
			parameters.Add(A1);
			parameters.Add(A2);
			parameters.Add(A3);

			return parameters;
		}

		public bool IsDecreased(int i, int j, double oci, double ocj, double nci, double ncj, double A1, double A2, double A3)
		{
			double val1 = A1 * SeedProb(i, oci) + A2 * SeedProb(j, ocj) + A3 * SeedProb(i, oci) * SeedProb(j, ocj);
			double val2 = A1 * SeedProb(i, nci) + A2 * SeedProb(j, ncj) + A3 * SeedProb(i, nci) * SeedProb(j, ncj);
			if (val2 >= val1)
				return false;
			return true;
		}

		public void UpdateProbEdge(int i, int j, double oci, double ocj, double nci, double ncj)
		{
			foreach (int e in bg.V2S[i])
			{
				prob_edge[e] = prob_edge[e] / (1 - SeedProb(i, oci));
				prob_edge[e] *= (1 - SeedProb(i, nci));
			}

			foreach (int e in bg.V2S[j])
			{
				prob_edge[e] = prob_edge[e] / (1 - SeedProb(j, ocj));
				prob_edge[e] *= (1 - SeedProb(j, ncj));
			}
		}

		public List<double> IterativeMinimize()
		{
			bool ooflag = true;

			double sum = 0;
			for (int h = 0; h < prob_edge.Count; ++h)
				sum += (1-prob_edge[h]);
			Console.WriteLine(sum * graph.numV / prob_edge.Count);

			List<int> nodes = new List<int>();
			foreach (int u in initNodes)
				nodes.Add(u);

            int tnd = 0;
			for (int rnd = 0; rnd < batch_num; ++rnd)
			{
                tnd = rnd;
				ooflag = false;
				Util.Shuffle(nodes);
				for (int ii = 0; ii < nodes.Count; ++ii)
				{
					int i = nodes[ii];
					for (int jj = ii + 1; jj < nodes.Count; ++jj)
					{
						int j = nodes[jj];
						if (Math.Abs(C[i] - 1.0) < GlobalVar.epsilon || Math.Abs(C[j] - 1.0) < GlobalVar.epsilon)
							continue;
						List<double> parameters = Parameters(i, j, C[i], C[j]);
						if (parameters != null)
						{
							double Bp = parameters[0];
							double left = parameters[1];
							double right = parameters[2];
							double A1 = parameters[3];
							double A2 = parameters[4];
							double A3 = parameters[5];

							double oci = C[i];
							double ocj = C[j];
							bool flag = false;

							List<double> sol = Util.EquSolver(type[i], type[j], A1, A2, A3, Bp);
							if (sol != null)
							{
								foreach (double root in sol)
								{
									if (root >= left && root <= right)
									{
										double nci = root;
										double ncj = Bp - root;
										if (IsDecreased(i, j, oci, ocj, nci, ncj, A1, A2, A3))
										{
											oci = nci;
											ocj = ncj;
											flag = true;
										}
									}
								}
							}
							double lci = left;
							double lcj = Bp - left;
							if (IsDecreased(i, j, oci, ocj, lci, lcj, A1, A2, A3))
							{
								oci = lci;
								ocj = lcj;
								flag = true;
							}



							double rci = right;
							double rcj = Bp - right;
							if (IsDecreased(i, j, oci, ocj, rci, rcj, A1, A2, A3))
							{
								oci = rci;
								ocj = rcj;
								flag = true;
							}

							if (flag)
							{
								if (Math.Abs(C[i] - 1.0) < GlobalVar.epsilon || Math.Abs(C[j] - 1.0) < GlobalVar.epsilon)
								{
									Console.WriteLine("C[" + i + "]=" + C[i] + "\tC[" + j + "]=" + C[j]);
									throw new Exception();
								}
								UpdateProbEdge(i, j, C[i], C[j], oci, ocj);
								C[i] = oci;
								C[j] = ocj;
								ooflag = true;
							}
						}
					}
				}

                if (!ooflag)
                    break;
			}

			return C;
		}

	}
}