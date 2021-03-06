﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA.Application.Domain.Commands;
using SGA.Application.Domain.Queries;
using SGA.Application.UI.Models;
using System.Linq;

namespace SGA.UI.Site.Controllers
{
    public class PetController : BaseController
    {
        private readonly IRegisterNewPetCommand _command;
        private readonly IPetQuery _petQuery;


        public PetController(IPetQuery petQuery, IRegisterNewPetCommand command)
        {
            _petQuery = petQuery;
            _command = command;
        }

        public IActionResult Index()
        {
            return SafeResultResponse(() =>
            {
                LoadTypePets();

                return View(new PetViewModel());

            }, RedirectToHome);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PetViewModel model)
        {
            return SafeResultResponse(() =>
            {
                if (!ModelState.IsValid)
                {
                    LoadTypePets();

                    return View(model);
                }

                _command.Execute(model);

                if (!_command.HasErrors())
                {
                    NotifySucess();

                    return RedirectToHome();
                }

                LoadTypePets();

                NotifyError(string.Join(",", _command.GetErrors()));

                return View(model);

            }, RedirectToHome);
        }

        private void LoadTypePets()
        {
            ViewBag.TypePetId = _petQuery.GetTypePets()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Description });
        }
    }
}