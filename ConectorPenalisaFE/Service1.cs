using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using ConfiguracionNS;

namespace ConectorPenalisaFE
{
    public partial class Service1 : ServiceBase
    {

        string nombre_Variante = "FUNDACION_PUERTO_PENALISA";
        string nombre_EventLog = "Conector_FE_";    //  Fuente del EventLog = "Conector_FE_" + nombre de empresa


        public Configuracion Config_Conector;
        public Service1()
        {
            InitializeComponent();

            //  Validamos que exista el log, sino lo creamos
            if(nombre_EventLog != "Conector_FE_" + nombre_Variante) nombre_EventLog += nombre_Variante;

            if (!System.Diagnostics.EventLog.SourceExists(nombre_EventLog))
            {
                System.Diagnostics.EventLog.CreateEventSource(nombre_EventLog, "Agente");
            }
            eventLog1 = new EventLog("Agente", ".", nombre_EventLog);
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Iniciando: Leyendo Configuracion.", EventLogEntryType.Information);


            //  Cargar Configuracion
            try
            {
                Config_Conector = Configuracion.CargarConfiguracion();
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("Error interno en la carga de la configuración: \n" + Helpers.GetExceptionDetails(e), EventLogEntryType.Error);
                ServiceController sc = new ServiceController(nombre_EventLog);

                if (sc.Status == ServiceControllerStatus.Running) sc.Stop();

                return;
            }
            tipoDoc_Flag = false;
            //  Ajustando Timer
            ConfigTimerService(Config_Conector.IntervaloTimerConector * 60000);


        }

        protected override void OnStop()
        {
            timer1.Stop();
            eventLog1.WriteEntry("Conector detenido.", EventLogEntryType.Error);
            
        }

        bool tipoDoc_Flag;  //  True: FAC. False: Notas
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            timer1.Stop();
            try
            {
                if (tipoDoc_Flag) Agente.EjecutarFac(Config_Conector, ref eventLog1);
                else Agente.EjecutarNotas(Config_Conector, ref eventLog1);
            }
            catch(Exception ex)
            {
                eventLog1.WriteEntry("Error General 001: " + Helpers.GetExceptionDetails(ex));
            }

            GC.Collect();

            timer1.Start();

            if (tipoDoc_Flag) tipoDoc_Flag = false;
            else tipoDoc_Flag = true;

        }

        public void ConfigTimerService(int interval)
        {
            this.timer1 = new Timer();
            this.timer1.Interval = interval; 
            this.timer1.Elapsed += new ElapsedEventHandler(this.OnTimer);
            this.timer1.Start();
        }
    }
}
