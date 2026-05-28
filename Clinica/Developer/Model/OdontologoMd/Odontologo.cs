using Clinica.Developer.Model.General;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.OdontologoMd
{
    public class Odontologo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("medico_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MedicoId { get; set; }

        [BsonElement("especialidad")]
        public string Especialidad { get; set; }

        [BsonElement("paciente")]
        public Paciente Paciente { get; set; }

        [BsonElement("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [BsonElement("entidad")]
        public string Entidad { get; set; }

        [BsonElement("antecedentes")]
        public List<Antecedente> Antecedentes { get; set; } = new List<Antecedente>();

        [BsonElement("motivo_consulta")]
        public string MotivoConsulta { get; set; }

        // PROPIOS DE ODONTOLOGIA
        [BsonElement("higiene_oral")]
        public string HigieneOral { get; set; }

        [BsonElement("estado_encias")]
        public string EstadoEncias { get; set; }

        [BsonElement("odontograma")]
        public List<Odontograma> Odontograma { get; set; }
        // -----------------------

        [BsonElement("tratamiento")]
        public List<string> Tratamiento { get; set; } = new List<string>();

        [BsonElement("recomendaciones")]
        public List<string> Recomendaciones { get; set; } = new List<string>();
    }
}
