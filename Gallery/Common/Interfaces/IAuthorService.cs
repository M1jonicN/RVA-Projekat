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
    public interface IAuthorService
    {
        [OperationContract]
        List<Author> GetAllAuthores();

        [OperationContract]
        string GetAuthorNameForWorkOfArt(int workOfArtId, string galleryPIB);
        [OperationContract]
        Author GetAuthorByWorkOfArtId(int wokrOfArtId);
        [OperationContract]
        bool SaveAuthorChanges(Author author);

        [OperationContract]
        bool DeleteAuhor(int authorID);

        [OperationContract]
        Author GetAuthorById(int authorId);

        [OperationContract]
        bool CreateNewAuthor(Author newAuthor);
    }
}
