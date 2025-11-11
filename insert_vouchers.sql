-- Script to insert 10 sample vouchers into VOUCHERS table
USE [HandicraftShop]
GO

-- Insert 10 vouchers with various discount types and conditions
INSERT INTO [dbo].[VOUCHERS] 
    ([VoucherName], [Code], [Description], [DiscountPercentage], [MaxReducing], [Quantity], [UsageCount], [ExpiryDate], [IsActive], [MinOrderValue], [MaxUsagePerUser])
VALUES
    -- Voucher 1: 10% off, max 50,000 VND
    ('Welcome Discount', 'WELCOME10', 'Giảm 10% cho đơn hàng đầu tiên, tối đa 50,000 VND', 10, 50000, 100, 0, DATEADD(month, 3, GETDATE()), 1, 200000, 1),
    
    -- Voucher 2: 15% off, max 100,000 VND
    ('Summer Sale', 'SUMMER15', 'Giảm 15% cho mùa hè, tối đa 100,000 VND', 15, 100000, 200, 0, DATEADD(month, 2, GETDATE()), 1, 300000, 2),
    
    -- Voucher 3: 20% off, max 150,000 VND
    ('Flash Sale', 'FLASH20', 'Giảm 20% trong thời gian giới hạn, tối đa 150,000 VND', 20, 150000, 50, 0, DATEADD(month, 1, GETDATE()), 1, 500000, 1),
    
    -- Voucher 4: 25% off, max 200,000 VND
    ('VIP Member', 'VIP25', 'Giảm 25% dành cho khách hàng VIP, tối đa 200,000 VND', 25, 200000, 30, 0, DATEADD(month, 6, GETDATE()), 1, 800000, 3),
    
    -- Voucher 5: 30% off, max 250,000 VND
    ('Black Friday', 'BF30', 'Giảm 30% ngày Black Friday, tối đa 250,000 VND', 30, 250000, 100, 0, DATEADD(month, 4, GETDATE()), 1, 1000000, 1),
    
    -- Voucher 6: 12% off, max 60,000 VND
    ('New Customer', 'NEW12', 'Giảm 12% cho khách hàng mới, tối đa 60,000 VND', 12, 60000, 150, 0, DATEADD(month, 3, GETDATE()), 1, 250000, 1),
    
    -- Voucher 7: 18% off, max 120,000 VND
    ('Weekend Special', 'WEEKEND18', 'Giảm 18% cuối tuần, tối đa 120,000 VND', 18, 120000, 80, 0, DATEADD(month, 2, GETDATE()), 1, 400000, 2),
    
    -- Voucher 8: 22% off, max 180,000 VND
    ('Birthday Special', 'BIRTHDAY22', 'Giảm 22% nhân dịp sinh nhật, tối đa 180,000 VND', 22, 180000, 60, 0, DATEADD(month, 5, GETDATE()), 1, 600000, 1),
    
    -- Voucher 9: 5% off, max 30,000 VND (small discount)
    ('Small Gift', 'GIFT5', 'Giảm 5% quà tặng nhỏ, tối đa 30,000 VND', 5, 30000, 500, 0, DATEADD(month, 1, GETDATE()), 1, 100000, 5),
    
    -- Voucher 10: 35% off, max 300,000 VND (highest discount)
    ('Mega Sale', 'MEGA35', 'Giảm 35% siêu khuyến mãi, tối đa 300,000 VND', 35, 300000, 25, 0, DATEADD(month, 1, GETDATE()), 1, 1500000, 1);

GO

-- Verify the inserted data
SELECT 
    [VoucherID],
    [VoucherName],
    [Code],
    [DiscountPercentage],
    [MaxReducing],
    [Quantity],
    [UsageCount],
    [ExpiryDate],
    [IsActive],
    [MinOrderValue],
    [MaxUsagePerUser]
FROM [dbo].[VOUCHERS]
ORDER BY [VoucherID] DESC;

