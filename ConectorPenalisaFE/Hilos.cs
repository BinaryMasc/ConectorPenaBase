using System;
using System.Collections.Generic;
using System.Threading;
using Documents;
using WSConnect;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ConectorPenalisaFE
{
    class Hilos
    {
        public int NumeroFacs { get; }
        public int NumeroNotas { get; }

        public string[] respuestasFac { get; }
        public bool[] errorFAC { get; }

        public string[] respuestasNotas { get; }
        public bool[] errorNotas { get; }


        public List<Documents.FAC.FAC> Facturas { get; }
        public List<Documents.DOCs.NC_ND> Notas{ get; }

        string url { get; }
        EventLog eventos;



        public Hilos(List<Documents.FAC.FAC> Facturas, List<Documents.DOCs.NC_ND> Notas, string url, EventLog eventos)
        {
            this.url = url;
            this.Facturas = Facturas;
            this.Notas = Notas;
            this.eventos = eventos;

            NumeroFacs = Facturas.Count;
            NumeroNotas = Notas.Count;

            respuestasFac = new string[NumeroFacs];
            respuestasNotas = new string[NumeroNotas];

            errorFAC = new bool[NumeroFacs];
            errorNotas = new bool[NumeroNotas];

            for (int i = 0; i < NumeroFacs;  i++) errorFAC[i] = false;
            for (int i = 0; i < NumeroNotas; i++) errorNotas[i] = false;


        }


        //  Crea hilos de ejecución para enviar los documentos de forma paralela
        public void Ejecutar(EventLog eventos)
        {
            List<Thread> hilo_proc = new List<Thread>();

            try
            {
                for (int i = 0; i < NumeroFacs; i++)
                {
                    int it_aux = i;


                    hilo_proc.Add(new Thread(() => WSConnectDBNet.EnviarDocumento(Funciones.LimpiarJson(JsonConvert.SerializeObject(Facturas[it_aux])), Facturas[it_aux].EmisionDocumento.infoProc.SerieDoc + Facturas[it_aux].EmisionDocumento.infoProc.corrDoc, Facturas[it_aux].EmisionDocumento.infoProc.SerieDoc, Facturas[it_aux].EmisionDocumento.infoProc.tipoDoc, url, eventos)));
                    hilo_proc[it_aux].Start();


                }

                for (int i = 0; i < NumeroNotas; i++)
                {
                    int it_aux = i;

                    hilo_proc.Add(new Thread(() => WSConnectDBNet.EnviarDocumento(Funciones.LimpiarJson(JsonConvert.SerializeObject(Notas[it_aux])), Notas[it_aux].EmisionDocumento.infoProc.SerieDoc + Notas[it_aux].EmisionDocumento.infoProc.corrDoc, Notas[it_aux].EmisionDocumento.infoProc.SerieDoc, Notas[it_aux].EmisionDocumento.infoProc.tipoDoc, url, eventos)));
                    hilo_proc[it_aux].Start();

                }

                foreach (Thread t in hilo_proc) t.Join();

                /*bool hilos_Activos = true;

                while (hilos_Activos)
                {
                    hilos_Activos = false;
                    for (int i = 0; i < hilo_proc.Count; i++)
                    {
                        if (hilo_proc[i].IsAlive) hilos_Activos = true;
                    }
                }*/


            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }


        }

        public void EjecutarEnSerie(EventLog eventos)
        {

            for(int i = 0; i < NumeroFacs; i++)
            {
                string correlativo = Facturas[i].EmisionDocumento.infoProc.corrDoc;
                string Tipo_doc    = Facturas[i].EmisionDocumento.infoProc.tipoDoc;
                string serie = Facturas[i].EmisionDocumento.infoProc.SerieDoc;
                WSConnectDBNet.EnviarDocumento(Funciones.LimpiarJson(JsonConvert.SerializeObject(Facturas[i])), correlativo, serie, Tipo_doc, url, eventos);
            }
            
            for (int i = 0; i < NumeroNotas; i++)
            {
                string correlativo = Notas[i].EmisionDocumento.infoProc.corrDoc;
                string Tipo_doc    = Notas[i].EmisionDocumento.infoProc.tipoDoc;
                string serie = Notas[i].EmisionDocumento.infoProc.SerieDoc;
                WSConnectDBNet.EnviarDocumento(Funciones.LimpiarJson(JsonConvert.SerializeObject(Notas[i])), correlativo, serie, Tipo_doc, url, eventos);
            }
        }

    }
}
