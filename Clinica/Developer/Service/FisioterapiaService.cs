using Clinica.Developer.DTO.Fisioterapia;
using Clinica.Developer.DTO.FisioterapiaDTO;
using Clinica.Developer.DTO.Paciente;
using Clinica.Developer.Model;
using Clinica.Developer.Model.Fisioterapia;
using Clinica.Developer.Model.Fisioterapia.Fisioterapia;
using Clinica.Developer.Model.FisioterapiaMd;
using Clinica.Developer.Model.General;
using Clinica.Developer.Repository;

namespace Clinica.Developer.Service
{
    public class FisioterapiaService
    {
        private readonly FisioterapiaRepository _repository;

        public FisioterapiaService(FisioterapiaRepository repository)
        {
            this._repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<FisioterapiaM> RegistrarConsulta(CrearConsultaFisDTO crear)
        {
            try
            {
                var entidadModelo = new FisioterapiaM
                {
                    MedicoId = crear.MedicoId,
                    Especialidad = crear.Especialidad.ToUpper(),
                    Entidad = crear.Entidad.ToUpper(),
                    FechaRegistro = DateTime.UtcNow,
                    MotivoConsulta = crear.MotivoConsulta.ToUpper(),
                    EvaluacionPostural = crear.EvaluacionPostural.ToUpper(),
                    PruebasEspeciales = crear.PruebasEspeciales.ToUpper(),
                    DiagnosticoFuncional = crear.DiagnosticoFuncional.ToUpper(),

                    Paciente = new Paciente
                    {
                        Nombre = crear.Paciente.Nombre.ToUpper(),
                        Edad = crear.Paciente.Edad,
                        Sexo = crear.Paciente.Sexo.ToUpper(),
                        Documento = new DocumentoIdentidad
                        {
                            Tipo = crear.Paciente.Documento.Tipo.ToUpper(),
                            Numero = crear.Paciente.Documento.Numero
                        }
                    },

                    Antecedentes = crear.Antecedentes.Select(a => new Antecedente
                    {
                        Tipo = a.Tipo.ToUpper(),
                        Observaciones = a.Observaciones
                    }).ToList(),

                    ArcosMovilidad = crear.ArcosMovilidad.Select(a => new ArcoMovilidad
                    {
                        Articulacion = a.Articulacion.ToUpper(),
                        Movimiento = a.Movimiento.ToUpper(),
                        GradosObtenidos = a.GradosObtenidos
                    }).ToList(),

                    FuerzaMuscular = crear.FuerzaMuscular.Select(f => new EvaluacionFuerza
                    {
                        GrupoMuscular = f.Musculo.ToUpper(),
                        EscalaDaniels = f.GradoFuerza
                    }).ToList(),

                    Tratamiento = crear.Tratamiento.Select(t => t.ToUpper()).ToList(),
                    Recomendaciones = crear.Recomendaciones.Select(r => r.ToUpper()).ToList()
                };

                await _repository.Insertar(entidadModelo);

                return entidadModelo;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                throw new Exception("Error al insertar la consulta de fisioterapia en la base de datos.", ex);
            }
        }
        public async Task<List<HistorialClinicoFisDTO>> ObtenerPorDocumento(string numeroDocumento)
        {
            var historias = await _repository.ObtenerPorDocumento(numeroDocumento);
            var resultadoJson = new List<HistorialClinicoFisDTO>();

            foreach (var h in historias)
            {
                var medico = h.MedicoDetalle?.FirstOrDefault();

                resultadoJson.Add(new HistorialClinicoFisDTO(
                    Id: h.Id,
                    MedicoNombre: medico?.Nombre ?? "Médico",
                    MedicoApellido: medico?.Apellido ?? "No Asignado",
                    MedicoEspecialidad: medico?.Especialidad ?? "N/A",
                    MedicoId: h.MedicoId,

                    Paciente: new PacienteDTO(
                        Nombre: h.Paciente.Nombre,
                        Edad: h.Paciente.Edad,
                        Sexo: h.Paciente.Sexo,
                        Documento: new DocumentoIdentidadDTO(
                            Tipo: h.Paciente.Documento.Tipo,
                            Numero: h.Paciente.Documento.Numero
                        )
                    ),

                    Entidad: h.Entidad,

                    Antecedentes: h.Antecedentes.Select(a => new AntecedenteDTO(
                        Tipo: a.Tipo,
                        Observaciones: a.Observaciones
                    )).ToList(),

                    MotivoConsulta: h.MotivoConsulta,
                    EvaluacionPostural: h.EvaluacionPostural,

                    ArcosMovilidad: h.ArcosMovilidad.Select(a => new ArcoMovilidadDTO(
                        Articulacion: a.Articulacion,
                        Movimiento: a.Movimiento,
                        GradosObtenidos: a.GradosObtenidos
                    )).ToList(),

                    FuerzaMuscular: h.FuerzaMuscular.Select(f => new EvaluacionFuerzaDTO(
                        Musculo: f.GrupoMuscular,
                        GradoFuerza: f.EscalaDaniels
                    )).ToList(),

                    PruebasEspeciales: h.PruebasEspeciales,
                    DiagnosticoFuncional: h.DiagnosticoFuncional,
                    Tratamiento: h.Tratamiento,
                    Recomendaciones: h.Recomendaciones
                ));
            }

            return resultadoJson;
        }

        // ACTUALIZAR
        public async Task<bool> ActualizarConsulta(string id, CrearConsultaFisDTO dto)
        {
            // Buscar la consulta existente por ID
            var consultaExistente = await _repository.ObtenerPorId(id);
            if (consultaExistente == null)
            {
                return false;
            }

            // Armamos el modelo con los nuevos datos manteniendo la estructura
            var modeloActualizado = new FisioterapiaM
            {
                Id = id, // Mantenemos el mismo ID
                MedicoId = consultaExistente.MedicoId, // Mantener MedicoId original
                Paciente = consultaExistente.Paciente, // Mantener Paciente original
                Especialidad = dto.Especialidad.ToUpper(),
                Entidad = dto.Entidad.ToUpper(),
                MotivoConsulta = dto.MotivoConsulta.ToUpper(),
                EvaluacionPostural = dto.EvaluacionPostural.ToUpper(),
                PruebasEspeciales = dto.PruebasEspeciales.ToUpper(),
                DiagnosticoFuncional = dto.DiagnosticoFuncional.ToUpper(),

                Antecedentes = dto.Antecedentes.Select(a => new Antecedente
                {
                    Tipo = a.Tipo.ToUpper(),
                    Observaciones = a.Observaciones
                }).ToList(),

                ArcosMovilidad = dto.ArcosMovilidad.Select(a => new ArcoMovilidad
                {
                    Articulacion = a.Articulacion.ToUpper(),
                    Movimiento = a.Movimiento.ToUpper(),
                    GradosObtenidos = a.GradosObtenidos
                }).ToList(),

                FuerzaMuscular = dto.FuerzaMuscular.Select(f => new EvaluacionFuerza
                {
                    GrupoMuscular = f.Musculo.ToUpper(),
                    EscalaDaniels = f.GradoFuerza
                }).ToList(),

                Tratamiento = dto.Tratamiento.Select(t => t.ToUpper()).ToList(),
                Recomendaciones = dto.Recomendaciones.Select(r => r.ToUpper()).ToList()
            };

            return await _repository.Actualizar(id, modeloActualizado);
        }

        // ELIMINAR
        public async Task<bool> EliminarConsulta(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _repository.Eliminar(id);
        }
    }
}
