using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Kids.Context;
using Kids.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Kids.Hubs
{
    public class LogHub:Hub
    {
        private readonly KidsContext _context;

        public LogHub(KidsContext context)
        {
            _context = context;
        }
        public async Task LoadActivityTask(string messege)
        {
            var httpContext = Context.GetHttpContext();

            var location = new Uri($"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}{httpContext?.Request.Path}{httpContext?.Request.QueryString}");
            var url = location.AbsoluteUri;

            var ip = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "ناشناخته";
            var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "نامشخص";

            string user = "کاربر ناشناس";
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                user = httpContext.User.Claims.FirstOrDefault(p => p.Type == "PhoneNumber")?.Value ?? "کاربر لاگین کرده ولی بدون شماره";
            }

            Log log = new Log()
            {
                Address = url,
                CreateDate = DateTime.Now,
                Device = userAgent,
                Massege = messege,
                UserName = user,
                IP = ip
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", messege, url, ip, time, userAgent, user);
        }

    }
}
