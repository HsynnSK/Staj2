-- Production Plans Seed Script (Append-Only Generator Version)
-- Safe to run multiple times on PlantNodeDb database
-- Appends a rich, high-density, multi-year dataset spanning late 2025, full 2026, and early 2027 WITHOUT deleting existing data.

-- 1. Lookup IDs into temp tables for index-based access
DECLARE @Products TABLE (Idx INT IDENTITY(1,1), Id INT);
INSERT INTO @Products (Id) SELECT Id FROM Products WHERE IsDeleted = 0;
DECLARE @ProductCount INT;
SELECT @ProductCount = COUNT(*) FROM @Products;

DECLARE @Machines TABLE (Idx INT IDENTITY(1,1), Id INT);
INSERT INTO @Machines (Id) SELECT Id FROM Machines WHERE IsDeleted = 0;
DECLARE @MachineCount INT;
SELECT @MachineCount = COUNT(*) FROM @Machines;

-- If no products or machines, print warning
IF @ProductCount = 0 OR @MachineCount = 0
BEGIN
    PRINT 'HATA: Veritabanında aktif Ürün veya Makine bulunamadı!';
    RETURN;
END

-- Define Date references
DECLARE @Now DATETIME = GETDATE();

-- Loop from 2025-11-01 to 2027-02-28
DECLARE @CurrentDate DATETIME = '2025-11-01';
DECLARE @EndDate DATETIME = '2027-02-28';

DECLARE @LoopCounter INT = 1;
DECLARE @PlanId INT;
DECLARE @ProdId INT;
DECLARE @MachId INT;
DECLARE @PlannedStart DATETIME;
DECLARE @PlannedEnd DATETIME;
DECLARE @Status INT;
DECLARE @Qty INT;

DECLARE @PlanCode1 VARCHAR(100);
DECLARE @PlanCode2 VARCHAR(100);

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Only insert plans every 3 days to simulate a realistic calendar distribution
    IF @LoopCounter % 3 = 0
    BEGIN
        -- Plan 1 (Line A)
        SET @PlannedStart = DATEADD(HOUR, 8, @CurrentDate); -- Starts at 08:00
        SET @PlanCode1 = 'PLN-' + CONVERT(VARCHAR, @PlannedStart, 112) + '-01';
        
        -- Check if plan already exists before inserting
        IF NOT EXISTS (SELECT 1 FROM [ProductionPlans] WHERE [PlanCode] = @PlanCode1 AND [IsDeleted] = 0)
        BEGIN
            SET @Qty = 500 + (@LoopCounter * 37) % 2000;
            SET @PlannedEnd = DATEADD(HOUR, 8 + ((@LoopCounter * 3) % 9), @PlannedStart);
            
            -- Status determination
            IF @PlannedEnd < @Now
            BEGIN
                -- Past plan: check if we should simulate an interruption (approx. 1 in 7 plans fails)
                IF @LoopCounter % 7 = 0
                    SET @Status = 5; -- NotCompleted (Interrupted)
                ELSE
                    SET @Status = 4; -- Completed
            END
            ELSE IF @PlannedStart <= @Now AND @PlannedEnd >= @Now
            BEGIN
                SET @Status = 3; -- InProgress
            END
            ELSE
            BEGIN
                SET @Status = 2; -- Scheduled
            END
            
            INSERT INTO [ProductionPlans] ([PlanCode], [Description], [PlannedStartDate], [PlannedEndDate], [Status], [TargetQuantity], [EstimatedEndDate], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (
                @PlanCode1,
                N'Üretim Hattı-A Bobin Sarımı (' + CONVERT(VARCHAR, @LoopCounter) + ')',
                @PlannedStart,
                @PlannedEnd,
                @Status,
                @Qty,
                @PlannedEnd,
                @Now,
                @Now,
                1, 1, 0
            );
            SET @PlanId = SCOPE_IDENTITY();
            
            -- Assign product (circular)
            SELECT @ProdId = Id FROM @Products WHERE Idx = (1 + (@LoopCounter % @ProductCount));
            INSERT INTO [ProductionPlanProducts] ([ProductionPlanId], [ProductId], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (@PlanId, @ProdId, @Now, @Now, 1, 1, 0);
            
            -- Assign machine (circular)
            SELECT @MachId = Id FROM @Machines WHERE Idx = (1 + (@LoopCounter % @MachineCount));
            INSERT INTO [ProductionPlanMachines] ([ProductionPlanId], [MachineId], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (@PlanId, @MachId, @Now, @Now, 1, 1, 0);

            -- If status is NotCompleted, insert overlapping malfunction log if not exists
            IF @Status = 5
            BEGIN
                DECLARE @Desc1 NVARCHAR(500) = N'Örnek Hata: ' + @PlanCode1 + N' sırasında rulman arızası.';
                IF NOT EXISTS (SELECT 1 FROM [MachineStatusLogs] WHERE [Description] = @Desc1 AND [IsDeleted] = 0)
                BEGIN
                    INSERT INTO [MachineStatusLogs] ([MachineId], [Status], [Description], [StartTime], [EndTime], [WeekNumber], [Year], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
                    VALUES (
                        @MachId,
                        5, -- Malfunction
                        @Desc1,
                        DATEADD(HOUR, 2, @PlannedStart),
                        DATEADD(HOUR, 4, @PlannedStart),
                        DATEPART(ISO_WEEK, @PlannedStart),
                        YEAR(@PlannedStart),
                        @Now, @Now, 1, 1, 0
                    );
                END
            END
        END

        -- Plan 2 (Line B - Parallel)
        SET @PlannedStart = DATEADD(HOUR, 10, @CurrentDate); -- Starts at 10:00
        SET @PlanCode2 = 'PLN-' + CONVERT(VARCHAR, @PlannedStart, 112) + '-02';
        
        -- Check if plan already exists before inserting
        IF NOT EXISTS (SELECT 1 FROM [ProductionPlans] WHERE [PlanCode] = @PlanCode2 AND [IsDeleted] = 0)
        BEGIN
            SET @Qty = 600 + (@LoopCounter * 53) % 1800;
            SET @PlannedEnd = DATEADD(HOUR, 6 + ((@LoopCounter * 5) % 11), @PlannedStart);
            
            IF @PlannedEnd < @Now
            BEGIN
                IF @LoopCounter % 9 = 0
                    SET @Status = 5; -- NotCompleted (Interrupted)
                ELSE
                    SET @Status = 4; -- Completed
            END
            ELSE IF @PlannedStart <= @Now AND @PlannedEnd >= @Now
            BEGIN
                SET @Status = 3; -- InProgress
            END
            ELSE
            BEGIN
                SET @Status = 2; -- Scheduled
            END

            INSERT INTO [ProductionPlans] ([PlanCode], [Description], [PlannedStartDate], [PlannedEndDate], [Status], [TargetQuantity], [EstimatedEndDate], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (
                @PlanCode2,
                N'Üretim Hattı-B Hassas Dilme (' + CONVERT(VARCHAR, @LoopCounter) + ')',
                @PlannedStart,
                @PlannedEnd,
                @Status,
                @Qty,
                @PlannedEnd,
                @Now,
                @Now,
                1, 1, 0
            );
            SET @PlanId = SCOPE_IDENTITY();
            
            -- Assign product (circular, offset)
            SELECT @ProdId = Id FROM @Products WHERE Idx = (1 + ((@LoopCounter + 3) % @ProductCount));
            INSERT INTO [ProductionPlanProducts] ([ProductionPlanId], [ProductId], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (@PlanId, @ProdId, @Now, @Now, 1, 1, 0);
            
            -- Assign machine (circular, offset)
            SELECT @MachId = Id FROM @Machines WHERE Idx = (1 + ((@LoopCounter + 2) % @MachineCount));
            INSERT INTO [ProductionPlanMachines] ([ProductionPlanId], [MachineId], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
            VALUES (@PlanId, @MachId, @Now, @Now, 1, 1, 0);

            -- If status is NotCompleted, insert overlapping maintenance log if not exists
            IF @Status = 5
            BEGIN
                DECLARE @Desc2 NVARCHAR(500) = N'Örnek Hata: ' + @PlanCode2 + N' sırasında plansız acil bakım.';
                IF NOT EXISTS (SELECT 1 FROM [MachineStatusLogs] WHERE [Description] = @Desc2 AND [IsDeleted] = 0)
                BEGIN
                    INSERT INTO [MachineStatusLogs] ([MachineId], [Status], [Description], [StartTime], [EndTime], [WeekNumber], [Year], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [IsDeleted])
                    VALUES (
                        @MachId,
                        4, -- Maintenance
                        @Desc2,
                        DATEADD(HOUR, 3, @PlannedStart),
                        DATEADD(HOUR, 5, @PlannedStart),
                        DATEPART(ISO_WEEK, @PlannedStart),
                        YEAR(@PlannedStart),
                        @Now, @Now, 1, 1, 0
                    );
                END
            END
        END
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    SET @LoopCounter = @LoopCounter + 1;
END

PRINT 'Seeding completed successfully! Multi-year timeline appended without data loss (2025-11-01 to 2027-02-28).';
