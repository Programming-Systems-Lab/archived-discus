
Quick instructions for installing the DISCUS Security Manager demo:
-------------------------------------------------------------------

1. Download and installing the mammoth Java Web Services Developer Pack
	http://java.sun.com/webservices/webservicespack.html

2. If not done already, unzip the contents of this zip file into a temporary directory

3. Open the build.properties file on any text editor and fix any necessary values:
	jwsdp.home=C:/jwsdp-1.0				# location of JWSDP
	jwsdp.url=http://localhost:8080		# server and port where JWSDP installed
	servicespace.id=1					# which service space is being configured
    and others.

	This demo comes with four different KeyStores for installing up to four different
	service space configurations. This way the four service spaces will "know" each other.

4. Run "ant install" on the command prompt to install the DISCUS Security Manager web service.

5. To test the service, use the enclosed SecurityManagerTester application and the XML files
   in the examples subdirectory. Note: the .NET framework must be installed to use the
   SecurityManagerTester application.

6. To use the Security Manager GUI, which can be used to create group and manage permissions, run
        "ant gui"
    at the command prompt. NOTE: you may have to edit the connection URL, especially
    on Windows where the "\" might get mangled out. The program will save the connection settings.

- To look at the database, run "ant dbmanager" and follow instructions printed out.
  This is a very primitive interface and requires SQL commands for everything.

- To reinstall, run "ant clean" first, then "ant install".

-------------------------------------------------------------------
To recompile the discus-security.jar from the sources:

1. Obtain the source files and place them in a "src" directory.

2. If necessary, modify the build.xml file to set the correct src and build directories.

3. Run "ant makejar". This will compile, then run xrpcc-server to create the web-server ties
   and wsdl file.


Any questions or problems, email Matias at mp2079@cs.columbia.edu
Last updated 6/26/02



