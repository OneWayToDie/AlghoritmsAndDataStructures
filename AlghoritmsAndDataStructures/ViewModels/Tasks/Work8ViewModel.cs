using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work8ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work8ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new NeighborhoodAverageTaskViewModel()
			};
		}
	}
}
