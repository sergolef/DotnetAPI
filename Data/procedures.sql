USE DotNetCourseDatabase
GO

ALTER PROCEDURE TutorialAppSchema.spUsers_Get
    /*EXEC TutorialAppSchema.spUsers_Get @UserId=3, @Active=0*/
    @UserId INT = NULL
    ,
    @Active BIT = NULL

As
BEGIN

    -- create temporarily table --
    -- check if table exists and if its table then drop it
    -- IF OBJECT_ID('tempdb..#AverageDeptSalary', 'U') IS NOT NULL
    --     BEGIN
    --         DROP TABLE #AverageDeptSalary
    --     END

    DROP TABLE IF EXISTS #AverageDeptSalary

    SELECT
        JobInfo.Department,
        AVG(UserSalary.Salary) as AvgSalary
    INTO #AverageDeptSalary
    FROM TutorialAppSchema.Users AS Users
        LEFT JOIN TutorialAppSchema.UserSalary as UserSalary
        ON UserSalary.UserId=Users.UserId
        LEFT JOIN TutorialAppSchema.UserJobInfo as JobInfo
        ON Users.UserId=JobInfo.UserId
    GROUP BY JobInfo.Department

    CREATE CLUSTERED INDEX cix_AverageDepSalary_Department ON #AverageDeptSalary(Department)

    SELECT Users.UserId,
        Users.FirstName,
        Users.LastName,
        Users.Email,
        Users.Gender,
        Users.Active,
        UserSalary.Salary,
        JobInfo.JobTitle,
        JobInfo.Department,
        AvgSalary

    FROM TutorialAppSchema.Users AS Users
        LEFT JOIN TutorialAppSchema.UserSalary as UserSalary
        ON UserSalary.UserId=Users.UserId
        LEFT JOIN TutorialAppSchema.UserJobInfo as JobInfo
        ON Users.UserId=JobInfo.UserId
        LEFT JOIN #AverageDeptSalary as AvgSalary
        ON AvgSalary.Department = JobInfo.Department
    -- OUTER APPLY(
    --     SELECT 
    --         JobInfo2.Department,
    --         AVG(UserSalary2.Salary) as AvgSalary
    --     FROM TutorialAppSchema.Users AS Users
    --     LEFT JOIN TutorialAppSchema.UserSalary as UserSalary2
    --         ON UserSalary2.UserId=Users.UserId
    --     LEFT JOIN TutorialAppSchema.UserJobInfo as JobInfo2
    --         ON Users.UserId=JobInfo2.UserId

    --     WHERE JobInfo2.Department = JobInfo.Department
    --     GROUP BY JobInfo2.Department
    -- ) As AvgSalary
    WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
        AND ISNULL(Users.Active, 0) = COALESCE(@Active, Users.Active, 0)
END


/** **/
USE DotNetCourseDatabase
GO

CREATE OR ALTER PROCEDURE TutorialAppSchema.spUser_Upsert
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(50),
    @Gender NVARCHAR(50),
    @JobTitle NVARCHAR(50),
    @Department NVARCHAR(50),
    @Salary DECIMAL(18,4),
    @Active BIT = 1,
    @UserId INT = NULL
AS
BEGIN
    -- SELECT * FROM TutorialAppSchema.UserSalary
    -- SELECT * FROM TutorialAppSchema.UserJobInfo

    IF NOT EXISTS (Select *
    from TutorialAppSchema.Users
    WHERE UserId = @UserId) 
        BEGIN

        IF NOT EXISTS (Select *
        from TutorialAppSchema.Users
        Where Email=@Email)
                BEGIN
            DECLARE @OutputUserId INT

            INSERT INTO TutorialAppSchema.Users
                (FirstName, LastName, Email, Gender, Active)
            VALUES(@FirstName, @LastName, @Email, @Gender, @Active)

            SET @OutputUserId = @@IDENTITY

            INSERT INTO TutorialAppSchema.UserSalary
                (UserId, Salary)
            VALUES(@OutputUserId, @Salary)

            INSERT INTO TutorialAppSchema.UserJobInfo
                (UserId, JobTitle, Department)
            VALUES(@OutputUserId, @JobTitle, @Department)
        END
    END
    ELSE
        BEGIN
        UPDATE TutorialAppSchema.Users
                SET FirstName=@FirstName,
                LastName=@LastName,
                Email=@Email,
                Gender=@Gender,
                Active=@Active
            Where UserId = @UserId

        UPDATE TutorialAppSchema.UserSalary
                SET Salary=@Salary
                Where UserId=@UserId

        UPDATE TutorialAppSchema.UserJobInfo
                SET JobTitle=@JobTitle, Department=@Department
                Where UserId=@UserId
    END

END

/**/

USE DotNetCourseDatabase
GO
CREATE PROCEDURE TutorialAppSchema.SPUser_Delete
    @UserId INT
AS
BEGIN
    DELETE FROM TutorialAppSchema.Users 
        WHERE UserId=@UserId
        
    DELETE FROM TutorialAppSchema.UserJobInfo
        WHERE UserId=@UserId

    DELETE FROM TutorialAppSchema.UserSalary
        WHERE UserId=@UserId
END

/**/

USE DotNetCourseDatabase
GO
CREATE OR ALTER PROCEDURE TutorialAppSchema.spPosts_Delete
/*EXEC TutorialAppSchema.spPosts_Delete @PostId = 5*/
    @PostId INT
AS
BEGIN
    DELETE FROM TutorialAppSchema.Posts 
        WHERE PostId=@PostId
END

/**/

USE DotNetCourseDatabase
GO

CREATE OR ALTER  PROCEDURE TutorialAppSchema.spPosts_Get
/* EXEC TutorialAppSchema.spPosts_Get @UserId = 1008, @SearchValue='Post'*/
/* EXEC TutorialAppSchema.spPosts_Get @PostId = 2, @SearchValue='Post'*/
    @UserId INT = NULL
    , @PostId INT = NULL
    , @SearchValue NVARCHAR(MAX) = NULL
    
AS
BEGIN
    Select * FROM TutorialAppSchema.Posts AS Posts
    WHERE Posts.UserId=ISNULL(@UserId, Posts.UserId)
        AND Posts.PostId=ISNULL(@PostId, Posts.PostId)
        AND (@SearchValue IS NULL 
            OR Posts.PostContent LIKE '%'+@SearchValue+'%'
            OR Posts.PostTitle LIKE '%'+@SearchValue+'%')
END

/**/

USE DotNetCourseDatabase
GO

CREATE OR ALTER PROCEDURE TutorialAppSchema.spPosts_Upster
    /* EXEC TutorialAppSchema.spPosts_Upster @UserId=1008, @PostTitle='New POST', @PostContent='Some content for new post'*/
    /* EXEC TutorialAppSchema.spPosts_Upster @PostId = 6, @UserId=1008, @PostTitle='New UPDATED', @PostContent='Some content for new post2'*/
    @UserId INT,
    @PostTitle NVARCHAR(50),
    @PostContent NVARCHAR(MAX),
    @PostId INT = NULL

AS
BEGIN
    IF NOT EXISTS (SELECT *
    FROM TutorialAppSchema.Posts
    WHERE PostId = @PostId)
        BEGIN
        INSERT INTO TutorialAppSchema.Posts
            (UserId, PostTitle, PostContent, PostCreated, PostUpdated)
        VALUES(@UserId, @PostTitle, @PostContent, GETDATE(), GETDATE())
    END
    ELSE
        BEGIN
        UPDATE TutorialAppSchema.Posts
                SET PostTitle=@PostTitle, 
                    PostContent=@PostContent, 
                    PostUpdated=GETDATE()
                Where PostId=@PostId
    END

END

/**/
USE DotNetCourseDatabase
GO

CREATE OR ALTER PROCEDURE TutorialAppSchema.spRegistration_Upster
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    IF NOT EXISTS (SELECT *
    FROM TutorialAppSchema.Auth
    WHERE Email=@Email)
        BEGIN
        INSERT INTO TutorialAppSchema.Auth
            (Email, PasswordHash, PasswordSalt)
        VALUES(@Email, @PasswordHash, @PasswordSalt)
    END
    ELSE
        BEGIN
        UPDATE TutorialAppSchema.Auth
                SET PasswordHash=@PasswordHash,
                    PasswordSalt=@PasswordSalt
                    Where Email=@Email
    END

END

CREATE OR ALTER PROCEDURE TutorialAppSchema.spLoginConfirmation_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT * FROM TutorialAppSchema.Auth
        WHERE Email=@Email
END


