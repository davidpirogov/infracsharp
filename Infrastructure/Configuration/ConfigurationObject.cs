using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace InfraCSharp.Infrastructure.Configuration
{
    /// <summary>
    /// Represents a base configuration class for XML serialisation
    /// </summary>
    [Serializable]
    public class ConfigurationObject
    {
        /// <summary>
        /// Loads a configuration object from the file system
        /// </summary>
        /// <typeparam name="T">A type of <code>ConfigurationObject</code></typeparam>
        /// <param name="inputFile">A <code>FileInfo</code> object that points to a file system location</param>
        /// <returns>A prepared deserialized object of type T</returns>
        public static T Load<T>(FileInfo inputFile) where T : ConfigurationObject, new()
        {
            T result = default(T);

            if (inputFile.Exists)
            {
                using (FileStream stream = File.OpenRead(inputFile.FullName))
                {
                    try
                    {
                        result = new XmlSerializer(typeof(T)).Deserialize(stream) as T;
                    }
                    catch (Exception ex)
                    {
                        throw new ConfigurationSerializationException(inputFile, ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Loads a ConfigurationObject from the file system
        /// </summary>
        /// <param name="inputFile">A <code>FileInfo</code> object that points to a file system location</param>
        /// <returns>A prepared <code>ConfigurationObject</code></returns>
        public static ConfigurationObject Load(FileInfo inputFile)
        {
            return Load<ConfigurationObject>(inputFile);
        }

        /// <summary>
        /// Saves a configuration object to the file system
        /// </summary>
        /// <typeparam name="T">A type of <code>ConfigurationObject</code></typeparam>
        /// <param name="outputFile">A <code>FileInfo</code> object that points to a file system location</param>
        /// <param name="forceDelete">A <code>bool</code> that denotes whether or not to delete the specified output file if it exists. Note: Defaults to <value>TRUE</value></param>
        public void Save<T>(FileInfo outputFile, bool forceDelete = true) where T : ConfigurationObject
        {
            try
            {
                if (outputFile.Exists && forceDelete)
                {
                    outputFile.Delete();
                }

                using (FileStream stream = new FileStream(outputFile.FullName, FileMode.CreateNew))
                {
                    new XmlSerializer(typeof(T)).Serialize(stream, this);
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationSerializationException(outputFile, ex);
            }
        }
    }
}
