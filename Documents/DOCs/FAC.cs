using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.FAC
{
    public class infoProc
    {
        public string idEmpr { get; set; }
        public string idArea { get; set; }
        public string tipoDoc { get; set; }
        public string SerieDoc { get; set; }
        public string corrDoc { get; set; }
        public string signDoc { get; set; }
        public string asgCorr { get; set; }
        public string genXml { get; set; }
        public string saveDoc { get; set; }
    }

    public class infoAdic
    {
        public List<datosVariables> DatosVariables = new List<datosVariables>();
    }

    public class datosVariables
    {
        public string LineaAdicional { get; set; }
        public string ValorAdicional { get; set; }
    }

    public class infoDoc
    {
        //  Constructor
        public infoDoc(string tipo, string data)
        {
            //  Los datos concatenados en la variable "data", deben venir en el mismo orden en el que se construye en JSON
            string[] datosRelleno = Funciones.separarDatos(data);
            Tipo = tipo;

            if (tipo == "Encabezado")
            {
                //  38 elementos + campo "Tipo"
                LineaEncabezado = datosRelleno[0];
                TipoDocumento = datosRelleno[1];
                Prefijo = datosRelleno[2];
                Correlativo = datosRelleno[3];
                FechaEmision = datosRelleno[4];
                HoraEmision = datosRelleno[5];
                TipoOperacion = datosRelleno[6];
                TipoFactura = datosRelleno[7];
                MonedaDocumento = datosRelleno[8];
                periodoFechaInicio = datosRelleno[9];
                periodoHoraInicio = datosRelleno[10];
                periodoFechaFin = datosRelleno[11];
                periodoHoraFin = datosRelleno[12];
                NotaDocumento = datosRelleno[13];
                TipoEmisor = datosRelleno[14];
                TipoIdenEmisor = datosRelleno[15];
                IdentificacionEmisor = datosRelleno[16];
                DigitoVerificadorEmisor = datosRelleno[17];
                RegimenEmisor = datosRelleno[18];
                CodigoRespEmisor = datosRelleno[19];
                NomComerEmisor = datosRelleno[20];
                RSocApeEmisor = datosRelleno[21];
                TipoAdquirente = datosRelleno[22];
                TipoIdenAdquirente = datosRelleno[23];
                IdentificacionAdquirente = datosRelleno[24];
                DigitoVerificadorAdquiriente = datosRelleno[25];
                RegimenAdquirente = datosRelleno[26];
                CodigoRespAdquiriente = datosRelleno[27];
                NomComerAdquirente = datosRelleno[28];
                RSocApeAdquirente = datosRelleno[29];
                TipoReceptorPago = datosRelleno[30];
                TipoIdenReceptorPago = datosRelleno[31];
                IdentificacionReceptorPago = datosRelleno[32];
                NombreReceptorPago = datosRelleno[33];
                TotalBrutoDocumento = datosRelleno[34];
                BaseImponibleDocumento = datosRelleno[35];
                TotalBrutoDocumentoImpu = datosRelleno[36];
                TotalDocumento = datosRelleno[37];
                if (datosRelleno.Length >= 39) NombreAdquirente = datosRelleno[38];
                ActividadEconomicaEmisor = "9329";

            }
            else if (tipo == "Direccion")
            {

                LineaDireccion = datosRelleno[0];
                TipoDireccion = datosRelleno[1];
                IdDUNS = datosRelleno[2];
                ApartadoPostal = datosRelleno[3];
                Direccion = datosRelleno[4];
                Area = datosRelleno[5];
                Ciudad = datosRelleno[6];
                Departamento = datosRelleno[7];
                CodigoDepartamento = datosRelleno[8];
                CodigoPais = datosRelleno[9];
                NombrePais = datosRelleno[10];

            }
            else if (tipo == "Contacto")
            {
                LineaContacto = datosRelleno[0];
                TipoContacto = datosRelleno[1];
                NombreContacto = datosRelleno[2];
                TelefonoContacto = datosRelleno[3];
                MailContacto = datosRelleno[4];
                DescripcionContacto = datosRelleno[5];
            }
            else if (tipo == "Impuesto")
            {
                LineaImpuesto = datosRelleno[0];
                MonedaImpuesto = datosRelleno[1];
                TotalImpuesto = datosRelleno[2];
                IndicadorImpuesto = datosRelleno[3];
                BaseImponible = datosRelleno[4];
                PorcentajeImpuesto = datosRelleno[5];
                NumeroImpuesto = datosRelleno[6];
                NombreImpuesto = datosRelleno[7];
            }
            else if (tipo == "MedioPago")
            {
                LineaMedioPago = datosRelleno[0];
                IdMedioPago = datosRelleno[1];
                CodigoMedioPago = datosRelleno[2];
                FechaMedioPago = datosRelleno[3];
                IdentificadorPago = datosRelleno[4];
            }
            else if (tipo == "Item")
            {
                LineaItem = datosRelleno[0];
                DescripcionItem = datosRelleno[1];
                CantidadItem = datosRelleno[2];
                UnidadMedidaItem = datosRelleno[3];
                MonedaItem = datosRelleno[4];
                ValorUnitarioItem = datosRelleno[5];
                CostoTotalItem = datosRelleno[6];
                CodigoTipoPrecio = datosRelleno[7];
                MarcaItem = datosRelleno[8];
                ModeloItem = datosRelleno[9];
                NotaItem = datosRelleno[10];
            }
            else if (tipo == "ImpuestoItem")
            {
                LineaImpuestoItem = datosRelleno[0];
                MonedaImpuestoItem = datosRelleno[1];
                TotalImpuestoItem = datosRelleno[2];
                BaseImponibleItem = datosRelleno[3];
                PorcentajeImpuestoItem = datosRelleno[4];
                NumeroImpuestoItem = datosRelleno[5];
                NombreImpuestoItem = datosRelleno[6];
                UnidadMedidaImpItem = datosRelleno[7];
            }
            else if (tipo == "CdgItem")
            {
                LineaCodigo = datosRelleno[0];
                TipoCodigo = datosRelleno[1];
            }
            else if (tipo == "CorreoDespacho")
            {
                LineaCorreoDespacho = datosRelleno[0];
                MailPara = datosRelleno[1];
                MailCopia = null;
                MailOculto = null;
                MailTipo = datosRelleno[4];
            }

        }

        public string Tipo { get; set; }
        public string LineaEncabezado { get; set; }
        public string TipoDocumento { get; set; }
        public string Prefijo { get; set; }
        public string Correlativo { get; set; }
        public string FechaEmision { get; set; }
        public string HoraEmision { get; set; }
        public string TipoOperacion { get; set; }
        public string TipoFactura { get; set; }
        public string MonedaDocumento { get; set; }
        public string periodoFechaInicio { get; set; }//
        public string periodoHoraInicio { get; set; }//
        public string periodoFechaFin { get; set; }//
        public string periodoHoraFin { get; set; }//
        //public string CodigoIntervalo { get; set; }
        //public string ValorIntervalo { get; set; }
        public string NotaDocumento { get; set; }
        public string TipoEmisor { get; set; }
        public string TipoIdenEmisor { get; set; }
        public string IdentificacionEmisor { get; set; }
        public string DigitoVerificadorEmisor { get; set; }
        public string RegimenEmisor { get; set; }
        public string CodigoRespEmisor { get; set; }
        public string NomComerEmisor { get; set; }
        public string RSocApeEmisor { get; set; }
        public string TipoAdquirente { get; set; }
        public string TipoIdenAdquirente { get; set; }
        public string IdentificacionAdquirente { get; set; }
        public string DigitoVerificadorAdquiriente { get; set; }
        public string RegimenAdquirente { get; set; }
        public string CodigoRespAdquiriente { get; set; }
        public string NomComerAdquirente { get; set; }
        public string RSocApeAdquirente { get; set; }
        public string TipoReceptorPago { get; set; }
        public string TipoIdenReceptorPago { get; set; }
        public string IdentificacionReceptorPago { get; set; }
        public string NombreReceptorPago { get; set; }
        public string TotalBrutoDocumento { get; set; }
        public string BaseImponibleDocumento { get; set; }
        public string TotalBrutoDocumentoImpu { get; set; }
        public string TotalDocumento { get; set; }
        public string NombreAdquirente { get; set; }
        public string ActividadEconomicaEmisor { get; set; }

        public string MetodoPagoTransporte { get; set; }
        public string CondicionEntrega { get; set; }
        public string LineaDireccion { get; set; }
        public string TipoDireccion { get; set; }
        public string IdDUNS { get; set; }
        public string ApartadoPostal { get; set; }
        public string Direccion { get; set; }
        public string Area { get; set; }
        public string Ciudad { get; set; }
        public string Departamento { get; set; }
        public string CodigoDepartamento { get; set; }
        public string CodigoPais { get; set; }
        public string NombrePais { get; set; }
        public string LineaContacto { get; set; }
        public string TipoContacto { get; set; }
        public string NombreContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string MailContacto { get; set; }
        public string DescripcionContacto { get; set; }
        public string LineaImpuesto { get; set; }
        public string MonedaImpuesto { get; set; }
        public string TotalImpuesto { get; set; }
        public string IndicadorImpuesto { get; set; }
        public string BaseImponible { get; set; }
        public string PorcentajeImpuesto { get; set; }
        public string NumeroImpuesto { get; set; }
        public string NombreImpuesto { get; set; }
        public string LineaMedioPago { get; set; }
        public string IdMedioPago { get; set; }
        public string CodigoMedioPago { get; set; }
        public string FechaMedioPago { get; set; }
        public string IdentificadorPago { get; set; }
        public string LineaItem { get; set; }
        public string DescripcionItem { get; set; }
        public string CantidadItem { get; set; }
        public string UnidadMedidaItem { get; set; }
        public string MonedaItem { get; set; }
        public string ValorUnitarioItem { get; set; }
        public string CostoTotalItem { get; set; }
        public string CodigoTipoPrecio { get; set; }
        public string MarcaItem { get; set; }
        public string ModeloItem { get; set; }
        public string NotaItem { get; set; }
        public string LineaImpuestoItem { get; set; }
        public string MonedaImpuestoItem { get; set; }
        public string TotalImpuestoItem { get; set; }
        public string BaseImponibleItem { get; set; }
        public string PorcentajeImpuestoItem { get; set; }
        public string NumeroImpuestoItem { get; set; }
        public string NombreImpuestoItem { get; set; }
        public string UnidadMedidaImpItem { get; set; }
        public string LineaCodigo { get; set; }
        public string TipoCodigo { get; set; }
        public string LineaCorreoDespacho { get; set; }
        public string MailPara { get; set; }
        public string MailCopia { get; set; }
        public string MailOculto { get; set; }
        public string MailTipo { get; set; }
    }

    public class emisionDocumento
    {
        public string IdentificadorEmpresa { get; set; }
        public string UsuarioEmpresa { get; set; }
        public string Token { get; set; }
        public infoProc infoProc { get; set; }
        public infoAdic infoAdic { get; set; }
        public List<infoDoc> infoDoc { get; set; }


    }


    public class FAC
    {
        public emisionDocumento EmisionDocumento { get; set; }
    }
}
