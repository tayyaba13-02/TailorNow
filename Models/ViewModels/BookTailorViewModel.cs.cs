namespace TailorrNow.Models.ViewModels
{
    public class BookTailorViewModel
    {
        public IEnumerable<Tailor> Tailors { get; set; } = new List<Tailor>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedService { get; set; } = "all";
    }
}