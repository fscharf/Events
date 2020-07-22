﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Events
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "entrar",
                "entrar",
                new { controller = "Login", action = "Login" }
            );

            routes.MapRoute(
                "autenticar",
                "autenticar",
                new { controller = "Login", action = "Auth" }
            );

            routes.MapRoute(
                "sair",
                "sair",
                new { controller = "Login", action = "Logout" }
            );

            routes.MapRoute(
                "cadastro",
                "cadastro",
                new { controller = "Register", action = "Register" }
            );

            routes.MapRoute(
                "cadastro_criar",
                "cadastro/criar",
                new { controller = "Register", action = "Create" }
            );

            routes.MapRoute(
                "admin",
                "admin",
                new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                "eventos",
                "eventos",
                new { controller = "Events", action = "Index" }
            );

            routes.MapRoute(
                "eventos_inscricao",
                "eventos/inscricao",
                new { controller = "Events", action = "Confirm" }
            );
            
            routes.MapRoute(
                "eventos_tituloevento",
                "eventos/titulo-evento",
                new { controller = "Events", action = "Event" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
