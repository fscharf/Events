﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Events.Models;

namespace Events.Controllers
{
    [Authorize(Roles = "3,4,5")]
    public class AdminController : Controller
    {
        public ActionResult Index() => View();

        // Admin for Events
        public ActionResult EventsList()
        {
            IEnumerable<EVENTO> eventList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventList = response.Content.ReadAsAsync<IEnumerable<EVENTO>>().Result;
            return View(eventList);
        }

        public ActionResult EventDetails(int id = 0)
        {
            if (id == 0)
            {
                return View(new EVENTO());
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("events/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<EVENTO>().Result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EventDetails(EVENTO eVENTO, int id = 0)
        {
            if (id == 0)
            {
                TempData["Error"] = "Evento não localizado.";
                return View(eVENTO);
            }
            else
            {
                if (Request.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in Request.Files)
                    {
                        var postedFile = Request.Files[file];
                        eVENTO.IMAGEM_URL = "~/Image/" + postedFile.FileName;
                        var filePath = Path.Combine(Server.MapPath("~/Image/"), postedFile.FileName);
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                }

                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("events/" + id.ToString(), eVENTO).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Dados atualizados com sucesso!";
                    return RedirectToAction("EventsList", "Admin");
                }
                else
                {
                    TempData["Error"] = "Ocorreu um erro inesperado.";
                    return View(eVENTO);
                }
            }
        }

        public ActionResult EventCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EventCreate(EVENTO eVENTO)
        {
            if (Request.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in Request.Files)
                {
                    var postedFile = Request.Files[file];
                    eVENTO.IMAGEM_URL = "~/Image/" + postedFile.FileName;
                    var filePath = Path.Combine(Server.MapPath("~/Image/"), postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }

                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("events", eVENTO).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Evento criado com sucesso.";
                    return RedirectToAction("EventsList", "Admin");
                }
                else
                {
                    TempData["Error"] = "Ocorreu um erro ao enviar os dados para API.";
                    return View("EventCreate", eVENTO);
                }
            }
            else
            {
                TempData["Error"] = "Por favor, faça upload de uma imagem.";
                return View("EventCreate", eVENTO);
            }
        }

        public ActionResult EventDelete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("events/" + id.ToString()).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Evento excluído com sucesso.";
                return RedirectToAction("EventsList", "Admin");
            }
            else
            {
                TempData["Error"] = "Ocorreu um erro ao enviar sua requisição.";
                return View();
            }
        }

        // Admin for Users
        public ActionResult UsersList()
        {
            IEnumerable<USUARIO> userList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("users").Result;
            userList = response.Content.ReadAsAsync<IEnumerable<USUARIO>>().Result;
            return View(userList);
        }

        public ActionResult UserDetails(int id = 0)
        {
            if (id == 0)
            {
                return View(new USUARIO());
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("users/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<USUARIO>().Result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDetails(USUARIO uSUARIO, int id = 0)
        {
            if (id == 0)
            {
                TempData["Error"] = "Usuário não localizado.";
                return View(uSUARIO);
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("users/" + id.ToString(), uSUARIO).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Dados atualizados com sucesso!";
                    return RedirectToAction("UsersList", "Admin");
                }
                else
                {
                    TempData["Error"] = "Ocorreu um erro inesperado.";
                    return View(uSUARIO);
                }
            }
        }

        public ActionResult UserCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreate(USUARIO uSUARIO)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("users", uSUARIO).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Usuário criado com sucesso.";
                return RedirectToAction("UsersList", "Admin");
            }
            else
            {
                TempData["Error"] = "Ocorreu um erro ao enviar os dados para API.";
                return View("UserCreate", uSUARIO);
            }
        }

        public ActionResult UserDelete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("users/" + id.ToString()).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Usuário excluído com sucesso.";
                return RedirectToAction("UsersList", "Admin");
            }
            else
            {
                TempData["Error"] = "Ocorreu um erro ao enviar sua requisição.";
                return View();
            }
        }
    }
}