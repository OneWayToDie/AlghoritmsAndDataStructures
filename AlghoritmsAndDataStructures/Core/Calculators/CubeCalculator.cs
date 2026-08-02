namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class CubeCalculator
	{
		public static CubeResult Compute(double edge)
		{
			if (edge <= 0)
				return new CubeResult(0, 0, 0, false);

			double face = edge * edge;
			double surface = 6 * face;
			double volume = edge * edge * edge;

			return new CubeResult(face, surface, volume, true);
		}
	}

	public class CubeResult
	{
		public double FaceArea { get; }
		public double TotalSurface { get; }
		public double Volume { get; }
		public bool Success { get; }

		public CubeResult(double faceArea, double totalSurface, double volume, bool success)
		{
			FaceArea = faceArea;
			TotalSurface = totalSurface;
			Volume = volume;
			Success = success;
		}
	}
}