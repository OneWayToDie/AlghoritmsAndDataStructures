using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work8ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work8ViewModel()
		{
			Title = "Практическая работа 8";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new NeighborhoodAverageTaskViewModel()
			};
		}
	}
}
