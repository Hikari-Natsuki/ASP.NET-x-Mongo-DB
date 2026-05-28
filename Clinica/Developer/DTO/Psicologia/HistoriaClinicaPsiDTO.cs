using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.Psicologia
{
    public record HistoriaClinicaPsiDTO(
        string Id,
        string MedicoNombre,
        string MedicoApellido,
        string MedicoEspecialidad,
        string MedicoId,
        PacienteDTO Paciente,
        string Entidad,
        List<AntecedenteDTO> Antecedentes,
        string MotivoConsulta,
        List<string> EnfermedadActual,
        string ExamenMental,
        List<string> Tratamiento,
        List<string> Recomendaciones)
    {
    }
}
