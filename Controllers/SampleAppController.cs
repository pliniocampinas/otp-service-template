using Microsoft.AspNetCore.Mvc;

namespace otp_service_template.Services;

public class SampleAppController : Controller
{
  [HttpGet]
  public IActionResult Index()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Index(string otp)
  {
    Console.WriteLine("verify");
    if (otp == "123456") // Exemplo estático
    {
      ViewBag.Message = "OTP verificado com sucesso!";
    }
    else
    {
      ViewBag.Message = "OTP inválido.";
    }

    return View();
  }
}