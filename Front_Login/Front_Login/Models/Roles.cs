using System;
using Newtonsoft.Json;

namespace Front_Login.Models
{
	public class Roles
	{
        [JsonProperty("idRol")]
        public int IdRol { get; set; }
        [JsonProperty("nombreRol")]
        public string NombreRol { get; set; }
        [JsonProperty("estadoRol")]
        public string EstadoRol { get; set; }
	}
}

