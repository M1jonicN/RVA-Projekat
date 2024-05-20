using Common.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IWorkOfArt
    {
        [OperationContract]
        List<WorkOfArt> GetWorkOfArtsForGallery(string galleryPib);
    }
}
