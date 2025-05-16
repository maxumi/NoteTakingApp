using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiCrossplatformApp.Messenger
{
    public class RefreshMessenger : ValueChangedMessage<string>
    {
        public RefreshMessenger(string value) : base(value)
        {
        }
    }
}
