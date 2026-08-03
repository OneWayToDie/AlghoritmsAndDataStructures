using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work2ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work2ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new GraphTaskViewModel()
			};
		}
	}
}
