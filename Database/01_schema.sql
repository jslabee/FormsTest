-- Shema za aplikacijo WindowsFormsApp1.
-- Skripta NE ustvari baze in ne izbere baze (brez CREATE DATABASE / USE):
-- zaženi jo nad bazo testapp, npr.:
--   mysql -u testapp_user -p testapp < 01_schema.sql

-- Tabeli naročil (03_orders.sql) se s tujim ključem sklicujeta na customers.
-- Če bi najprej brisali customers, bi MySQL zavrnil DROP (FK), zato najprej odvisni tabeli.
DROP TABLE IF EXISTS order_items;
DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS customers;

CREATE TABLE customers (
    id         INT UNSIGNED NOT NULL AUTO_INCREMENT,
    first_name VARCHAR(100) NOT NULL,        -- obvezno (enako pravilo kot CustomerValidator)
    last_name  VARCHAR(100) NULL,
    email      VARCHAR(255) NULL,
    phone      VARCHAR(50)  NULL,
    -- Optimistično zaklepanje: aplikacija pri UPDATE preveri, ali je updated_at še enak tistemu,
    -- ki ga je prebrala. DATETIME(6) = mikrosekunde, da dve spremembi v isti sekundi nista "enaki".
    -- ON UPDATE poskrbi, da se čas spremeni tudi ob urejanju mimo aplikacije (npr. v Adminerju).
    updated_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    PRIMARY KEY (id),
    -- Seznam v aplikaciji je urejen po priimku in imenu.
    KEY ix_customers_name (last_name, first_name)
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_0900_ai_ci;
