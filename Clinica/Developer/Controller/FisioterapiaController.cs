using Clinica.Developer.DTO.Fisioterapia;
using Clinica.Developer.DTO.FisioterapiaDTO;
using Clinica.Developer.Model.FisioterapiaMd;
using Clinica.Developer.Service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Clinica.Developer.Controller
{
    [ApiController]
    [Route("/fisioterapia")]
    public class FisioterapiaController : ControllerBase
    {
        private FisioterapiaService _fisioService;
        private readonly ILogger<FisioterapiaController> _logger;


        public FisioterapiaController(FisioterapiaService fisioService, ILogger<FisioterapiaController> logger)
        {
            this._fisioService = fisioService;
            _logger = logger;
        }

        // Registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarConsulta([FromBody] DTO.Fisioterapia.CrearConsultaFisDTO crear)
        {
            try
            {
                if (crear == null)
                {
                    return BadRequest("El cuerpo de la petición no puede estar vacío.");
                }
                _logger.LogInformation($"Iniciando registro de consulta de fisioterapia para paciente " +   
                    $"con documento: {crear.Paciente.Documento.Numero}"); 

                var modelo = await _fisioService.RegistrarConsulta(crear);

                _logger.LogInformation($"Datos ingresados: {crear}");
                return Ok(new { NumeroDocumento = modelo.Paciente.Documento.Numero, Modelo = modelo });
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

        // Buscar por número de documento
        [HttpGet("buscar/{numeroDocumento}")]
        public async Task<IActionResult> ObtenerPorDocumento(string numeroDocumento)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroDocumento))
                {
                    return BadRequest("El número de documento es obligatorio.");
                }
                List<HistorialClinicoFisDTO> historial =
                    await _fisioService.ObtenerPorDocumento(numeroDocumento);

                if (historial == null || historial.Count == 0)
                {
                    return NotFound($"No se encontraron consultas de fisioterapia para el número de documento: {numeroDocumento}");
                }
                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Actualizar consulta
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] CrearConsultaFisDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("El cuerpo de la petición no puede estar vacío.");
                }
                bool actualizado = await _fisioService.ActualizarConsulta(id, dto);
                if (!actualizado)
                {
                    return NotFound($"No se encontró una consulta con ID: {id} para actualizar.");
                }
                return Ok(new { mensaje = $"Consulta con ID: {id} actualizada exitosamente." });
            }
            catch (MongoException mex)
            {
                return StatusCode(500, $"Error al actualizar la consulta en la base de datos: {mex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Eliminar consulta
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID de la consulta es obligatorio.");
                }
                bool eliminado = await _fisioService.EliminarConsulta(id);
                if (!eliminado)
                {
                    return NotFound($"No se encontró una consulta con ID: {id} para eliminar.");
                }
                return Ok($"Consulta con ID: {id} eliminada exitosamente.");
            }
            catch (MongoException mex)
            {
                return StatusCode(500, $"Error al eliminar la consulta en la base de datos: {mex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
