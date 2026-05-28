using Clinica.Developer.Model;
using Clinica.Developer.Model.OdontologoMd;
using Clinica.Developer.Model.PsicologiaMd;
using MongoDB.Driver;

namespace Clinica.Developer.Repository
{
    public class OdontologoRepository
    {
        private readonly IMongoCollection<Odontologo> _coleccion;
        private readonly IMongoDatabase _database;

        public OdontologoRepository(IMongoClient mongoClient, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(
                    "El nombre de la base de datos no puede estar vacío.",
                    nameof(databaseName));
            }
            _database = mongoClient.GetDatabase(databaseName);
            _coleccion = _database.GetCollection<Odontologo>("Odontologia");
            if (_coleccion == null)
            {
                throw new InvalidOperationException(
                    "No fue posible obtener la colección Odontologia.");
            }

        }

        // Insertar
        public async Task Insertar(Odontologo odontologo)
        {
            try
            {
                await _coleccion.InsertOneAsync(odontologo);

            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al insertar la historia clinica: " + ex.Message, ex);
            }
        }

        // Obtener por ID
        public async Task<List<OdontologoMedico>> ObtenerPorDocumento(string numeroDocumento)
        {
            if(string.IsNullOrWhiteSpace(numeroDocumento))
            {
                throw new ArgumentException(
                    "La cédula no puede estar vacía.", nameof(numeroDocumento));
            }

            var coleccionMedicos = _database.GetCollection<MedicoConsulta>("PersonalMedico");

            return await _coleccion.Aggregate()
                .Match(h => h.Paciente.Documento.Numero == numeroDocumento)
                .Lookup<Odontologo, MedicoConsulta, OdontologoMedico>(
                    coleccionMedicos,
                    h => h.MedicoId,
                    m => m.Id,
                    resultado => resultado.MedicoDetalle
                )
                .ToListAsync();
        }

        // Actualizar
        public async Task<bool> Actualizar(string id, Odontologo historiaActualizada)
        {
            var filtro = Builders<Odontologo>.Filter.Eq(h => h.Id, id);
            var resultado = await _coleccion.ReplaceOneAsync(filtro, historiaActualizada);
            return resultado.ModifiedCount > 0;
        }

        // Eliminar por ID
        public async Task<bool> Eliminar(string id)
        {
            var filtro = Builders<Odontologo>.Filter.Eq(h => h.Id, id);
            var resultado = await _coleccion.DeleteOneAsync(filtro);
            return resultado.DeletedCount > 0;
        }
    }
}
