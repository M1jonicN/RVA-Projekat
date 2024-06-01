using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public enum Style
    {
        [EnumMember]
        Realism,
        [EnumMember]
        Naturalism,
        [EnumMember]
        Photorealism,
        [EnumMember]
        Surrealism,
        [EnumMember]
        Magical_realism,
        [EnumMember]
        Symbolism,
        [EnumMember]
        Minimalism,
        [EnumMember]
        Expressionism,
        [EnumMember]
        Colorism,
        [EnumMember]
        Constructivism,
        [EnumMember]
        Dadaism,
        [EnumMember]
        Fauvism,
        [EnumMember]
        Geometricism,
        [EnumMember]
        Hyperrealism,
        [EnumMember]
        Naive_art,
        [EnumMember]
        Op_art,
        [EnumMember]
        Suprematism,
        [EnumMember]
        Entity_Deleted
    }
}
