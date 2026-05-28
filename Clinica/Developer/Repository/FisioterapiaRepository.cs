using Clinica.Developer.Model;
using Clinica.Developer.Model.Fisioterapia;
using Clinica.Developer.Model.FisioterapiaMd;
using Clinica.Developer.Model.PsicologiaMd;
using MongoDB.Driver;
using MongoDB.Driver.Linq; // Agrega esta directiva using para IMongoQueryable

namespace Clinica.Developer.Repository
{
    public class FisioterapiaRepository
    {
        private readonly IMongoCollection<FisioterapiaM> _coleccion;
        private readonly IMongoDatabase _database;

        public FisioterapiaRepository(IMongoClient mongoClient, string databaseName)
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
            _coleccion = _database.GetCollection<FisioterapiaM>("fisioterapia");
            if (_coleccion == null)
            {
                throw new InvalidOperationException(
                    "No fue posible obtener la colección fisioterapia.");
            }
        }

        // Insertar
        public async Task Insertar(FisioterapiaM fisioterapia)
        {
            await _coleccion.InsertOneAsync(fisioterapia);
        }

        // Buscar por número de documento
        public async Task<List<FisioterapiaMedico>> ObtenerPorDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                throw new ArgumentException(
                    "El número de documento no puede estar vacío.",
                    nameof(numeroDocumento));
            }

            var coleccionMedicos =
                _database.GetCollection<MedicoConsulta>("PersonalMedico");

            return await _coleccion.Aggregate()
                .Match(h => h.Paciente.Documento.Numero == numeroDocumento)
                .Lookup<FisioterapiaM, MedicoConsulta, FisioterapiaMedico>(
                    coleccionMedicos,
                    h => h.MedicoId,
                    m => m.Id,
                    h => h.MedicoDetalle
                )
                .ToListAsync();
        }

        // Actualizar por ID
        public async Task<bool> Actualizar(string id, FisioterapiaM entidad)
        {
            var resultado = await _coleccion.ReplaceOneAsync(h => h.Id == id, entidad);
            return resultado.ModifiedCount > 0;
        }

        // Eliminar por ID
        public async Task<bool> Eliminar(string id)
        {
            var resultado = await _coleccion.DeleteOneAsync(h => h.Id == id);
            return resultado.DeletedCount > 0;
        }

        public async Task<FisioterapiaM> ObtenerPorId(string id)
        {
            return await _coleccion.Find(h => h.Id == id).FirstOrDefaultAsync();
        }
    
    }
}
