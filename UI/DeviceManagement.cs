using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using PInvoke;

namespace UI
{
    class DeviceManagement
    {
        private IntPtr DeviceNotificationHandle = IntPtr.Zero;
        private IntPtr SubclassedWindowHandler = IntPtr.Zero;

        public Boolean SubscribeDeviceNotification(IntPtr hRecipient, string InterfaceGuid)
        {
            //this.SubclassHWnd(hRecipient);
            //SubclassedWindowHandler = hRecipient;

            Dbt.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new Dbt.DEV_BROADCAST_DEVICEINTERFACE(InterfaceGuid);
            this.DeviceNotificationHandle = Winuser.RegisterDeviceNotification(hRecipient, devBroadcastDeviceInterface, Dbt.DEVICE_NOTIFY_WINDOW_HANDLE);
            return (this.DeviceNotificationHandle == IntPtr.Zero) ? false : true;
        }

        public Boolean UnsubscribeDeviceNotification()
        {
            //this.DeSubclassHWnd(this.SubclassedWindowHandler);

            return Winuser.UnregisterDeviceNotification(this.DeviceNotificationHandle);
        }

    }
}

// http://msdn.microsoft.com/en-us/library/4ca6d5z7.aspx
