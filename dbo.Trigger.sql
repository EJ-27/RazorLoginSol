CREATE TRIGGER PreventDuplicateEvent
ON Event
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM Event e
        INNER JOIN inserted i
        ON e.Event_date = i.Event_date 
        AND e.Event_start_time = i.Event_start_time 
		AND e.Event_end_time = i.Event_end_time 
        AND e.Event_Location = i.Event_Location
    )
    BEGIN
        RAISERROR ('An event is already scheduled at this date, time, and location.', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        -- Insert if no duplicate is found
        INSERT INTO Event (Event_date, Event_start_time, Event_end_time, Event_Location, Event_employee_rep_ID, Event_ID, Event_name)
        SELECT Event_date, Event_start_time, Event_end_time, Event_Location, Event_employee_rep_ID, Event_ID, Event_name
        FROM inserted;
    END
END;
