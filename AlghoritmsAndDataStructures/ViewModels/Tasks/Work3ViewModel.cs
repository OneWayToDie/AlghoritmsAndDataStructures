using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work3ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work3ViewModel()
		{
			Title = "Практическая работа 3";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new AreaTaskViewModel()
			};
		}
	}
}