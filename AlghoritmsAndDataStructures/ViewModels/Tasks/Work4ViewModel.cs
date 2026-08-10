using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work4ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work4ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new SeriesSumTaskViewModel(),
				new AverageTaskViewModel()
			};
		}
	}
}
