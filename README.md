# ms-graph-emails

I am maintaining an old **.NET Framework application (v4.7.2)**. 
It currently uses an in-house solution to send emails and read a mailbox on an Exchange Server, 
but this solution is being decommissioned and **all applications are supposed to move to Microsoft Graph**.

I have used MS Graph in other applications that are written in .NET Core, and it works in all of those. 
However, I cannot get it to work in this .NET Framework one.

This repository contains two soltutions:

* a **.NET Framework 4.8.1** version with the same code as below, which **does not work**:
  * the authentication succeeds, but the code throws an exception when trying to send emails.
* a **.NET (Core) 6** version, which features virtually the same code, which **does work**.

Both use the same NuGet package: **MS Graph 4.54.0** and its dependencies.

The .NET (Core) 6 solution contains virtually the same code as the .NET Framework version.

To me it signals that the NuGet package **does not work correctly with .NET Framework**. 
If that is indeed the case, it is a major problem for me 
since upgrading the legacy project to .NET Core is not a feasible solution in the short term 
(since it would require a significant rewrite).

If you download either solution to test locally, please read the README in the root of each solution:
each project lacks a file containing the necessary configuration settings, 
but an example of what it should look like is present in the README file 
(`app.config` for the .NET Framework solution, `appsettings.json` for the .NET 6 solution).
You will need to provide the necessary TenantId etc. yorself, of course.
