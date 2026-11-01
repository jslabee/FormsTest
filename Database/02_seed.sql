-- Testni podatki: 20 strank s šumniki (č, š, ž, ć, đ).
-- Zaženi po 01_schema.sql nad bazo testapp, npr.:
--   mysql -u testapp_user -p --default-character-set=utf8mb4 testapp < 02_seed.sql

SET NAMES utf8mb4;

INSERT INTO customers (first_name, last_name, email, phone) VALUES
    ('Janez',    'Novak',       'janez.novak@example.si',       '041 123 456'),
    ('Marija',   'Horvat',      'marija.horvat@example.si',     '031 234 567'),
    ('Žiga',     'Kovačič',     'ziga.kovacic@example.si',      '040 345 678'),
    ('Špela',    'Krajnc',      'spela.krajnc@example.si',      '051 456 789'),
    ('Črt',      'Zupančič',    'crt.zupancic@example.si',      '041 567 890'),
    ('Nataša',   'Potočnik',    'natasa.potocnik@example.si',   '031 678 901'),
    ('Matjaž',   'Kos',         'matjaz.kos@example.si',        '040 789 012'),
    ('Tjaša',    'Vidmar',      'tjasa.vidmar@example.si',      '051 890 123'),
    ('Rok',      'Golob',       'rok.golob@example.si',         '041 901 234'),
    ('Urška',    'Turk',        'urska.turk@example.si',        '031 012 345'),
    ('Aleš',     'Božič',       'ales.bozic@example.si',        '040 111 222'),
    ('Mojca',    'Kralj',       'mojca.kralj@example.si',       '051 222 333'),
    ('Primož',   'Žagar',       'primoz.zagar@example.si',      '041 333 444'),
    ('Anže',     'Rozman',      'anze.rozman@example.si',       '031 444 555'),
    ('Lučka',    'Petrič',      'lucka.petric@example.si',      '040 555 666'),
    ('Gašper',   'Šinkovec',    NULL,                           '051 666 777'),
    ('Nuša',     'Jerič',       'nusa.jeric@example.si',        NULL),
    ('Boštjan',  'Čeh',         'bostjan.ceh@example.si',       '041 777 888'),
    ('Đurđa',    'Ćosić',       'durda.cosic@example.si',       '031 888 999'),
    ('Klemen',   'Hočevar',     NULL,                           NULL);
