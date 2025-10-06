using System;
using Newtonsoft.Json;
namespace Front_Login.Models
{
    public class Usuario
    {
        [JsonProperty("idUsuario")]
        public int IdUsuario { get; set; }
        [JsonProperty("nombreUsuario")]
        public string NombreUsuario { get; set; }
        [JsonProperty("apellidoUsuario")]
        public string ApellidosUsuario { get; set; }
        [JsonProperty("emailUsuario")]
        public string EmailUsuario { get; set; }
        [JsonProperty("userUsuario")]
        public string UserUsuario { get; set; }
        [JsonProperty("passUsuario")]
        public string PassUsuario { get; set; }
        [JsonProperty("estadoUsuario")]
        public string Estado { get; set; }
        [JsonProperty("fkIdRol")]
        public Roles FkIdRol { get; set; }

    }
}

