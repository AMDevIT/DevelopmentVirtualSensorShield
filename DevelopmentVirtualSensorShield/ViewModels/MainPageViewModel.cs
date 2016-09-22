using DevelopmentVirtualSensorShield.Diagnostics;
using DevelopmentVirtualSensorShield.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentVirtualSensorShield.ViewModels
{
    public class MainPageViewModel
        : ViewModelBase
    {
        #region Fields

        private DateTimeOffset lastTimestamp;
        private double pressure;
        private double altitude;
        private double? trueNorthHeading;
        private double accelerationX;
        private double accelerationY;
        private double accelerationZ;

        #endregion

        #region Properties

        public DateTimeOffset LastTimestamp
        {
            get
            {
                return this.lastTimestamp;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.lastTimestamp, value);
                });
                
            }
        }

        public double Pressure
        {
            get
            {
                return this.pressure;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.pressure, value);
                });                
            }
        }

        public double Altitude
        {
            get
            {
                return this.altitude;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.altitude, value);
                });                
            }
        }
        
        public double? TrueNorthHeading
        {
            get
            {
                return this.trueNorthHeading;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.trueNorthHeading, value);
                });                
            }
        }

        public double AccelerationX
        {
            get
            {
                return this.accelerationX;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.accelerationX, value);
                });                
            }
        }

        public double AccelerationY
        {
            get
            {
                return this.accelerationY;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.accelerationY, value);
                });                
            }
        }

        public double AccelerationZ
        {
            get
            {
                return this.accelerationZ;
            }
            private set
            {
                this.Dispatch(() =>
                {
                    this.SetProperty(ref this.accelerationZ, value);
                });                
            }
        }

        #endregion

        #region .ctor

        public MainPageViewModel()
        {
            App currentApplication = null;

            currentApplication = App.Current as App;
            currentApplication.HardwareMonitor.StartSensors();

            Tracer.Default.VerboseLevel = LogLevels.Error;

#if INTERACTIVE_MODE
           
            if (currentApplication != null && currentApplication.HardwareMonitor != null)
                currentApplication.HardwareMonitor.ReadingChanged += HardwareMonitor_ReadingChanged;
#endif 

        }

        #endregion

        #region Event Handlers

        private void HardwareMonitor_ReadingChanged(object sender, EventArgs e)
        {
            HardwareMonitor senderMonitor = null;

            senderMonitor = sender as HardwareMonitor;
#if DEBUG
            StringBuilder debugDataBuilder = null;

            if (senderMonitor != null)
            {
                debugDataBuilder = new StringBuilder();
                debugDataBuilder.AppendFormat("Sensors data ({0})", senderMonitor.Timestamp.ToString());
                debugDataBuilder.AppendLine();
                debugDataBuilder.Append("Pressure (hpa): ");
                debugDataBuilder.AppendLine(senderMonitor.Pressure.ToString());
                debugDataBuilder.Append("Altitude change (mt): ");
                debugDataBuilder.AppendLine(senderMonitor.AltitudeChange.ToString());
                if (senderMonitor.TrueNorthHeading != null)
                {
                    debugDataBuilder.AppendFormat("Compass data. True north heading: {0}, Accuracy: {1}",
                                                  senderMonitor.TrueNorthHeading.Value.ToString(),
                                                  senderMonitor.CompassAccuracy.ToString());
                    debugDataBuilder.AppendLine();
                }
                else
                    debugDataBuilder.AppendLine("Compass data not available.");
                debugDataBuilder.AppendFormat("AccelerationX: {0} AccelerationY: {1} AccelerationZ: {2}",
                                              senderMonitor.AccelerationX,
                                              senderMonitor.AccelerationY,
                                              senderMonitor.AccelerationZ);
                debugDataBuilder.AppendLine();
                Tracer.Default.Log(debugDataBuilder.ToString(), LogLevels.Verbose);
            }
#endif 

            if (senderMonitor != null)
            {
                this.LastTimestamp = senderMonitor.Timestamp;

                this.AccelerationX = senderMonitor.AccelerationX;
                this.AccelerationY = senderMonitor.AccelerationY;
                this.AccelerationZ = senderMonitor.AccelerationZ;

                this.Altitude = senderMonitor.AltitudeChange;
                this.Pressure = senderMonitor.Pressure;
            }
        }

        #endregion
    }
}
