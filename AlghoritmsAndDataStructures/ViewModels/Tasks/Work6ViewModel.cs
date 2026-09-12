using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work6ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work6ViewModel()
		{
			Title = "Практическая работа 6";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new SeriesTaskViewModel()
			};
		}
	}
}