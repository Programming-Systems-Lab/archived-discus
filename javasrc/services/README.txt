
Basic instructions for deploying the SecurityManagerService to a Tomcat JWSDP installation.

- Install the Java Web Services Developer Pack, available at http://java.sun.com/webservices/webservicespack.html

- Copy to the $JWSDP/lib/common directory all the required JAR files for the XML Security libraries
  (available at http://xml.apache.org/security/index.html). The Castor JAR files already come with
  the JWSDP distribution

- Edit the $JWSDPconf/server.xml file to add a database resouce.
  For example:

    <!-- Default Virtual Host -->
      <Host    name="localhost" appBase="webapps" debug="0" unpackWARs="false">
		<DefaultContext>
			<Resource name="jdbc/SecurityManagerDB" reloadable="true"
	   		auth="Container" type="javax.sql.DataSource"/>
			<ResourceParams name="jdbc/SecurityManagerDB">
			   <parameter>
			      <name>driverClassName</name>
			      <value>org.postgresql.Driver</value>
			   </parameter>
			   <parameter>
			      <name>driverName</name>
			      <value>
			         jdbc:postgresql://liberty.psl.cs.columbia.edu/discusDemo
			      </value>
			   </parameter>
			   <parameter>
			      <name>user</name>
			      <value>matias</value>
			   </parameter>
			   <parameter>
			      <name>password</name>
			      <value>discus</value>
			   </parameter>
			</ResourceParams>
		</DefaultContext>
      </Host>

- The config/ss1 and config/ss2 show sample deployment configuration files for two different service spaces.

To use these files:
- Edit the build.properties file to adjust the path and location of the Tomcat server.
- Edit the web.xml file to set the correct passwords for the KeyStore object.

To build the server files:
- On the directory where the build.xml and web.xml files are, run
    ant xrpcc-server
    ant install

    (The ant tool should be in your path -- it comes with the JWDSP distribution).