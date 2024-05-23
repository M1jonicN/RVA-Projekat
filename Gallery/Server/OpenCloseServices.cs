﻿using Common.Contracts;
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

        private static ServiceHost authService;
        private static ServiceHost galleryService;
        private static ServiceHost woaService;
        private static ServiceHost authorService;

        public static void Open()
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

                NetTcpBinding bindingWoa = new NetTcpBinding();
                string addressWoa = "net.tcp://localhost:8087/WorkOfArt";
                woaService = new ServiceHost(typeof(WorkOfArtService));
                woaService.AddServiceEndpoint(typeof(IWorkOfArt), bindingWoa, addressWoa);

                NetTcpBinding bindingAuthor = new NetTcpBinding();
                string addressAuthor = "net.tcp://localhost:8088/Author";
                authorService = new ServiceHost(typeof(AuthorService));
                authorService.AddServiceEndpoint(typeof(IAuthor), bindingAuthor, addressAuthor);

                authService.Open();
                Console.WriteLine("Authentication Service opened...");
                galleryService.Open();
                Console.WriteLine("Gallery Service opened...");
                woaService.Open();
                Console.WriteLine("Work Of Art Service opened...");
                authorService.Open();
                Console.WriteLine("Author Service opened...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while opening services. {ex}");
                throw;
            }
        }

        public static void Close()
        {
            try
            {
                authService.Close();
                Console.WriteLine("Authentication Service closed...");
                galleryService.Close();
                Console.WriteLine("Gallery Service closed...");
                woaService.Close();
                Console.WriteLine("Work Of Art Service closed...");
                authorService.Close();
                Console.WriteLine("Author Service closed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while closing services", ex);
                throw;
            }
        }
    }
}
