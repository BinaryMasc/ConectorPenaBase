using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Documents;
using WSConnect;
using ConfiguracionNS;


namespace ConectorPenalisaFE
{
    public static class Agente
    {
        static bool debug = false;

        public static void EjecutarFac(Configuracion config, ref EventLog eventos)
        {
            if(debug) eventos.WriteEntry("DEBUG: Entrando a EjecutarFacturas");

            //  Nos conectamos a la BBDD y verificamos que haya alguna fila por extraer
            SqlDataReader consulta;

            try
            {
                consulta = ConectBBDD.HacerConsultaFAC(config);
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 101 al hacer consulta a la base de datos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);

                return;
            }

            if (consulta == null)
            {
                //eventos.WriteEntry("Error interno 111 al hacer consulta a la base de datos. La consulta devolvió NULL: Verifique su configuración.", EventLogEntryType.Error);
                if (debug) eventos.WriteEntry("DEBUG: Saliendo de iteración por consulta = null (Facturas)");
                return;
            }


            if (!consulta.HasRows) return;

            //---


            // Pedimos el Token a DBNet para construir los documentos:
            string token = "";
            WSConnectDBNet wSConnectDBNet = new WSConnectDBNet(config.URLWSDBNet);

            try
            {
                token = wSConnectDBNet.ObtenerToken(config.idEmpresaDBNet, config.UserEmpresaDBNet, config.AutorizacionEmpresaDBNet);
            }
            catch (Exception e)
            {

                eventos.WriteEntry("Error interno 102 al obtener el token: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                return;
            }

            //---


            List<Documents.FAC.FAC> Lista_FACs = new List<Documents.FAC.FAC>();
            List<Documents.DOCs.NC_ND> Lista_Notas = new List<Documents.DOCs.NC_ND>();

            Lista_FACs = LlenarDocumentos.ConstruirDocumentosConsultaFAC(consulta, config, token);
            //Lista_Notas = LlenarDocumentos.ConstruirDocumentosConsultaNotas(consulta, config, token);


            //  Empezamos a enviar los documentos

            Hilos hilos_Agente = new Hilos(Lista_FACs, Lista_Notas, config.URLWSDBNet, eventos);

            try
            {
                hilos_Agente.EjecutarEnSerie(eventos);  //  Enviamos los documentos
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 103 al enviar documentos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                return;
            }


            //---

            try
            {
                ConectBBDD.ActualizarCamposBBDD(config, WSConnectDBNet.respuestas);  //  Actualizamos la info en la base de datos
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 104 al actualizar campos de la base de datos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                WSConnectDBNet.respuestas.Clear();
                return;
            }

            WSConnectDBNet.respuestas.Clear();

            //---

            if (config.GenerarJsonDocumentosConector)
            {
                try
                {
                    Funciones.GuardarDocumentos(config.RutaGuardadoJsonDocumentosFACConector, config.RutaGuardadoJsonDocumentosNOTASConector,
                                                Lista_FACs, Lista_Notas);
                }
                catch (Exception e)
                {
                    eventos.WriteEntry("Error interno 121 al guardar documentos en el equipo: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                    return;
                }
            }
            if(debug) eventos.WriteEntry("DEBUG: saliendo de EjecutarFacturas (Facturas)");
        }


        //---

        public static void EjecutarNotas(Configuracion config, ref EventLog eventos)
        {
            if(debug) eventos.WriteEntry("DEBUG: Entrando a EjecutarNotas");
            //  Nos conectamos a la BBDD y verificamos que haya alguna fila por extraer
            SqlDataReader consulta;

            try
            {
                consulta = ConectBBDD.HacerConsultaNotasCredito(config);
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 201 al hacer consulta a la base de datos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);

                return;
            }

            if (consulta == null)
            {
                //eventos.WriteEntry("Error interno 111 al hacer consulta a la base de datos. La consulta devolvió NULL: Verifique su configuración.", EventLogEntryType.Error);
                if (debug) eventos.WriteEntry("DEBUG: Saliendo de iteración por consulta = null (Notas)");
                return;
            }


            if (!consulta.HasRows) return;

            if (debug) eventos.WriteEntry("DEBUG: Etapa de consultas pasada. (Notas)");
            //---

            // Pedimos el Token a DBNet para construir los documentos:
            string token = "";
            WSConnectDBNet wSConnectDBNet = new WSConnectDBNet(config.URLWSDBNet);
            
            try
            {
                token = wSConnectDBNet.ObtenerToken(config.idEmpresaDBNet, config.UserEmpresaDBNet, config.AutorizacionEmpresaDBNet);
            }
            catch (Exception e)
            {

                eventos.WriteEntry("Error interno 202 al obtener el token: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                return;
            }

            //---


            List<Documents.FAC.FAC> Lista_FACs = new List<Documents.FAC.FAC>();
            List<Documents.DOCs.NC_ND> Lista_Notas = new List<Documents.DOCs.NC_ND>();

            //Lista_FACs = LlenarDocumentos.ConstruirDocumentosConsultaFAC(consulta, config, token);

            //if(debug)eventos.WriteEntry("Debug: nota crédito escaneada: " + consulta[2].ToString() + "\nCampos escaneados: " + consulta.FieldCount.ToString());
            try
            {

                Lista_Notas = LlenarDocumentos.ConstruirDocumentosConsultaNotas(consulta, config, token);
            }
            catch(Exception e)
            {
                eventos.WriteEntry("Error interno 2025 construir documento (Notas): " + "\n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error) ;
                return;
            }


            //  Empezamos a enviar los documentos

            Hilos hilos_Agente = new Hilos(Lista_FACs, Lista_Notas, config.URLWSDBNet, eventos);

            try
            {
                hilos_Agente.EjecutarEnSerie(eventos);  //  Enviamos los documentos
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 203 al enviar documentos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                return;
            }


            //---

            try
            {
                ConectBBDD.ActualizarCamposBBDD(config, WSConnectDBNet.respuestas);  //  Actualizamos la info en la base de datos
            }
            catch (Exception e)
            {
                eventos.WriteEntry("Error interno 204 al actualizar campos de la base de datos: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                WSConnectDBNet.respuestas.Clear();
                return;
            }

            WSConnectDBNet.respuestas.Clear();

            //---

            if (config.GenerarJsonDocumentosConector)
            {
                try
                {
                    Funciones.GuardarDocumentos(config.RutaGuardadoJsonDocumentosFACConector, config.RutaGuardadoJsonDocumentosNOTASConector,
                                                Lista_FACs, Lista_Notas);
                }
                catch (Exception e)
                {
                    eventos.WriteEntry("Error interno 221 al guardar documentos en el equipo: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                    return;
                }
            }
            if(debug) eventos.WriteEntry("DEBUG: saliendo de EjecutarNotas");
        }
    }
}
