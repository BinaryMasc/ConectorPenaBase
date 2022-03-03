using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ConfiguracionNS
{
    public class Configuracion : FileConfig
    {

        //static string path = "C:\\Users\\" + Environment.UserName + "\\Documents\\ConectorFE\\data.config";   

        public Configuracion() { }

        //  Carga las configuraciones desde el archivo de configuración. Devuelve true si la operación fue un exito.
        public static Configuracion CargarConfiguracion()
        {

            Configuracion config = new Configuracion();

            string path = "C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\data.config";

            if (!File.Exists(path)) 
                throw new Exception("El archivo de configuración: '" + path + "' no existe. ");


            try
            {
                FileConfig fconfig = JsonConvert.DeserializeObject<FileConfig>(File.ReadAllText(path));


                //  Se empieza a llenar la información
                //  InfoAut DBNet:
                config.idEmpresaDBNet = fconfig.idEmpresaDBNet;
                config.UserEmpresaDBNet = fconfig.UserEmpresaDBNet;
                config.AutorizacionEmpresaDBNet = fconfig.AutorizacionEmpresaDBNet;
                config.URLWSDBNet = fconfig.URLWSDBNet;

                //  InfoProc:
                config.idArea_InfoProc = fconfig.idArea_InfoProc;
                config.signDoc_InfoProc = fconfig.signDoc_InfoProc;
                config.asgCorr_InfoProc = fconfig.asgCorr_InfoProc;
                config.genXml_InfoProc = fconfig.genXml_InfoProc;
                config.saveDoc_InfoProc = fconfig.saveDoc_InfoProc;

                //  Dirección:
                config.TipoDireccion = fconfig.TipoDireccion;
                config.IdDUNS = fconfig.IdDUNS;
                config.ApartadoPostal = fconfig.ApartadoPostal;
                config.Direccion = fconfig.Direccion;
                config.Area = fconfig.Area;
                config.Ciudad = fconfig.Ciudad;
                config.Departamento = fconfig.Departamento;
                config.CodigoDepartamento = fconfig.CodigoDepartamento;
                config.CodigoPais = fconfig.CodigoPais;
                config.NombrePais = fconfig.NombrePais;

                //  Contacto
                config.TipoContacto = fconfig.TipoContacto;
                config.NombreContacto = fconfig.NombreContacto;
                config.TelefonoContacto = fconfig.TelefonoContacto;
                config.MailContacto = fconfig.MailContacto;
                config.DescripcionContacto = fconfig.DescripcionContacto;

                //  Base de datos
                config.Server_BBDD = fconfig.Server_BBDD;
                config.Nombre_BBDD = fconfig.Nombre_BBDD;
                config.NombreVistaFAC = fconfig.NombreVistaFAC;
                config.NombreVistaNotas = fconfig.NombreVistaNotas;
                config.Seguridad_Integrada_BBDD = fconfig.Seguridad_Integrada_BBDD;
                config.Usuario_BBDD = fconfig.Usuario_BBDD;
                config.Password_BBDD = fconfig.Password_BBDD;

                //  Config General Conector
                config.IntervaloTimerConector = fconfig.IntervaloTimerConector;
                config.GenerarJsonDocumentosConector = fconfig.GenerarJsonDocumentosConector;
                config.RutaGuardadoRespuestasFAC = fconfig.RutaGuardadoRespuestasFAC;
                config.RutaGuardadoRespuestasNotas = fconfig.RutaGuardadoRespuestasNotas;
                config.RutaGuardadoJsonDocumentosFACConector = fconfig.RutaGuardadoJsonDocumentosFACConector;
                config.RutaGuardadoJsonDocumentosNOTASConector = fconfig.RutaGuardadoJsonDocumentosNOTASConector;

                
            }
            catch(Exception ex) { throw ex; }

            return config;

        }

        public static void GuardarConfiguracion(FileConfig fconfig)
        {
            string path = "C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\data.config";
            //  Crea las carpetas si no existen
            if (!Directory.Exists("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\"))
                Directory.CreateDirectory("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena");

            if (!Directory.Exists("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Facturas_Generadas\\"))
                Directory.CreateDirectory("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Facturas_Generadas");

            if (!Directory.Exists("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Notas_Generadas\\"))
                Directory.CreateDirectory("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Notas_Generadas");

            if (!Directory.Exists("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Notas_Generadas\\"))
                Directory.CreateDirectory("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Respuestas_FAC");

            if (!Directory.Exists("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Notas_Generadas\\"))
                Directory.CreateDirectory("C:\\Users\\" + "Administrador" + "\\Documents\\ConectorFE_FunClubPena\\Respuestas_Notas");

            //  Guardamos la configuración
            File.WriteAllText(path, JsonConvert.SerializeObject(fconfig));
        }

    }

    public class FileConfig
    {
        public string idEmpresaDBNet { get; set; }
        public string UserEmpresaDBNet { get; set; }
        public string AutorizacionEmpresaDBNet { get; set; }
        public string URLWSDBNet { get; set; }

        public string idArea_InfoProc { get; set; }
        public string signDoc_InfoProc { get; set; }
        public string asgCorr_InfoProc { get; set; }
        public string genXml_InfoProc { get; set; }
        public string saveDoc_InfoProc { get; set; }


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


        public string TipoContacto { get; set; }
        public string NombreContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string MailContacto { get; set; }
        public string DescripcionContacto { get; set; }


        public string Server_BBDD { get; set; }
        public string Nombre_BBDD { get; set; }
        public string NombreVistaFAC { get; set; }
        public string NombreVistaNotas { get; set; }
        public bool Seguridad_Integrada_BBDD { get; set; }
        public string Usuario_BBDD { get; set; }
        public string Password_BBDD { get; set; }


        public int IntervaloTimerConector { get; set; }
        public bool GenerarJsonDocumentosConector { get; set; }
        public string RutaGuardadoRespuestasFAC { get; set; }
        public string RutaGuardadoRespuestasNotas { get; set; }
        public string RutaGuardadoJsonDocumentosFACConector { get; set; }
        public string RutaGuardadoJsonDocumentosNOTASConector { get; set; }
    }
}


