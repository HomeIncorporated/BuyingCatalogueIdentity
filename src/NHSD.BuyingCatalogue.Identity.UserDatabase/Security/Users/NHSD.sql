﻿CREATE USER [NHSD-ISAPI]
	FOR LOGIN [NHSD-ISAPI]
	WITH DEFAULT_SCHEMA = dbo;
GO

GRANT CONNECT TO [NHSD-ISAPI];
GO

ALTER ROLE db_datareader
ADD MEMBER [NHSD-ISAPI];
GO

ALTER ROLE db_datawriter
ADD MEMBER [NHSD-ISAPI];
GO
