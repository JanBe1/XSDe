using XSDe.Models;

namespace XSDe.Services.Executors
{
    public interface IActionExecutor
    {
        Task ExecuteAsync(ButtonMapping mapping, CancellationToken cancellationToken = default);
    }
}
