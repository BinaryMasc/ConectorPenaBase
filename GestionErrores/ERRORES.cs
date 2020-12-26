using System.Collections.Generic;
using Newtonsoft.Json;

namespace Errors
{
    public class ERRORES
    {
        public List<ERROR> lista_errores = new List<ERROR>();
        private bool existe_error;

        public bool Existe_error { get => existe_error; set => existe_error = value; }

        public ERRORES()
        {
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public ERRORES FromString(string origen)
        {
            return JsonConvert.DeserializeObject<ERRORES>(origen);
        }
    }
}
