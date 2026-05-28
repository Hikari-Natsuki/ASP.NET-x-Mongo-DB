using Clinica.Developer.Model.General;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model
{
    public class MedicoConsulta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; }

        [BsonElement("apellido")]
        public string Apellido { get; set; }

        [BsonElement("documento")]
        public DocumentoIdentidad Documento { get; set; }

        [BsonElement("especialidad")]
        public string Especialidad { get; set; }

        
    }
}
