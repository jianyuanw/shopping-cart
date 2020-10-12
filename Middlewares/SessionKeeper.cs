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

        public async Task Invoke(HttpContext context, ITempDataDictionaryFactory factory, DbT10Software db)
        {
            // check and get exisiting user lastaccesstime
            string lastAccess = context.Request.Cookies["lastAccessTime"];

            // Check if lastAccessTime session object is available
            if (lastAccess == null)
            {
                // When lastAccessTime is null, it is a new session
                context.Response.Cookies.Append("lastAccessTime", DateTime.Now.ToString(), new CookieOptions
                {
                    Secure = true,
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
                    context.Response.Redirect("/SessionTimeout/Index");

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

                    // Uses injected TempData factory to fetch TempData from context before inserting text
                    ITempDataDictionary TempData = factory.GetTempData(context);
                    TempData["Alert"] = "warning|Your session has timed-out!";

                    return;
                } 
                else
                {
                    // if user still activ, keep Update last access time stamp
                    context.Response.Cookies.Append("lastAccessTime", DateTime.Now.ToString(), new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax
                    });
                }
            }
              
            await next(context);
        }
        
    }
}
