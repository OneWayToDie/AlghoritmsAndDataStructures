using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work2ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work2ViewModel()
		{
			Title = "Практическая работа 2";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new GraphTaskViewModel()
			};
		}
	}
}