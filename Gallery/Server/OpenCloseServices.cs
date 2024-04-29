using Common.Contracts;
using Common.Services;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class OpenCloseServices
    {
        private ServiceHost authService;
        private ServiceHost galleryService;

        public OpenCloseServices()
        {

        }

        public void Open()
        {
            try
            {
                NetTcpBinding bindingAuth = new NetTcpBinding();
                string addressAuth = "net.tcp://localhost:8085/Authentifiaction";
                authService = new ServiceHost(typeof(AuthService));
                authService.AddServiceEndpoint(typeof(IAuthService), bindingAuth, addressAuth);

                NetTcpBinding bindingGallery = new NetTcpBinding();
                string addressGallery = "net.tcp://localhost:8086/Gallery";
                galleryService = new ServiceHost(typeof(GalleryService));
                galleryService.AddServiceEndpoint(typeof(IGalleryService), bindingGallery, addressGallery);

                authService.Open();
                Console.WriteLine("Authentification Service opened...");
                galleryService.Open();
                Console.WriteLine("Gallery Service opened...");
            }
            catch (Exception ex)
            {
                // Rukovanje izuzetkom prilikom otvaranja servisa
                Console.WriteLine($"An error occurred while opening services: {ex.Message}");
            }
        }

        public void Close()
        {
            try
            {
                authService.Close();
                Console.WriteLine("Authentification Service closed...");
                galleryService.Close();
                Console.WriteLine("Gallery Service closed...");
            }
            catch (Exception ex)
            {
                // Rukovanje izuzetkom prilikom zatvaranja servisa
                Console.WriteLine($"An error occurred while closing services: {ex.Message}");
            }
        }
    }
}
