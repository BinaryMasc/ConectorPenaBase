using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ConfiguracionNS;
using WSConnect;
using System.IO;
using Errors;

namespace ConectorPenalisaFE
{
    public static class ConectBBDD
    {

        private static SqlConnection ConexionBBDD(Configuracion config)
        {
            string conexionString = "";

            if (config.Seguridad_Integrada_BBDD)
                conexionString = "Server=" + config.Server_BBDD + ";Integrated Security=True; Database=" + config.Nombre_BBDD;

            else
                conexionString = "Server=" + config.Server_BBDD + ";Database=" + config.Nombre_BBDD +
                    ";User Id=" + config.Usuario_BBDD + ";Password=" + config.Password_BBDD + ";";

            SqlConnection conexion = new SqlConnection(conexionString);

            conexion.Open();

            return conexion;
        }

        public static void ActualizarCamposBBDD(Configuracion config, List<WSConnect.Res> respuestas)
        {
            SqlConnection conexion = ConexionBBDD(config);

            for (int i = 0; i < respuestas.Count; i++)
            {
                if (respuestas[i].estado == EstadoRespuesta.RespuestaNull) continue;  //  Respuestas null se omiten para reintento

                else if (respuestas[i].estado == EstadoRespuesta.EnviadoConError)
                {
                    string respuesta = respuestas[i].respuesta["MensajeRespuesta"] != null ? respuestas[i].respuesta["MensajeRespuesta"] : respuestas[i].respuesta["Resultado"];

                    if (respuestas[i].tipodoc == "FE") ActualizarCampoBBDDFAC(config, conexion, "NOK", respuestas[i].correlativo, respuestas[i].prefijo);

                    else ActualizarCampoBBDDNotas(config, conexion, "NOK", respuestas[i].correlativo, respuestas[i].prefijo);


                    //  En caso de haber devuelto error, se registrará la información en el equipo

                    if (respuestas[i].tipodoc == "FE")
                    {
                        File.WriteAllText(config.RutaGuardadoRespuestasFAC + "FAC_" + respuestas[i].prefijo + respuestas[i].correlativo + ".txt", Base64Decode(respuesta));
                    }
                    else
                    {
                        File.WriteAllText(config.RutaGuardadoRespuestasNotas + "Note_" + respuestas[i].prefijo + respuestas[i].correlativo + ".txt", Base64Decode(respuesta));
                    }

                }
                else // Enviado con éxito
                {
                    if (respuestas[i].tipodoc == "FE") ActualizarCampoBBDDFAC(config, conexion, "OK", respuestas[i].correlativo, respuestas[i].prefijo);
                    else ActualizarCampoBBDDNotas(config, conexion, "OK", respuestas[i].correlativo, respuestas[i].prefijo);
                }
            }
        }


        //  FAC
        public static SqlDataReader HacerConsultaFAC(Configuracion config)
        {
            SqlConnection conexion = ConexionBBDD(config);

            string consultastring = "SELECT " + campos_VistaFAC + " FROM " + config.NombreVistaFAC + " WHERE ";


            using(SqlDataReader r = ObtenerNumerosdeFacturas(config, ref conexion))
            {
                int i = 0;
                if (!r.HasRows) return null;
                
                while(r.Read())
                {
                    string corr = r[0].ToString();
                    string pref = r[1].ToString();

                    if (i == 0) consultastring += "(Correlativo = '" + corr + "'";
                    else consultastring += " OR (Correlativo = '" + corr + "'";

                    consultastring += " AND Prefijo = '" + pref + "')";

                    i++;

                }
            }

            SqlCommand CMDConexion = new SqlCommand(consultastring, conexion);
            SqlDataReader consulta = CMDConexion.ExecuteReader();

            return consulta;
        }

        private static SqlDataReader ObtenerNumerosdeFacturas(Configuracion config, ref SqlConnection conexion)
        {
            string consultaString = "SELECT DISTINCT TOP 1 Correlativo, Prefijo From " + config.NombreVistaFAC + " WHERE Estado is null";

            SqlCommand CMDConexion = new SqlCommand(consultaString, conexion);
            SqlDataReader consulta = CMDConexion.ExecuteReader();

            return consulta;
        }

        private static void ActualizarCampoBBDDFAC(Configuracion config, SqlConnection conexion, string estado, string correlativo, string prefijo)
        {
            string command_Update = "UPDATE " + config.NombreVistaFAC + " SET Estado = '" + estado + "' WHERE Correlativo = " + correlativo + " AND Prefijo = '" + prefijo + "'";

            SqlCommand CMDConexion = new SqlCommand(command_Update, conexion);

            CMDConexion.ExecuteNonQuery();
        }


        //static string EX = "SELECT DISTINCT correlativo TOP 10 " + "FROM VISTAF WHERE Estado = ''";

        //  Notas

        public static SqlDataReader HacerConsultaNotasCredito(Configuracion config)
        {
            SqlConnection conexion = ConexionBBDD(config);

            string consultastring = "SELECT " + campos_VistaNotas + " FROM " + config.NombreVistaNotas + " WHERE ";


            using (SqlDataReader r = ObtenerNumerosdeNotas(config, ref conexion))
            {
                int i = 0;
                if (!r.HasRows) return null;

                while (r.Read())
                {
                    string corr = r[0].ToString();
                    string pref = r[1].ToString();

                    if (i == 0) consultastring += "(Correlativo = '" + corr + "'";
                    else consultastring += " OR (Correlativo = '" + corr + "'";

                    consultastring += " AND Prefijo = '" + pref + "')";

                    i++;

                }
            }

            SqlCommand CMDConexion = new SqlCommand(consultastring, conexion);
            SqlDataReader consulta = CMDConexion.ExecuteReader();

            return consulta;
        }

        private static SqlDataReader ObtenerNumerosdeNotas(Configuracion config, ref SqlConnection conexion)
        {
            string consultaString = "SELECT DISTINCT TOP 1 Correlativo, Prefijo From " + config.NombreVistaNotas + " WHERE Estado is null";

            SqlCommand CMDConexion = new SqlCommand(consultaString, conexion);
            SqlDataReader consulta = CMDConexion.ExecuteReader();

            return consulta;
        }

        private static void ActualizarCampoBBDDNotas(Configuracion config, SqlConnection conexion, string estado, string correlativo, string prefijo)
        {
            string command_Update = "UPDATE " + config.NombreVistaNotas + " SET Estado = '" + estado + "' WHERE Correlativo = " + correlativo + " AND Prefijo = '" + prefijo + "'";

            SqlCommand CMDConexion = new SqlCommand(command_Update, conexion);

            CMDConexion.ExecuteNonQuery();
        }

        //---

        public static string campos_VistaFAC = "TipoDocumento,Prefijo,Correlativo,FechaEmision,HoraEmision,TipoOperacion,TipoFactura,MonedaDocumento,periodoFechaInicio," +
                "periodoHoraInicio,periodoFechaFin,periodoHoraFin,NotaDocumento,TipoEmisor,TipoIdenEmisor,DigitoVerificadorEmisor," +
                "IdentificacionEmisor,RegimenEmisor,CodigoRespEmisor,NomComerEmisor,RSocApeEmisor,TipoAdquirente,TipoIdenAdquirente," +
                "IdentificacionAdquirente,DigitoVerificadorAdquiriente,RegimenAdquirente,CodigoRespAdquiriente,NomComerAdquirente," +
                "RSocApeAdquirente,TipoReceptorPago,TipoIdenReceptorPago,IdentificacionReceptorPago,NombreReceptorPago,TotalBrutoDocumento," +
                "BaseImponibleDocumento,TotalBrutoDocumentoImpu,TotalDocumento,NombreAdquiriente," +    //  Encabezado
                "TipoDireccion,IdDUNS,ApartadoPostal,Direccion,Area,Ciudad,Departamento,CodigoDepartamento,CodigoPais,NombrePais," +
                "TipoContacto,NombreContacto,TelefonoContacto,MailContacto,CodigoSocio,MonedaImpuesto,TotalImpuesto,IndicadorImpuesto,BaseImponible," +
                "PorcentajeImpuesto,NumeroImpuesto,NombreImpuesto,IdMedioPagos,CodigoMedioPago,FechaMedioPago,IdentificadorPago,LineaItem," +
                "DescripcionItem,CantidadItem,UnidadMedidaItem,MonedaItem,ValorUnitarioItem,CostoTotalItem,CodigoTipoPrecio,MarcaItem,ModeloItem," +
                "NotaItem,LineaImpuestoItem,MonedaImpuestoItem,TotalImpuestoItem,BaseImponibleItem,PorcentajeImpuestoItem,NumeroImpuestoItem," +
                "NombreImpuestoItem,UnidadMedidaImpItem," +
                "CodigoItem,BogItem,ReferenciaItem,DocumentoItem," +
                "Cargos1,Cargos2,Abonos,SubtotalCargos1,SubtotalCargos2,SubtotalAbonos,OtrosCargos,CargosDelMes,AbonosDelMes,ValorEnMora," +
                "PagueAntesDe,ValorAPagar,TipodeComprobante,TotalImpuesto2,IndicadorImpuesto2,BaseImponible2,PorcentajeImpuesto2,NumeroImpuesto2,NombreImpuesto2,DescuentoAntesDel15";


        public static string campos_VistaNotas =
            //  Encabezado (34 elementos) [0 - 33]
            "TipoDocumento,Prefijo,Correlativo,FechaEmision,HoraEmision,TipoOperacion,TipoFactura,MonedaDocumento,NotaDocumento,TipoEmisor,TipoIdenEmisor,IdentificacionEmisor,DigitoVerificadorEmisor,RegimenEmisor,CodigoRespEmisor,NomComerEmisor,RSocApeEmisor,TipoAdquirente,TipoIdenAdquirente,IdentificacionAdquirente,DigitoVerificadorAdquiriente,RegimenAdquirente,CodigoRespAdquiriente,NomComerAdquirente,RSocApeAdquirente,TipoReceptorPago,TipoIdenReceptorPago,IdentificacionReceptorPago,NombreReceptorPago,TotalBrutoDocumento,BaseImponibleDocumento,TotalBrutoDocumentoImpu,TotalDocumento,NombreAdquiriente," +
            //  Dirección (10 elementos) [34 - 43]
            "TipoDireccion,IdDUNS,ApartadoPostal,Direccion,Area,Ciudad,Departamento,CodigoDepartamento,CodigoPais,NombrePais," +
            //  Contacto (4 elementos) [44 - 47]
            "TipoContacto,NombreContacto,TelefonoContacto,MailContacto," +

            //  Impuesto (7 elementos) [48 - 54]
            "MonedaImpuesto,TotalImpuesto,IndicadorImpuesto,BaseImponible,PorcentajeImpuesto,NumeroImpuesto,NombreImpuesto," +

            //  Item (11 elementos) [55 - 65]
            "LineaItem,DescripcionItem,CantidadItem,UnidadMedidaItem,MonedaItem,CodigoTipoPrecio,ValorUnitarioItem,CostoTotalItem,MarcaItem,ModeloItem,NotaItem," +

            // ImpuestoItem (7 elementos) [66 - 72]
            "LineaImpuestoItem, MonedaImpuestoItem, TotalImpuestoItem, BaseImponibleItem, PorcentajeImpuestoItem, NumeroImpuestoItem,NombreImpuestoItem,"+
            
            //  Referencia (3 elementos) [73 - 75]
            "PrefijoFacturaAfectada,NumeroFacturaAfectada,FechaEmisionFacturaAfectada," +

            //  MedioPago (4 elementos) [76 - 79]
            "IdMedioPagos, CodigoMedioPago, FechaMedioPago, IdentificadorPago," +

            //  Impuesto2 (6 elementos) [80 - 86]
            "TotalImpuesto2, IndicadorImpuesto2, BaseImponible2, PorcentajeImpuesto2, NumeroImpuesto2, NombreImpuesto2";


        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
