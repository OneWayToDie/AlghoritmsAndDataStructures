using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work5ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work5ViewModel()
		{
			Title = "Практическая работа 5";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new ArrayTaskViewModel()
			};
		}
	}
}