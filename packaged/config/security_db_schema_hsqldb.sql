-- for hsqldb

CREATE TABLE Groups (
    groupid             integer identity,
    groupname           varchar(32) NOT NULL,
    PRIMARY KEY(groupid)
);

-- some sample groups
INSERT INTO Groups VALUES(1,'Group 1');
INSERT INTO Groups VALUES(2,'Group 2');
INSERT INTO Groups VALUES(3,'Group 3');
INSERT INTO Groups VALUES(4,'Group 4');


CREATE TABLE ServicePermissions (
    permissionid        integer IDENTITY,
    groupid             integer NOT NULL,
    serviceName         varchar(32) NOT NULL,
    methodName          varchar(32) NOT NULL,
    params              varchar NOT NULL,
    numInvokations      integer NOT NULL,
    methodImplementation varchar(64) NOT NULL,
    FOREIGN KEY(groupid) REFERENCES Groups(groupid)
);

-- sample permissions
INSERT INTO SERVICEPERMISSIONS VALUES(null, 3,'service1','getData','foo',5,'getDataByFoo')
INSERT INTO SERVICEPERMISSIONS VALUES(null, 3,'service1','getData','foo,bar',5,'getDataByFooAndBar')
INSERT INTO SERVICEPERMISSIONS VALUES(null, 3,'service','doSomething','',3,'doSomething')

CREATE TABLE ServiceSpaceGroups (
    groupid             integer NOT NULL,
    servicespaceid      integer NOT NULL,
    PRIMARY KEY(groupid,servicespaceid),
    FOREIGN KEY(groupid) REFERENCES Groups(groupid)
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
    treaty      binary NOT NULL,
    PRIMARY KEY(treatyid)
);

CREATE TABLE Keystore (
    keystoreid  integer NOT NULL,
    keystore    binary NOT NULL,
    PRIMARY KEY(keystoreid)
);
