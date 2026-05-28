using Clinica.Developer.Model.General;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.Fisioterapia
{
    public class FisioterapiaM
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

        // PROPIOS DE FISIOTERAPIA
        [BsonElement("evaluacion_postural")]
        public string EvaluacionPostural { get; set; }

        [BsonElement("arcos_movilidad")]
        public List<Fisioterapia.ArcoMovilidad> ArcosMovilidad { get; set; }

        [BsonElement("fuerza_muscular")]
        public List<Fisioterapia.EvaluacionFuerza> FuerzaMuscular { get; set; }

        [BsonElement("pruebas_especiales")]
        public string PruebasEspeciales { get; set; }

        [BsonElement("diagnostico_funcional")]
        public string DiagnosticoFuncional { get; set; }
        // -----------------------

        [BsonElement("tratamiento")]
        public List<string> Tratamiento { get; set; } = new List<string>();

        [BsonElement("recomendaciones")]
        public List<string> Recomendaciones { get; set; } = new List<string>();
    }
}
