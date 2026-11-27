using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DKbase.Business;
using DKbase.dll;
using DKbase.Entities;
using DKbase.Models;
using DKbase.Tests.TestInfra;
using DKbase.web;
using DKbase.web.capaDatos;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace DKbase.Tests.Modelos
{
    /// <summary>
    /// Constructores de copia y modelos simples. Los constructores de copia se caracterizan con un snapshot:
    /// se llena el origen con valores distintos por propiedad y se congela qué propiedades del destino se copian.
    /// Si en la refactorización se agrega o se pierde una propiedad copiada, el snapshot lo muestra.
    /// </summary>
    public class ModelosTests : TestBase
    {
        private static int _semilla;

        private static object ValorDistinto(Type t, string nombre)
        {
            var sub = Nullable.GetUnderlyingType(t) ?? t;
            int n = 100 + Math.Abs(nombre.GetHashCode() % 900) + (_semilla++);
            if (sub == typeof(string)) return "s_" + nombre;
            if (sub == typeof(int)) return n;
            if (sub == typeof(long)) return (long)n;
            if (sub == typeof(decimal)) return n + 0.25m;
            if (sub == typeof(double)) return n + 0.25d;
            if (sub == typeof(bool)) return true;
            if (sub == typeof(DateTime)) return new DateTime(2020, 1, 1).AddDays(n);
            if (sub.IsEnum) { var v = Enum.GetValues(sub); return v.GetValue(v.Length - 1); }
            if (sub.IsGenericType && sub.GetGenericTypeDefinition() == typeof(List<>)) return Activator.CreateInstance(sub);
            return null;
        }

        private static T Lleno<T>() where T : new()
        {
            _semilla = 0;
            var o = new T();
            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && p.GetIndexParameters().Length == 0))
            {
                var v = ValorDistinto(p.PropertyType, p.Name);
                if (v != null) p.SetValue(o, v);
            }
            return o;
        }

        private static SortedDictionary<string, string> Comparar(object origen, object destino)
        {
            var props = origen.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name);
            var r = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var p in destino.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0))
            {
                var vd = p.GetValue(destino);
                if (!props.TryGetValue(p.Name, out var po)) { r[p.Name] = "solo en destino"; continue; }
                var vo = po.GetValue(origen);
                r[p.Name] = vo == null ? "origen null"
                    : ReferenceEquals(vo, vd) && !(vo is ValueType) && !(vo is string) ? "misma referencia"
                    : Equals(vo, vd) ? "copiada"
                    : "NO copiada";
            }
            return r;
        }

        // CARACTERIZACIÓN: posible bug - new cProductosGenerico(cProductos) NO copia 15 propiedades del producto, entre ellas
        // pro_troquel, pro_isCadenaFrio, pro_requierevale, pro_canmaxima, pro_vtasolotransfer, pro_entransfer, pro_AltoCosto
        // y PrecioConDescuentoOferta (ver "NO copiada" en el snapshot).
        // CARACTERIZACIÓN: posible bug - CargarTransferYTransferDetalle no copia tde_DescripcionDeProducto ni tfr_provincia.
        [Fact]
        public Task deberia_mantener_las_propiedades_copiadas_por_los_constructores_de_copia()
        {
            var producto = Lleno<cProductos>();
            var transferDetalle = Lleno<cTransferDetalle>();
            var generico = new cProductosGenerico();
            generico.CargarTransferYTransferDetalle(transferDetalle);
            var buscador = Lleno<cjSonBuscadorProductos>();

            return Verifier.Verify(new
            {
                cProductosGenerico_desde_cProductos = Comparar(producto, new cProductosGenerico(producto)),
                cProductosGenerico_CargarTransferYTransferDetalle = Comparar(transferDetalle, generico),
                cjSonBuscadorProductos_copia = Comparar(buscador, new cjSonBuscadorProductos(buscador)),
            });
        }

        [Fact]
        public void deberia_crear_un_buscador_vacio_cuando_la_copia_recibe_null()
        {
            var r = new cjSonBuscadorProductos(null);
            r.listaProductos.Should().BeNull();
            r.listaSucursal.Should().BeNull();
            r.CantidadRegistroTotal.Should().Be(0);
        }

        [Fact]
        public void deberia_compartir_las_listas_cuando_copia_un_buscador()
        {
            // CARACTERIZACIÓN: posible bug - la "copia" es superficial: modificar la lista de la copia modifica el original.
            var original = new cjSonBuscadorProductos { listaSucursal = new List<string> { "CC" } };
            var copia = new cjSonBuscadorProductos(original);

            copia.listaSucursal.Add("RO");

            original.listaSucursal.Should().Equal("CC", "RO");
        }

        [Fact]
        public void deberia_inicializar_stock_vacio_y_producto_encontrado_cuando_se_crea_un_generico()
        {
            var a = new cProductosGenerico();
            var b = new cProductosGenerico(new cProductos());
            foreach (var g in new[] { a, b })
            {
                g.listaSucursalStocks.Should().NotBeNull().And.BeEmpty();
                g.isProductoNoEncontrado.Should().BeFalse();
            }
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_se_crea_un_generico_desde_un_producto_null()
        {
            FluentActions.Invoking(() => new cProductosGenerico((cProductos)null)).Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void deberia_asignar_campos_de_sucursal_cuando_se_usa_el_constructor_con_datos()
        {
            var s = new cSucursal(5, "CC", "RO");
            s.Should().BeEquivalentTo(new { sde_codigo = 5, sde_sucursal = "CC", sde_sucursalDependiente = "RO", suc_codigo = (string)null, suc_montoMinimo = 0m });
        }

        [Fact]
        public void deberia_usar_el_nombre_de_sucursal_como_nombre_del_envio_y_codigo_vacio()
        {
            var e = new cTiposEnvios(3, "Reparto");
            e.Should().BeEquivalentTo(new cTiposEnvios { env_id = 3, env_codigo = "", env_nombre = "Reparto" });
            new cTiposEnvios().env_codigo.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void deberia_tomar_usuario_cliente_y_token_cuando_se_arma_la_respuesta_de_autenticacion(bool conNombre)
        {
            var u = new User { usu_codigo = 7, cli_codigo = 1234, idRol = 3, login = "l" };

            var r = conNombre ? new AuthenticateResponse(u, "tok", "Perez, Ana") : new AuthenticateResponse(u, "tok");

            r.Should().BeEquivalentTo(new AuthenticateResponse { usu_codigo = 7, cli_codigo = 1234, Token = "tok", ApNombre = conNombre ? "Perez, Ana" : null });
        }

        [Fact]
        public void deberia_dejar_todo_en_default_cuando_la_respuesta_de_autenticacion_se_crea_vacia()
        {
            new AuthenticateResponse().Should().BeEquivalentTo(new { usu_codigo = 0, cli_codigo = 0, Token = (string)null, ApNombre = (string)null });
        }

        [Fact]
        public void deberia_lanzar_excepcion_cuando_la_respuesta_de_autenticacion_recibe_usuario_null()
        {
            FluentActions.Invoking(() => new AuthenticateResponse(null, "t")).Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void deberia_inicializar_resultado_en_menos_uno_y_sin_objeto()
        {
            var o = new object();
            new Resultado().Should().BeEquivalentTo(new { estado = -1, obj = (object)null });
            new Resultado(3).Should().BeEquivalentTo(new { estado = 3, obj = (object)null });
            new Resultado(4, o).obj.Should().BeSameAs(o);
        }

        [Fact]
        public void deberia_devolver_pedido_de_prueba_hola_mundo_cuando_se_usa_core_service()
        {
            // CARACTERIZACIÓN: posible bug - CoreService y CoreServiceTest son implementaciones de prueba ("Hola Mundo")
            // que viven en el código de producción e ignoran el request.
            ICoreService real = new CoreService();
            ICoreService prueba = new CoreServiceTest();

            var r1 = real.TomarPedidoConIdCarrito(null);
            var r2 = prueba.TomarPedidoConIdCarrito(new TomarPedidoConIdCarritoRequest { pIdCarrito = 5 });

            (r1.Error, r1.MensajeEnFactura).Should().Be(("Hola MundoUUUUU", "Hola MundoUUUUU"));
            (r2.Error, r2.MensajeEnFactura).Should().Be(("Hola MundoAAAA", "Hola MundoAAAAJJJ"));
        }

        [Fact]
        public void deberia_compartir_el_campo_entre_web_error_y_su_stack_trace()
        {
            // CARACTERIZACIÓN: posible bug - web_Error_StackTrace lee y escribe el mismo campo que web_Error.
            var p = new cDllPedido { web_Error = "a" };
            p.web_Error_StackTrace = "b";
            p.web_Error.Should().Be("b");
        }
    }
}
