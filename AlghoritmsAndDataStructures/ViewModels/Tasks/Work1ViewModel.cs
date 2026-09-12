using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work1ViewModel : IWorkViewModel
	{
		public string Title { get; }

		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work1ViewModel()
		{
			Title = "Практическая работа 1";
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new CubeTaskViewModel(),
				new FractionTaskViewModel()
			};
		}
	}
}