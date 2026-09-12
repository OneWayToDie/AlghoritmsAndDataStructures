using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work4ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work4ViewModel()
		{
			Title = "Практическая работа 4";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new SeriesSumTaskViewModel(),
				new AverageTaskViewModel()
			};
		}
	}
}