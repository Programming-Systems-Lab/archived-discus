
CREATE TABLE ServiceInvokationPermission (
    clientServiceSpaceId int,
    serviceName         varchar(32) NOT NULL,
    methodName          varchar(32) NOT NULL,
    params              varchar NOT NULL,
    numInvokations      integer NOT NULL
);

CREATE TABLE Treaties (
    treatyid    integer NOT NULL,
    status      integer NOT NULL default('0'),
    createdate  date NOT NULL default('now'),
    modifydate  date NOT NULL default('now'),
    treaty      bytea NOT NULL,
    PRIMARY KEY(treatyid)
);