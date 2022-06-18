using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DKbase.dll
{
    public enum dllEstadosPedido
    {
        Anulado = 0,
        EnPreparacion = 1,
        EnSucursal = 2,
        Enviado = 3,
        PendienteDeFacturar = 4,
        Detenido = 5
    }
    public enum dllEstadoCheque
    {
        Aceptado = 0,
        Cambiado = 1,
        Depositado = 2,
        EnCartera = 3,
        Rechazado = 4,
        Retirado = 5
    }
    public enum dllTipoComprobante
    {
        NN = -1,
        FAC = 0,
        REC = 1,
        NDE = 2,
        NCR = 3,
        NDI = 4,
        NCI = 5,
        RES = 9,
        CIE = 10,
        OSC = 13
    }

    public enum dllMotivoDevolucion
    {
        BienFacturadoMalEnviado = 1,
        ProductoMalEstado = 2,
        FacturadoNoPedido = 3,
        ProductoDeMasSinSerFacturado = 4,
        VencimientoCorto = 5,
        ProductoFallaFabricante = 6,
        Vencido = 7,
        PedidoPorError = 8
    }

    public class cResumen
    {
        public string Numero { get; set; }
        public string NumeroSemana { get; set; }
        public DateTime? PeriodoDesde { get; set; }
        public string PeriodoDesdeToString { get; set; }
        public DateTime? PeriodoHasta { get; set; }
        public string PeriodoHastaToString { get; set; }
        public decimal? TotalResumen { get; set; }
        public List<cResumenDetalle> lista { get; set; }
    }
    public class cResumenDetalle
    {
        public string Descripcion { get; set; }
        public string Dia { get; set; }
        public string Importe { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string NumeroResumen { get; set; }
        public string TipoComprobante { get; set; }
        //public string TipoComprobanteToString { get; set; }
    }
    public class cCtaCteMovimiento
    {
        public string Atraso { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public DateTime? FechaPago { get; set; }
        public string FechaPagoToString { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoToString { get; set; }
        public decimal? Importe { get; set; }
        public string MedioPago { get; set; }
        public string NumeroComprobante { get; set; }
        public string NumeroRecibo { get; set; }
        public string Pago { get; set; }
        public decimal? Saldo { get; set; }
        public string Semana { get; set; }
        public dllTipoComprobante TipoComprobante { get; set; }
        public string TipoComprobanteToString { get; set; }
        public List<cVencimientoResumen> lista { get; set; }
    }

    public class cVencimientoResumen
    {
        public string Tipo { get; set; }
        public string NumeroComprobante { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoToString { get; set; }
        public double? Importe { get; set; }
    }

    public class cFichaCtaCte
    {
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        //public dkInterfaceWeb.TiposComprobante TipoComprobante { get; set; }
        public dllTipoComprobante TipoComprobante { get; set; }
        public string TipoComprobanteToString { get; set; }
        public string Comprobante { get; set; }
        public string Motivo { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoToString { get; set; }
        public decimal? Debe { get; set; }
        public decimal? Haber { get; set; }
        public decimal? Saldo { get; set; }
    }
    public class cComprobanteDiscriminado
    {
        public string Comprobante { get; set; }
        public string Destinatario { get; set; }
        public string DetallePercepciones { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoGravado { get; set; }
        public decimal MontoIvaInscripto { get; set; }
        public decimal MontoIvaNoInscripto { get; set; }
        public decimal MontoPercepcionDGR { get; set; }
        public decimal MontoTotal { get; set; }
        public string NumeroComprobante { get; set; }
    }
    public class cNotaDeDebito
    {
        public string CantidadHojas { get; set; }
        public string Destinatario { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoGravado { get; set; }
        public decimal MontoIvaInscripto { get; set; }
        public decimal MontoIvaNoInscripto { get; set; }
        public decimal MontoPercepcionDGR { get; set; }
        public decimal MontoTotal { get; set; }
        public string Motivo { get; set; }
        public string Numero { get; set; }
        public List<cNotaDeDebitoDetalle> lista { get; set; }
    }
    public class cNotaDeDebitoDetalle
    {
        public string Descripcion { get; set; }
        public string Importe { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string NumeroNotaDeDebito { get; set; }

    }
    public class cRecibo
    {
        public string Numero { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public string FechaAnulacion { get; set; }
        public string Destinatario { get; set; }
        public string DireccionDestinatario { get; set; }
        public string LocalidadDestinatario { get; set; }
        public string CondicionIVADestinatarioToString { get; set; }
        public double CuitDestinatario { get; set; }
        public int NumeroCliente { get; set; }
        public int NumeroCuentaCorriente { get; set; }
        public string TipoEnvioToString { get; set; }
        public string CodigoReparto { get; set; }
        public decimal MontoTotal { get; set; }
        public string MontoTOTALenLetras { get; set; }
        public string CantidadHojas { get; set; }
        public string MontoEnDolares { get; set; }
        public bool ComprobantePAMI { get; set; }
        public List<cReciboDetalle> lista { get; set; }
    }
    public class cReciboDetalle
    {
        public string NumeroRecibo { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string Descripcion { get; set; }
        public string Importe { get; set; }
        public string ID { get; set; }

    }
    public class cNotaDeCredito
    {
        public string CantidadHojas { get; set; }
        public string Destinatario { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoGravado { get; set; }
        public decimal MontoIvaInscripto { get; set; }
        public decimal MontoIvaNoInscripto { get; set; }
        public decimal MontoPercepcionDGR { get; set; }
        public decimal MontoTotal { get; set; }
        public string Motivo { get; set; }
        public string Numero { get; set; }
        public int TotalUnidades { get; set; }
        public List<cNotaDeCreditoDetalle> lista { get; set; }
    }
    public class cNotaDeCreditoDetalle
    {
        public string Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string Importe { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string NumeroNotaDeCredito { get; set; }
        public string PrecioPublico { get; set; }
        public string PrecioUnitario { get; set; }
        public string Troquel { get; set; }

    }
    public class cFactura
    {
        public string CantidadHojas { get; set; }
        public int CantidadRenglones { get; set; }
        public string CodigoFormaDePago { get; set; }
        public decimal DescuentoEspecial { get; set; }
        public decimal DescuentoNetos { get; set; }
        public decimal DescuentoPerfumeria { get; set; }
        public decimal DescuentoWeb { get; set; }
        public string Destinatario { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoGravado { get; set; }
        public decimal MontoIvaInscripto { get; set; }
        public decimal MontoIvaNoInscripto { get; set; }
        public decimal MontoPercepcionDGR { get; set; }
        public decimal MontoTotal { get; set; }
        public string Numero { get; set; }
        public int NumeroCuentaCorriente { get; set; }
        public string NumeroRemito { get; set; }
        public int TotalUnidades { get; set; }
        public decimal MontoPercepcionMunicipal { get; set; }
        public bool? FacturaTrazable { get; set; }
        public List<cFacturaDetalle> lista { get; set; }
    }
    public class cFacturaDetalle
    {
        public string Cantidad { get; set; }
        public string Caracteristica { get; set; }
        public string Descripcion { get; set; }
        public string Importe { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string PrecioPublico { get; set; }
        public string PrecioUnitario { get; set; }
        public string Troquel { get; set; }

    }
    public class Resultado
    {
        public Resultado()
        {
            estado = -1;
            obj = null;
        }
        public Resultado(int pEstado)
        {
            estado = pEstado;
            obj = null;
        }
        public Resultado(int pEstado, Object pObj)
        {
            estado = pEstado;
            obj = pObj;
        }
        public int estado { get; set; }
        public object obj { get; set; }
    }
    public class cDllPedidoItem
    {
        public int Cantidad { get; set; }
        public string Caracteristica { get; set; }
        public int Faltas { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string FechaIngresoToString { get; set; }
        public string NombreObjetoComercial { get; set; }
    }

    public class cDllProductosAndCantidad
    {
        public string codProductoNombre { get; set; }
        public int IdTransfer { get; set; }
        public int cantidad { get; set; }
        public bool isOferta { get; set; }
    }
    public class cDllPedido
    {
        public string web_Sucursal { get; set; }
        public int CantidadRenglones { get; set; }
        public int CantidadUnidad { get; set; }
        public string Error { get; set; }
        public dllEstadosPedido Estado { get; set; }
        public string EstadoToString { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string FechaIngresoToString { get; set; }
        public string FechaIngresoHoraToString { get; set; }
        public List<cDllPedidoItem> Items { get; set; }
        public List<cDllPedidoItem> ItemsConProblemasDeCreditos { get; set; }
        public string Login { get; set; }
        public string MensajeEnFactura { get; set; }
        public string MensajeEnRemito { get; set; }
        public decimal MontoTotal { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroRemito { get; set; }
        public bool Cancelado { get; set; }
        public string DetalleSucursal { get; set; }
        public decimal CreditoInicial { get; set; }
    }
    public class cDllPedidoTransfer
    {
        public string web_Sucursal { get; set; }
        public int CantidadRenglones { get; set; }
        public int CantidadUnidad { get; set; }
        public string Error { get; set; }
        public dllEstadosPedido Estado { get; set; }
        public string EstadoToString { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string FechaIngresoToString { get; set; }
        public string FechaIngresoHoraToString { get; set; }
        public List<cDllPedidoItem> Items { get; set; }
        public List<cDllPedidoItem> ItemsConProblemasDeCreditos { get; set; }
        public string Login { get; set; }
        public string MensajeEnFactura { get; set; }
        public string MensajeEnRemito { get; set; }
        public decimal MontoTotal { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroRemito { get; set; }
        public bool Cancelado { get; set; }
        public decimal CreditoInicial { get; set; }
    }
    public class cDllCtaResumenMovimiento
    {
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        public dllTipoComprobante TipoComprobante { get; set; }
        public string TipoComprobanteToString { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal Importe { get; set; }

    }
    public class cDllRespuestaResumenAbierto
    {
        public List<cDllCtaResumenMovimiento> lista { get; set; }
        public bool isPoseeCuenta { get; set; }
        public decimal ImporteTotal { get; set; }
    }
    public class cDllChequeRecibido
    {
        public string Banco { get; set; }
        public dllEstadoCheque Estado { get; set; }
        public string EstadoToString { get; set; }
        public string Fecha { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoToString { get; set; }
        public decimal Importe { get; set; }
        public string Numero { get; set; }
    }
    public class cDllSaldosComposicion
    {
        public decimal? SaldoTotal { get; set; }
        public decimal? SaldoCtaCte { get; set; }
        public decimal? SaldoResumenAbierto { get; set; }
        public decimal? SaldoChequeCartera { get; set; }
        public bool isPoseeCuentaResumen { get; set; }
    }
    public class cComprobantesDiscriminadosDePuntoDeVenta
    {
        public string Comprobante { get; set; }
        public string Destinatario { get; set; }
        //lista.get_Item(0).DetallePercepciones;
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoExento { get; set; }
        public decimal MontoGravado { get; set; }
        public decimal MontoIvaInscripto { get; set; }
        public decimal MontoIvaNoInscripto { get; set; }
        public decimal MontoPercepcionesDGR { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoPercepcionesMunicipal { get; set; }
        public string NumeroComprobante { get; set; }

    }
    public class cPlan
    {
        public string Nombre { get; set; }
        public bool PideSemana { get; set; }
    }
    public class cPlanillaObSoc
    {
        public string Anio { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal Importe { get; set; }
        public string Mes { get; set; }
        public string Quincena { get; set; }
        public string Semana { get; set; }

    }
    public class cObraSocialCliente
    {
        public string CantidadHojas { get; set; }
        public string Destinatario { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaToString { get; set; }
        public decimal MontoTotal { get; set; }
        public string NombrePlan { get; set; }
        public int NumeroPlanilla { get; set; }
        //public string NumeroObraSocialCliente { get; set; }
        public List<cObraSocialClienteItem> lista { get; set; }

    }
    public class cObraSocialClienteItem
    {
        public string Descripcion { get; set; }
        public string Importe { get; set; }
        public string NumeroHoja { get; set; }
        public int NumeroItem { get; set; }
        public string NumeroObraSocialCliente { get; set; }
    }
    public class cCbteParaImprimir
    {
        public DateTime FechaComprobante { get; set; }
        public string FechaComprobanteToString { get; set; }
        public string NumeroComprobante { get; set; }
        public string TipoComprobante { get; set; }
    }
    public class cConsObraSocial
    {
        public string Detalle { get; set; }
        public DateTime FechaComprobante { get; set; }
        public string FechaComprobanteToString { get; set; }
        public decimal Importe { get; set; }
        public string NumeroComprobante { get; set; }
        public string TipoComprobante { get; set; }
    }

    public class cLote
    {
        public string ID { get; set; }
        public string NombreProducto { get; set; }
        public string NumeroLote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string FechaVencimientoToString { get; set; }
    }

    public class cDevolucionItemPrecarga
    {
        public int dev_numeroitem { get; set; }
        public int dev_numerocliente { get; set; }
        public string dev_numerofactura { get; set; }
        public string dev_numerosolicituddevolucion { get; set; }
        public string dev_nombreproductodevolucion { get; set; }
        public DateTime dev_fecha { get; set; }
        public string dev_fechaToString { get; set; }
        public dllMotivoDevolucion dev_motivo { get; set; }
        public int dev_numeroitemfactura { get; set; }
        public string dev_nombreproductofactura { get; set; }
        public double dev_cantidad { get; set; }
        public string dev_numerolote { get; set; }
        public DateTime dev_fechavencimientolote { get; set; }
        public string dev_fechavencimientoloteToString { get; set; }
        public string dev_estado { get; set; }
        public string dev_mensaje { get; set; }
        public double dev_cantidadrecibida { get; set; }
        public double dev_cantidadrechazada { get; set; }
        public string dev_idsucursal { get; set; }
        public string dev_numerosolicitudNC { get; set; }
    }
}
