using System.Threading.Tasks;

namespace BillTracker.Shared
{
    public interface IHandle<in TInput, TOutput>
    {
        Task<TOutput> Handle(TInput input);
    }
}
