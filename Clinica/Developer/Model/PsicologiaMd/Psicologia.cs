using Clinica.Developer.Model.General;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.PsicologiaMd
{
    public class Psicologia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("MedicoId")]
        public string MedicoId { get; set; }

        [BsonElement("Entidad")]
        public string Entidad { get; set; }

        [BsonElement("FechaConsulta")]
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;

        [BsonElement("Paciente")]
        public Paciente Paciente { get; set; }

        [BsonElement("antecedentes")]
        public List<Antecedente> Antecedentes { get; set; }

        [BsonElement("motivo_consulta")]
        public string MotivoConsulta { get; set; }

        [BsonElement("enfermedad_actual")]
        public List<string> EnfermedadActual { get; set; }

        [BsonElement("examen_mental")]
        public string ExamenMental { get; set; }

        [BsonElement("tratamiento")]
        public List<string> Tratamiento { get; set; }

        [BsonElement("recomendaciones")]
        public List<string> Recomendaciones { get; set; }
    }
}
