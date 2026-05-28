using Clinica.Developer.DTO.Fisioterapia;
using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.FisioterapiaDTO
{
    public record HistorialClinicoFisDTO(
        string Id,
        string MedicoNombre,
        string MedicoApellido,
        string MedicoEspecialidad,
        string MedicoId,
        PacienteDTO Paciente,
        string Entidad,
        List<AntecedenteDTO> Antecedentes,
        string MotivoConsulta,
        string EvaluacionPostural,
        List<ArcoMovilidadDTO> ArcosMovilidad,
        List<EvaluacionFuerzaDTO> FuerzaMuscular,
        string PruebasEspeciales,
        string DiagnosticoFuncional,
        List<string> Tratamiento,
        List<string> Recomendaciones)
    {
    }
}
