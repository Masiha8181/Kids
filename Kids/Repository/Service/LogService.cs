using Kids.Context;
using Kids.Hubs;
using Kids.Models;
using Kids.Repository.Interface;
using Microsoft.AspNetCore.SignalR;

namespace Kids.Repository.Service
{
    public class LogService : ILogService
    {
        private readonly KidsContext _context;
        private readonly IHubContext<LogHub> _hubContext;

        public LogService(KidsContext context, IHubContext<LogHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task LogActivity(string message, HttpContext context)
        {
            var location = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            var url = location.AbsoluteUri;
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "ناشناخته";
            var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "نامشخص";

            string user = "کاربر ناشناس";
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                user = context.User.Claims.FirstOrDefault(p => p.Type == "PhoneNumber")?.Value ?? "کاربر لاگین کرده ولی بدون شماره";
            }

            var log = new Log
            {
                Address = url,
                CreateDate = DateTime.Now,
                Device = userAgent,
                Massege = message,
                UserName = user,
                IP = ip
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message, url, ip, time, userAgent, user);
        }
    }

}
