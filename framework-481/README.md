
This is a solution I use to test sending emails using MS Graph in a .NET Framework (4.8.1) project. 

This is basically a copy of the code from the actual project, merely reduced here to a console app to allow me to easily test the functionality and adapt the code where necessary.

One file that is not in this repository is the App.config file, for obvious reasons: it contains sensitive data which cannot be divulged.

You can create the file manually, it should look like the XML code below. 
Note that you should replace all the "_REDACTED_" bits with your own values, of course.

The use of an outdated version of NLog is due to the same being used in the ancient project (and upgrading is not possible). 
Log entries are done using `Trace` calls, these also appear in the console while the console is running.

Note that the actual project is running on enterprise servers, and that the proxy setting is required in such cases. 
However, on my developer laptop I can chose to switch this on or off. 
(Ideally it would work with the setting being "on" of course.)

`
<configuration>
	<appSettings>
		<add key="MSGraph.ClientId" value="_REDACTED_" />
		<add key="MSGraph.TenantId" value="_REDACTED_" />
		<add key="MSGraph.Secret" value="_REDACTED_" />
		<add key="MSGraph.ProxyAddress" value="_REDACTED_" />
		<add key="MSGraph.UseProxy" value="false" />

		<add key="Email.Sender" value="_REDACTED_" />
		<add key="Email.Destination" value="_REDACTED_" />
	</appSettings>

	<system.diagnostics>
		<trace autoflush="true">
			<listeners>
				<add name="MyNLogTraceListener" type="NLog.NLogTraceListener, NLog" />
			</listeners>
		</trace>
	</system.diagnostics>

	<startup>
		<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8.1" />
	</startup>

	<runtime>
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
			<dependentAssembly>
				<assemblyIdentity name="Microsoft.IdentityModel.Abstractions" publicKeyToken="31bf3856ad364e35" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-6.26.1.0" newVersion="6.26.1.0" />
			</dependentAssembly>
			<dependentAssembly>
				<assemblyIdentity name="System.Runtime.CompilerServices.Unsafe" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
			</dependentAssembly>
			<dependentAssembly>
				<assemblyIdentity name="System.Memory" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-4.0.1.2" newVersion="4.0.1.2" />
			</dependentAssembly>
			<dependentAssembly>
				<assemblyIdentity name="System.Text.Json" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-7.0.0.1" newVersion="7.0.0.1" />
			</dependentAssembly>
			<dependentAssembly>
				<assemblyIdentity name="Microsoft.Bcl.AsyncInterfaces" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-7.0.0.0" newVersion="7.0.0.0" />
			</dependentAssembly>
			<dependentAssembly>
				<assemblyIdentity name="System.Diagnostics.DiagnosticSource" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
				<bindingRedirect oldVersion="0.0.0.0-4.0.5.0" newVersion="4.0.5.0" />
			</dependentAssembly>
		</assemblyBinding>
	</runtime>
</configuration>
`
