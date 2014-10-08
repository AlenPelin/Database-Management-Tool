# Database Management Tool #

This is a very small tool that allows you to attach or detach database via Context Menu in Windows Explorer. 

#### Downloads ####

[Download version 1.1](http://alienlab.co.uk/database-management-tool/downloads/database-management-tool%20v1.1.zip)

#### Installation ####

1. Extract into some folder (e.g. C:\Program Files\Database Management Tool)
1. Run the DoInstall.bat script

#### Uninstallation ####

1. Run the DoUninstall.bat script
1. Delete files

#### Known Issues ####

1. You need manually ensure that MS SQL Server has MS full access to the .MDF file if the MS SQL Server is configured to use virtual user accounts like NT Service\MSSQLSERVER.  **Workaround:** Use Network Service account instead. [How-to](http://www.sqlservercentral.com/blogs/steve_jones/2011/12/15/how-to-change-the-sql-server-service-cccount/).

#### Release History ####

v.1.1 

* Support of SQL 2008

* Attach/Detach several databases at once

v.1.0

* Support of SQL 2012

* Attach/Detach single database at once

### Screenshots ###

![Database Management Tool - Explorer.png](https://bitbucket.org/repo/krX9Xb/images/769454004-Database%20Management%20Tool%20-%20Explorer.png)

![Database Manager Tool - Attach.png](https://bitbucket.org/repo/krX9Xb/images/1290022301-Database%20Manager%20Tool%20-%20Attach.png)