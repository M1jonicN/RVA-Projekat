using Common.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    [ServiceContract]
    public interface IGalleryService
    {
        [OperationContract]
        List<Author> GetAllAuthores();

        [OperationContract]
        List<Gallery> GetAllGalleries();

        [OperationContract]
        List<WorkOfArt> GetAllWorkOfArts();
    }
}
