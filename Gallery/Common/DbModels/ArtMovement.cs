using System.Runtime.Serialization;

namespace Common.DbModels
{
    [DataContract]
    public enum ArtMovement
    {
        [EnumMember]
        Renaissance,
        [EnumMember]
        Baroque,
        [EnumMember]
        Classicism,
        [EnumMember]
        Romanticism,
        [EnumMember]
        Impressionism,
        [EnumMember]
        Expressionism,
        [EnumMember]
        Cubism,
        [EnumMember]
        Contemporary_art,
        [EnumMember]
        PostImpressionism,
        [EnumMember]
        Postmodernism,
        [EnumMember]
        Minimalism,
        [EnumMember]
        Pop_art,
        [EnumMember]
        Painting,
        [EnumMember]
        Sculpture,
        [EnumMember]
        Architecture,
        [EnumMember]
        Photography,
        [EnumMember]
        Film,
        [EnumMember]
        Entity_Deleted
    }
}