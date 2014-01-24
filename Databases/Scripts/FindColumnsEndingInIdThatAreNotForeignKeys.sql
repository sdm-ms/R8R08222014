-- Useful for finding any foreign key columns that are not set
-- Could make more powerful by limiting results to those that 
SELECT C.TABLE_NAME, C.COLUMN_NAME, 'Maybe' AS [MissingForeignKey]
FROM 
	-- We need all columns, becuase we need to know those that end with 'Id', aren't primary keys, and arent' foreign keys
	[RaterooDebug].[INFORMATION_SCHEMA].[COLUMNS] AS C
	LEFT JOIN
	(
		SELECT CU.TABLE_NAME, CU.COLUMN_NAME, TC.CONSTRAINT_TYPE
		FROM
			-- Needed for the CONSTRAINT_TYPE
			[RaterooDebug].[INFORMATION_SCHEMA].[TABLE_CONSTRAINTS] AS TC 
			INNER JOIN 
			-- Needed for the TABLE_NAME and COLUMN_NAME
			[RaterooDebug].[INFORMATION_SCHEMA].[CONSTRAINT_COLUMN_USAGE] AS CU
			-- I believe CONSTRAINT_NAME must be unique.  Hopefully that's true.
			ON TC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
		WHERE TC.CONSTRAINT_TYPE = 'FOREIGN KEY' OR TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
		
	) AS CC
	-- This is the only way I know of matching up columns from different sources...if their table names and column names are equal
	ON C.TABLE_NAME = CC.TABLE_NAME AND C.COLUMN_NAME = CC.COLUMN_NAME
WHERE C.COLUMN_NAME LIKE '%Id' AND CC.CONSTRAINT_TYPE IS NULL
ORDER BY C.TABLE_NAME, C.COLUMN_NAME