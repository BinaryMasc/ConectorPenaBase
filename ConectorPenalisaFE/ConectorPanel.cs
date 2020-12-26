using System;
using System.Timers;
using System.Diagnostics;
using System.Windows.Forms;
using ConfiguracionNS;

namespace ConectorPenalisaFE
{
    public partial class ConectorPanel : Form
    {
        Configuracion config = new Configuracion();
        public ConectorPanel()
        {
            InitializeComponent();

            //  Validamos que exista el log, sino lo creamos
            if (!System.Diagnostics.EventLog.SourceExists("Conector_FE"))
            {
                System.Diagnostics.EventLog.CreateEventSource("Conector_FE", "Agente");
            }
            eventLog1 = new EventLog("Agente", ".", "Conector_FE");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Iniciar")
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void button_Configurar_Click(object sender, EventArgs e)
        {
            Form F1 = new FormConfig();
            F1.ShowDialog();
            F1.Dispose();
        }

        private void button_Visor_Click(object sender, EventArgs e)
        {
            Process visor_eventos = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo(@"C:\Windows\System32\eventvwr.exe");
            processStartInfo.Arguments = @"/l:C:\Windows\System32\Winevt\Logs\Agente.evtx";
            visor_eventos.StartInfo = processStartInfo;
            visor_eventos.Start();
        }

        private void Start()
        {
            button1.Text = "Detener";
            TipoDoc_FFlag = false;

            eventLog1.WriteEntry("Iniciando Conector: Leyendo Configuracion.", EventLogEntryType.Information);


            //  Cargar Configuracion
            try
            {
                config = Configuracion.CargarConfiguracion();
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("Error interno en la carga de la configuración: " + e.Message, EventLogEntryType.Error);
                Stop();
                return;
            }

            //  Ajustando Timer
            ConfigTimerService(config.IntervaloTimerConector * 60000);


            //  Pruebaa
            //Agente.Ejecutar(config, ref eventLog1);
        }

        private void Stop()
        {
            button1.Text = "Iniciar";

            timer1.Stop();
            eventLog1.WriteEntry("Conector detenido.", EventLogEntryType.Information);
        }

        bool TipoDoc_FFlag;
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            timer1.Stop();

            try
            {
                if (TipoDoc_FFlag) Agente.EjecutarFac(config, ref eventLog1);
                else Agente.EjecutarNotas(config, ref eventLog1);
            } catch (Exception ex)
            {
                eventLog1.WriteEntry("Error General 001: " + Helpers.GetExceptionDetails(ex));
            }


            GC.Collect();

            if (TipoDoc_FFlag) TipoDoc_FFlag = false;
            else TipoDoc_FFlag = true;

            timer1.Start();


        }

        public void ConfigTimerService(int interval)
        {
            this.timer1 = new System.Timers.Timer();
            this.timer1.Interval = interval;
            this.timer1.Elapsed += new ElapsedEventHandler(this.OnTimer);
            this.timer1.Start();
        }
    }
}
