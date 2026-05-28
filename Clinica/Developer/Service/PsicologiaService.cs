using Clinica.Developer.DTO.Paciente;
using Clinica.Developer.DTO.Psicologia;
using Clinica.Developer.Model;
using Clinica.Developer.Model.General;
using Clinica.Developer.Model.PsicologiaMd;
using MongoDB.Driver;
using Psicología.Developer.Repository;

namespace Psicología.Developer.Service
{
    public class PsicologiaService
    {
        private readonly PsicologiaRepository _repository;
        private readonly IMongoCollection<MedicoConsulta> _medicoColeccion;

        public PsicologiaService(PsicologiaRepository repository)
        {
            this._repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Psicologia> RegistrarConsulta(CrearConsultaPsiDTO consultaDTO)
        {
            try
            {
                var entidadModelo = new Psicologia
                {
                    MedicoId = consultaDTO.MedicoId,
                    Entidad = consultaDTO.Entidad.ToUpper(),
                    MotivoConsulta = consultaDTO.MotivoConsulta,
                    ExamenMental = consultaDTO.ExamenMental,
                    EnfermedadActual = consultaDTO.EnfermedadActual,
                    Tratamiento = consultaDTO.Tratamiento,
                    Recomendaciones = consultaDTO.Recomendaciones,
                    Paciente = new Paciente
                    {
                        Nombre = consultaDTO.Paciente.Nombre.ToUpper(),
                        Edad = consultaDTO.Paciente.Edad,
                        Sexo = consultaDTO.Paciente.Sexo.ToUpper(),
                        Documento = new DocumentoIdentidad
                        {
                            Tipo = consultaDTO.Paciente.Documento.Tipo.ToUpper(),
                            Numero = consultaDTO.Paciente.Documento.Numero
                        }
                    },
                    Antecedentes = consultaDTO.Antecedentes.Select(a => new Antecedente
                    {
                        Tipo = a.Tipo.ToUpper(),
                        Observaciones = a.Observaciones
                    }).ToList()
                };

                await _repository.Insertar(entidadModelo);
                
                return entidadModelo;
            }
            catch (MongoException ex)
            {
                throw new MongoException($"Error al registrar la consulta: {ex.Message}", ex);

            }
        }
        // Obtener todos
        public async Task<List<Psicologia>> ObtenerTodasLasConsultas()
        {
            return await _repository.ObtenerTodos();
        }

        // Obtener por número de documento
        public async Task<List<HistoriaClinicaPsiDTO>> ObtenerHistorialPorDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                throw new ArgumentException(
                    "El número de documento es obligatorio.",
                    nameof(numeroDocumento));
            }

            var historias = await _repository.ObtenerPorDocumento(numeroDocumento);

            var resultadoJson = new List<HistoriaClinicaPsiDTO>();

            foreach (var h in historias)
            {
                var medico = h.MedicoDetalle?.FirstOrDefault();

                resultadoJson.Add(new HistoriaClinicaPsiDTO(
                    h.Id,
                    medico?.Nombre ?? "Médico",
                    medico?.Apellido ?? "No Asignado",
                    medico?.Especialidad ?? "N/A",
                    h.MedicoId,

                    new PacienteDTO(
                        h.Paciente.Nombre,
                        new DocumentoIdentidadDTO(
                            h.Paciente.Documento.Tipo,
                            h.Paciente.Documento.Numero
                        ),
                        h.Paciente.Edad,
                        h.Paciente.Sexo
                    ),
                    h.Entidad,
                    h.Antecedentes?
                        .Select(a => new AntecedenteDTO(
                            a.Tipo,
                            a.Observaciones
                        ))
                        .ToList() ?? new List<AntecedenteDTO>(),

                    h.MotivoConsulta,
                    h.EnfermedadActual ?? new List<string>(),
                    h.ExamenMental,
                    h.Tratamiento ?? new List<string>(),
                    h.Recomendaciones ?? new List<string>()
                ));
            }

            return resultadoJson;
        }

        // Actualizar consulta por ID
        public async Task<bool> ActualizarHistoria(string id, CrearConsultaPsiDTO consultaDTO)
        {
            try
            {
                var historiaEditada = new Psicologia
                {
                    Id = id,
                    Entidad = consultaDTO.Entidad,
                    MotivoConsulta = consultaDTO.MotivoConsulta,
                    ExamenMental = consultaDTO.ExamenMental,
                    EnfermedadActual = consultaDTO.EnfermedadActual,
                    Tratamiento = consultaDTO.Tratamiento,
                    Recomendaciones = consultaDTO.Recomendaciones,
                    Paciente = new Paciente
                    {
                        Nombre = consultaDTO.Paciente.Nombre,
                        Edad = consultaDTO.Paciente.Edad,
                        Sexo = consultaDTO.Paciente.Sexo,
                        Documento = new DocumentoIdentidad
                        {
                            Tipo = consultaDTO.Paciente.Documento.Tipo,
                            Numero = consultaDTO.Paciente.Documento.Numero
                        }
                    },
                    Antecedentes = consultaDTO.Antecedentes.Select(a => new Antecedente
                    {
                        Tipo = a.Tipo,
                        Observaciones = a.Observaciones
                    }).ToList(),
                    FechaConsulta = DateTime.UtcNow
                };
                return await _repository.Actualizar(id, historiaEditada);
            }
            catch (MongoException ex)
            {
                throw new MongoException($"Error al actualizar la consulta: {ex.Message}", ex);
            }
        }
        // Eliminar consulta por ID
        public async Task<bool> EliminarHistoria(string id)
        {
            return await _repository.Eliminar(id);
        }
    }
}
