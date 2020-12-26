using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft;
using RestSharp;
using Errors;

namespace WSConnect
{
    public class WSConnectDBNet
    {
        string URL;
        public WSConnectDBNet(string URLWS)
        {
            URL = URLWS;
        }

        public ERRORES errores_wsconnectdbnet = new ERRORES();

        public static List<Res> respuestas = new List<Res>();


        //  Función que recibe las credenciales de la empresa para hacer el request del token y devuelve el token.
        public string ObtenerToken(string idCompany, string userCompany, string authorizationCompany)
        {
            string jsonRequest = ConstruirJSONRequestToken(idCompany, userCompany, authorizationCompany);
            string result = "";
            dynamic objResponse;
            try
            {
                result = ConsumirPost("/GetToken", jsonRequest);
            }
            catch (Exception e)
            {
                ERROR error = new ERROR();
                error.Codigo_error = 1;
                error.Descripcion_error = "Error al solicitar token. " +
                    " Revisar la descripción técnica para obtener mas información";
                error.Descripcion_tecnica = e.Message;
                error.Hora_error = DateTime.Now;
                errores_wsconnectdbnet.Existe_error = true;
                errores_wsconnectdbnet.lista_errores.Add(error);

                throw e;
            }

            objResponse = JsonConvert.DeserializeObject(result);

            if (objResponse["Token"].ToString() == null)
            {
                ERROR error = new ERROR();
                error.Codigo_error = 2;
                error.Descripcion_error = "Error al solicitar token. " + "Respuesta: NOK";
                error.Descripcion_tecnica = objResponse["MensajeRespuesta"].ToString();
                error.Hora_error = DateTime.Now;
                errores_wsconnectdbnet.Existe_error = true;
                errores_wsconnectdbnet.lista_errores.Add(error);
                throw new Exception();
            }
            else
            {
                return objResponse["Token"].ToString();
            }



            //JObject root = JObject.Parse(result);
            //JProperty property = (JProperty)root.First;
            ////property.Value["Token"];

            //string token = property.Value.ToString();

            //return token;
            ////return result;
        }


        //  Función que recibe recurso y el request en string de formato JSON UTF-8 para realizar conexión de WS mediante método post.
        public dynamic ConsumirPost(string resource, string json)
        {
            try
            {
                var client = new RestClient(URL);
                var request = new RestRequest(Method.POST);

                client.ConfigureWebRequest((r) =>
                {
                    r.ServicePoint.Expect100Continue = false;
                    r.KeepAlive = true;
                });

                request.Resource = resource;
                request.AddHeader("content-type", "application/json;charset=utf-8");
                request.AddParameter("application/json", json, ParameterType.RequestBody);


                IRestResponse response = client.Execute(request);

                dynamic datos = JsonConvert.DeserializeObject(response.Content).ToString();

                return datos;
            }
            catch (Exception ex)
            {
                ERROR error = new ERROR();
                error.Codigo_error = 1;
                error.Descripcion_error = "Error al conectar vía WS" +
                    " Revisar la descripción técnica para obtener mas información";
                error.Descripcion_tecnica = ex.Message;
                error.Hora_error = DateTime.Now;
                errores_wsconnectdbnet.Existe_error = true;
                errores_wsconnectdbnet.lista_errores.Add(error);
                throw ex;
            }
        }


        //  Función que recibe un string de JSON y envía el documento a la plataforma del PT DBNet. En caso de error, devuelve null
        public dynamic EnviarDocumento(string documento, string correlativo, EventLog eventos)
        {
            dynamic respuesta;
            try
            {
                respuesta = JsonConvert.DeserializeObject(ConsumirPost("/EmisionDocumentoUBL21", documento));
            }
            catch (Exception e)
            {
                ERROR error = new ERROR();
                error.Codigo_error = 2;
                error.Descripcion_error = "Error al enviar documento. " + "Revisar la descripción técnica para más información";
                error.Descripcion_tecnica = e.Message;
                error.Hora_error = DateTime.Now;
                errores_wsconnectdbnet.Existe_error = true;
                errores_wsconnectdbnet.lista_errores.Add(error);

                //throw e;
                //eventos.WriteEntry("Error al enviar documento " + correlativo + ": " + e.Message, EventLogEntryType.Error);

                return e.Message;
            }

            if(respuesta["msg"] != null)
            {
                return null;
            }

            return respuesta;

            /*if (respuesta["CodigoRespuesta"].ToString() == "OK")
            {
                return respuesta;
            }
            else
            {
                //ERROR error = new ERROR();
                //error.Codigo_error = 2;
                //error.Descripcion_error = "Error al enviar documento. " + "Respuesta: " + respuesta["CodigoRespuesta"].ToString();

                //if(respuesta["MensajeRespuesta"] != null)
                    //error.Descripcion_tecnica = "MensajeRespuesta: " + respuesta["MensajeRespuesta"].ToString();
                //else
                    //Se accederá a este bloque en caso de que el formato del JSON no sea el que ellos manejan. ejemplo: enviar un campo numérico en vez de string.
                    //error.Descripcion_tecnica = "Resultado: " + Base64Decode(respuesta["Resultado"].ToString());

                //error.Hora_error = DateTime.Now;
                //errores_wsconnectdbnet.Existe_error = true;
                //errores_wsconnectdbnet.lista_errores.Add(error);

                //throw new Exception(error.Descripcion_tecnica);
                //eventos.WriteEntry("Error al enviar documento " + correlativo + ": " + Base64Decode(respuesta["Resultado"].ToString()), EventLogEntryType.Error);
                
                return respuesta;
            }*/
        }

        //  Sobrecarga método estático para hilos. devuelve respuesta por referencia
        public static void EnviarDocumento(string documento, string correlativo, string prefijo, string tipodoc, string url, EventLog eventos)
        {
            WSConnectDBNet ws = new WSConnectDBNet(url);
            dynamic respuesta = ws.EnviarDocumento(documento, correlativo, eventos);


            if(respuesta == null)
            {
                Res res = new Res(null, correlativo, prefijo, tipodoc, EstadoRespuesta.RespuestaNull);
                respuestas.Add(res);
            }
            else
            {
                if (respuesta["CodigoRespuesta"] == "OK")
                {
                    Res res = new Res(respuesta, correlativo, prefijo, tipodoc, EstadoRespuesta.EnviadoConExito);
                    respuestas.Add(res);
                }
                else
                {
                    Res res = new Res(respuesta, correlativo, prefijo, tipodoc, EstadoRespuesta.EnviadoConError);
                    respuestas.Add(res);
                }
            }
        }
        
        private string ConstruirJSONRequestToken(string idCompany, string userCompany, string authorizationCompany)
        {
            JsonToken.body body = new JsonToken.body
            {
                GetToken = new JsonToken.getToken
                {
                    IdentificadorEmpresa = idCompany,
                    UsuarioEmpresa = userCompany,
                    AutorizacionEmpresa = authorizationCompany
                }
            };

            return JsonConvert.SerializeObject(body);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }

    public class Res
    {
        public Res(dynamic respuesta, string correlativo, string prefijo, string tipodoc, EstadoRespuesta estado)
        {
            this.respuesta = respuesta;
            this.correlativo = correlativo;
            this.estado = estado;
            this.tipodoc = tipodoc;
            this.prefijo = prefijo;
        }
        public dynamic respuesta { get; set; }
        public dynamic prefijo { get; set; }
        public string correlativo { get; set; }
        public string tipodoc { get; set; }
        public EstadoRespuesta estado { get; set; }
    }

    public enum EstadoRespuesta
    {
        EnviadoConError = 0,
        EnviadoConExito = 1,
        RespuestaNull = 2
    }
}
