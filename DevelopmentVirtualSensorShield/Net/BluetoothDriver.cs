using DevelopmentVirtualSensorShield.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace DevelopmentVirtualSensorShield.Net
{
    public class BluetoothDriver
    {
        #region Consts

        private const String VirtualSensorsShieldServiceUUID = "F28CF798-90A2-4EEB-9F08-5F56CF1A345A";
        private const String VirtualSensorsShieldServiceName = "Virtual Sensors Shield";
        private const byte SdpServiceNameAttributeType = (4 << 3) | 5;        
        private const UInt16 SdpServiceNameAttributeId = 0x100;                                          // The Id of the Service Name SDP attribute

        #endregion

        #region Fields

        private RfcommServiceProvider rfCommServiceProvider = null;
        private StreamSocket streamSocket = null;
        private DataReader dataReader = null;
        private DataWriter dataWriter = null;
        private StreamSocketListener socketListener = null;
        private BluetoothDevice remoteDevice = null;

        #endregion

        #region .ctor

        public BluetoothDriver()
        {
        }        

        #endregion

        #region Methods

        public async Task Start()
        {
            Guid serviceID = Guid.Empty;
            Guid advertisementID = Guid.Empty;
            String serviceIDString = null;

            serviceID = new Guid(VirtualSensorsShieldServiceUUID);

            Tracer.Default.Log(String.Format("Creating RfCommServiceProvider with service ID {0}.", 
                               serviceID.ToString()), 
                               LogLevels.Verbose);
            try
            {
                this.rfCommServiceProvider = await RfcommServiceProvider.CreateAsync(RfcommServiceId.FromUuid(serviceID));
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x800710DF)
            {
                // The Bluetooth radio may be off.
                System.Diagnostics.Debug.WriteLine("Make sure your Bluetooth Radio is on: " + ex.Message);
                return;
            }

            if (this.rfCommServiceProvider != null)
            {
                Tracer.Default.Log(String.Format("RfCommServiceProvider created with service ID {0}.", serviceID.ToString()));

                this.socketListener = new StreamSocketListener();
                this.socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                Tracer.Default.Log("StreamSocketListener created.");

                serviceIDString = this.rfCommServiceProvider.ServiceId.AsString();

                try
                {
                    await this.socketListener.BindServiceNameAsync(serviceIDString, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                }
                catch(Exception exc)
                {
                    Tracer.Default.Log(exc.ToString(), LogLevels.Error);
                }

                Tracer.Default.Log(String.Format("StreamSocketListener bound to service {0}.", serviceIDString));

                this.SetServiceAttribute();
                this.rfCommServiceProvider.StartAdvertising(this.socketListener, true);

                Tracer.Default.Log("Bluetooth advertising started");
            }
            else
            {
                Tracer.Default.Log(String.Format("Cannot create RfCommServiceProvider with service ID {0}.", serviceID.ToString()),
                                   LogLevels.Error);
            }
        }           
        
        private void Stop()
        {
            if (this.remoteDevice != null)
            {
                this.remoteDevice.Dispose();
                this.remoteDevice = null;
            }

            if (this.rfCommServiceProvider != null)
            {
                this.rfCommServiceProvider.StopAdvertising();
                this.rfCommServiceProvider = null;
            }

            if (this.socketListener != null)
            {
                this.socketListener.Dispose();
                this.socketListener = null;
            }

            if (this.dataWriter != null)
            {
                this.dataWriter.Dispose();
                this.dataWriter = null;
            }

            if (this.dataReader != null)
            {
                this.dataReader.Dispose();
                this.dataReader = null;
            }

            if (this.streamSocket != null)
            {
                this.streamSocket.Dispose();
                this.streamSocket = null;
            }
        }     

        public void Send(object data)
        {

        }

        private void SetServiceAttribute()
        {
            DataWriter attributeWriter = null;

            attributeWriter = new DataWriter();
            attributeWriter.WriteByte(SdpServiceNameAttributeType);
            attributeWriter.WriteByte((byte)VirtualSensorsShieldServiceName.Length);
            attributeWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            attributeWriter.WriteString(VirtualSensorsShieldServiceName);

            if (this.rfCommServiceProvider != null)
                this.rfCommServiceProvider.SdpRawAttributes.Add(SdpServiceNameAttributeId, 
                                                                attributeWriter.DetachBuffer());

        }

        #endregion

        #region Event Handlers

        private async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            this.streamSocket = args.Socket;

            this.remoteDevice = await BluetoothDevice.FromHostNameAsync(this.streamSocket.Information.RemoteHostName);
            this.dataWriter = new DataWriter(this.streamSocket.OutputStream);
            this.dataReader = new DataReader(this.streamSocket.InputStream);

            Tracer.Default.Log(String.Format("Connected to remote device {0}", this.remoteDevice.Name));
        }

        private void BluetoothLEPublisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            Tracer.Default.Log(String.Format("BT LE Publisher status: {0}, Error: {1}", 
                                             args.Status, 
                                             args.Error));
        }

        #endregion

    }
}
