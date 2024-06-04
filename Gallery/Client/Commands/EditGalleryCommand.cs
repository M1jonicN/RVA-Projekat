using Common.DbModels;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class EditGalleryCommand : GalleryCommand
    {
        private IGalleryService _galleryService;
        private Gallery _gallery;
        private Gallery _oldGallery;
        public EditGalleryCommand(Gallery gallery, IGalleryService galleryService, Gallery oldGallery)
        {
            _gallery = gallery;
            _galleryService = galleryService;
            _oldGallery = oldGallery;

        }
        public override void Execute()
        {
            _galleryService.SaveGalleryChanges(_gallery);
        }

        public override void UnExecute()
        {
            _galleryService.SaveGalleryChanges(_oldGallery);
            
        }
    }
}
