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
    public interface IWorkOfArtService
    {
        [OperationContract]
        List<WorkOfArt> GetAllWorkOfArts();

        [OperationContract]
        List<WorkOfArt> GetWorkOfArtsForGallery(string galleryPib);

        [OperationContract]
        bool UpdateWorkOfArt(WorkOfArt workOfArt);

        [OperationContract]
        void GetAllWorkOfArtsDeletedForAuthorId(int authorID);

        [OperationContract]
        bool DeleteWorkOfArt(int workOfArtId);

        [OperationContract]
        WorkOfArt GetWorkOfArtById(int workOfArtId);

        [OperationContract]
        bool CreateNewWorkOfArt(WorkOfArt newWorkOfArt);
    }
}
