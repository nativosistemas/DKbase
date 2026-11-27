using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DKbase.Tests.TestInfra
{
    public sealed class HttpLlamada
    {
        public HttpMethod Metodo { get; init; }
        public string Url { get; init; }
        public string Cuerpo { get; init; }
        public string ContentType { get; init; }
        public string Authorization { get; init; }
        public override string ToString() => $"{Metodo} {Url} {Cuerpo}";
    }

    /// <summary>
    /// Handler HTTP falso. Se instala en el HttpClient estático privado de capaAPI por reflexión
    /// (reemplazando el handler interno de HttpMessageInvoker). Sin red.
    /// </summary>
    public sealed class FakeHttpHandler : HttpMessageHandler
    {
        public static FakeHttpHandler Actual { get; private set; }
        public List<HttpLlamada> Llamadas { get; } = new List<HttpLlamada>();
        private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _cola = new();
        public Func<HttpRequestMessage, HttpResponseMessage> PorDefecto { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.NotFound);

        public FakeHttpHandler Responder(Func<HttpRequestMessage, HttpResponseMessage> f) { _cola.Enqueue(f); return this; }
        public FakeHttpHandler ResponderJson(string json, HttpStatusCode status = HttpStatusCode.OK)
            => Responder(_ => new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
        public FakeHttpHandler ResponderStatus(HttpStatusCode status) => Responder(_ => new HttpResponseMessage(status));
        public FakeHttpHandler ResponderExcepcion(Exception ex) => Responder(_ => throw ex);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string cuerpo = request.Content == null ? null : await request.Content.ReadAsStringAsync();
            Llamadas.Add(new HttpLlamada
            {
                Metodo = request.Method,
                Url = request.RequestUri?.ToString(),
                Cuerpo = cuerpo,
                ContentType = request.Content?.Headers.ContentType?.MediaType,
                Authorization = request.Headers.Authorization?.ToString(),
            });
            var f = _cola.Count > 0 ? _cola.Dequeue() : PorDefecto;
            return f(request);
        }

        public static FakeHttpHandler InstalarEnCapaApi()
        {
            var campoClient = typeof(DKbase.web.capaDatos.capaAPI).GetField("client", BindingFlags.NonPublic | BindingFlags.Static);
            var client = (HttpClient)campoClient.GetValue(null);
            var campoHandler = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);
            var h = new FakeHttpHandler();
            campoHandler.SetValue(client, h);
            client.DefaultRequestHeaders.Authorization = null;
            Actual = h;
            return h;
        }
    }
}
