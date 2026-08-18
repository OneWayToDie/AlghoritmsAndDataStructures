using System;

namespace AlghoritmsAndDataStructures.Models
{
	public class SeriesResult
	{
		public double X { get; set; }
		public double Sum { get; set; }
		public int Terms { get; set; }
		public double Exact => Math.Exp(-X);
		public double Error => Math.Abs(Sum - Exact);
	}
}