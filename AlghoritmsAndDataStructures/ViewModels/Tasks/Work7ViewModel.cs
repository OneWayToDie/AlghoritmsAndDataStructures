using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work7ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work7ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new QuickSortTaskViewModel()
			};
		}
	}
}
