﻿using Events.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Events.Controllers
{
    public class EventsController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(int? page, string currentFilter, string searchString, string searchDate)
        {
            IEnumerable<EVENTO> eventList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventList = response.Content.ReadAsAsync<IEnumerable<EVENTO>>().Result;
            if (searchString != null || searchDate != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                eventList = eventList.Where(x => x.TITULO.Contains(searchString)
                                            || x.DESCRICAO.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(searchDate))
            {
                eventList = eventList.Where(x => x.DATA.Equals(Convert.ToDateTime(searchDate)));
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            eventList = eventList.Where(x => x.ATIVO == 1);

            return View(eventList.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id = 0)
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

        [Authorize]
        public ActionResult Subscribe(INSCRICAO iNSCRICAO, int id = 0)
        {
            IEnumerable<EVENTO> eventsList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventsList = response.Content.ReadAsAsync<IEnumerable<EVENTO>>().Result;

            var eventDetails = eventsList.Where(x => x.COD_EVENTO == id).FirstOrDefault();

            var userAuth = (ClaimsIdentity)User.Identity;
            if (userAuth.IsAuthenticated)
            {
                var identity = userAuth.Claims.Where(c => c.Type == ClaimTypes.Sid).FirstOrDefault().Value;

                IEnumerable<INSCRICAO> subsList;
                HttpResponseMessage responseMessage = GlobalVariables.WebApiClient.GetAsync("subs").Result;
                if (responseMessage.IsSuccessStatusCode)
                {                   
                    subsList = responseMessage.Content.ReadAsAsync<IEnumerable<INSCRICAO>>().Result;
                    var subExists = subsList.Where(x => x.COD_USUARIO == Convert.ToInt32(identity) && x.COD_EVENTO == id)
                                            .Any(x => x.COD_EVENTO == id && x.COD_USUARIO == Convert.ToInt32(identity));

                    if (userAuth.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "3" || c.Value == "4" || c.Value == "5")))
                    {
                        TempData["Error"] = "Seu nível de acesso não permite participar de eventos.";
                        return Redirect("/Events/Details/" + id);
                    }
                    else if (subExists)
                    {
                        TempData["Error"] = "Você já está cadastrado nesse evento.";
                        return Redirect("/Events/Details/" + id);
                    }
                    else
                    {
                        iNSCRICAO.COD_USUARIO = Convert.ToInt32(identity);
                        iNSCRICAO.COD_EVENTO = id;
                        eventDetails.VAGAS -= 1;

                        response = GlobalVariables.WebApiClient.PutAsJsonAsync("events/" + id.ToString(), eventDetails).Result;
                        responseMessage = GlobalVariables.WebApiClient.PostAsJsonAsync("subs", iNSCRICAO).Result;
                        TempData["Success"] = "Inscrição realizada com sucesso!";
                        return RedirectToAction("MyEvents");
                    }
                }
                else
                {
                    TempData["Error"] = "Ocorreu um erro ao efetuar sua requisição.";
                    return Redirect("/Events/Details/" + id);
                }
            }
            else
            {
                TempData["Error"] = "Inicie a sessão para continuar.";
                return RedirectToAction("Login", "Users");
            }
        }

        [Authorize(Roles = "1,2")]
        public ActionResult MyEvents()
        {
            List<INSCRICAO> subsList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("subs").Result;
            subsList = response.Content.ReadAsAsync<List<INSCRICAO>>().Result;
            
            List<EVENTO> eventsList;
            response = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventsList = response.Content.ReadAsAsync<List<EVENTO>>().Result;

            SubsViewModel subsViewModel = new SubsViewModel();
            subsViewModel.InscricaoVM = subsList;
            subsViewModel.EventoVM = eventsList;

            return View(subsViewModel);
        }

        [Authorize]
        public ActionResult DeleteSub(int id)
        {

            IEnumerable<INSCRICAO> subsList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("subs").Result;
            subsList = response.Content.ReadAsAsync<IEnumerable<INSCRICAO>>().Result;
            var subDetails = subsList.Where(x => x.COD_INSCRICAO == id).FirstOrDefault();

            IEnumerable<EVENTO> eventsList;
            HttpResponseMessage responseMessage = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventsList = responseMessage.Content.ReadAsAsync<IEnumerable<EVENTO>>().Result;
            var eventDetails = eventsList.Where(x => x.COD_EVENTO == subDetails.COD_EVENTO).FirstOrDefault();

            responseMessage = GlobalVariables.WebApiClient.DeleteAsync("subs/" + id.ToString()).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                eventDetails.VAGAS += 1;

                response = GlobalVariables.WebApiClient.PutAsJsonAsync("events/" + eventDetails.COD_EVENTO.ToString(), eventDetails).Result;
                TempData["Success"] = "Inscrição cancelada com sucesso.";
                return RedirectToAction("MyEvents", "Events");
            }
            else
            {
                TempData["Error"] = "Ocorreu um erro ao enviar sua requisição.";
                return RedirectToAction("MyEvents", "Events");
            }
        }

        [AllowAnonymous]
        public ActionResult Subscription(int? id)
        {
            SubsViewModel subsViewModel = new SubsViewModel();
            subsViewModel.COD_INSCRICAO = id;

            return View(subsViewModel);
        }

        [AllowAnonymous]
        public ActionResult GeneratePDF(int? id)
        {
            return new Rotativa.ActionAsPdf("Subscription/" + id.ToString());
        }


        [AllowAnonymous]
        public ActionResult Certificate(int? id)
        {
            List<INSCRICAO> subsList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("subs").Result;
            subsList = response.Content.ReadAsAsync<List<INSCRICAO>>().Result;

            var subDetails = subsList.Where(x => x.COD_INSCRICAO == id).FirstOrDefault();

            List<EVENTO> eventsList;
            response = GlobalVariables.WebApiClient.GetAsync("events").Result;
            eventsList = response.Content.ReadAsAsync<List<EVENTO>>().Result;

            var eventDetails = eventsList.Where(x => x.COD_EVENTO == subDetails.COD_EVENTO).FirstOrDefault();

            List<USUARIO> usersList;
            response = GlobalVariables.WebApiClient.GetAsync("users").Result;
            usersList = response.Content.ReadAsAsync<List<USUARIO>>().Result;

            var userDetails = usersList.Where(x => x.COD_USUARIO == subDetails.COD_USUARIO).FirstOrDefault();

            SubsViewModel subsViewModel = new SubsViewModel();
            subsViewModel.NOME = userDetails.NOME;
            subsViewModel.TITULO = eventDetails.TITULO;
            subsViewModel.DATA = eventDetails.DATA;
            subsViewModel.DATA_HORA_PARTICIPACAO = subDetails.DATA_HORA_PARTICIPACAO;
            subsViewModel.COD_INSCRICAO = id;

            return View(subsViewModel);
        }

        [AllowAnonymous]
        public ActionResult NewPDF(int? id)
        {
            var newPDF = new Rotativa.ActionAsPdf("Certificate/" + id.ToString())
            {
                PageOrientation = 0,
                PageSize = Rotativa.Options.Size.A5             
            };
            return newPDF;
        }
    }
}