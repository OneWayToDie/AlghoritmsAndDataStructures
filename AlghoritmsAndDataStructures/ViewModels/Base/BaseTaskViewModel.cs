using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Helpers;

namespace AlghoritmsAndDataStructures.ViewModels.Base
{
	public abstract class BaseTaskViewModel : System.ComponentModel.INotifyPropertyChanged
	{
		private string _resultText = string.Empty;
		private ObservableCollection<string> _history = new ObservableCollection<string>();
		private const int MaxHistoryCount = 20;

		public virtual string ResultText
		{
			get => _resultText;
			set
			{
				_resultText = value;
				OnPropertyChanged(nameof(ResultText));
			}
		}

		public ObservableCollection<string> History
		{
			get => _history;
			set
			{
				_history = value;
				OnPropertyChanged(nameof(History));
			}
		}

		public ICommand ComputeCommand { get; }
		public ICommand ClearHistoryCommand { get; }

		public abstract string HistoryKey { get; }

		public BaseTaskViewModel()
		{
			ComputeCommand = new RelayCommand(ExecuteCompute, CanCompute);
			ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);

			// Загружаем историю из файла
			var savedHistory = HistoryStorage.Load(HistoryKey);
			foreach (var item in savedHistory)
				History.Add(item);
		}

		protected abstract void ExecuteCompute(object parameter);
		protected virtual bool CanCompute(object parameter) => true;

		public abstract string Title { get; }

		protected void AddHistoryEntry(string entry)
		{
			History.Insert(0, entry);
			if (History.Count > MaxHistoryCount)
				History.RemoveAt(History.Count - 1);
			HistoryStorage.Save(HistoryKey, new List<string>(History));
		}

		private void ExecuteClearHistory(object parameter)
		{
			History.Clear();
			HistoryStorage.Save(HistoryKey, new List<string>(History));
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}
	}
}