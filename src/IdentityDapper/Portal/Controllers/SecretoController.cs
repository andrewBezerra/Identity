using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDapperCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;

namespace Portal.Controllers
{
    public class SecretoController : Controller
    {
        private readonly SignInManager<ApplicationUser> signinmanager;
        public UserManager<ApplicationUser> usermanager { get; }

        public SecretoController(SignInManager<ApplicationUser> _signinmanager,
                                 UserManager<ApplicationUser> _usermanager)
        {
            signinmanager = _signinmanager;
            usermanager = _usermanager;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Index2()
        {
            return View();
        }

        public IActionResult Deny()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var user =await usermanager.FindByEmailAsync(login.Usuario);

            if (user == null)
            {
                ModelState.AddModelError("Falha na autenticação", "Usuário ou senha incorretos.");
                return View();
            }

            var result = await signinmanager.PasswordSignInAsync(user, login.Senha, true, false);
           
          
            return View();
        }

    }
}
