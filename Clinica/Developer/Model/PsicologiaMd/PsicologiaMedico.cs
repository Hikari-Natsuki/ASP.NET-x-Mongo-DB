using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.PsicologiaMd
{
    public class PsicologiaMedico: Psicologia
    {
        [BsonElement("medico_detalle")]
        public List<MedicoConsulta> MedicoDetalle { get; set; }
    }
}
