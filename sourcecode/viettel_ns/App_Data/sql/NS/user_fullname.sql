declare @username nvarchar(100)							set @username='chuctc'


--###-- 

select Email, FullName from aspnet_Membership
where	UserId in (					
				select UserId from aspnet_Users					
				where	UserName=@username)
