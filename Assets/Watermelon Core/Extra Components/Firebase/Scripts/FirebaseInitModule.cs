using Watermelon;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Firebase")]
    public class FirebaseInitModule : InitModule
    {
        public override string ModuleName => "Firebase";

        public override void CreateComponent(GameObject holderObject)
        {
            Firebase.InitSDK().Forget();
        }
    }
}