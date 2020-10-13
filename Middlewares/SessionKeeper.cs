using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using SA51_CA_Project_Team10.DBs;
using SA51_CA_Project_Team10.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace SA51_CA_Project_Team10.Middlewares
{
    public class SessionKeeper
    {
        private readonly RequestDelegate next;

        public SessionKeeper (RequestDelegate next)
        {
            this.next = next;
            
        }

        public async Task Invoke(HttpContext context, ITempDataProvider tdp, DbT10Software db)
        {
            // check and get exisiting user lastaccesstime
            string lastAccess = context.Request.Cookies["lastAccessTime"];

            // Check if lastAccessTime session object is available
            if (lastAccess == null)
            {
                // When lastAccessTime is null, it is a new session
                context.Response.Cookies.Append("lastAccessTime", DateTime.Now.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });

            } else
            {
                // When lastAccessTime is present, check if it has passed 20 minutes
                DateTime lastAccessDateTime = Convert.ToDateTime(lastAccess);

                // If now is more than 20 mins from lastAccessTime
                if (DateTime.Now.CompareTo(lastAccessDateTime.AddMinutes(20)) == 1)
                {
                    //if user not active, remove the last accesstime and redirect to session timeout controller
                    //controller will clean up session and redirect to gallery page with session timeout message
                    context.Response.Cookies.Delete("lastAccessTime");

                    string sessionId = context.Request.Cookies["sessionId"];

                    if (sessionId != null)
                    {
                        var session = db.Sessions.FirstOrDefault(session => session.Id == sessionId);

                        if (session != null)
                        {
                            db.Sessions.Remove(session);
                            db.SaveChanges();
                        }
                    }

                    context.Response.Cookies.Delete("sessionId");

                    // Uses injected TempDataProvider to add TempData into context without controller
                    tdp.SaveTempData(context, new Dictionary<string, object> { ["Alert"] = "warning|Your session has timed-out!" });

                    context.Response.Redirect("/Gallery/Index");

                    return;
                } 
                else
                {
                    // if user still active, keep Update last access time stamp
                    context.Response.Cookies.Append("lastAccessTime", DateTime.Now.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax
                    });
                }
            }
              
            await next(context);
        }
        
    }
}
