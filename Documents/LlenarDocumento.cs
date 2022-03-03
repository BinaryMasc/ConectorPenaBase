using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using ConfiguracionNS;

namespace Documents
{
    public static class LlenarDocumentos
    {
        //  Toma la última consulta hecha a la BBDD y crea los documentos recibidos de la vista.
        public static List<FAC.FAC> ConstruirDocumentosConsultaFAC(SqlDataReader consulta, Configuracion config, string token)
        {
            List<FAC.FAC> facturas = new List<FAC.FAC>();



            string fac_Anterior  = "";
            string nota_Anterior = "";

            while (consulta.Read())
            {

                string Tipo_DOC = consulta[0].ToString();
                string Numero_DOC = consulta[2].ToString();

                //
                if (Tipo_DOC == "FE" && Numero_DOC != fac_Anterior)
                {

                    ConstruirFAC(consulta, config, token, ref facturas);

                    fac_Anterior = Numero_DOC;
                }
                //
                else if (Tipo_DOC != "FE" && nota_Anterior != Numero_DOC)
                {
                    nota_Anterior = Numero_DOC;

                    continue;
                }
                //
            }

            return facturas;
        }

        public static List<DOCs.NC_ND> ConstruirDocumentosConsultaNotas(SqlDataReader consulta, Configuracion config, string token)
        {
            List<DOCs.NC_ND> Notas = new List<DOCs.NC_ND>();
            /*
            string fac_Anterior = "";
            string nota_Anterior = "";

            while (consulta.Read())
            {

                string Tipo_DOC = consulta[0].ToString();
                string Numero_DOC = consulta[2].ToString();

                //
                if (Tipo_DOC == "FE" && Numero_DOC != fac_Anterior)
                {
                    fac_Anterior = Numero_DOC;
                    continue;
                }
                //
                else if (Tipo_DOC != "FE" && nota_Anterior != Numero_DOC)
                {

                    ConstruirNC_ND(consulta, config, token, ref Notas);


                    nota_Anterior = Numero_DOC;
                }*/

            //
            ConstruirNC_ND(consulta, config, token, ref Notas);

            return Notas;
        }

        //  Llena una factura mediante los datos administrados por la consulta a la vista y la devuelve agregada en una lista por referencia
        private static void ConstruirFAC(SqlDataReader consulta, Configuracion config, string token, ref List<FAC.FAC> facturas)
        {

            string idEmpresa   = config.idEmpresaDBNet;
            string UserEmpresa = config.UserEmpresaDBNet;


            //  Crear Linea Dirección adquiriente
            string LineaDireccionEmisor = string.Concat("1|", config.TipoDireccion, '|', config.IdDUNS, '|', config.ApartadoPostal, '|',
                                                              config.Direccion, '|', config.Area, '|', config.Ciudad, '|', config.Departamento, '|',
                                                              config.CodigoDepartamento, '|', config.CodigoPais, '|', config.NombrePais);
            //  Crear Linea Contacto adquiriente
            string LineaContactoEmisor  = string.Concat("1|", config.TipoContacto, '|', config.NombreContacto, '|', config.TelefonoContacto, '|',
                                                              config.MailContacto, '|', config.DescripcionContacto);

            //

            const int CAMPOS_ENCABEZADO = 38;
            const int CAMPOS_DIRECCION = 10;
            const int CAMPOS_CONTACTO = 5;
            const int CAMPOS_IMPUESTO = 7;
            const int CAMPOS_MEDIOPAGO = 4;
            const int CAMPOS_ITEM = 11;
            const int CAMPOS_IMPUESTOITEM = 8;
            const int CAMPOS_INFO_ADICIONAL_REPETIBLE = 4;

            const int INDICE_ENCABEZADO = 0;
            const int INDICE_DIRECCION = 38;
            const int INDICE_CONTACTO = 48;
            const int INDICE_IMPUESTO = 53;
            const int INDICE_MEDIOPAGO = 60;
            const int INDICE_ITEM = 64;
            const int INDICE_IMPUESTOITEM = 75;
            const int INDICE_INFO_ADICIONAL_REPETIBLE = 83;
            const int INDICE_DATOS_ADICIONALES = 87;

            //  Indices de campos adicionales de ítem
            const int IndiceNotaItem = 74;
            const int IndiceCodigoItem = 83;
            const int IndiceBodegaItem = 84;
            const int IndiceReferenciaItem = 85;
            const int IndiceDocumentoItem = 86;
            //
            const int IndiceFechaEmision = 3;
            const int IndiceFechaPeriodoInicioFacturacion = 8;
            const int IndiceFechaPeriodoFinFacturacion = 10;
            const int IndiceNotaDocumento = 12;
            //
            const int IndiceMailContactoCliente = 51;
            //
            const int IndiceTotalImpuesto2 = 86 + 13 + 1;
            const int IndiceIndicadorImpuesto2 = 86 + 13 + 2;
            const int IndiceIndicadorImpuestoBaseImponible2 = 86 + 13 + 3;
            const int IndicePorcentajeImpuesto2 = 86 + 13 + 4;
            const int IndiceNumeroImpuesto2 = 86 + 13 + 5;
            const int IndiceNombreImpuesto2 = 86 + 13 + 6;
            const int IndiceDescuentoVisual = 86 + 13 + 7;





            string correlativo = "";
            string serie = "";
            string lineaCorreoDespacho = "";
            string fechaDoc = consulta[IndiceFechaEmision].ToString();



            //  Acá se almacenará toda la información recolectada
            List<Documents.FAC.infoDoc> informacionDocumentoFAC = new List<Documents.FAC.infoDoc>();

            //  Acá se almacenarán los datos adicionales de la FAC
            List<Documents.FAC.datosVariables> DatosVariables = new List<Documents.FAC.datosVariables>();

            int factura_Anterior = -1;
            string Pref_Anterior = "";

            //  Comienza el recorrido por los campos para extraer
            //  TODO: desde un principio se ha manejado la instrucción Do while acá, cosa que no se usa en la instrucción de las
            //        Notas de crédito, por alguna razón el do while(consulta.Read()) funciona acá y en las notas no, por lo
            //        que se usa solo while(consulta.Read()) en la linea 654.
            do
            {
                int iterador = 0;

                int factura = (int)Convert.ToDouble(consulta[2].ToString());
                string prefijo = consulta[1].ToString();

                //  Verificar que la factura previa tenga más campos de ítem:
                if (factura == factura_Anterior && prefijo == Pref_Anterior)
                {
                    //llenar item
                    string data = "";
                    iterador = INDICE_ITEM;


                    data += consulta[iterador].ToString();    //  Primer campo
                    iterador++;

                    while (iterador < INDICE_ITEM + CAMPOS_ITEM)
                    {
                        if (iterador == INDICE_ITEM + 6) data += "|" + consulta[INDICE_IMPUESTOITEM + 3].ToString();
                        else if (iterador  == IndiceNotaItem) data += "|" + $"#Bodega: [{consulta[IndiceBodegaItem].ToString()}]; #Referencia: [{consulta[IndiceReferenciaItem].ToString()}]; #Fecha: [{fechaDoc}]; #Codigo: [{consulta[IndiceCodigoItem].ToString()}]; #Documento: [{consulta[IndiceDocumentoItem].ToString()}];";
                        else data += "|" + consulta[iterador].ToString();

                        iterador++;
                    }

                    Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Item", data);

                    //---


                    //  Llenar ImpuestoItem
                    data = "";
                    iterador = INDICE_IMPUESTOITEM;


                    data += consulta[iterador].ToString();    //  Primer campo
                    iterador++;

                    while (iterador < INDICE_IMPUESTOITEM + CAMPOS_IMPUESTOITEM)
                    {

                        data += "|" + consulta[iterador].ToString();

                        iterador++;
                    }

                    Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "ImpuestoItem", data);

                    //
                    factura_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                    Pref_Anterior = consulta[1].ToString();
                    continue;

                }

                else if (factura_Anterior != -1) break;

                while (iterador < consulta.FieldCount)
                {
                    string data = "";

                    //  Llenar Encabezado
                    if (iterador == INDICE_ENCABEZADO)
                    {
                        data = "1";
                        string varAux = "";
                        string tipo_adquiriente = "";

                        while (iterador < CAMPOS_ENCABEZADO)
                        {

                            //  Invertir campos para agregar a la factura
                            if (iterador == 15)
                            {
                                varAux = consulta[iterador].ToString();
                                iterador++;
                                continue;
                            }
                            else if (iterador == 16)
                            {

                                data += "|" + consulta[iterador].ToString();
                                data += "|" + varAux;
                                iterador++;

                                continue;
                            }

                            //  En caso de ser un campo formato Hora, convertir a string Tipo[D-HH:MI:SS] para evitar error por parte del PT
                            if (iterador == 4 || iterador == 9 || iterador == 11)
                            {
                                data += "|" + Funciones.formatoHoraString(consulta[iterador].ToString());
                                iterador++;

                                continue;
                            }

                            //  Registrar Serie
                            if(iterador == 1)
                            {
                                serie = consulta[iterador].ToString();
                            }

                            //  Registrar correlativo (campo (2))
                            if (iterador == 2)
                            {
                                correlativo = consulta[iterador].ToString();
                                data += "|" + correlativo;
                                iterador++;

                                continue;
                            }

                            if(iterador == IndiceNotaDocumento)
                            {
                                data += $"|#Periodo desde: [{consulta[IndiceFechaPeriodoInicioFacturacion].ToString()}];" +
                                    $" #Periodo hasta: [{consulta[IndiceFechaPeriodoFinFacturacion].ToString()}];" +
                                    $" #ValorDescuento: [{consulta[IndiceDescuentoVisual].ToString()}];";
                                iterador++;

                                continue;
                            }

                            //  Registrar tipo adquiriente
                            if (iterador == 21) tipo_adquiriente = consulta[iterador].ToString();

                            //  Crear campo "NombreAdquiriente" en caso de que el adquiriente sea una persona natural
                            if (iterador == 37)
                            {
                                if (tipo_adquiriente != "02" && tipo_adquiriente != "2")
                                {
                                    iterador++;
                                    continue;
                                }
                            }

                            data += "|" + consulta[iterador].ToString();

                            iterador++;

                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Encabezado", data);

                    }

                    //  Llenar Direcciones
                    if (iterador == INDICE_DIRECCION)
                    {
                        data = "2";
                        int iteradorTemp = 0;
                        while (iteradorTemp < CAMPOS_DIRECCION)
                        {

                            data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Direccion", LineaDireccionEmisor);
                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Direccion", data);

                    }

                    //  Llenar Contactos
                    if (iterador == INDICE_CONTACTO)
                    {
                        data = "2";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_CONTACTO)
                        {
                            data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Contacto", LineaContactoEmisor);
                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Contacto", data);

                    }

                    //  Llenar Impuesto
                    if (iterador == INDICE_IMPUESTO)
                    {
                        //  Si base imponible es 0, omitir
                        if (consulta[INDICE_IMPUESTO + 3].ToString() == "0.00")
                            iterador = INDICE_MEDIOPAGO;


                        else
                        {
                            data = "1";
                            int iteradorTemp = 0;

                            while (iteradorTemp < CAMPOS_IMPUESTO)
                            {

                                data += "|" + consulta[iterador].ToString();
                                iterador++;
                                iteradorTemp++;
                            }

                            Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Impuesto", data);

                        }

                        //  Definimos el segundo impuesto en caso de que venga != ""
                        if (consulta[IndiceIndicadorImpuestoBaseImponible2].ToString() != "")
                            Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Impuesto", $"2|COP|{consulta[IndiceTotalImpuesto2].ToString()}|{consulta[IndiceIndicadorImpuesto2].ToString()}|{consulta[IndiceIndicadorImpuestoBaseImponible2].ToString()}|{consulta[IndicePorcentajeImpuesto2].ToString()}|{consulta[IndiceNumeroImpuesto2].ToString()}|{consulta[IndiceNombreImpuesto2].ToString()}");


                    }



                    //string lineaMedioPago = $"1|{consulta[INDICE_MEDIOPAGO]}|{consulta[INDICE_MEDIOPAGO + 1]}|{consulta[INDICE_MEDIOPAGO + 2]}|{consulta[INDICE_MEDIOPAGO + 3]}";
                    //Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "MedioPago", lineaMedioPago);

                    //iterador += CAMPOS_MEDIOPAGO;

                    //  Llenar MedioPago
                    if (iterador == INDICE_MEDIOPAGO)
                    {
                        data = "1";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_MEDIOPAGO)
                        {
                            data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "MedioPago", data);

                    }

                    //  Llenar primer Item
                    if (iterador == INDICE_ITEM)
                    {
                        data = "";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_ITEM)
                        {

                            if (iteradorTemp == 0) data += consulta[iterador].ToString();
                            //  Se toma el campo de base imponible 
                            else if (iteradorTemp == 6) data += "|" + consulta[INDICE_IMPUESTOITEM + 3].ToString();

                            else if (iterador == IndiceNotaItem) data += "|" + $"#Bodega: [{consulta[IndiceBodegaItem].ToString()}]; #Referencia: [{consulta[IndiceReferenciaItem].ToString()}]; #Fecha: [{fechaDoc}]; #Codigo: [{consulta[IndiceCodigoItem].ToString()}]; #Documento: [{consulta[IndiceDocumentoItem].ToString()}];";
                            
                            else data += "|" + consulta[iterador].ToString();

                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "Item", data);

                    }

                    //  Llenar ImpuestoItem
                    if (iterador == INDICE_IMPUESTOITEM)
                    {
                        data = "";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_IMPUESTOITEM)
                        {
                            if (iteradorTemp == 0) data += consulta[iterador].ToString();
                            else data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "ImpuestoItem", data);

                    }

                    //  Campos Adicionales Item
                    if(iterador == INDICE_INFO_ADICIONAL_REPETIBLE)
                    {
                        iterador += CAMPOS_INFO_ADICIONAL_REPETIBLE;
                    }

                    //  Adicional: Correo Despacho para el envío de factura
                    lineaCorreoDespacho = $"1|{consulta[IndiceMailContactoCliente].ToString()}|||SEND_FACT_EMIT";

                    //  Llenar datos adicionales
                    if (iterador == INDICE_DATOS_ADICIONALES)
                    {
                        const int CampoInicial = INDICE_DATOS_ADICIONALES;
                        
                        const int INDICE_CARGOS1 = 0 + CampoInicial;
                        const int INDICE_CARGOS2 = 1 + CampoInicial;
                        const int INDICE_ABONOS  = 2 + CampoInicial;
                        const int INDICE_SUBTOTAL_CARGOS1 = 3 + CampoInicial;
                        const int INDICE_SUBTOTAL_CARGOS2 = 4 + CampoInicial;
                        const int INDICE_SUBTOTAL_ABONOS  = 5 + CampoInicial;
                        const int INDICE_OTROS_CARGOS = 6 + CampoInicial;
                        const int INDICE_CargosDelMes = 7 + CampoInicial;
                        const int INDICE_AbonosDelMes = 8 + CampoInicial;
                        const int INDICE_ValorEnMora  = 9 + CampoInicial;
                        const int INDICE_PagueAntesDe = 10 + CampoInicial;
                        const int INDICE_ValorAPagar  = 11 + CampoInicial;
                        const int INDICE_TipoFac      = 12 + CampoInicial;

                        

                        //  Definir tipo de factura
                        Funciones.crearLineaAdicional(ref DatosVariables, "TIPO: " + consulta[INDICE_TipoFac].ToString());



                        if (consulta[INDICE_TipoFac].ToString() != "1")
                        {
                            factura_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                            Pref_Anterior = consulta[1].ToString();
                            break;
                        }



                        string Cargos1 = consulta[INDICE_CARGOS1].ToString();
                        string Cargos2 = consulta[INDICE_CARGOS2].ToString();
                        string Abonos  = consulta[INDICE_ABONOS].ToString();


                        //  Subtotales
                        string SubtotalCargos1 = (consulta[INDICE_SUBTOTAL_CARGOS1].ToString());
                        string SubtotalCargos2 = (consulta[INDICE_SUBTOTAL_CARGOS2].ToString());
                        string SubtotalAbonos  = (consulta[INDICE_SUBTOTAL_ABONOS].ToString());

                        //  Totales
                        string OtrosCargos =  (consulta[INDICE_OTROS_CARGOS].ToString());
                        string CargosDelMes = (consulta[INDICE_CargosDelMes].ToString());
                        string ValorEnMora =  (consulta[INDICE_ValorEnMora].ToString());
                        string PagueAntesDe = consulta[INDICE_PagueAntesDe].ToString();
                        string AbonosDelMes = (consulta[INDICE_AbonosDelMes].ToString());
                        string ValorAPagar =  (consulta[INDICE_ValorAPagar].ToString());


                        Funciones.crearLineaAdicional(ref DatosVariables, Cargos1);
                        Funciones.crearLineaAdicional(ref DatosVariables, Cargos2);
                        Funciones.crearLineaAdicional(ref DatosVariables, Abonos);


                        //  Llenar Subtotales
                        data = "Subtotales[SubtotalCargos1: ";
                        data += SubtotalCargos1 + ", ";

                        data += "SubtotalCargos2: ";
                        data += SubtotalCargos2 + ", ";

                        data += "SubtotalAbonos: ";
                        data += SubtotalAbonos + "]";

                        Funciones.crearLineaAdicional(ref DatosVariables, data);

                        //---



                        //  Llenar totales
                        data = "Totales[OtrosCargos: ";
                        data += OtrosCargos + ", ";

                        data += "CargosDelMes: ";
                        data += CargosDelMes + ", ";

                        data += "AbonosDelMes: ";
                        data += AbonosDelMes + ", ";

                        data += "ValorEnMora: ";
                        data += ValorEnMora + ", ";

                        data += "PagueAntesDe: ";
                        data += PagueAntesDe + ", ";

                        data += "ValorAPagar: ";
                        data += ValorAPagar + "]";

                        Funciones.crearLineaAdicional(ref DatosVariables, data);

                        //---
                        factura_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                        Pref_Anterior = consulta[1].ToString();
                        break;
                        
                    }


                    factura_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                    Pref_Anterior = consulta[1].ToString();


                }


            } while (consulta.Read());

            //---

            Funciones.AgregarCampoFAC(ref informacionDocumentoFAC, "CorreoDespacho", lineaCorreoDespacho);

            //---

            Documents.FAC.infoProc infoprocFAC = new Documents.FAC.infoProc
            {
                idEmpr = idEmpresa,
                idArea = "BOGT",
                tipoDoc = "FE",
                SerieDoc = serie,
                corrDoc = correlativo,
                signDoc = "SI",
                asgCorr = "NO",
                genXml = "SI",
                saveDoc = "SI"
            };

            Documents.FAC.infoAdic infoAdicional = new Documents.FAC.infoAdic
            {
                DatosVariables = DatosVariables
            };

            Documents.FAC.emisionDocumento infoemisionDoc = new Documents.FAC.emisionDocumento
            {
                IdentificadorEmpresa = idEmpresa,
                UsuarioEmpresa = UserEmpresa,
                Token = token,
                infoProc = infoprocFAC,
                infoAdic = infoAdicional,
                infoDoc = informacionDocumentoFAC


            };

            Documents.FAC.FAC FACTURA = new Documents.FAC.FAC()
            {
                EmisionDocumento = infoemisionDoc,
            };


            facturas.Add(FACTURA);

        }

        //  Llena una nota mediante los datos administrados por la consulta a la vista y la devuelve agregada en una lista por referencia
        private static void ConstruirNC_ND(SqlDataReader consulta, Configuracion config, string token, ref List<DOCs.NC_ND> Notas)
        {
            string idEmpresa = config.idEmpresaDBNet;
            string UserEmpresa = config.UserEmpresaDBNet;


            //  Crear Linea Dirección adquiriente
            string LineaDireccionEmisor = string.Concat("1|", config.TipoDireccion, '|', config.IdDUNS, '|', config.ApartadoPostal, '|',
                                                              config.Direccion, '|', config.Area, '|', config.Ciudad, '|', config.Departamento, '|',
                                                              config.CodigoDepartamento, '|', config.CodigoPais, '|', config.NombrePais);
            //  Crear Linea Contacto adquiriente
            string LineaContactoEmisor = string.Concat("1|", config.TipoContacto, '|', config.NombreContacto, '|', config.TelefonoContacto, '|',
                                                              config.MailContacto, '|', config.DescripcionContacto);

            string correlativo = "";
            string serie = "";
            string tipoDoc = "";
            string lineaCorreoDespacho = "";



            //  Constantes
            const int IndiceMailContactoCliente = 47;
            const int INDICE_TIPOADQUIRIENTE = 17;

            const int INDICE_ENCABEZADO = 0;
            const int INDICE_DIRECCION = 34;
            const int INDICE_CONTACTO = 44;
            const int INDICE_IMPUESTO = 48;
            const int INDICE_ITEM = 55;
            const int INDICE_IMPUESTOITEM = 66;
            const int INDICE_REFERENCIA = 73;
            const int INDICE_MEDIOPAGO = 76;
            const int INDICE_IMPUESTO2 = 80;

            const int CAMPOS_ENCABEZADO = 34;
            const int CAMPOS_DIRECCION = 10;
            const int CAMPOS_CONTACTO = 4;
            const int CAMPOS_IMPUESTO = 7;
            const int CAMPOS_ITEM = 11;
            const int CAMPOS_IMPUESTOITEM = 7;
            const int CAMPOS_REFERENCIA = 3;

            //  Acá se almacenará toda la información recolectada
            List<Documents.DOCs.infoDoc> InformacionDocumentoNota = new List<Documents.DOCs.infoDoc>();

            string lineareferencia = "";
            string lineaMedioPago = "";

            bool medioPagoRegistrado = false;

            int DOC_Anterior = -1;
            string Pref_Anterior = "";

            //  Comienza el recorrido por los campos para extraer
            while (consulta.Read())
            {

                //--- TODO
                /*
                string consulta2;

                consulta.Read();

                try
                {
                    consulta2 = consulta[2].ToString();
                }
                catch(Exception ex)
                {
                    string excep = ex.Message;
                }
                */
                //---

                int iterador = 0;

                int documento = (int)Convert.ToDouble(consulta[2].ToString());
                string prefijo = consulta[1].ToString();
                tipoDoc = consulta[0].ToString();



                //  Verificar que la Nota previa tenga más campos de ítem:
                if (prefijo == Pref_Anterior && DOC_Anterior == documento)
                {
                    //  Se llena impuestoItem e Item
                    //llenar item
                    string data = "";
                    iterador = INDICE_ITEM;


                    data += consulta[iterador].ToString();    //  Primer campo
                    iterador++;

                    while (iterador < INDICE_ITEM + CAMPOS_ITEM)
                    {

                        data += "|" + consulta[iterador].ToString();

                        iterador++;
                    }


                    Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Item", data);


                    //  Llenar ImpuestoItem
                    data = "";
                    iterador = INDICE_IMPUESTOITEM;


                    data += consulta[iterador].ToString();    //  Primer campo
                    iterador++;

                    while (iterador < INDICE_IMPUESTOITEM + CAMPOS_IMPUESTOITEM)
                    {

                        data += "|" + consulta[iterador].ToString();

                        iterador++;
                    }

                    Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "ImpuestoItem", data);


                    //---

                    //
                    DOC_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                    Pref_Anterior = consulta[1].ToString();
                    continue;

                }
                else if (DOC_Anterior != -1) break;

                while (iterador < consulta.FieldCount)
                {
                    string data = "";

                    string tipo_adquiriente = consulta[INDICE_TIPOADQUIRIENTE].ToString();

                    //  Llenar Encabezado
                    if (iterador == INDICE_ENCABEZADO)
                    {
                        data = "1";

                        while (iterador < CAMPOS_ENCABEZADO)
                        {

                            //  En caso de ser un campo formato Hora, convertir a string Tipo[D-HH:MI:SS] para evitar error por parte del PT
                            if (iterador == 4)
                            {

                                data += "|" + Funciones.formatoHoraString(consulta[iterador].ToString());
                                iterador++;

                                continue;
                            }

                            //  Registrar Serie
                            if (iterador == 1)
                            {
                                serie = consulta[iterador].ToString();
                            }

                            //  Registrar correlativo (campo (2))
                            if (iterador == 2)
                            {
                                correlativo = consulta[iterador].ToString();
                                data += "|" + correlativo;
                                iterador++;

                                continue;
                            }


                            //  Crear campo "NombreAdquiriente" en caso de que el adquiriente sea una persona natural
                            if (iterador == 33)
                            {
                                if (tipo_adquiriente != "02" && tipo_adquiriente != "2")
                                {
                                    iterador++;
                                    continue;
                                }
                            }


                            data += "|" + consulta[iterador].ToString();

                            iterador++;
                        }
                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Encabezado", data);

                    }

                    //  Llenar Direcciones
                    if (iterador == INDICE_DIRECCION)
                    {
                        data = "2";
                        int iteradorTemp = 0;
                        while (iteradorTemp < CAMPOS_DIRECCION)
                        {

                            data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Direccion", LineaDireccionEmisor);
                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Direccion", data);

                    }

                    //  Llenar Contactos
                    if (iterador == INDICE_CONTACTO)
                    {
                        data = "2";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_CONTACTO)
                        {
                            data += "|" + consulta[iterador].ToString();
                            iterador++;
                            iteradorTemp++;
                        }
                        data += "|";

                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Contacto", LineaContactoEmisor);
                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Contacto", data);

                    }

                    //  Llenar Impuesto
                    if (iterador == INDICE_IMPUESTO)
                    {

                        if (consulta[INDICE_IMPUESTO + 3].ToString() == "0.00")
                            iterador = INDICE_ITEM;


                        else
                        {
                            data = "1";
                            int iteradorTemp = 0;

                            while (iteradorTemp < CAMPOS_IMPUESTO)
                            {

                                data += "|" + consulta[iterador].ToString();
                                iterador++;
                                iteradorTemp++;
                            }

                            Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Impuesto", data);
                        }



                        //  Definimos el segundo impuesto en caso de que venga != ""
                        if (consulta[INDICE_IMPUESTO2 + 2].ToString() != "" && consulta[INDICE_IMPUESTO2 + 2].ToString() != "0.00")
                        {
                            int carry = INDICE_IMPUESTO2;
                            var impuesto2 = $"2|COP|{consulta[carry++].ToString()}|{consulta[carry++].ToString()}|{consulta[carry++].ToString()}|{consulta[carry++].ToString()}|{consulta[carry++].ToString()}|{consulta[carry++].ToString()}";
                            Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Impuesto", impuesto2);
                        }


                    }



                    if (!medioPagoRegistrado)
                    {
                        lineaMedioPago = $"1|{consulta[INDICE_MEDIOPAGO]}|{consulta[INDICE_MEDIOPAGO + 1]}|{consulta[INDICE_MEDIOPAGO + 2]}|{consulta[INDICE_MEDIOPAGO + 3]}";
                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "MedioPago", lineaMedioPago);

                        medioPagoRegistrado = true;
                    }

                    
                    //  Llenar primer Item
                    if (iterador == INDICE_ITEM)
                    {
                        data = "";
                        int iteradorTemp = 0;

                        while (iteradorTemp < CAMPOS_ITEM)
                        {

                            if (iteradorTemp == 0) data += consulta[iterador].ToString();
                            //  Se toma el campo de base imponible 
                            ///else if (iteradorTemp == 6) data += "|" + consulta[INDICE_IMPUESTOITEM + 3].ToString();

                            ///else if (iterador == IndiceNotaItem) data += "|" + $"#Bodega: [{consulta[IndiceBodegaItem].ToString()}]; #Referencia: [{consulta[IndiceReferenciaItem].ToString()}]; #Fecha: [{fechaDoc}]; #Codigo: [{consulta[IndiceCodigoItem].ToString()}]; #Documento: [{consulta[IndiceDocumentoItem].ToString()}];";

                            else data += "|" + consulta[iterador].ToString();

                            iterador++;
                            iteradorTemp++;
                        }

                        Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Item", data);

                    }

                    //  Llenar ImpuestoItem
                    if (iterador == INDICE_IMPUESTOITEM)
                    {
                        //  Se evalúa que la base imponible != 0, sino, se omite
                        if (consulta[INDICE_IMPUESTOITEM + 3].ToString() != "" && consulta[INDICE_IMPUESTOITEM + 3].ToString() != "0.00")
                        {
                            data = "";
                            int iteradorTemp = 0;

                            while (iteradorTemp < CAMPOS_IMPUESTOITEM)
                            {
                                if (iteradorTemp == 0) data += consulta[iterador].ToString();
                                else data += "|" + consulta[iterador].ToString();
                                iterador++;
                                iteradorTemp++;
                            }

                            Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "ImpuestoItem", data);
                        }

                    }


                    //  Llenar Referencia
                    if (iterador == INDICE_REFERENCIA)
                    {
                        data = $"1|FA|{consulta[iterador].ToString() + consulta[iterador + 1].ToString()}|{consulta[iterador + 2].ToString()}|FE";

                        //data += "|" + consulta[iterador].ToString();

                        iterador += CAMPOS_REFERENCIA + 1;

                        //data += "|" + "FE";
                        lineareferencia = data;
                    }

                    //  Adicional: Correo Despacho para el envío de documento
                    lineaCorreoDespacho = $"1|{consulta[IndiceMailContactoCliente].ToString()}|||SEND_FACT_EMIT";

                    if (iterador > INDICE_REFERENCIA)
                    {
                        DOC_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                        Pref_Anterior = consulta[1].ToString();
                        break;
                    }

                    DOC_Anterior = (int)Convert.ToDouble(consulta[2].ToString());
                    Pref_Anterior = consulta[1].ToString();



                    iterador++;
                }


            }


            Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "Referencia", lineareferencia);
            Funciones.AgregarCampoNota(ref InformacionDocumentoNota, "CorreoDespacho", lineaCorreoDespacho);

            //  Construcción documento
            DOCs.infoProc infoproceso = new DOCs.infoProc()
            {
                idEmpr = idEmpresa,
                idArea = "BOGT",
                tipoDoc = tipoDoc,
                SerieDoc = serie,
                corrDoc = correlativo,
                signDoc = "SI",
                asgCorr = "NO",
                genXml = "SI",
                saveDoc = "SI"
            };

            Documents.DOCs.emisionDocumento infoemisionDoc = new Documents.DOCs.emisionDocumento
            {
                IdentificadorEmpresa = idEmpresa,
                UsuarioEmpresa = UserEmpresa,
                Token = token,
                infoProc = infoproceso,
                infoDoc = InformacionDocumentoNota


            };

            Documents.DOCs.NC_ND notaGenerada = new Documents.DOCs.NC_ND()
            {
                EmisionDocumento = infoemisionDoc,
            };


            Notas.Add(notaGenerada);
        }

        //  Funcionamiento de las funciones ConstruirFAC Y ConstruirNC_ND:
        /*
        * +----------------------------------------------------------------------------------------------------------------------
        * |  Se realiza una consulta a la base de datos solicitando todos los campos requeridos para la factura y se rcibe por parámetros.
        * |  
        * |  Las constantes INDICES representan el índice del array de la consulta desde donde se empieza a leer un campo en específico.
        * |  
        * |  Las constantes CAMPOS representan el número de campos que estas tienen que leer de la estructura 
        * |
        * |  Estos datos serán concatenados, separados por "|" para luego llevarse a una función que se encargue de separar y llenar los 
        * |  campos de la factura para la construcción del archivo JSON. El orden de los campos SÍ importa para el PT.
        * +-----------------------------------------------------------------------------------------------------------------------
        * 
        *   EJEMPLOS 
        *   
        *   //  Encabezado: 
        *   //  LineaEncabezado|TipoDocumento|Prefijo|Correlativo|FechaEmision|HoraEmision|TipoOperacion|TipoFactura|MonedaDocumento|periodoFechaInicio|periodoHoraInicio|periodoFechaFin|periodoHoraFin|NotaDocumento|TipoEmisor|TipoIdenEmisor|IdentificacionEmisor|DigitoVerificadorEmisor|RegimenEmisor|CodigoRespEmisor|NomComerEmisor|RSocApeEmisor|TipoAdquirente|TipoIdenAdquirente|IdentificacionAdquirente|DigitoVerificadorAdquiriente|RegimenAdquirente|CodigoRespAdquiriente|NomComerAdquirente|RSocApeAdquirente|TipoReceptorPago|TipoIdenReceptorPago|IdentificacionReceptorPago|NombreReceptorPago|TotalBrutoDocumento|BaseImponibleDocumento|TotalBrutoDocumentoImpu|TotalDocumento
               
        *   //  Direccion
        *   //  LineaDireccion|TipoDireccion|IdDUNS|ApartadoPostal|Direccion|Area|Ciudad|Departamento|CodigoDepartamento|CodigoPais|NombrePais

        *   //  Contacto
        *   //  LineaContacto|TipoContacto|NombreContacto|TelefonoContacto|MailContacto|DescripcionContacto

        *   //  Impuesto
        *   //  LineaImpuesto|MonedaImpuesto|TotalImpuesto|IndicadorImpuesto|BaseImponible|PorcentajeImpuesto|NumeroImpuesto|NombreImpuesto

        *   //  MedioPago
        *   //  LineaMedioPago|IdMedioPago|CodigoMedioPago|FechaMedioPago|IdentificadorPago

        *   //  Item
        *   //  LineaItem|DescripcionItem|CantidadItem|UnidadMedidaItem|MonedaItem|ValorUnitarioItem|CostoTotalItem|CodigoTipoPrecio|MarcaItem|ModeloItem|NotaItem

        *   //  ImpuestoItem
        *   //  LineaImpuestoItem|MonedaImpuestoItem|TotalImpuestoItem|BaseImponibleItem|PorcentajeImpuestoItem|NumeroImpuestoItem|NombreImpuestoItem|UnidadMedidaImpItem

        */
    }
}
