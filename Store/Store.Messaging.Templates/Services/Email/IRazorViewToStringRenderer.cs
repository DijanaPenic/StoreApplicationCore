using System.Threading.Tasks;

namespace Store.Messaging.Templates.Services.Email
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}