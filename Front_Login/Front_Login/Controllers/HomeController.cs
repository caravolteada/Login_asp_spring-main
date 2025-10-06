using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Front_Login.Models;
using Newtonsoft.Json;

namespace Front_Login.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var usuarioJson = HttpContext.Session.GetString("Usuario");

        // ✅ DEBUG: Ver qué hay en la sesión
        System.Diagnostics.Debug.WriteLine($"Usuario en sesión: {usuarioJson}");

        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Login");
        }

        var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

        // ✅ DEBUG: Ver si se deserializa correctamente
        System.Diagnostics.Debug.WriteLine($"Usuario deserializado: {usuario?.NombreUsuario} {usuario?.ApellidosUsuario}");

        ViewBag.NombreCompleto = $"{usuario?.NombreUsuario} {usuario?.ApellidosUsuario}";
        ViewBag.Usuario = usuario;

        return View();
    }

    public IActionResult Privacy()
    {
        // Verificamos autenticación también para Privacy
        var usuarioJson = HttpContext.Session.GetString("Usuario");
        if (string.IsNullOrEmpty(usuarioJson))
        {
            return RedirectToAction("Index", "Login");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

