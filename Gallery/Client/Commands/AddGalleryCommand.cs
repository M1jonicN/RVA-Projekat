using Client.ViewModels;
using Common.DbModels;
using Common.Helpers;
using Common.Services;
using log4net;
using System.Windows;

namespace Client.Commands
{
    public class AddGalleryCommand : GalleryCommand
    {
        private  Gallery _gallery;
        private readonly IGalleryService _galleryService;
        private static readonly ILog log = LogManager.GetLogger(typeof(AddGalleryCommand));

        public AddGalleryCommand(Gallery gallery, IGalleryService galleryService)
        {
            _gallery = gallery;
            _galleryService = galleryService;
            PIB = gallery.PIB;
        }

        public override void Execute()
        {
            Gallery novaGalerija = _galleryService.CreateGallery(_gallery);
            if (novaGalerija != null)
            {
                log.Info($"New gallery created with PIB {_gallery.PIB}.");
                _gallery = novaGalerija;
            }
            else
            {
                _gallery.IsDeleted = false;
                _galleryService.SaveGalleryChanges(_gallery);

            }
        }

        public override void UnExecute()
        {
            _gallery.IsDeleted = true;
            _galleryService.SaveGalleryChanges(_gallery);
         
        }
    }
}
