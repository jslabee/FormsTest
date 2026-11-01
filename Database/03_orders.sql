-- Naročila in postavke naročil (master-detail) + testni podatki.
-- Zaženi po 01_schema.sql in 02_seed.sql nad bazo testapp, npr.:
--   mysql -u testapp_user -p --default-character-set=utf8mb4 testapp < 03_orders.sql

SET NAMES utf8mb4;

DROP TABLE IF EXISTS order_items;
DROP TABLE IF EXISTS orders;

-- Glava naročila. Skupnega zneska NE hranimo - izračuna se iz postavk
-- (ena resnica; ni nevarnosti, da se vsota in postavke razhajata).
CREATE TABLE orders (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    customer_id INT UNSIGNED NOT NULL,
    order_date  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    status      VARCHAR(20)  NOT NULL DEFAULT 'Novo',
    note        VARCHAR(500) NULL,
    PRIMARY KEY (id),
    -- Indeks za "vsa naročila stranke, najnovejša najprej". Ker se začne s customer_id,
    -- ga InnoDB uporabi tudi za tuji ključ (sicer bi za FK ustvaril svojega).
    KEY ix_orders_customer_date (customer_id, order_date),
    -- RESTRICT: stranke z naročili ni mogoče izbrisati (aplikacija to javi uporabniku).
    CONSTRAINT fk_orders_customer FOREIGN KEY (customer_id)
        REFERENCES customers (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT chk_orders_status CHECK (status IN ('Novo', 'Poslano', 'Zaključeno', 'Stornirano'))
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_0900_ai_ci;

-- Postavke naročila. CASCADE: izbris naročila izbriše tudi njegove postavke.
CREATE TABLE order_items (
    id           INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    order_id     INT UNSIGNED  NOT NULL,
    product_name VARCHAR(200)  NOT NULL,
    quantity     INT           NOT NULL,
    unit_price   DECIMAL(10,2) NOT NULL,   -- DECIMAL, nikoli FLOAT/DOUBLE za denar (zaokroževanje)
    PRIMARY KEY (id),
    KEY ix_order_items_order (order_id),
    CONSTRAINT fk_order_items_order FOREIGN KEY (order_id)
        REFERENCES orders (id) ON DELETE CASCADE,
    -- Na to omejitev se zanaša "Simuliraj napako": aplikacija namerno vstavi postavko s
    -- količino -1, baza jo zavrne in celotna transakcija se razveljavi.
    CONSTRAINT chk_order_items_quantity CHECK (quantity > 0),
    CONSTRAINT chk_order_items_price CHECK (unit_price >= 0)
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_0900_ai_ci;

-- Testni podatki. Id-ji so navedeni izrecno, da se postavke zanesljivo vežejo na prava naročila.
INSERT INTO orders (id, customer_id, order_date, status, note) VALUES
    ( 1,  1, '2025-09-03 09:15:00', 'Zaključeno', NULL),
    ( 2,  1, '2025-11-21 14:02:00', 'Zaključeno', 'Dostava na naslov podjetja'),
    ( 3,  1, '2026-02-10 10:30:00', 'Poslano',    NULL),
    ( 4,  2, '2025-10-05 08:45:00', 'Zaključeno', NULL),
    ( 5,  2, '2026-03-18 16:20:00', 'Novo',       'Pokličite pred dostavo'),
    ( 6,  3, '2025-12-01 11:11:00', 'Zaključeno', NULL),
    ( 7,  3, '2026-01-15 13:40:00', 'Stornirano', 'Kupec je odstopil od nakupa'),
    ( 8,  3, '2026-04-02 09:05:00', 'Poslano',    NULL),
    ( 9,  4, '2026-05-12 15:30:00', 'Novo',       NULL),
    (10,  5, '2025-08-19 10:00:00', 'Zaključeno', NULL),
    (11,  5, '2026-06-07 12:25:00', 'Poslano',    'Račun na e-pošto'),
    (12,  6, '2026-02-28 17:45:00', 'Zaključeno', NULL),
    (13,  7, '2025-11-11 11:11:00', 'Zaključeno', NULL),
    (14,  7, '2026-07-01 08:00:00', 'Novo',       NULL),
    (15,  9, '2026-03-03 14:14:00', 'Poslano',    NULL),
    (16, 11, '2025-10-30 10:10:00', 'Zaključeno', 'Darilno pakiranje'),
    (17, 12, '2026-01-20 09:30:00', 'Zaključeno', NULL),
    (18, 13, '2026-08-08 18:00:00', 'Novo',       NULL),
    (19, 15, '2026-04-22 13:00:00', 'Poslano',    NULL),
    (20, 18, '2026-09-09 09:09:00', 'Novo',       'Prevzem v trgovini'),
    (21, 19, '2026-05-05 16:16:00', 'Zaključeno', NULL),
    (22, 20, '2026-10-01 10:45:00', 'Novo',       NULL);

INSERT INTO order_items (order_id, product_name, quantity, unit_price) VALUES
    ( 1, 'Pisarniški stol Ergo',       1, 189.90),
    ( 1, 'Podloga za miško',           2,   9.50),
    ( 2, 'Monitor 27" QHD',            2, 279.00),
    ( 2, 'HDMI kabel 2 m',             2,  12.90),
    ( 2, 'Nosilec za monitor',         1,  49.99),
    ( 3, 'Brezžična tipkovnica',       1,  59.90),
    ( 4, 'Prenosnik 14"',              1, 899.00),
    ( 4, 'Torba za prenosnik',         1,  39.90),
    ( 5, 'Spletna kamera Full HD',     1,  69.00),
    ( 5, 'Slušalke z mikrofonom',      1,  45.50),
    ( 6, 'Tiskalnik laserski',         1, 229.00),
    ( 6, 'Toner črn',                  3,  64.90),
    ( 7, 'Zunanji disk 2 TB',          1,  89.90),
    ( 8, 'USB ključ 64 GB',            5,   8.99),
    ( 8, 'Čitalec kartic',             1,  14.50),
    ( 9, 'Namizna svetilka LED',       2,  34.90),
    (10, 'Pisarniška miza',            1, 349.00),
    (10, 'Predalnik',                  1, 129.00),
    (11, 'Papir A4 (500 listov)',     10,   4.99),
    (11, 'Kemični svinčnik (10 kos)',  3,   6.50),
    (12, 'Usmerjevalnik Wi-Fi 6',      1, 119.00),
    (13, 'Grafična tablica',           1, 199.00),
    (14, 'Brezžična miška',            2,  24.90),
    (14, 'Baterije AA (8 kos)',        2,   7.90),
    (15, 'Zvočnik Bluetooth',          1,  79.00),
    (16, 'Tablica 10"',                1, 329.00),
    (16, 'Zaščitni ovitek',            1,  19.90),
    (17, 'Monitor 24" FHD',            1, 149.00),
    (18, 'SSD 1 TB',                   2,  89.00),
    (18, 'RAM 16 GB',                  2,  54.90),
    (19, 'Projektor',                  1, 499.00),
    (20, 'Razdelilec USB-C',           1,  39.90),
    (21, 'Pametna ura',                1, 249.00),
    (21, 'Polnilec USB-C 65 W',        1,  44.90),
    (22, 'Mikrofon USB',               1,  99.00);
