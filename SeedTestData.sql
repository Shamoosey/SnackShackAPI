-- Declare GUID variables for users
DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User5Id UNIQUEIDENTIFIER = NEWID();

DECLARE @CurrencyWODId UNIQUEIDENTIFIER = '6922D54E-5509-4E5C-AEAA-4399A90F7073';
DECLARE @CurrencyBWLId UNIQUEIDENTIFIER = '18A8C6A6-18A1-4421-9BFD-5886A011BE17';
DECLARE @CurrencyMILId UNIQUEIDENTIFIER = '26654D57-D733-4646-A7CD-78DB9BA09A24';

-- Declare table variables to capture the inserted Account Ids
DECLARE @InsertedAccounts TABLE (AccountId UNIQUEIDENTIFIER, AccountName NVARCHAR(MAX));

-- Insert users and capture their GUIDs
INSERT INTO [SnackShack].[dbo].[Users] (Id, Email, Username, CreatedDate, IsAdmin)
VALUES 
    (@User1Id, 'user1@example.com', 'user1', '2025-01-22 10:00:00', 0),
    (@User2Id, 'user2@example.com', 'user2', '2025-01-22 10:10:00', 1),
    (@User3Id, 'user3@example.com', 'user3', '2025-01-22 10:20:00', 0),
    (@User4Id, 'user4@example.com', 'user4', '2025-01-22 10:30:00', 0),
    (@User5Id, 'user5@example.com', 'user5', '2025-01-22 10:40:00', 1);

-- Insert accounts and capture the inserted Ids into @InsertedAccounts table variable
INSERT INTO [SnackShack].[dbo].[Accounts] (Id, AccountName, Amount, CreatedDate, UserId, CurrencyId)
OUTPUT Inserted.Id, Inserted.AccountName INTO @InsertedAccounts(AccountId, AccountName)
VALUES
    (NEWID(), 'user1_account', 100.00, '2025-01-22 00:00:00', @User1Id, @CurrencyWODId),  -- User1, Currency WOD
    (NEWID(), 'user2_account', 200.00, '2025-01-22 10:10:00', @User2Id, @CurrencyBWLId),  -- User2, Currency BWL
    (NEWID(), 'user3_account', 150.00, '2025-01-22 10:00:00', @User3Id, @CurrencyMILId);  -- User3, Currency MIL

-- Declare variables to hold the actual inserted Account IDs
DECLARE @AccountWODId UNIQUEIDENTIFIER;
DECLARE @AccountBWLId UNIQUEIDENTIFIER;
DECLARE @AccountMILId UNIQUEIDENTIFIER;

-- Retrieve the Account Ids from the table variable based on AccountName
SELECT 
    @AccountWODId = AccountId
FROM @InsertedAccounts
WHERE AccountName = 'user1_account';

SELECT 
    @AccountBWLId = AccountId
FROM @InsertedAccounts
WHERE AccountName = 'user2_account';

SELECT 
    @AccountMILId = AccountId
FROM @InsertedAccounts
WHERE AccountName = 'user3_account';

-- Insert transactions using the correct account IDs
INSERT INTO [SnackShack].[dbo].[Transactions] 
    (Id, Amount, TransactionDate, TransactionType, Notes, IsPending, InitiatedByUserId, SenderAccountId, ReceiverAccountId)
VALUES
    (NEWID(), 100.00, '2025-01-22 00:00:00', 3, 'Account-to-Account transfer from WOD account', 1, @User1Id, @AccountWODId, @AccountWODId),  -- Account-to-Account transfer (TransactionType: 3)
    (NEWID(), 50.00, '2025-01-22 10:20:00', 3, 'Account-to-Account transfer from BWL account', 1, @User3Id, @AccountBWLId, @AccountBWLId),  -- Account-to-Account transfer
    (NEWID(), 200.00, '2025-01-22 10:10:00', 0, 'User-to-User transfer from MIL account', 1, @User2Id, @AccountMILId, @AccountBWLId),  -- User-to-User transfer (TransactionType: 0)
    (NEWID(), 150.00, '2025-01-22 10:00:00', 3, 'Account-to-Account transfer from user1 account', 0, @User1Id, @AccountMILId, @AccountMILId),  -- Account-to-Account transfer, pending (IsPending: 0)
    (NEWID(), 75.00, '2025-01-22 10:30:00', 0, 'User-to-User transfer from user4 account', 1, @User4Id, @AccountMILId, @AccountBWLId),  -- User-to-User transfer
    (NEWID(), 250.00, '2025-01-22 10:40:00', 1, 'Account-to-Bank transfer from user5 account', 0, @User5Id, @AccountBWLId, @AccountBWLId);  -- Account-to-Bank transfer (TransactionType: 1)
