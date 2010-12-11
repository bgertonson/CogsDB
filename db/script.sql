Declare @dbname nvarchar(64), @username nvarchar(64), @password nvarchar(64)

Set @dbname   = 'sample_db'
Set @username = 'cogs_db_user'
Set @password = 'D0cDbU$3r'

Declare @sql nvarchar(4000)

Set @sql = 'Create Database ' + @dbname
Execute(@sql)

If Not Exists (Select * From sys.sql_logins Where name = @username)
begin
	Set @sql = 'Create Login ' + @username + ' With Password = ''' + @password + '''';
	Execute(@sql)
end

Set @sql = 'Use ' + @dbname + ';
			Create User ' + @username + ';
			exec sp_addrolemember ''db_datareader'', ''' + @username + ''';
			exec sp_addrolemember ''db_datawriter'', ''' + @username + ''';

create table documents(
	[id]           varchar(128),
	[type]         varchar(128),
	[doc]          nvarchar(max),
	[meta]         nvarchar(max),
	[create-date]  datetime,
	[modify-date]  datetime,
	constraint pk_document_id primary key (id)
);

create table [id-blocks](
	[type]       varchar(128),
	[last-block] int,
	constraint pk_idblocks_id primary key ([type])
);'
Execute(@sql)

