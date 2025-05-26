namespace Kids.Repository.Interface;

public interface ILogService
{
    Task LogActivity(string message, HttpContext context);
}