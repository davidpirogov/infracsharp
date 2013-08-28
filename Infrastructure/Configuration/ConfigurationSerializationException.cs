using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InfraCSharp.Infrastructure.Configuration
{
    /// <summary>
    /// Represents an XML Configuration Serialization Exception
    /// </summary>
    public class ConfigurationSerializationException : SerializationException 
    {
        /// <summary>
        /// Gets the exception message for this object
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Concat("Exception while attempting an XML serialization operation:", " ", base.Message);
            }
        }

        /// <summary>
        /// Gets the file that triggered this XML serialization exception
        /// </summary>
        public FileInfo OffendingFile
        {
            get;
            protected set;
        }


        /// <summary>
        /// Creates a new <code>ConfigurationSerializationException</code> based on the supplied exception
        /// </summary>
        public ConfigurationSerializationException(FileInfo triggeredFile, Exception ex) : base(ex.Message, ex)
        {
            OffendingFile = triggeredFile;
        }
    }
}
