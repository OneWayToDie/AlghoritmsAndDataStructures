using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work7ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work7ViewModel()
		{
			Title = "Практическая работа 7";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new QuickSortTaskViewModel()
			};
		}
	}
}
