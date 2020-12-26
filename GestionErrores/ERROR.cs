using System;
using Newtonsoft.Json;

namespace Errors
{
    public class ERROR
    {
        private int codigo_error;
        private string descripcion_error;
        private string descripcion_tecnica;
        private DateTime hora_error;

        public int Codigo_error { get => codigo_error; set => codigo_error = value; }
        public string Descripcion_error { get => descripcion_error; set => descripcion_error = value; }
        public string Descripcion_tecnica { get => descripcion_tecnica; set => descripcion_tecnica = value; }
        public DateTime Hora_error { get => hora_error; set => hora_error = value; }

        public ERROR() { }

        public ERROR(int codigo_error, string descripcion_error)
        {
            Codigo_error = codigo_error;
            Descripcion_error = descripcion_error;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public ERROR FromString(string origen)
        {
            return JsonConvert.DeserializeObject<ERROR>(origen);
        }
    }
}
