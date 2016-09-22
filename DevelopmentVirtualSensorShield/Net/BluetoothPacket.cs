using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentVirtualSensorShield.Net
{
    [DataContract]
    public class BluetoothPacket
    {
        [DataMember]
        public DateTimeOffset Timestamp
        {
            get;
            set;
        }

        [DataMember]
        public String JSonPayload
        {
            get;
            set;
        }

        #region .ctor

        public BluetoothPacket()
        {

        }

        public BluetoothPacket(String jsonPayload)
        {
            if (String.IsNullOrEmpty(jsonPayload))
                throw new ArgumentNullException("jsonPayload");

            this.Timestamp = DateTime.Now;
            this.JSonPayload = jsonPayload;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            String json = null;

            json = JsonConvert.SerializeObject(this);
            return json;
        }

        #endregion
    }
}
