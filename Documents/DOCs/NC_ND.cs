using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.DOCs
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

    public class infoDoc
    {
        public infoDoc(string tipo, string data)
        {
            //  Los datos concatenados en la variable "data", deben venir en el mismo orden en el que se construye en JSON
            string[] datosRelleno = Funciones.separarDatos(data);
            Tipo = tipo;

            if(tipo == "Encabezado")
            {
                LineaEncabezado = datosRelleno[0];
                TipoDocumento = datosRelleno[1];
                Prefijo = datosRelleno[2];
                Correlativo = datosRelleno[3];
                FechaEmision = datosRelleno[4];
                HoraEmision = datosRelleno[5];
                TipoOperacion = datosRelleno[6];
                TipoFactura = datosRelleno[7];
                MonedaDocumento = datosRelleno[8];
                NotaDocumento = datosRelleno[9];
                TipoEmisor = datosRelleno[10];
                TipoIdenEmisor = datosRelleno[11];
                IdentificacionEmisor = datosRelleno[12];
                DigitoVerificadorEmisor = datosRelleno[13];
                RegimenEmisor = datosRelleno[14];
                CodigoRespEmisor = datosRelleno[15];
                NomComerEmisor = datosRelleno[16];
                RSocApeEmisor = datosRelleno[17];
                TipoAdquirente = datosRelleno[18];
                TipoIdenAdquirente = datosRelleno[19];
                IdentificacionAdquirente = datosRelleno[20];
                DigitoVerificadorAdquiriente = datosRelleno[21];
                RegimenAdquirente = datosRelleno[22];
                CodigoRespAdquiriente = datosRelleno[23];
                NomComerAdquirente = datosRelleno[24];
                RSocApeAdquirente = datosRelleno[25];
                TipoReceptorPago = datosRelleno[26];
                TipoIdenReceptorPago = datosRelleno[27];
                IdentificacionReceptorPago = datosRelleno[28];
                NombreReceptorPago = datosRelleno[29];
                TotalBrutoDocumento = datosRelleno[30];
                BaseImponibleDocumento = datosRelleno[31];
                TotalBrutoDocumentoImpu = datosRelleno[32];
                TotalDocumento = datosRelleno[33];
                if(TipoAdquirente == "02" || TipoAdquirente == "2") NombreAdquirente = datosRelleno[34];
                ActividadEconomicaEmisor = "3600";
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
            else if (tipo == "Item")
            {
                LineaItem = datosRelleno[0];
                DescripcionItem = datosRelleno[1];
                CantidadItem = datosRelleno[2];
                UnidadMedidaItem = datosRelleno[3];
                MonedaItem = datosRelleno[4];
                CodigoTipoPrecio = datosRelleno[5];
                ValorUnitarioItem = datosRelleno[6];
                CostoTotalItem = datosRelleno[7];
                MarcaItem = datosRelleno[9];
                ModeloItem = datosRelleno[10];
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
            }
            else if (tipo == "Referencia")
            {
                LineaReferencia = datosRelleno[0];
                TipoReferencia = datosRelleno[1];
                NumeroDocReferencia = datosRelleno[2];
                FechaDocRefe = datosRelleno[3];
                TipoDocReferencia = datosRelleno[4];
            }
            else if (tipo == "CorreoDespacho")
            {
                LineaCorreoDespacho = datosRelleno[0];
                MailPara = datosRelleno[1];
                MailCopia = datosRelleno[2];
                MailOculto = datosRelleno[3];
                MailTipo = datosRelleno[4];
            }
            else if (tipo == "MedioPago")
            {
                LineaMedioPago = datosRelleno[0];
                IdMedioPago = datosRelleno[1];
                CodigoMedioPago = datosRelleno[2];
                FechaMedioPago = datosRelleno[3];
                IdentificadorPago = datosRelleno[4];
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
        public string CodigoTipoPrecio { get; set; }
        public string ValorUnitarioItem { get; set; }
        public string CostoTotalItem { get; set; }
        public string DatosTecnicosItem { get; set; }
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
        public string TipoIdentificacionMandante { get; set; }
        public string IdentificacionMandante { get; set; }
        public string DvMandante { get; set; }
        public string LineaReferencia { get; set; }
        public string TipoReferencia { get; set; }
        public string NumeroDocReferencia { get; set; }
        public string FechaDocRefe { get; set; }
        public string TipoDocReferencia { get; set; }
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
        public IList<infoDoc> infoDoc { get; set; }
    }

    public class NC_ND
    {
        public emisionDocumento EmisionDocumento { get; set; }
    }

}
