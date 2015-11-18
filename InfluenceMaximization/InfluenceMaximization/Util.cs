using System;
using System.Collections.Generic;

namespace InfluenceMaximization
{
    // This class is for finding roots of \frac{d UI(C)}{d c_i}=0 in line 5 of Algorithm 1
    // Should be modified if you want to use seed probability functions other than p_u(c_u)=c_u^2, p_u(c_u)=c_u or p_u(c_u)=2c_u-c_u^2
	public static class Util
	{
		// Return roots of Quadratic Equation, if not available, return null
		public static List<double> RootsQuadraticEqu(double a, double b, double c)
        {
            double delta;
            delta = b * b - 4 * a * c;
			if (delta < 0)
				return null;
		
			List<double> ret = new List<double>();
			ret.Add((-b + Math.Sqrt (delta)) / (2 * a));
			ret.Add((-b - Math.Sqrt (delta)) / (2 * a));

			return ret;
        }


        // Return roots of Cubic Equation
		// reference 1: http://www.codeproject.com/Articles/798474/To-Solve-a-Cubic-Equation
		// reference 2: http://www.1728.org/cubic2.htm
        public static List<double> RootsCubicEqu(double a, double b, double c, double d)
        {
			double eps = 1e-8;
			double f = ((3 * c / a) - (b * b / (a * a))) / 3;
			double g = ((2 * b * b * b / (a * a * a)) - (9 * b * c / (a * a)) + (27 * d / a)) / 27;
			double h = g * g / 4 + f * f * f / 27;

			List<double> ret = new List<double> ();

//			Console.WriteLine ("f: {0}, g: {1}, h: {2}", f, g, h);

			if (Math.Abs (f) <= eps && Math.Abs (g) <= eps && Math.Abs (h) <= eps) { // all 3 roots are real and equal
				ret.Add (-1.0 * Math.Pow (d / a, 1.0 / 3.0));
			} else if (h > 0) { //only 1 root is real
				double R = -(g / 2) + Math.Pow (h, 0.5);
				double S = Math.Pow (R, 1.0 / 3.0);
				double T = -(g / 2) - Math.Pow (h, 0.5);
				double U;
				if (T < 0)
					U = -1 * Math.Pow (-T, 1.0 / 3.0);
				else
					U = Math.Pow (T, 1.0 / 3.0);
//				Console.WriteLine ("{0}, {1}, {2}, {3}", R, S, T, U);
				ret.Add (S + U - b / (3 * a));
			} else if (h <= 0) { // all 3 roots are real
				double i = Math.Pow(g*g/4-h, 0.5);
				double j = Math.Pow (i, 1.0 / 3.0);
				double K = Math.Acos (-g / (2 * i));
				double L = -j;
				double M = Math.Cos (K / 3);
				double N = Math.Pow (3.0, 0.5) * Math.Sin (K / 3);
				double P = -1 * (b / (3 * a));
//				Console.WriteLine ("{0}, {1}, {2}, {3}, {4}, {5}, {6}", i, j, K, L, M, N, P);
				ret.Add (2 * j * Math.Cos (K / 3) - b / (3 * a));
				ret.Add (L * (M + N) + P);
				ret.Add (L * (M - N) + P);
			}

			return ret;	
        }
        
        // Obj = A1*Pp(Cp) + A2*Pq(B'-Cp) + A3*Pp(Cp)*Pq(B'-Cp) + CONST
        // funcselp, funcselq: {1: x^2, 2: x, 3: 2x-x^2}
		public static List<double> EquSolver(int funcselp, int funcselq, double A1, double A2, double A3, double B)
        {
			List<double> ret = new List<double> ();
			List<double> tmp = new List<double> ();

			if (funcselp == 1) {
				switch(funcselq) {
                    case 1:
						tmp = RootsCubicEqu(2*A3, 3*A3*B, (A1+A2+A3*B*B), -1*A2*B);
                        break;
                    case 2:
						tmp = RootsQuadraticEqu(-3*A3, 2*(A1+A3*B), -A2);
                        break;
                    case 3:
						tmp = RootsCubicEqu(-2*A3, 3*(A3*B-A3), (A1-A2-A3*B*B), A2*B-A2+2*A3*B);
                        break;
                }
            }
            else if (funcselp == 2) {
                switch (funcselq) {
                    case 1:
						tmp = RootsQuadraticEqu(3*A3, 2*A2-4*A3*B, A1-2*A2*B+A3*B*B);
                        break;
					case 2:
						tmp.Add ((A1-A2+A3*B)/(2*A3));
                        break;
                    case 3:
						tmp = RootsQuadraticEqu(-3*A3, 2*A3*B-4*A3-2*A2, A1+2*A2*B-2*A2+2*A3*B-A3*B*B);
                        break;
                }
            }
            else if (funcselp == 3) {
                switch (funcselq) {
                    case 1:
						tmp = RootsCubicEqu(-2*A3, 3*A3*(B+1), (A2-A1-B*B*A3-4*B*A3), (A1-A2*B+A3*B*B));
                        break;
                    case 2:
						tmp = RootsQuadraticEqu(3*A3, -2*A1-A3*B, 2*A1-A2+2*A3*B-4*A3);
                        break;
                    case 3:
						tmp = RootsCubicEqu(2*A3, 3*A3*B, (A3*B*B-2*A3*B-4*A3-A1-A2), (A1-A2+A2*B+A3*B-A3*B*B));
                        break;
                }
            }

			// assert the roots [0,1]
			if (tmp != null) {
				foreach (double x in tmp) {
					if (x >= 0 && x <= 1)
						ret.Add (x);
				}
			} else
				return null;

			return ret;
        }

		public static void PrintList(List<double> L)
		{
			Console.WriteLine (L.ToString ());
			foreach (double l in L)
				Console.WriteLine (l);
		}

		public static void Shuffle(List<int> L)
		{
			Random rd = new Random ();
			for (int i = 0; i < L.Count; ++i) 
			{
				int offset = rd.Next (L.Count - i);
				int ind = i + offset;
				int tmp = L [i];
				L [i] = L[ind];
				L [ind] = tmp;
			}
		}
	}
}
	
// Util Test
// 2*x3 - 4*x2 - 22x + 24 = 0 / 4 -3 1
//List<double> l1 = Util.RootsCubicEqu(2,-4,-22,24);
//Util.PrintList (l1);
//// 3*x3 - 10*x2 + 14x + 27 = 0 / -1
//List<double> l2 = Util.RootsCubicEqu(3, -10, 14, 27);
//Util.PrintList (l2);
//// x3 + 6*x2 + 12x + 8 = 0 / -2
//List<double> l3 = Util.RootsCubicEqu(1, 6, 12, 8);
//Util.PrintList (l3);
//// x2 + 3*x -4 = 0 / -4 1
//List<double> l4 = Util.RootsQuadraticEqu(1, 3, -4);
//Util.PrintList (l4);