using Common.DbModels;
using Common.Helpers;
using Common.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class DuplicateGalleryCommand : GalleryCommand
    {
        private Common.DbModels.Gallery _gallery;
        private readonly IGalleryService _galleryService;
        private static readonly ILog log = LogManager.GetLogger(typeof(AddGalleryCommand));
        private bool undo = false;

        private ObservableCollection<Gallery> galleries = new ObservableCollection<Gallery>();

        public DuplicateGalleryCommand(Gallery gallery, IGalleryService galleryService, ObservableCollection<Gallery> Galleries)
        {
            _gallery = gallery;
            _galleryService = galleryService;
            PIB = gallery.PIB;
            galleries = Galleries;
        }

        public override void Execute()
        {
            if (!undo)
            {
                var duplicatedWorkOfArts = new List<Common.DbModels.WorkOfArt>();
                if (_gallery.WorkOfArts != null)
                {
                    foreach (var workOfArt in _gallery.WorkOfArts)
                    {
                        duplicatedWorkOfArts.Add(new WorkOfArt
                        {
                            // Pretpostavljajući da WorkOfArt ima ove atribute, dodajte sve potrebne atribute za kopiranje
                            ID = workOfArt.ID,
                            ArtName = workOfArt.ArtName,
                            ArtMovement = workOfArt.ArtMovement,
                            Style = workOfArt.Style,
                            AuthorID = workOfArt.AuthorID,
                            AuthorName = workOfArt.AuthorName,
                            GalleryPIB = workOfArt.GalleryPIB,
                            IsDeleted = workOfArt.IsDeleted,
                            // Dodajte ostale atribute koje je potrebno kopirati
                        });
                    }
                }
    
                // Kreiranje duplikata galerije
                var duplicateGallery = new Common.DbModels.Gallery
                {
                    PIB = PibHelper.GenerateUniquePIB(galleries.ToList()),
                    Address = _gallery.Address,
                    MBR = _gallery.MBR,
                    WorkOfArts = duplicatedWorkOfArts,
                    IsDeleted = _gallery.IsDeleted
                };

                // Dodavanje duplikata galerije u bazu podataka
                
                _galleryService.CreateNewGallery(duplicateGallery);
                //  log.Info($"{_loggedInUser.Username} successfully duplicated the gallery with PIB {gallery.PIB}.");
            }
            else
            {
                _gallery.IsDeleted = false;
                _galleryService.SaveGalleryChanges(_gallery);
            }
        }

        public override void UnExecute()
        {
            // Instead of actually deleting, just mark as IsDeleted
            _gallery.IsDeleted = true;
            _galleryService.SaveGalleryChanges(_gallery);
            undo = true;
        }
    }
}
