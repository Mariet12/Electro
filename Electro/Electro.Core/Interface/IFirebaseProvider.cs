// File: Core/Interface/IFirebaseProvider.cs
using FirebaseAdmin.Messaging;

namespace Electro.Core.Interface
{
    public interface IFirebaseProvider
    {
        FirebaseAdmin.Messaging.FirebaseMessaging? CustomerMessaging { get; }
        FirebaseAdmin.Messaging.FirebaseMessaging? GetByRole(string role);
        bool IsReady { get; }
    }

}
