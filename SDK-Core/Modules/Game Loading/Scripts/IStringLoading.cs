#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Watermelon
{
    public interface IStringLoading
    {
#if UNITASK_ENABLED
        public UniTask<string> GetStringAsync();
#else
        public Task<string> GetStringAsync();
#endif
    }
}