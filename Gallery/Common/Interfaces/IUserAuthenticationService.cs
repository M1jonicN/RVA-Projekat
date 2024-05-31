using Common.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts
{
    [ServiceContract]
    public interface IUserAuthenticationService
    {
        [OperationContract]
        User Login(string username, string password);

        [OperationContract]
        bool Register(string username, string password, string firstName, string lastName);

        [OperationContract]
        bool Logout(string username);

        [OperationContract]
        User FindUser(string username);

        [OperationContract]
        bool SaveChanges(User user);

    }
}
