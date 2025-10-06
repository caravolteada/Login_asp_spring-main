using System;
using Newtonsoft.Json;

namespace Front_Login.Models
{
	public class LoginRequest
	{
		[JsonProperty("userUsuario")]
		public string UserUsUario { get; set; }
        [JsonProperty("passUsuario")]
        public String PassUsuario { get; set; }
		
	}
}

