using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Developer.Model.OdontologoMd
{
    public class OdontologoMedico: Odontologo
    {
        [BsonElement("medico_detalle")]
        public List<MedicoConsulta> MedicoDetalle { get; set; }
    }
}
