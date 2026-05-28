using Clinica.Developer.DTO.Paciente;

namespace Clinica.Developer.DTO.Fisioterapia
{
    public record CrearConsultaFisDTO(
        PacienteDTO Paciente,
        string MedicoId,
        string Especialidad,
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
    {}
}
