using Clinica.Developer.DTO.OdontologiaDTO;
using Clinica.Developer.Model.OdontologoMd;
using Clinica.Developer.Service;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Developer.Controller
{
    [ApiController]
    [Route("/odontologia")]
    public class OdontologoControlller : ControllerBase
    {
        private readonly OdontologoService _service;

        public OdontologoControlller(OdontologoService service)
        {
            this._service = service;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarConsulta([FromBody] CrearConsultaOdoDTO consultaDTO)
        {
            try
            {
                if (consultaDTO == null)
                {
                    return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
                }

                Odontologo? resultado = await _service.RegistrarConsulta(consultaDTO);
                return Created("Consulta registrada correctamente.", resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la consulta: {ex.Message}");
            }
        }
        [HttpGet("buscar/{numeroDocumento}")]
        public async Task<IActionResult> ObtenerHistorialPorDocumento(string numeroDocumento)
        {
            if (string.IsNullOrEmpty(numeroDocumento))
            {
                return BadRequest("El número de documento es obligatorio.");
            }
            try
            {
                List<HistoriaClinicaOdoDTO>? historial = await _service.ObtenerHistorialPorDocumento(numeroDocumento);

                if (historial == null || historial.Count == 0)
                {
                    return NotFound($"No se encontraron historias clínicas para el paciente con documento: {numeroDocumento}");
                }

                return Ok(historial);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el historial: {ex.Message}");
            }
        }

        // Actualizar
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(string id, [FromBody] CrearConsultaOdoDTO consultaDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("El ID de la historia clínica es requerido en la URL.");
                }

                if (consultaDTO == null)
                {
                    return BadRequest("Los datos de actualización no pueden estar vacíos.");
                }

                bool actualizar = await _service.ActualizarHistoria(id, consultaDTO);

                if (!actualizar)
                {
                    return NotFound($"No se encontró la historia clínica con ID: {id}");
                }
                return Ok(new { Mensaje = "Historia clínica actualizada correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la historia clínica: {ex.Message}");
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

                bool eliminadoExitosamente = await _service.EliminarHistoria(id);

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