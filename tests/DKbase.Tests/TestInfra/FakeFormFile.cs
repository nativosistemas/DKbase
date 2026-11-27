using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DKbase.Tests.TestInfra
{
    /// <summary>Archivo subido falso (límite externo: request HTTP multipart).</summary>
    public sealed class FakeFormFile : IFormFile
    {
        private readonly byte[] _contenido;
        public FakeFormFile(string nombre, byte[] contenido) { FileName = nombre; Name = "file"; _contenido = contenido; }
        public FakeFormFile(string nombre, string texto) : this(nombre, Encoding.UTF8.GetBytes(texto)) { }
        public string ContentType { get; set; } = "application/octet-stream";
        public string ContentDisposition { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public long Length => _contenido.Length;
        public string Name { get; }
        public string FileName { get; }
        public void CopyTo(Stream target) => target.Write(_contenido, 0, _contenido.Length);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) => target.WriteAsync(_contenido, 0, _contenido.Length, cancellationToken);
        public Stream OpenReadStream() => new MemoryStream(_contenido);
    }
}
