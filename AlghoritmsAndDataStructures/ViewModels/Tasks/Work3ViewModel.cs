using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work3ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work3ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new AreaTaskViewModel()
			};
		}
	}
}