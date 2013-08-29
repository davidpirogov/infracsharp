InfraCSharp
===========

A C# Infrastructure project that contains the basic things that I always seem to use.

Usage
=====

Logging
-------

Assuming you have created and configured an Apache log4net config file at ```./Log/config.log```

```CSharp
  Logging log = new Logging(Environment.CurrentDirectory + "/Log", "config");
```

This will create a new object that you can use to call log entries to:

```CSharp
  log.Info("Example Message");
  log.Error(caughtExceptionObject);
```

From here, you have five levels of log, from the least verbose to the most:
  1. Fatal
  2. Error
  3. Warn
  4. Info
  5. Debug
  
You can also pass in parameters that will automatically be formatted according to [System.String](http://msdn.microsoft.com/en-us/library/system.string.format.aspx) rules:

```CSharp
  log.Error("Error reading {0} bytes from file: {1}", numBytes, sourceFile.FullName);
```

Configuration
-------------

Configuration is aimed at serializing and deserializing XML-based configuration files into C# first-class objects.

Assuming the following xml configuration file called ```app-settings.xml```

```XML
  <?xml version="1.0" encoding="utf-8" ?>
  <app-config>
    <key name="foo">bar<key>
    <credentials>
      <username>User</username>
      <password>*************</password>
    </credentials>
  </app-config>
```

All one needs is to create a basic class within their application:

```CSharp
using System;
using System.Xml.Serialization;

namespace App
{
    [Serializable]
    [XmlRoot("app-config")]
    public class AppSettings : InfraCSharp.ConfigurationObject {
        [XmlElement("key")]
        public AppSettingsKey Key { get; set; }
        [XmlElement("credentials")]
        public AppSettingsCredentials Credentials { get; set; }
    }
    
    public class AppSettingsKey { 
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
    
    public class AppSettingsCredentials { 
        [XmlElement("username")];
        public string Username;
        [XmlElement("password")];
        public string Password;
    }
}
```

Once that basic class has been created, it's a simple matter of calling the Load method:

```CSharp
  AppSettings settings = InfraCSharp.ConfigurationObject<AppSettings>.Load("app-settings.xml");
  
  settings.Key.Name.ToString();             // Outputs: foo
  settings.Key.Value.ToString();            // Outputs: bar
  settings.Credentials.Username.ToString(); // Outputs: User
  settings.Credentials.Password.ToString(); // Outputs: *************
```




