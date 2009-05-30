
CuiseControl .NET Configuration

(A) The follwoing files belong to the CCNEt Configuration:
- ccnet.config
- ccnet.exe.config
- ccservice.exe.config

- dashboard.config

- MsTest9Report.xsl
- MsTest9Summary.xsl


(B) Copy the files to the following places:
    (path may differ according to your installation)

To C:\Program Files\CruiseControl.NET\server
- ccnet.config
- ccnet.exe.config
- ccservice.exe.config

To C:\Program Files\CruiseControl.NET\webdashboard
- dashboard.config

To C:\Program Files\CruiseControl.NET\server\xsl
and to C:\Program Files\CruiseControl.NET\webdashboard\xsl
- MsTest9Report.xsl
- MsTest9Summary.xsl


(C) Output Paths
- The main output of the ccnet is written to E:\ccnet\Artifacts\NuvoControl
- The build logfiles are in E:\ccnet\Artifacts\NuvoControl\buildlogs
- The working space for NuvoControl is E:\ccnet\NuvoControl


(D) Restart the ccnet service or executable
- ccnet.exe at C:\Program Files\CruiseControl.NET\server
- ccservice.exe at C:\Program Files\CruiseControl.NET\server


(E) The Web Ui is available at
http://localhost/ccnet  or
http://imfeldc.dyndns.org/ccnet  (where imfeldc.dyndns.org [85.5.86.81])



Ch.Imfeld / 30-May-2009