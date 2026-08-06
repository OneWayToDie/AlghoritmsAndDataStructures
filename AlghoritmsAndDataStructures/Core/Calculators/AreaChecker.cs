namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class AreaChecker
	{
		/// <summary>
		/// Проверяет, попадает ли точка (x, y) в заштрихованную область.
		/// </summary>
		public static bool Check(double x, double y, double a, double b, double r, out string message)
		{
			if (a <= 0 || b <= 0 || r <= 0)
			{
				message = "Ошибка: параметры a, b, R должны быть положительными.";
				return false;
			}

			// Левая верхняя область: внутри прямоугольника и внутри окружности
			bool leftUpper = (x <= 0) && (y >= 0) &&
							 (x >= -a) && (x <= 0) &&
							 (y >= 0) && (y <= b) &&
							 (x * x + y * y <= r * r);

			// Правая нижняя область: внутри прямоугольника и снаружи окружности
			bool rightLower = (x >= 0) && (y <= 0) &&
							  (x >= 0) && (x <= a) &&
							  (y >= -b) && (y <= 0) &&
							  (x * x + y * y >= r * r);

			bool result = leftUpper || rightLower;
			message = result ? "Точка попадает в заштрихованную область." : "Точка не попадает в заштрихованную область.";
			return result;
		}
	}
}