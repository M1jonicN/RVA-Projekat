using Common.DbModels;

namespace Client.ViewModels
{
    public class WorkOfArtDetailsViewModel : BaseViewModel
    {
        public WorkOfArt WorkOfArt { get; }
        public Author Author { get; }

        public WorkOfArtDetailsViewModel(WorkOfArt workOfArt, Author author)
        {
            WorkOfArt = workOfArt;
            Author = author;
        }
    }
}
