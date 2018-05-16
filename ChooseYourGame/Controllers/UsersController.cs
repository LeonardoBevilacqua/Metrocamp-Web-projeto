using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChooseYourGame.Controllers
{
    public class UsersController : Controller
    {
        private readonly ChooseYourGameContext _contexto;

        public UsersController(ChooseYourGameContext contexto)
        {
            _contexto = contexto;
        }

        
    }
}