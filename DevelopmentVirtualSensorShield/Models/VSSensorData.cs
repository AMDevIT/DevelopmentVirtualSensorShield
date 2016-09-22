using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentVirtualSensorShield.Models
{
    [DataContract]
    public class VSSensorData
    {
        #region Properties

        [DataMember]
        public DateTime Timestamp
        {
            get;
            set;
        }

        [DataMember]
        public double AccelerationX
        {
            get;
            set;
        }

        [DataMember]
        public double AccelerationY
        {
            get;
            set;
        }

        [DataMember]
        public double AccelerationZ
        {
            get;
            set;
        }

        #endregion
    }
}
