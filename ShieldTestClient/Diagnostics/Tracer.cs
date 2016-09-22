using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShieldTestClient.Diagnostics
{
    public class Tracer
    {
        #region Singleton

        private static readonly Tracer defaultTracer = new Tracer();

        public static Tracer Default
        {
            get
            {
                return defaultTracer;
            }
        }

        #endregion

        #region Events

        public event EventHandler<String> EventLogged;

        #endregion        

        #region Properties

        public LogLevels VerboseLevel
        {
            get;
            set;
        }

        #endregion

        #region .ctor

        public Tracer()
        {
            this.VerboseLevel = LogLevels.Verbose;
        }

        #endregion

        #region Methods

        public void Log(String message, LogLevels logLevel = LogLevels.Info, [CallerMemberName] String memberName = null)
        {
            StringBuilder logBuilder = null;

            if (logLevel <= this.VerboseLevel)
            {
                logBuilder = new StringBuilder();
                logBuilder.AppendFormat("[{0}:{1} - {2}] {3}",
                                        memberName,
                                        logLevel.ToString(),
                                        DateTime.Now.ToString("G"),
                                        message);
                System.Diagnostics.Debug.WriteLine(logBuilder.ToString());

                this.EventLogged?.Invoke(this, logBuilder.ToString());
            }
        }

        #endregion
    }
}
