using System.Threading.Tasks;

namespace Notifications.Worker.Domain.Interfaces;

public interface ITemplateRenderer
{
    Task<string> RenderAsync<T>(string templateName, T model);
}
