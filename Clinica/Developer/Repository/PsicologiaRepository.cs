using Clinica.Developer.Model;
using Clinica.Developer.Model.PsicologiaMd;
using MongoDB.Driver;

namespace Psicología.Developer.Repository
{
    public class PsicologiaRepository
    {
        private readonly IMongoCollection<Psicologia> _coleccion;
        private readonly IMongoDatabase _database;

        public PsicologiaRepository(IMongoClient mongoClient, string databaseName)
        {
            if (mongoClient == null)
            {
                throw new ArgumentNullException(nameof(mongoClient),
                    "El cliente de MongoDB no fue configurado.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(
                    "El nombre de la base de datos no puede estar vacío.",
                    nameof(databaseName));
            }

            _database = mongoClient.GetDatabase(databaseName);

            _coleccion = _database.GetCollection<Psicologia>("psicologia");

            if (_coleccion == null)
            {
                throw new InvalidOperationException(
                    "No fue posible obtener la colección psicologia.");
            }
        }

        // Insertar
        public async Task Insertar(Psicologia psicologia)
        {
            // Inserta el registro de psicología en la colección unificada
            await _coleccion.InsertOneAsync(psicologia);
        }

        // Obtener todos
        public async Task<List<Psicologia>> ObtenerTodos()
        {
            // El filtro h => true le dice a Mongo que traiga todo sin restricciones
            return await _coleccion.Find(h => true).ToListAsync();
        }

        // Obtener por número de documento
        public async Task<List<PsicologiaMedico>> ObtenerPorDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                throw new ArgumentException(
                    "El número de documento no puede estar vacío.",
                    nameof(numeroDocumento));
            }

            var coleccionMedicos = _database.GetCollection<MedicoConsulta>("PersonalMedico");

            return await _coleccion.Aggregate()
                .Match(h => h.Paciente.Documento.Numero == numeroDocumento)
                .Lookup<Psicologia, MedicoConsulta, PsicologiaMedico>(
                    coleccionMedicos,
                    h => h.MedicoId,
                    m => m.Id,
                    resultado => resultado.MedicoDetalle
                )
                .ToListAsync();
        }
        public async Task<bool> Actualizar(string id, Psicologia historiaActualizada)
        {
            var filtro = Builders<Psicologia>.Filter.Eq(h => h.Id, id);
            var resultado = await _coleccion.ReplaceOneAsync(filtro, historiaActualizada);
            return resultado.ModifiedCount > 0;
        }
        // Eliminar por ID
        public async Task<bool> Eliminar(string id)
        {
            var filtro = Builders<Psicologia>.Filter.Eq(h => h.Id, id);
            var resultado = await _coleccion.DeleteOneAsync(filtro);
            return resultado.DeletedCount > 0;
        }
    }
}
