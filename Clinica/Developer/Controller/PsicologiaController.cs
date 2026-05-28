using Clinica.Developer.DTO.Psicologia;
using Clinica.Developer.Model.PsicologiaMd;
using Microsoft.AspNetCore.Mvc;
using Psicología.Developer.Service;

namespace Psicología.Developer.Controller
{
    [ApiController]
    [Route("/psicologia")]
    public class PsicologiaController : ControllerBase
    {
        private PsicologiaService _psicoService;
        private readonly ILogger<PsicologiaController> _logger;
        public PsicologiaController(PsicologiaService psicoService, ILogger<PsicologiaController> logger)
        {
            this._psicoService = psicoService;
            this._logger = logger;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarConsulta([FromBody] CrearConsultaPsiDTO consultaDTO)
        {
            try
            {
                if (consultaDTO == null)
                {
                    return BadRequest("El cuerpo de la petición no puede estar vacío.");
                }

                _logger.LogInformation($"Iniciando registro de consulta psicológica para paciente " +
                    $"con documento: {consultaDTO.Paciente.Documento.Numero}",

                    consultaDTO.Paciente.Documento.Numero);

                Psicologia? modelo = await _psicoService.RegistrarConsulta(consultaDTO);

                _logger.LogInformation("Datos recibidos: " + consultaDTO);
                return Ok(new { Documento = modelo.Paciente.Documento.Numero, Modelo = modelo });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError($"Error al registrar consulta: {ioe.Message}");
                return BadRequest(ioe.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error interno del servidor: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("obtener_todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                List<Psicologia> consultas = await _psicoService.ObtenerTodasLasConsultas();
                return Ok(consultas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("buscar/{numeroDocumento}")]
        public async Task<IActionResult> ObtenerPorDocumento(string numeroDocumento)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroDocumento))
                {
                    return BadRequest("El número de documento es obligatorio.");
                }

                List<HistoriaClinicaPsiDTO>? historial = 
                    await _psicoService.ObtenerHistorialPorDocumento(numeroDocumento);
                if (historial == null || historial.Count == 0)
                {
                    return NotFound($"No se encontraron historias clínicas para el paciente " +
                        $"con documento: {numeroDocumento}");
                }

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] CrearConsultaPsiDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID de la historia clínica es requerido en la URL.");
                }

                if (dto == null)
                {
                    return BadRequest("Los datos de actualización no pueden estar vacíos.");
                }

                bool actualizadoExitosamente = await _psicoService.ActualizarHistoria(id, dto);

                if (!actualizadoExitosamente)
                {
                    return NotFound($"No se encontró ninguna historia clínica con el ID: {id} para actualizar.");
                }

                return Ok(new { Mensaje = "Historia clínica actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarHistoria(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID de la historia clínica es requerido en la URL.");
                }

                bool eliminadoExitosamente = await _psicoService.EliminarHistoria(id);

                if (!eliminadoExitosamente)
                {
                    return NotFound($"No se encontró ninguna historia clínica con el ID: {id} para eliminar.");
                }

                return Ok(new { Mensaje = "Historia clínica eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}