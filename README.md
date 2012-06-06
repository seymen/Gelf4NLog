# Gelf4NLog
Gelf4NLog is an [NLog] target implementation to push log messages to [GrayLog2]. It implements the [Gelf] specification and communicates with GrayLog server via UDP.

## Solution
Solution is comprised of 3 projects: *Target* is the actual NLog target implementation, *UnitTest* contains the unit tests for the NLog target, and *ConsoleRunner* is a simple console project created in order to demonstrate the library usage.
## Usage
Use Nuget:
```
PM> Install-Package Gelf4NLog.Target
```
### Configuration
Here is a sample nlog configuration snippet:
```xml
<configSections>
  <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog" />
</configSections>

<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

	<extensions>
	  <add assembly="Gelf4NLog.Target"/>
	</extensions>

	<targets>
	  <!-- Other targets (e.g. console) -->
    
	  <target name="gelf" 
			  xsi:type="Gelf" 
			  hostip="192.168.1.7" 
			  hostport="12201" 
			  facility="console-runner"
	  />
	</targets>

	<rules>
	  <logger name="*" minlevel="Debug" writeTo="gelf" />
	</rules>

</nlog>
```

Options are the following:
* __name:__ arbitrary name given to the target
* __type:__ set this to "Gelf"
* __hostip:__ IP address of the GrayLog2 server
* __hostport:__ Port number that GrayLog2 server is listening on
* __facility:__ The graylog2 facility to send log messages

###Code
```c#
//excerpt from ConsoleRunner
var eventInfo = new LogEventInfo
    			{
					Message = comic.Title,
					Level = LogLevel.Info,
				};
eventInfo.Properties.Add("Publisher", comic.Publisher);
eventInfo.Properties.Add("ReleaseDate", comic.ReleaseDate);
Logger.Log(eventInfo);
```

[NLog]: http://nlog-project.org/
[GrayLog2]: http://graylog2.org/
[Gelf]: http://graylog2.org/about/gelf