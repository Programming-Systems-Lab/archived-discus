-- for mysql
-- newer tables at the top

CREATE TABLE ServiceSpaces (
    serviceSpaceId      integer AUTO_INCREMENT,
    serviceSpaceName    varchar(32) NOT NULL,
    trustLevel          integer,
        -- used for deciding who to send UDDI requests too only for now
        -- (not for incoming request verification)
    PRIMARY KEY (serviceSpaceId)
);

INSERT INTO ServiceSpaces VALUES(0,'Dummy service space',0);
INSERT INTO ServiceSpaces VALUES(1,'1',3);
INSERT INTO ServiceSpaces VALUES(2,'1',3);
INSERT INTO ServiceSpaces VALUES(3,'2',3);
INSERT INTO ServiceSpaces VALUES(4,'4',3);

CREATE TABLE ServiceSpaceEndpoints (
    endpointId          integer AUTO_INCREMENT,
    serviceSpaceId      integer not null,
    pipeAd              text not null,
    PRIMARY KEY(endpointId),
    FOREIGN KEY(serviceSpaceId) REFERENCES ServiceSpaces(serviceSpaceId)
);

CREATE TABLE Groups (
    groupid             integer auto_increment,
    groupname           varchar(32) NOT NULL,
    PRIMARY KEY(groupid)
);

-- some sample groups
INSERT INTO Groups VALUES(1,'Group 1');
INSERT INTO Groups VALUES(2,'Group 2');
INSERT INTO Groups VALUES(3,'Group 3');
INSERT INTO Groups VALUES(4,'Group 4');


CREATE TABLE ServicePermissions (
    permissionid        integer AUTO_INCREMENT,
    groupid             integer NOT NULL,
    serviceName         varchar(32) NOT NULL,
    methodName          varchar(32) NOT NULL,
    params              varchar(32) NOT NULL,
    numInvokations      integer NOT NULL,
    methodImplementation varchar(64) NOT NULL,
    PRIMARY KEY(permissionid),
    FOREIGN KEY(groupid) REFERENCES Groups(groupid)
);

-- sample permissions
INSERT INTO SERVICEPERMISSIONS VALUES(1, 3,'service1','getData','foo',5,'getDataByFoo');
INSERT INTO SERVICEPERMISSIONS VALUES(2, 3,'service1','getData','foo,bar',5,'getDataByFooAndBar');
INSERT INTO SERVICEPERMISSIONS VALUES(3, 3,'service','doSomething','',3,'doSomething');

CREATE TABLE ServiceSpaceGroups (
    groupid             integer NOT NULL,
    servicespaceid      integer NOT NULL,
    PRIMARY KEY(groupid,servicespaceid),
    FOREIGN KEY(groupid) REFERENCES Groups(groupid),
    FOREIGN KEY(servicespaceid) REFERENCES ServiceSpaces(servicespaceid)
);

INSERT INTO ServiceSpaceGroups VALUES(1,1);
INSERT INTO ServiceSpaceGroups VALUES(2,2);
INSERT INTO ServiceSpaceGroups VALUES(3,3);
INSERT INTO ServiceSpaceGroups VALUES(4,4);

CREATE TABLE Treaties (
    treatyid    integer NOT NULL,
    status      integer,
    createdate  date,
    modifydate  date,
    treaty      blob NOT NULL,
    PRIMARY KEY(treatyid)
);

CREATE TABLE Keystore (
    keystoreid  integer NOT NULL,
    keystore    blob NOT NULL,
    PRIMARY KEY(keystoreid)
);
