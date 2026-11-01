-- Velika tabela za demo "počasna poizvedba brez indeksa" (obrazec Demo napak).
-- Vsebuje 500.000 vrstic; stolpec ref_code namerno NIMA indeksa - aplikacija ga v demu
-- doda in odstrani (ix_activity_log_ref_code), da se v EXPLAIN vidi razlika.
-- Zaženi po 01_schema.sql, npr.:
--   mysql -u testapp_user -p --default-character-set=utf8mb4 testapp < 05_activity_log.sql
-- Polnjenje traja nekaj sekund.

SET NAMES utf8mb4;

DROP TABLE IF EXISTS activity_log;

CREATE TABLE activity_log (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    customer_id INT UNSIGNED NOT NULL,
    action      VARCHAR(20)  NOT NULL,
    ref_code    VARCHAR(20)  NOT NULL,   -- BREZ indeksa (namerno)
    details     VARCHAR(200) NULL,
    created_at  DATETIME     NOT NULL,
    PRIMARY KEY (id)
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_0900_ai_ci;

-- Rekurzivni CTE ustvari zaporedje 1..500000; privzeta meja rekurzije je 1000.
SET SESSION cte_max_recursion_depth = 500000;

INSERT INTO activity_log (customer_id, action, ref_code, details, created_at)
WITH RECURSIVE seq (n) AS (
    SELECT 1
    UNION ALL
    SELECT n + 1 FROM seq WHERE n < 500000
)
SELECT 1 + (n % 20),
       ELT(1 + (n % 4), 'prijava', 'ogled', 'naročilo', 'odjava'),
       CONCAT('REF-', LPAD(n, 7, '0')),
       CONCAT('Testni zapis št. ', n),
       '2025-01-01 00:00:00' + INTERVAL (n % 525600) MINUTE
  FROM seq;
