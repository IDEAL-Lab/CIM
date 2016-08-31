using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluenceMaximization
{
	public static class GlobalVar
	{
		public static double epsilon = 1e-6;
		public static int MC = 80000;
		public static int mH = 5000000;
		public static int batch_num = 100; // number of batches of iterations in Coordinate Descent Algorithm
        	public static double Alpha = 1.0;
        	public static double b = 0.05;

        	// St = 1 and End = 5 mean we set B = 10,20,30,40,50 and run CIM algorithms respectively
        	public static int St = 1;
        	public static int End = 5;
	}
}

