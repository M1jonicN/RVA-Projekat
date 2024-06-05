using Common.DbModels;
using Common.Services;

namespace Client.Commands
{
    public class DeleteGalleryCommand : GalleryCommand
    {
        private IGalleryService _galleryService;
        private Gallery _gallery { get; set; }   
        public DeleteGalleryCommand(Gallery gallery, IGalleryService galleryService)
        {
            _galleryService = galleryService;
            _gallery = gallery;
        }
        public override void Execute()
        {
            _gallery.IsDeleted = true;
            _galleryService.SaveGalleryChanges(_gallery);   

        }

        public override void UnExecute()
        {
            _gallery.IsDeleted = false;
            _galleryService.SaveGalleryChanges(_gallery);
        }
    }
}
