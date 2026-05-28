using Clinica.Developer.Model.Fisioterapia;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.FisioterapiaMd
{
    public class FisioterapiaMedico: FisioterapiaM
    {
        [BsonElement("medico_detalle")]
        public List<MedicoConsulta> MedicoDetalle { get; set; }
    }
}
