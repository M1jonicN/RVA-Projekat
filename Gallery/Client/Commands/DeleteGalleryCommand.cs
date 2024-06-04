using Common.DbModels;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class DeleteGalleryCommand : GalleryCommand
    {
        private IGalleryService _galleryService;
        private Common.DbModels.Gallery _gallery { get; set; }   
        public DeleteGalleryCommand(Common.DbModels.Gallery gallery, IGalleryService galleryService)
        {
            _galleryService = galleryService;
            this._gallery = gallery;
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
