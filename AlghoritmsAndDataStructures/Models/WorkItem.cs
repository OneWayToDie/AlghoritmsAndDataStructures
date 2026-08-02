namespace AlghoritmsAndDataStructures.Models
{
	public class WorkItem
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string IconPath { get; set; }
		public bool IsAvailable { get; set; }

		public WorkItem()
		{
			Title = "";
			IconPath = "";
		}
	}
}
