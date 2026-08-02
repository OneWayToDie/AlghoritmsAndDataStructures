using System.Windows.Input;
using AlghoritmsAndDataStructures.Helpers;

namespace AlghoritmsAndDataStructures.ViewModels.Base
{
	public abstract class BaseTaskViewModel : System.ComponentModel.INotifyPropertyChanged
	{
		private string _resultText = string.Empty;
		public string ResultText
		{
			get => _resultText;
			set
			{
				_resultText = value;
				OnPropertyChanged(nameof(ResultText));
			}
		}

		public ICommand ComputeCommand { get; }

		public BaseTaskViewModel()
		{
			ComputeCommand = new RelayCommand(ExecuteCompute, CanCompute);
		}

		protected abstract void ExecuteCompute(object parameter);
		protected virtual bool CanCompute(object parameter) => true;

		public abstract string Title { get; }

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}
	}
}