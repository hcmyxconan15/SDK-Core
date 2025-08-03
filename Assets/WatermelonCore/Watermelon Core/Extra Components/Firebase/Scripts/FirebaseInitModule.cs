#if UNITASK_ENABLED
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
#if FIREBASE_ENABLED
using Firebase.Extensions;
#endif
using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Firebase")]
    public class FirebaseInitModule : InitModule
    {
        public override string ModuleName => "Firebase";

        public override void CreateComponent(GameObject holderObject)
        {
#if FIREBASE_ENABLED
#if UNITASK_ENABLED
            Firebase.InitSDK().Forget();
#else
            Firebase.InitSDK();
#endif
#endif
        }
    }
}