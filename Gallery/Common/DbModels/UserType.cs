using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public enum UserType
    {
        [EnumMember]
        Admin,
        [EnumMember]
        User
    }
}