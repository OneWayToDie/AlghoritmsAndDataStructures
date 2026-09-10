using System;

namespace AlgorithmsLauncher
{
	public class HistoryEntry
	{
		public DateTime Timestamp { get; set; }
		public string TaskName { get; set; }
		public string InputSummary { get; set; }
		public string ResultSummary { get; set; }
		public string Elapsed { get; set; }
	}
}
