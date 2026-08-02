namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public interface ICoreCalculator<TInput, TOutput>
	{
		TOutput Compute(TInput input);
	}
}
