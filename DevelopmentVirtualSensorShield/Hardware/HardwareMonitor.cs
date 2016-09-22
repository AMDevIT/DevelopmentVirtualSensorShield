using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace DevelopmentVirtualSensorShield.Hardware
{
    public class HardwareMonitor
    {
        #region Events

        public event EventHandler ReadingChanged;

        #endregion

        #region Fields

        private object lockObject = null;
        private Gyrometer gyrometer = null;
        private Accelerometer accelerometer = null;
        private Barometer barometer = null;
        private Altimeter altimeter = null;
        private Compass compass = null;

        #endregion

        #region Properties

        /// <summary>
        /// Timestamp of the latest changed sensor reading.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get;
            private set;
        }

        public double AccelerationX
        {
            get;
            private set;
        }

        public double AccelerationY
        {
            get;
            private set;
        }

        public double AccelerationZ
        {
            get;
            private set;
        }

        /// <summary>
        /// Altitude change in meters
        /// </summary>
        public double AltitudeChange
        {
            get;
            private set;
        }

        /// <summary>
        /// True north compass heading
        /// </summary>
        public double? TrueNorthHeading
        {
            get;
            private set;
        } 

        /// <summary>
        /// Current compass accuracy
        /// </summary>
        public MagnetometerAccuracy CompassAccuracy
        {
            get;
            private set;
        }

        /// <summary>
        /// Pressure (hpa)
        /// </summary>
        public double Pressure
        {
            get;
            private set;
        }

        #endregion

        #region .ctor

        public HardwareMonitor()
        {
            this.lockObject = new object();            
        }

        #endregion

        #region Methods

        public void StartSensors()
        {
            this.gyrometer = Gyrometer.GetDefault();
            this.accelerometer = Accelerometer.GetDefault();
            this.compass = Compass.GetDefault();
            this.barometer = Barometer.GetDefault();
            this.altimeter = Altimeter.GetDefault();
            

            if (this.gyrometer != null)
                this.gyrometer.ReadingChanged += Gyrometer_ReadingChanged;

            if (this.accelerometer != null)
                this.accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

            if (this.compass != null)
                this.compass.ReadingChanged += Compass_ReadingChanged;

            if (this.barometer != null)
                this.barometer.ReadingChanged += Barometer_ReadingChanged;

            try
            {
                if (this.altimeter != null)
                    this.altimeter.ReadingChanged += Altimeter_ReadingChanged;
            }
            catch(Exception)
            {

            }
        }        

        public void DisposeSensors()
        {            
            try
            {
                if (this.gyrometer != null)
                    this.gyrometer.ReadingChanged -= Gyrometer_ReadingChanged;

                if (this.accelerometer != null)
                    this.accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;

                if (this.compass != null)
                    this.compass.ReadingChanged -= Compass_ReadingChanged;

                if (this.barometer != null)
                    this.barometer.ReadingChanged -= Barometer_ReadingChanged;

                if (this.altimeter != null)
                    this.altimeter.ReadingChanged -= Altimeter_ReadingChanged;

                this.gyrometer = null;
                this.accelerometer = null;
                this.compass = null;
                this.barometer = null;
                this.altimeter = null;
            }
            catch(Exception)
            {

            }
        }

        #endregion

        #region Events Handlers

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            lock (this.lockObject)
            {
                this.AccelerationX = args.Reading.AccelerationX;
                this.AccelerationY = args.Reading.AccelerationY;
                this.AccelerationZ = args.Reading.AccelerationZ;                
                this.Timestamp = args.Reading.Timestamp;
            }
            this.ReadingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Barometer_ReadingChanged(Barometer sender, BarometerReadingChangedEventArgs args)
        {
            lock (this.lockObject)
            {                
                this.Pressure = args.Reading.StationPressureInHectopascals;
                this.Timestamp = args.Reading.Timestamp;
            }

            this.ReadingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Gyrometer_ReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            this.ReadingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Altimeter_ReadingChanged(Altimeter sender, AltimeterReadingChangedEventArgs args)
        {            
            lock(this.lockObject)
            {
                this.AltitudeChange = args.Reading.AltitudeChangeInMeters;
                this.Timestamp = args.Reading.Timestamp;
            }

            this.ReadingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            lock (this.lockObject)
            {
                this.TrueNorthHeading = args.Reading.HeadingTrueNorth;
                this.CompassAccuracy = args.Reading.HeadingAccuracy;
                this.Timestamp = args.Reading.Timestamp;
            }

            this.ReadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
