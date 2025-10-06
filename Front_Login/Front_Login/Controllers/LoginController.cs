using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Front_Login.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Front_Login.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Verificamos si ya está autenticado
            var usuarioJson = HttpContext.Session.GetString("Usuario");
            if (!string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Index", "Home");  // ← Redirigir al home
            }

            return View();
        }

        // Acción para logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            // Validación de campos vacíos
            if (string.IsNullOrEmpty(model.UserUsUario) || string.IsNullOrEmpty(model.PassUsuario))
            {
                TempData["Error"] = "Por favor ingrese todos los campos";
                return View("Index", model);
            }

            /// Verificar intentos fallidos
            var failedAttempts = HttpContext.Session.GetInt32("FailedAttempts") ?? 0;
            var blockUntil = HttpContext.Session.GetString("BlockUntil");

            if (blockUntil != null && DateTime.TryParse(blockUntil, out var blockTime))
            {
                if (DateTime.Now < blockTime)
                {
                    var remainingTime = (blockTime - DateTime.Now).Seconds;
                    // Limpiar TempData anterior y solo usar ViewBag
                    ViewBag.IsBlocked = true;
                    ViewBag.BlockTime = remainingTime;
                    TempData.Remove("Error"); // ✅ Limpiar error anterior
                    return View("Index", model);
                }
                else
                {
                    // Tiempo de bloqueo expirado, resetear contador
                    HttpContext.Session.Remove("FailedAttempts");
                    HttpContext.Session.Remove("BlockUntil");
                    failedAttempts = 0;
                }
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await _httpClient.PostAsync("http://localhost:8080/api/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<Usuario>(jsonResponse);

                    if (usuario != null)
                    {
                        HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(usuario));

                        // Resetear intentos fallidos en login exitoso
                        HttpContext.Session.Remove("FailedAttempts");
                        HttpContext.Session.Remove("BlockUntil");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Error al procesar la respuesta del servidor";
                        return View("Index", model);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Credenciales incorrectas - incrementar intentos fallidos
                    failedAttempts++;
                    HttpContext.Session.SetInt32("FailedAttempts", failedAttempts);

                    if (failedAttempts >= 3)
                    {
                        // Bloquear por 30 segundos
                        var newBlockTime = DateTime.Now.AddSeconds(30);
                        HttpContext.Session.SetString("BlockUntil", newBlockTime.ToString());
                        ViewBag.IsBlocked = true;
                        ViewBag.BlockTime = 30;
                    }
                    else
                    {
                        TempData["Error"] = "Usuario o contraseña incorrectos";
                    }

                    return View("Index", model);
                }
                else
                {
                    TempData["Error"] = "El servicio no está disponible. Por favor intente más tarde.";
                    return View("Index", model);
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Error de conexión. API fuera de servicio";
                return View("Index", model);
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] = "Tiempo de espera agotado. El servicio no responde.";
                return View("Index", model);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error inesperado. Por favor contacte al administrador.";
                return View("Index", model);
            }
        }

    }
}

