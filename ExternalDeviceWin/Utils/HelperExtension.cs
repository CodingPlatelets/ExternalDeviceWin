using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ExternalDeviceWin.Utils
{
    public static class HelperExtension
    {
        public static unsafe T ReadAs<T>(this Socket socket) where T : unmanaged
        {
            int size = sizeof(T);
            byte[] buf = new byte[size];

            int received = socket.Receive(buf, SocketFlags.None);
            if (received != size)
            {
                return default(T);
            }

            IntPtr ptrElem = Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
            return Marshal.PtrToStructure<T>(ptrElem);
        }

        public static unsafe T ReadAndUnpackAs<T>(this Socket socket) where T : unmanaged
        {
            int size = sizeof(T);
            byte[] buf = new byte[size];

            int received = socket.Receive(buf, SocketFlags.None);
            if (received != size)
            {
                return default(T);
            }

            fixed (byte* pByte = buf)
            {
                int* intBytes = (int*)pByte;
                unpack(intBytes, size);
            }

            IntPtr ptrElem = Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
            return Marshal.PtrToStructure<T>(ptrElem);
        }

        public static unsafe void pack(int* data, int size)
        {
            int i;
            size = size / 4;
            for (i = 0; i < size; i++)
            {
                data[i] = IPAddress.HostToNetworkOrder(data[i]);
            }
            //swap setup
            i = data[size - 1];
            data[size - 1] = data[size - 2];
            data[size - 2] = i;
        }

        static unsafe void unpack(int* data, int size)
        {
            int i;
            size = size / 4;
            if(size < 2)
            {
                throw new ExternalException("size must at least 8 byte");
            }
            for (i = 0; i < size; i++)
            {
                data[i] = IPAddress.NetworkToHostOrder(data[i]);
            }

            i = data[size - 1];
            data[size - 1] = data[size - 2];
            data[size - 2] = i;
        }

        public static unsafe bool SendAs<T>(this Socket socket, T data) where T : unmanaged
        {
            int size = sizeof(T);
            byte[] buf = new byte[size];

            IntPtr ptrElem = Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0);
            Marshal.StructureToPtr<T>(data, ptrElem, true);

            int sent = socket.Send(buf, SocketFlags.None);
            if (sent != size)
            {
                return false;
            }

            return true;
        }

        public static string ReadBusId(this Socket socket)
        {
            byte[] busId = new byte[32];
            if (socket.Receive(busId) != 32)
            {
                return string.Empty;
            }

            var handle = GCHandle.Alloc(busId, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStringAnsi(handle.AddrOfPinnedObject())  ?? string.Empty;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
