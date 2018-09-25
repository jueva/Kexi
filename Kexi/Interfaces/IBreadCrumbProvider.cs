using System.Threading.Tasks;

namespace Kexi.Interfaces
{
    public interface IBreadCrumbProvider
    {
        Task<bool> DoBreadcrumbAction(string breadPath);
    }
}