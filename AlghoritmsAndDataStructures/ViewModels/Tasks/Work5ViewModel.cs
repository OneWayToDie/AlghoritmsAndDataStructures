using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work5ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work5ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new ArrayTaskViewModel()
			};
		}
	}
}