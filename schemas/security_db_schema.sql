
CREATE TABLE ServiceInvokationPermission (
    clientServiceSpaceId int,
    serviceName         varchar(32) NOT NULL,
    methodName          varchar(32) NOT NULL,
    params              varchar NOT NULL,
    PRIMARY KEY(clientServiceSpaceId)
);

CREATE TABLE Treaties (
    treatyid    int,
    treaty      bytea NOT NULL,
    PRIMARY KEY(treatyid)
);