using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work1ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work1ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new CubeTaskViewModel(),
				new FractionTaskViewModel()
			};
		}
	}
}