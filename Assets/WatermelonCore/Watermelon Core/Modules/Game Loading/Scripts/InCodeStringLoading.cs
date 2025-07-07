#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Watermelon
{
    public struct InCodeStringLoading : IStringLoading
    {
        private string _strResult;

        public InCodeStringLoading(string strResult)
        {
            _strResult = strResult;
        }

#if UNITASK_ENABLED
        public UniTask<string> GetStringAsync()
        {
            return new UniTask<string>(_strResult);
        }        
#else
        public Task<string> GetStringAsync()
        {
            return Task.FromResult(_strResult);
        }
#endif
    }
}