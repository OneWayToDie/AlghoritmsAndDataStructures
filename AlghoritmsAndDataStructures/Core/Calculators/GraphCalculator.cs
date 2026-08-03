using System;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class GraphCalculator
	{
		/// <summary>
		/// Вычисляет значение функции Y = f(X, R) по графику (вариант 6).
		/// </summary>
		/// <param name="x">Аргумент</param>
		/// <param name="r">Параметр R (положительное число, не равное 5 или 8)</param>
		/// <returns>Значение Y, либо null в случае ошибки (с сообщением в out)</returns>
		public static double? Compute(double x, double r, out string errorMessage)
		{
			errorMessage = null;

			// Проверка корректности R
			if (r <= 0)
			{
				errorMessage = "Параметр R должен быть положительным.";
				return null;
			}
			if (r == 5 || r == 8)
			{
				errorMessage = "Параметр R не может быть равен 5 или 8 (деление на ноль).";
				return null;
			}

			// Определение интервала и вычисление Y
			if (x <= -5)
			{
				return -3.0;
			}
			else if (x <= -r)
			{
				// Левая наклонная прямая: от (-5, -3) до (-r, 0)
				double slope = 3.0 / (5.0 - r);
				return slope * (x + 5.0) - 3.0;
			}
			else if (x <= r)
			{
				// Дуга окружности: верхняя половина окружности радиусом r с центром в (0,0)
				double argument = r * r - x * x;
				if (argument < 0)
				{
					// Теоретически не должно случиться при x в [-r, r], но на всякий случай
					errorMessage = "Ошибка вычисления: подкоренное выражение отрицательное.";
					return null;
				}
				return Math.Sqrt(argument);
			}
			else if (x <= 8)
			{
				// Правая наклонная прямая: от (r, 0) до (8, -3)
				double slope = -3.0 / (8.0 - r);
				return slope * (x - r);
			}
			else // x > 8
			{
				return -3.0;
			}
		}
	}
}
