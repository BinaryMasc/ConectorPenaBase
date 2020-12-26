using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Documents
{
    //  Funciones adicionales
    public static class Funciones
    {


        //  Función que recibe el string de todos los datos a rellenar para separarlos en un array de strings
        public static string[] separarDatos(string data, char separador = '|')
        {
            char[] dataArray = data.ToCharArray();
            int indexElements = 1;

            for (int i = 0; i < dataArray.Length; i++)
            {
                if (dataArray[i] == separador) indexElements++;
            }

            string[] result = new string[indexElements];

            int ActualSeparator = 0;

            for (int i = 0; i < indexElements; i++)
            {
                string partInfo = "";
                ActualSeparator = 0;
                for (int j = 0; j < dataArray.Length; j++)
                {

                    if (dataArray[j] != separador && i == ActualSeparator)
                    {
                        partInfo += dataArray[j];
                    }

                    else if (dataArray[j] == separador)
                    {

                        if (ActualSeparator == i) break;
                        ActualSeparator++;
                    }
                }

                result[i] = partInfo;

            }

            return result;

        }

        //  Agrega un bloque de información de algún tipo a la factura a la lista para posteriormente serializarlo
        public static void AgregarCampoFAC(ref List<Documents.FAC.infoDoc> ListainfoDocs, string tipo, string data)
        {
            //FAC.infoDoc infoDocTemporal = new FAC.infoDoc(tipo, data);

            ListainfoDocs.Add(new FAC.infoDoc(tipo, data));
        }

        //  Agrega un bloque de información de algún tipo a la Nota a la lista para posteriormente serializarlo
        public static void AgregarCampoNota(ref List<Documents.DOCs.infoDoc> ListainfoDocs, string tipo, string data)
        {
            //DOCs.infoDoc infoDocTemporal = new DOCs.infoDoc(tipo, data);

            ListainfoDocs.Add(new DOCs.infoDoc(tipo, data));
        }

        //  Agrega un nuevo elemento a la factura tipo "Línea adicional"
        public static void crearLineaAdicional(ref List<Documents.FAC.datosVariables> listaDatos, string campo)
        {
            FAC.datosVariables DatosVariables = new FAC.datosVariables
            {
                LineaAdicional = (listaDatos.Count + 1).ToString(),
                ValorAdicional = campo
            };

            listaDatos.Add(DatosVariables);
        }

        public static void llenarDocumentoNCND(ref List<Documents.DOCs.infoDoc> ListainfoDocs, string tipo, string data)
        {
            DOCs.infoDoc infoDocTemporal = new DOCs.infoDoc(tipo, data);

            ListainfoDocs.Add(infoDocTemporal);
        }


        //  Función recibe el archivo Json construído y limpia o elimina los campos cuyo contenido sea igual a null.
        public static string LimpiarJson(string json)
        {
            char[] arrjson = json.ToCharArray();
            bool jsonClear = false;

            while (!jsonClear)
            {
                jsonClear = true;
                for (int i = 0; i < arrjson.Length; i++)
                {
                    if (arrjson[i] == 'n' && arrjson[i + 1] == 'u' && arrjson[i + 2] == 'l' && arrjson[i + 3] == 'l')
                    {
                        jsonClear = false;
                        int from = -1;
                        int to = i + 4;
                        for (int j = i; j > 0; j--)
                        {
                            if (arrjson[j] == ',')
                            {
                                from = j;
                                break;
                            }

                        }

                        for (int k = from; k < to; k++)
                        {
                            arrjson[k] = '\0';
                        }

                        continue;
                    }
                }
            }

            string result = "";

            for (int i = 0; i < arrjson.Length; i++)
            {
                if (arrjson[i] != '\0')
                    result += arrjson[i];
            }


            return result;
        }

        //  Crea los documentos enviados en la ruta especificada
        public static void GuardarDocumentos(string directorioFacs, string directorioNotas, List<FAC.FAC> FACs, List<DOCs.NC_ND> Notas)
        {
            if (!Directory.Exists(directorioFacs))  throw new Exception("No existe el directorio '" + directorioFacs  + "' Para guardar los documentos");
            if (!Directory.Exists(directorioNotas)) throw new Exception("No existe el directorio '" + directorioNotas + "' Para guardar los documentos");

            //Directory.CreateDirectory(ruta)

            for (int i = 0; i < FACs.Count; i++)
            {
                string json = JsonConvert.SerializeObject(FACs[i]);
                json = LimpiarJson(json);

                string jsonName = "FAC_" + FACs[i].EmisionDocumento.infoProc.SerieDoc + FACs[i].EmisionDocumento.infoProc.corrDoc + ".json";

                File.WriteAllText(directorioFacs + jsonName, json);

            }

            for (int i = 0; i < Notas.Count; i++)
            {
                string json = JsonConvert.SerializeObject(Notas[i]);
                json = LimpiarJson(json);

                string jsonName = "Nota_" + Notas[i].EmisionDocumento.infoProc.SerieDoc + Notas[i].EmisionDocumento.infoProc.corrDoc + ".json";

                File.WriteAllText(directorioNotas + jsonName, json);

            }
        }

        public static string formatoHoraString(string hour)
        {
            string ret = "";
            for (int i = 0; i < 8; i++)
            {
                ret += hour[i];
            }
            return ret;
        }

        
    }
}
