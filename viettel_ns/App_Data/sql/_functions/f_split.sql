USE [viettel_ns]
GO

/****** Object:  UserDefinedFunction [dbo].[F_Split]    Script Date: 27/03/2018 2:52:27 CH ******/
DROP FUNCTION [dbo].[f_split]
GO

/****** Object:  UserDefinedFunction [dbo].[F_Split]    Script Date: 27/03/2018 2:52:27 CH ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create FUNCTION [dbo].[f_split]
(    
      @Input NVARCHAR(MAX)
)
RETURNS @Output TABLE (
      Item NVARCHAR(2000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
      DECLARE @Character CHAR(1)


      SET @StartIndex = 1
	  SET @Character = ','

      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END

      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)

            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)

            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END

      RETURN
END
GO
