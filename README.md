# DescriptionCreator
The tool reads file with tabbed values:
table name    column name   description

The file will be transformed to two SQL files for SQL Server and Oracle.
The new files will contain SQL commands for adding descriptions to a table or a column

The original file empty descriptions can be get from query:
SQL Server
----------

-- for tables and columns

select table_name, column_name, Description
from (
  select st.name [table_name], null [column_name], sep.value [Description]
  from sys.tables st
    left join sys.extended_properties sep on st.object_id = sep.major_id  and sep.name = 'MS_Description'
  where st.schema_id = 1 and (sep.minor_id is null or sep.minor_id = 0) 
  UNION 
  select st.name [table_name], sc.name [column_name], sep.value [Description]
  from sys.tables st
    inner join sys.columns sc on st.object_id = sc.object_id
    left join sys.extended_properties sep 
      on st.object_id = sep.major_id and sc.column_id = sep.minor_id and sep.name = 'MS_Description'
  where st.schema_id = 1
) as Qu
where 
  Description is null
order by table_name, column_name

-- for views

select st.name [view_name], sep.value [Description]
from sys.views st
 left join sys.extended_properties sep on st.object_id = sep.major_id  and sep.name = 'MS_Description'
where sep.value is null
order by st.name

Oracle
------
--for tables and views

SELECT table_name, comments, owner       
FROM all_tab_comments     
where 
  owner = 'NETMETER' 
  and (comments is null or comments = '')
order by table_name

--for table columns

SELECT  TABLE_NAME, COLUMN_NAME, C.COMMENTS  
FROM user_col_comments C join user_tab_cols K  
USING (TABLE_NAME, COLUMN_NAME)  
WHERE 
  (C.comments is null or C.comments = '')
order by TABLE_NAME


Then the result can by copied to Excel. Descriptions should be filled and result saved to tabbed file
