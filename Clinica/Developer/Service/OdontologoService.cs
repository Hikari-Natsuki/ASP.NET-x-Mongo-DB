using Clinica.Developer.DTO.OdontologiaDTO;
using Clinica.Developer.DTO.Paciente;
using Clinica.Developer.DTO.Psicologia;
using Clinica.Developer.Model;
using Clinica.Developer.Model.General;
using Clinica.Developer.Model.OdontologoMd;
using Clinica.Developer.Repository;
using MongoDB.Driver;

namespace Clinica.Developer.Service
{
    public class OdontologoService
    {
        private readonly OdontologoRepository _repository;

        public OdontologoService(OdontologoRepository repository)
        {
            this._repository = repository;
        }

        public async Task<Odontologo> RegistrarConsulta(CrearConsultaOdoDTO consultaDTO)
        {
            try
            {
                var entidadModelo = new Odontologo
                {
                    MedicoId = consultaDTO.MedicoId,
                    Entidad = consultaDTO.Entidad.ToUpper(),
                    MotivoConsulta = consultaDTO.MotivoConsulta,
                    Tratamiento = consultaDTO.Tratamiento,
                    Recomendaciones = consultaDTO.Recomendaciones,
                    // Paciente
                    Paciente = new Paciente
                    {
                        Nombre = consultaDTO.Paciente.Nombre.ToUpper(),
                        Edad = consultaDTO.Paciente.Edad,
                        Sexo = consultaDTO.Paciente.Sexo.ToUpper(),
                        // Documento
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
                    }).ToList(),

                    HigieneOral = consultaDTO.HigieneOral.ToUpper(),
                    EstadoEncias = consultaDTO.EstadoEncias,

                    Odontograma = consultaDTO.Odontograma.Select(a => new Odontograma
                    {
                        NumeroDiente = a.NumeroDiente,
                        Cara = a.Cara.ToUpper(),
                        Estado = a.Estado.ToUpper(),
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
        public async Task<List<HistoriaClinicaOdoDTO>> ObtenerHistorialPorDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                throw new ArgumentException(
                    "El número de documento es obligatorio.",
                    nameof(numeroDocumento));
            }

            var historias = await _repository.ObtenerPorDocumento(numeroDocumento);

            var resultadoJson = new List<HistoriaClinicaOdoDTO>();

            foreach (var h in historias)
            {
                var medico = h.MedicoDetalle?.FirstOrDefault();

                resultadoJson.Add(new HistoriaClinicaOdoDTO(
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
                    h.HigieneOral,
                    h.EstadoEncias,
                    h.Odontograma?
                        .Select(o => new OdontogramaDTO(
                            o.NumeroDiente,
                            o.Cara,
                            o.Estado,
                            o.Observaciones
                        ))
                        .ToList() ?? new List<OdontogramaDTO>(),
                    h.Tratamiento ?? new List<string>(),
                    h.Recomendaciones ?? new List<string>()
                ));
            }

            return resultadoJson;
        }
        public async Task<bool> ActualizarHistoria(string id, CrearConsultaOdoDTO consultaDTO)
        {
            try
            {
                var entidadModelo = new Odontologo
                {
                    Id = id,
                    MedicoId = consultaDTO.MedicoId,
                    Entidad = consultaDTO.Entidad.ToUpper(),
                    MotivoConsulta = consultaDTO.MotivoConsulta,
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
                    }).ToList(),
                    HigieneOral = consultaDTO.HigieneOral.ToUpper(),
                    EstadoEncias = consultaDTO.EstadoEncias,
                    Odontograma = consultaDTO.Odontograma.Select(o => new Odontograma
                    {
                        NumeroDiente = o.NumeroDiente,
                        Cara = o.Cara.ToUpper(),
                        Estado = o.Estado.ToUpper(),
                        Observaciones = o.Observaciones
                    }).ToList()
                };
                
                return await _repository.Actualizar(id, entidadModelo);
            }
            catch (MongoException ex)
            {
                throw new MongoException($"Error al actualizar la historia clínica: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarHistoria(string id)
        {
            return await _repository.Eliminar(id);
        }
    }
}
