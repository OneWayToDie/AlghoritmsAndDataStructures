using System.Collections.ObjectModel;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class Work6ViewModel
	{
		public ObservableCollection<BaseTaskViewModel> Tasks { get; private set; }

		public Work6ViewModel()
		{
			Tasks = new ObservableCollection<BaseTaskViewModel>
			{
				new SeriesTaskViewModel()
			};
		}
	}
}