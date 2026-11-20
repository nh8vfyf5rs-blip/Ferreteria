using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Hubo un error al registrar el usuario");

            var (success, error) = await _authService.RegisterAsync(dto);

            if (!success)
            {
                return Conflict(new
                {
                    Message = error
                });
            }

            return Ok(new { success = true });
        }

        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["Acceso_Token"];

            if (!string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    errorMessage = "Los datos son inválidos"
                });

            var (success, result, error) = await _authService.LoginAsync(dto);

            if (!success || result == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    errorMessage = error ?? "Error al iniciar sesión"
                });
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };

            Response.Cookies.Append(
                "Acceso_Token",
                result.AccessToken,
                cookieOptions);

            Response.Cookies.Append(
                "RefreshToken",
                result.RefreshToken,
                cookieOptions);

            return Ok(new
            {
                success = true,
                message = "Login exitoso"
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Acceso_Token");
            Response.Cookies.Delete("RefreshToken");

            return Json(new
            {
                success = true,
                message = "Sesión cerrada exitosamente"
            });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Parámetros inválidos");

            var confirmado = await _authService.ConfirmEmailAsync(email, token);

            if (confirmado)
            {
                TempData["Success"] =
                    "Correo confirmado exitosamente. Ahora puedes iniciar sesión.";

                return RedirectToAction("Login");
            }

            TempData["Error"] = "El token es inválido o ya expiró.";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgoPasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    success = false,
                    errorMessage = "El correo electrónico es requerido"
                });
            }

            var sent = await _authService.SendResetPasswordLinkAsync(dto.Email);

            if (sent)
                return Ok(new { success = true });

            return NotFound(new
            {
                success = false,
                errorMessage = "Correo no encontrado"
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordRequestDto
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _authService.ResetPasswordAsync(dto);

            if (success)
                return Ok(new { success = true });

            ModelState.AddModelError("", "Token inválido o expirado.");

            return View(dto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var success = await _authService.ChangePasswordAsync(userName, dto);

            if (success)
            {
                TempData["Success"] = "Contraseña cambiada exitosamente.";

                Response.Cookies.Delete("Acceso_Token");
                Response.Cookies.Delete("RefreshToken");

                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "La contraseña actual es incorrecta.");

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var newTokens = await _authService.RefreshAccessTokenAsync(refreshToken);

            if (newTokens == null)
                return Unauthorized();

            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newTokens.ExpiresAt
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newTokens.RefreshTokenExpiresAt
            };

            Response.Cookies.Append(
                "Acceso_Token",
                newTokens.AccessToken,
                accessOptions);

            Response.Cookies.Append(
                "RefreshToken",
                newTokens.RefreshToken,
                refreshOptions);

            return Ok();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}