
CREATE SEQUENCE groups_seq;
CREATE TABLE Groups (
    groupid             integer DEFAULT nextval('groups_seq'),
    groupname           varchar(32) NOT NULL,
    PRIMARY KEY(groupid)
);

CREATE TABLE ServicePermissions (
    groupid             integer NOT NULL,
    serviceName         varchar(32) NOT NULL,
    methodName          varchar(32) NOT NULL,
    params              varchar NOT NULL,
    numInvokations      integer NOT NULL,
    methodImplementation varchar(64) NOT NULL,
    FOREIGN KEY(groupid) REFERENCES Groups
);

CREATE TABLE ServiceSpaceGroups (
    groupid             integer NOT NULL,
    servicespaceid      integer NOT NULL,
    PRIMARY KEY(groupid,servicespaceid),
    FOREIGN KEY(groupid) REFERENCES Groups
);


CREATE TABLE Treaties (
    treatyid    integer NOT NULL,
    status      integer NOT NULL default('0'),
    createdate  date NOT NULL default('now'),
    modifydate  date NOT NULL default('now'),
    treaty      bytea NOT NULL,
    PRIMARY KEY(treatyid)
);

CREATE TABLE Keystore (
    keystoreid  integer NOT NULL,
    keystore    bytea NOT NULL,
    PRIMARY KEY(keystoreid)
);
