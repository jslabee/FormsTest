-- Shranjene procedure.
-- Zaženi po 03_orders.sql z odjemalcem mysql (pozna ukaz DELIMITER), npr.:
--   mysql -u testapp_user -p --default-character-set=utf8mb4 testapp < 04_procedures.sql

SET NAMES utf8mb4;

DROP PROCEDURE IF EXISTS sp_customer_stats;

-- DELIMITER ni SQL ukaz, ampak ukaz odjemalca mysql: telo procedure vsebuje ";",
-- zato začasno zamenjamo ločilo ukazov, da odjemalec ne pošlje procedure po kosih.
DELIMITER //

-- Statistika stranke:
--   OUT parametri: število naročil, skupni znesek in datum zadnjega naročila,
--   rezultat (result set): 5 izdelkov, za katere je stranka porabila največ.
-- Stornirana naročila ne štejejo.
CREATE PROCEDURE sp_customer_stats(
    IN  p_customer_id   INT UNSIGNED,
    OUT p_order_count   INT,
    OUT p_total_amount  DECIMAL(12,2),
    OUT p_last_order    DATETIME
)
BEGIN
    SELECT COUNT(DISTINCT o.id),
           COALESCE(SUM(i.quantity * i.unit_price), 0),
           MAX(o.order_date)
      INTO p_order_count, p_total_amount, p_last_order
      FROM orders o
      LEFT JOIN order_items i ON i.order_id = o.id
     WHERE o.customer_id = p_customer_id
       AND o.status <> 'Stornirano';

    SELECT i.product_name                 AS ProductName,
           -- SUM nad INT vrne DECIMAL; CAST v celo število, da se lepo preslika v int.
           CAST(SUM(i.quantity) AS SIGNED) AS TotalQuantity,
           SUM(i.quantity * i.unit_price) AS TotalAmount
      FROM orders o
      JOIN order_items i ON i.order_id = o.id
     WHERE o.customer_id = p_customer_id
       AND o.status <> 'Stornirano'
     GROUP BY i.product_name
     ORDER BY TotalAmount DESC
     LIMIT 5;
END //

DELIMITER ;
