using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public class Node
	{
		public int id;
		public double pp;
		public Node(int id, double pp)
		{
			this.id = id;
			this.pp = pp;
		}
	}
	public class Graph
	{
		public List<List<Node>> adj;
		public List<List<Node>> rAdj;
		public int numV;
		public int numE;

		public Graph(int numV)
		{
			this.numV = numV;
			this.adj = new List<List<Node>> ();
			this.rAdj = new List<List<Node>> ();
			for (int i = 0; i < numV; ++i) 
			{
				List<Node> adjList = new List<Node> ();
				this.adj.Add (adjList);
				List<Node> rAdjList = new List<Node> ();
				this.rAdj.Add (rAdjList);
			}
		}

		public Graph (string FileName)
		{
			StreamReader reader = new StreamReader (FileName);
			string line;

			line = reader.ReadLine ();
			string[] args = line.Split ();
			int numV = int.Parse (args [0]);
			int numE = int.Parse (args [1]);
			this.numV = numV;
			this.numE = numE;
			this.adj = new List<List<Node>> ();
			this.rAdj = new List<List<Node>> ();
			for (int i = 0; i < numV; ++i) 
			{
				List<Node> adjList = new List<Node> ();
				this.adj.Add (adjList);
				List<Node> rAdjList = new List<Node> ();
				this.rAdj.Add (rAdjList);
 			}

			while ((line = reader.ReadLine ()) != null) 
			{
				string[] strs = line.Split ();
				int u, v, deg;
				double pp;
				u = int.Parse (strs [0]);
				v = int.Parse (strs [1]);
				deg = int.Parse (strs [2]);
				pp = 1.0 / (double)deg;

				Node node = new Node (v, pp);
				Node rNode = new Node (u, pp);
				this.adj [u].Add (node);
				this.rAdj [v].Add (rNode);
			}
			reader.Close ();
			Console.WriteLine ("graph has been built");
		}

        //Gadget graph is for computing UI(C). See the proof of Theorem 4.
		public Graph gadgetGraph(List<double> sp)
		{
			Graph gg = new Graph (this.numV * 2);
			for (int u = 0; u < this.numV; ++u) 
			{
				Node node = new Node (u + this.numV, sp [u]);
				gg.adj [u].Add (node);
				Node rNode = new Node (u, sp [u]);
				gg.rAdj [u + this.numV].Add (rNode);
			}

			for (int u = 0; u < this.numV; ++u) 
			{
				foreach (Node node in this.adj[u]) 
				{
					Node nnode = new Node (node.id + this.numV, node.pp);
					gg.adj [u + this.numV].Add (nnode);
					Node rnNode = new Node (u + this.numV, node.pp);
					gg.rAdj [node.id + this.numV].Add (rnNode);
				}
			}

			return gg;
		}
	}
}
