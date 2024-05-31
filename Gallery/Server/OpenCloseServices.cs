using Common.Contracts;
using Common.Interfaces;
using Common.Services;
using log4net;
using Server.Services;
using System;
using System.ServiceModel;

namespace Server
{
    public class OpenCloseServices
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OpenCloseServices));

        private static ServiceHost UserAuthenticationService;
        private static ServiceHost galleryService;
        private static ServiceHost woaService;
        private static ServiceHost authorService;

        public static void Open()
        {
            try
            {
                NetTcpBinding bindingAuth = new NetTcpBinding();
                string addressAuth = "net.tcp://localhost:8085/Authentifiaction";
                UserAuthenticationService = new ServiceHost(typeof(UserAuthenticationService));
                UserAuthenticationService.AddServiceEndpoint(typeof(IUserAuthenticationService), bindingAuth, addressAuth);

                NetTcpBinding bindingGallery = new NetTcpBinding();
                string addressGallery = "net.tcp://localhost:8086/Gallery";
                galleryService = new ServiceHost(typeof(GalleryService));
                galleryService.AddServiceEndpoint(typeof(IGalleryService), bindingGallery, addressGallery);

                NetTcpBinding bindingWoa = new NetTcpBinding();
                string addressWoa = "net.tcp://localhost:8087/WorkOfArt";
                woaService = new ServiceHost(typeof(WorkOfArtService));
                woaService.AddServiceEndpoint(typeof(IWorkOfArtService), bindingWoa, addressWoa);

                NetTcpBinding bindingAuthor = new NetTcpBinding();
                string addressAuthor = "net.tcp://localhost:8088/Author";
                authorService = new ServiceHost(typeof(AuthorService));
                authorService.AddServiceEndpoint(typeof(IAuthorService), bindingAuthor, addressAuthor);

                UserAuthenticationService.Open();
                log.Info("Authentication Service opened...");
                galleryService.Open();
                log.Info("Gallery Service opened...");
                woaService.Open();
                log.Info("Work Of Art Service opened...");
                authorService.Open();
                log.Info("Author Service opened...");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while opening services", ex);
                throw;
            }
        }

        public static void Close()
        {
            try
            {
                UserAuthenticationService.Close();
                log.Info("Authentication Service closed...");
                galleryService.Close();
                log.Info("Gallery Service closed...");
                woaService.Close();
                log.Info("Work Of Art Service closed...");
                authorService.Close();
                log.Info("Author Service closed...");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while closing services", ex);
                throw;
            }
        }
    }
}
