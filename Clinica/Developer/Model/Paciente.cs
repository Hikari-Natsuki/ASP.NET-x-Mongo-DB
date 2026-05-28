using Clinica.Developer.Model.General;

namespace Clinica.Developer.Model
{
    public class Paciente
    {
        public string Nombre { get; set; }
        public DocumentoIdentidad Documento { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; }
    }
}
