using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.OdontologiaDTO
{
    public record HistoriaClinicaOdoDTO(
        string Id,
        string MedicoNombre,
        string MedicoApellido,
        string MedicoEspecialidad,
        string MedicoId,
        PacienteDTO Paciente,
        string Entidad,
        List<AntecedenteDTO> Antecedentes,
        string MotivoConsulta,
        string HigieneOral,
        string EstadoEncias,
        List<OdontogramaDTO> Odontograma,
        List<string> Tratamiento,
        List<string> Recomendaciones)
    {

    }
}
