-- Not done yet...
SELECT C.TABLE_NAME, C.COLUMN_NAME, 'Yes' AS [MissingForeignKey]
FROM 
	(
		SELECT * FROM
		(
			-- We need all columns, becuase we need to know those that end with 'Id', aren't primary keys, and arent' foreign keys
			[RaterooDebug].[INFORMATION_SCHEMA].[COLUMNS] AS Cols
			LEFT JOIN
			(
				SELECT ColumnConstraintUsage.TABLE_NAME, ColumnConstraintUsage.COLUMN_NAME, 
					TableConstraints.CONSTRAINT_TYPE
				FROM
					-- Needed for the CONSTRAINT_TYPE
					[RaterooDebug].[INFORMATION_SCHEMA].[TABLE_CONSTRAINTS] AS TableConstraints 
					INNER JOIN 
					-- Needed for the TABLE_NAME and COLUMN_NAME
					[RaterooDebug].[INFORMATION_SCHEMA].[CONSTRAINT_COLUMN_USAGE] AS ColumnConstraintUsage
					-- I believe CONSTRAINT_NAME must be unique.  Hopefully that's true.
					ON TableConstraints.CONSTRAINT_NAME = ColumnConstraintUsage.CONSTRAINT_NAME
				WHERE 
					TableConstraints.CONSTRAINT_TYPE = 'FOREIGN KEY' OR 
					TableConstraints.CONSTRAINT_TYPE = 'PRIMARY KEY'
			) AS ForeignOrPrimaryKeyColumns
			ON 
				-- This is the only way I know of matching up columns from different sources...if their table names and column names are equal
				Cols.TABLE_NAME = ForeignOrPrimaryKeyColumns.TABLE_NAME AND 
				Cols.COLUMN_NAME = ForeignOrPrimaryKeyColumns.COLUMN_NAME
		)
	) AS 
INNER JOIN
	[RaterooDebug].[INFORMATION_SCHEMA].[TABLES] AS T
ON
	CI.COLUMN_NAME LIKE T.TABLE_NAME + 'Id'
WHERE C.COLUMN_NAME LIKE '%Id' AND CC.CONSTRAINT_TYPE IS NULL
ORDER BY C.TABLE_NAME, C.COLUMN_NAME