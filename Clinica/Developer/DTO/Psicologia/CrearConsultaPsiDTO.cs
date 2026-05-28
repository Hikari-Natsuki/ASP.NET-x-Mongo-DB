using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.Psicologia
{
    public record CrearConsultaPsiDTO(
        PacienteDTO Paciente,
        string MedicoId,
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
