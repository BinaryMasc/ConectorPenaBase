namespace WSConnect
{
    //  Class para serializarlo en JSON
    public class JsonToken
    {
        public class body
        {
            public getToken GetToken = new getToken();
        }

        public class getToken
        {
            public string IdentificadorEmpresa { get; set; }
            public string UsuarioEmpresa { get; set; }
            public string AutorizacionEmpresa { get; set; }
        }
    }
}
