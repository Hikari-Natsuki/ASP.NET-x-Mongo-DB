using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.OdontologiaDTO
{
    public record CrearConsultaOdoDTO(
        string MedicoId,
        string Especialidad,
        PacienteDTO Paciente,
        string Entidad,
        List<AntecedenteDTO> Antecedentes,
        string MotivoConsulta,
        // PROPIOS DE ODONTOLOGIA
        string HigieneOral,
        string EstadoEncias,
        List<OdontogramaDTO> Odontograma,
     // -----------------------
        List<string> Tratamiento,
        List<string> Recomendaciones
     )
    {
    }
}
