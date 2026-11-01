# Priprava na tehnični intervju – WinForms, .NET Framework 4.7, Dapper, MySQL

Ta dokument je vodič po primerih v aplikaciji. Za vsako temo so kratka razlaga, mesto v kodi
(datoteka + metoda) in nekaj vprašanj, ki jih lahko dobiš na intervjuju, z odgovori.
V kodi so ista mesta označena s komentarjem `// INTERVJU:`.

## Kazalo

1. [Zagon in priprava baze](#1-zagon-in-priprava-baze)
2. [Arhitektura: plasti, vmesniki, dependency injection](#2-arhitektura-plasti-vmesniki-dependency-injection)
3. [using in connection pooling](#3-using-in-connection-pooling)
4. [Parametri in SQL injection](#4-parametri-in-sql-injection)
5. [Transakcije (naročilo s postavkami)](#5-transakcije-naročilo-s-postavkami)
6. [Dapper: multi-mapping in QueryMultiple](#6-dapper-multi-mapping-in-querymultiple)
7. [Straničenje (LIMIT/OFFSET) in iskanje z LIKE](#7-straničenje-limitoffset-in-iskanje-z-like)
8. [Shranjene procedure](#8-shranjene-procedure)
9. [Optimistično zaklepanje](#9-optimistično-zaklepanje)
10. [Indeksi in EXPLAIN](#10-indeksi-in-explain)
11. [async/await in deadlock zaradi .Result](#11-asyncawait-in-deadlock-zaradi-result)
12. [IProgress&lt;T&gt; in CancellationToken](#12-iprogresst-in-cancellationtoken)
13. [BackgroundWorker (legacy)](#13-backgroundworker-legacy)
14. [Invoke / InvokeRequired](#14-invoke--invokerequired)
15. [BindingSource, BindingList&lt;T&gt; in master-detail](#15-bindingsource-bindinglistt-in-master-detail)
16. [Validacija z ErrorProvider](#16-validacija-z-errorprovider)
17. [Izvoz v CSV s SaveFileDialog](#17-izvoz-v-csv-s-savefiledialog)
18. [Globalna obravnava napak in log](#18-globalna-obravnava-napak-in-log)
19. [Odzivnost UI: debounce in "race condition"](#19-odzivnost-ui-debounce-in-race-condition)
20. [Testi enot](#20-testi-enot)
21. [Hitri pregled: kje je kaj](#21-hitri-pregled-kje-je-kaj)

---

## 1. Zagon in priprava baze

- `docker compose up -d` zažene MySQL 8.4 (`max_connections = 30`, vklopljen general in slow log).
- Ob **prvem** zagonu kontejnerja se po abecedi izvedejo skripte iz `Database/`:
  - `01_schema.sql` – tabela `customers` (s stolpcem `updated_at` za optimistično zaklepanje),
  - `02_seed.sql` – 20 strank,
  - `03_orders.sql` – tabeli `orders` in `order_items` (tuji ključi, indeksi, CHECK) + testna naročila,
  - `04_procedures.sql` – shranjena procedura `sp_customer_stats`,
  - `05_activity_log.sql` – tabela s 500.000 vrsticami za demo indeksov.
- Bazo ponastaviš z `docker compose down -v` in ponovnim `up -d`.
- Obstoječo bazo (ustvarjeno pred temi primeri) nadgradiš brez izgube podatkov:
  ```sql
  ALTER TABLE customers ADD COLUMN updated_at DATETIME(6) NOT NULL
      DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);
  ```
  nato poženeš `03`, `04` in `05` (npr. `docker exec -i testapp-mysql mysql -utestapp_user -p testapp < Database/03_orders.sql`).
- Testi: v Visual Studiu *Test → Run All Tests* (Test Explorer) ali iz terminala
  `vstest.console.exe WindowsFormsApp1.Tests\bin\Debug\WindowsFormsApp1.Tests.dll`.
- Testni CSV za uvoz: `WindowsFormsApp1/Data/uvoz_strank.csv` (500 vrstic, 10 namerno neveljavnih).

---

## 2. Arhitektura: plasti, vmesniki, dependency injection

**Razlaga.** Aplikacija ima tri plasti: `Forms/` (prikaz), `Services/` (poslovna pravila, brez
referenc na WinForms) in `Repositories/` (SQL, Dapper). Servisi poznajo samo vmesnika
`ICustomerRepository` in `IOrderRepository`, ki ju dobijo v konstruktorju. Vse konkretne razrede
sestavi `Program.CreateMainForm` – t. i. *composition root* (DI brez ogrodja, "pure DI").

**V kodi:**
- `Program.cs` → `CreateMainForm`
- `Repositories/ICustomerRepository.cs`, `Repositories/IOrderRepository.cs`
- `Services/CustomerService.cs` → konstruktor; `Services/OrderService.cs` → konstruktor
- `Forms/MainForm.cs` → konstruktor (dobi servise, ne ustvarja jih)

**Vprašanja:**
- *Zakaj servis ne ustvari repozitorija sam (`new CustomerRepository(...)`)?*
  Ker bi bil potem trdno vezan na MySQL. Z vmesnikom v konstruktorju lahko v testih podamo lažni
  repozitorij, v produkciji pa pravega – servis se ne spremeni (načelo *Dependency Inversion*).
- *Kaj je composition root?* Edino mesto (ob zagonu), kjer se odločimo za konkretne implementacije
  in jih povežemo. Ostala koda dobi odvisnosti od zunaj.
- *Zakaj servisi nimajo referenc na `System.Windows.Forms`?* Da jih lahko testiramo brez UI in
  uporabimo tudi v drugi aplikaciji (npr. spletni storitvi). Obrazec samo prikazuje.

---

## 3. using in connection pooling

**Razlaga.** Vsaka metoda repozitorija odpre svojo povezavo v `using`. `Dispose` povezave ne zapre
fizično, ampak jo vrne v *pool* – naslednji `Open` jo dobi nazaj v mikrosekundah. Če `using`
manjka, povezava ostane "v uporabi", pool odpira nove, dokler strežnik ne vrne
**"Too many connections"** (v našem dockerju pri 30).

**V kodi:**
- Pravilno: vse metode v `Repositories/CustomerRepository.cs` in `Repositories/OrderRepository.cs`
- Demo: *Orodja → Demo napak → 1a/1b*; `Repositories/DemoRepository.cs` →
  `OpenConnectionsWithoutUsingAsync` (napačno) in `OpenConnectionsWithUsingAsync` (pravilno,
  pokaže, da je bila uporabljena ena sama fizična povezava – `CONNECTION_ID()`).

**Vprašanja:**
- *Zakaj povezave ne hranimo v polju razreda in je odpremo enkrat?* Ker je pooling poceni, odprta
  povezava pa zaseda vir na strežniku, ni varna za več niti in se lahko prekine (timeout, restart
  strežnika). Pravilo: "odpri pozno, zapri zgodaj".
- *Kaj naredi `using`?* Prevajalnik ga prevede v `try { ... } finally { obj.Dispose(); }` – Dispose
  se pokliče tudi ob izjemi.
- *Kako bi v produkciji odkril puščanje povezav?* `SHOW PROCESSLIST` / `Threads_connected` raste,
  povezave v stanju *Sleep*, napaka 1040; v kodi iščeš `new MySqlConnection` brez `using`.

---

## 4. Parametri in SQL injection

**Razlaga.** Vrednosti vedno podajamo kot parametre (`@Name`). Parameter je vedno samo *vrednost*,
nikoli del ukaza. Z lepljenjem niza vnos `x' OR '1'='1` spremeni pogoj in poizvedba vrne vse
vrstice; MySqlConnector privzeto izvede tudi več ukazov v enem klicu, zato bi `'; DROP TABLE ...`
res izbrisal tabelo (demo zato znak `;` zavrne).

**V kodi:**
- Pravilno: vse poizvedbe v repozitorijih, npr. `CustomerRepository.GetByIdAsync`
- Demo 3a/3b: `DemoRepository.BuildUnsafeSql`, `FindByLastNameUnsafeAsync`, `FindByLastNameSafeAsync`;
  `Services/DemoService.cs` → `SqlInjectionUnsafeAsync`, `SqlInjectionSafeAsync`

**Vprašanja:**
- *Ali je dovolj, če narekovaje podvojim (`'` → `''`)?* Ne. Obstajajo drugi ubežni znaki (`\` v
  MySQL), kodne strani, številski konteksti brez narekovajev ... Edina zanesljiva rešitev so parametri.
- *Ali lahko z parametrom podam ime tabele ali stolpca?* Ne – parametri so samo za vrednosti. Imena
  sestaviš iz **bele liste** konstant (glej `DemoRepository.EnsureRefCodeIndexAsync`).
- *Ali parametri ščitijo tudi pred `%` v LIKE?* Ne, glej poglavje 7.

---

## 5. Transakcije (naročilo s postavkami)

**Razlaga.** Glava naročila in vse postavke se shranijo v **eni transakciji**: ali se shrani vse
ali nič. Kljukica **"Simuliraj napako"** za veljavnimi postavkami doda postavko s količino -1;
baza jo zavrne (CHECK omejitev `chk_order_items_quantity`), koda naredi `Rollback` in v bazi ne
ostane niti glava niti prve postavke. Obrazec ostane odprt z ohranjenim vnosom.

**V kodi:**
- `Repositories/OrderRepository.cs` → `CreateAsync` (`BeginTransactionAsync`, `transaction:` v
  vsakem Dapper klicu, `CommitAsync`, `RollbackAsync` + `throw;`)
- `Services/OrderService.cs` → `CreateAsync` (validacija, kopija naročila, dodana napačna postavka)
- `Forms/OrderEditForm.cs` → `btnSave_Click`
- Tudi `CustomerRepository.UpdateAsync` (UPDATE + SELECT v isti transakciji) in `InsertBatchAsync`.

**Vprašanja:**
- *Kaj pomeni ACID?* Atomarnost (vse ali nič), konsistentnost (omejitve veljajo), izolacija
  (sočasne transakcije se ne vidijo "na pol"), trajnost (po COMMIT podatki preživijo izpad).
- *Kaj se zgodi, če pozabiš `transaction:` parameter v Dapper klicu?* MySqlConnector vrže napako,
  ker ukaz na povezavi z aktivno transakcijo ne sme teči mimo nje. (Pri nekaterih drugih
  ponudnikih bi ukaz tiho tekel izven transakcije.)
- *Zakaj `throw;` in ne `throw ex;`?* `throw;` ohrani prvotni stack trace, `throw ex;` ga ponastavi
  na trenutno vrstico.
- *Ali se ob rollbacku vrne tudi AUTO_INCREMENT?* Ne – številka je porabljena, v Id-jih nastane
  luknja. Id-ji zato niso "zaporedne številke računov".
- *Privzeta izolacija v MySQL InnoDB?* `REPEATABLE READ`.

---

## 6. Dapper: multi-mapping in QueryMultiple

**Razlaga.**
- **Multi-mapping**: ena poizvedba z JOIN vrne vrstico na *postavko*; Dapper vsako vrstico razreže na
  `Order` in `OrderItem` pri stolpcu, podanem v `splitOn` (drugi `Id`). Slovar poskrbi, da vsako
  naročilo ustvarimo enkrat in mu dodajamo postavke.
- **QueryMultiple**: več SELECT-ov v enem obisku baze; rezultate beremo zaporedno (`Read*`).

**V kodi:**
- Multi-mapping: `OrderRepository.GetByCustomerAsync`
- QueryMultiple: `OrderRepository.GetByIdAsync` (glava + postavke) in
  `CustomerRepository.SearchPagedAsync` (`COUNT(*)` + stran)

**Vprašanja:**
- *Kaj pomeni `splitOn` in kaj gre narobe, če je napačen?* Ime stolpca, kjer se začne naslednji
  objekt. Če ga Dapper ne najde, vrže napako; če je napačen, se stolpci preslikajo v napačen objekt.
- *JOIN + multi-mapping ali QueryMultiple?* JOIN je ena poizvedba, a ponavlja podatke glave v vsaki
  vrstici. QueryMultiple prenese manj podatkov in je preprostejši za mapiranje (dve poizvedbi, en
  round-trip). Pri globokih hierarhijah je QueryMultiple navadno bolj pregleden.
- *Kaj je problem N+1?* Ena poizvedba za seznam naročil + ena za postavke **vsakega** naročila (N
  dodatnih). Rešitev: JOIN/multi-mapping ali `WHERE order_id IN @Ids` (Dapper razširi seznam).

---

## 7. Straničenje (LIMIT/OFFSET) in iskanje z LIKE

**Razlaga.** Glavno okno prikaže 10 strank na stran (`LIMIT @Limit OFFSET @Offset`), iskanje
deluje po imenu, priimku in emailu z `LIKE @Pattern`. Skupno število zadetkov in stran se prebereta
v enem klicu (QueryMultiple). `ORDER BY` vsebuje na koncu še `id`, da je vrstni red enoličen.
Uporabnikove `%`, `_` in `\` ubežimo, sicer bi `%` zadel vse.

**V kodi:**
- `CustomerRepository.SearchPagedAsync`, `CustomerRepository.BuildLikePattern`
- `CustomerService.SearchAsync`
- `MainForm.ReloadCustomersAsync`, `btnPrevPage_Click`, `btnNextPage_Click`

**Vprašanja:**
- *Zakaj je OFFSET pri velikih tabelah počasen in kaj je alternativa?* Baza mora prebrati in
  zavreči vse prejšnje vrstice. Alternativa je *keyset pagination*:
  `WHERE (last_name, id) > (@LastName, @LastId) ORDER BY last_name, id LIMIT 10`.
- *Zakaj potrebuješ enoličen ORDER BY?* Brez njega vrstni red vrstic z enakim priimkom ni določen in
  se lahko med stranmi ista vrstica pojavi dvakrat ali nikoli.
- *Ali `LIKE '%novak%'` uporabi indeks?* Ne (vodilni `%`). `LIKE 'novak%'` ga lahko. Za iskanje
  "vsebuje" pri velikih podatkih se uporablja FULLTEXT indeks.

---

## 8. Shranjene procedure

**Razlaga.** `sp_customer_stats` vrne statistiko stranke: tri **OUT** parametre (število naročil,
skupni znesek, zadnje naročilo) in **rezultat** (top 5 izdelkov). V Dapperju: `DynamicParameters` z
`ParameterDirection.Output` in `commandType: CommandType.StoredProcedure`. Vrednosti OUT parametrov
so na voljo šele, ko je rezultat prebran do konca.

**V kodi:**
- `Database/04_procedures.sql`
- `CustomerRepository.GetStatsAsync`
- `MainForm.btnStats_Click` (gumb **Statistika**)

**Vprašanja:**
- *Prednosti/slabosti procedur?* + manj prometa, logika blizu podatkov, ločene pravice (EXECUTE).
  − logika razpršena med aplikacijo in bazo, težje verzioniranje, testiranje in razhroščevanje,
  vezanost na ponudnika baze.
- *Zakaj v skripti `DELIMITER //`?* Telo procedure vsebuje `;`. `DELIMITER` je ukaz odjemalca
  `mysql` (ne SQL), da ne pošlje procedure po kosih. Iz aplikacije ga ne potrebuješ.
- *Kako dobiš OUT parameter z Dapperjem?* `parameters.Add("ime", dbType: ..., direction:
  ParameterDirection.Output)` in po izvedbi `parameters.Get<T>("ime")`.

---

## 9. Optimistično zaklepanje

**Razlaga.** Stolpec `updated_at DATETIME(6)` je "verzija" vrstice. `UPDATE ... WHERE id = @Id AND
updated_at = @UpdatedAt` uspe samo, če vrstice vmes ni spremenil nihče drug. Če ne najde vrstice,
repozitorij preveri, ali vrstica še obstaja (→ `NotFound`) ali jo je spremenil drug (→ `Conflict`).
Servis konflikt prevede v `ConcurrencyException`, obrazec uporabnika opozori in osveži podatke.

**V kodi:**
- `CustomerRepository.UpdateAsync`, `Models/UpdateResult.cs`
- `CustomerService.UpdateAsync`, `Services/ConcurrencyException.cs`
- `MainForm.RunSaveAsync` (`catch (ConcurrencyException)`)
- Demo 6: `DemoService.ConcurrencyConflictAsync`. Ročno: odpri dve okni aplikacije in uredi isto
  stranko, ali pa jo vmes spremeni v Adminerju (`http://localhost:8080`).

**Vprašanja:**
- *Optimistično ali pesimistično zaklepanje?* Optimistično: nič ne zaklepaš, konflikt zaznaš ob
  shranjevanju – primerno, ko so konflikti redki in uporabnik dolgo ureja. Pesimistično
  (`SELECT ... FOR UPDATE`) drži zaklep ves čas – v namizni aplikaciji tvegano (uporabnik gre na kavo).
- *Zakaj `DATETIME(6)` in ne `DATETIME`?* Sekundna natančnost: dve shranjevanji v isti sekundi bi
  imeli enak čas in konflikt ne bi bil zaznan. Alternativa je celoštevilski stolpec `version`.
- *Kaj je "lost update"?* Dva uporabnika prebereta isto vrstico, oba shranita – drugi tiho prepiše
  spremembe prvega.

---

## 10. Indeksi in EXPLAIN

**Razlaga.** Tabela `activity_log` ima 500.000 vrstic. Iskanje po `ref_code` brez indeksa pomeni
*full table scan* (`type = ALL`, `rows ≈ 500.000`, ~90 ms); z indeksom `type = ref`, `rows = 1`,
< 1 ms. Demo indeks ustvari/odstrani sam in pokaže EXPLAIN v mreži.

**V kodi:**
- `Database/05_activity_log.sql`, `Database/03_orders.sql` (indeksi na tujih ključih)
- `DemoRepository.RunRefCodeQueryAsync`, `EnsureRefCodeIndexAsync`; demo 4a/4b

**Vprašanja:**
- *Kdaj indeks NE pomaga?* Funkcija nad stolpcem (`WHERE YEAR(datum) = 2026`), vodilni `%` v LIKE,
  implicitna pretvorba tipa, nizka selektivnost (npr. stolpec spol), pri sestavljenem indeksu
  manjka vodilni stolpec.
- *Slabosti indeksov?* Prostor in počasnejši INSERT/UPDATE/DELETE (vzdrževanje B-drevesa).
- *Kako najdeš počasne poizvedbe v produkciji?* Slow query log (vklopljen v `docker-compose.yml`,
  `long_query_time = 0.5`), `EXPLAIN` / `EXPLAIN ANALYZE`, Performance Schema.
- *Zakaj ima `orders` indeks `(customer_id, order_date)`?* Pokrije poizvedbo "naročila stranke po
  datumu" in služi tudi tujemu ključu (vodilni stolpec je `customer_id`).

---

## 11. async/await in deadlock zaradi .Result

**Razlaga.** `await` sprosti UI nit; po koncu se nadaljevanje izvede spet na UI niti (zajeti
`SynchronizationContext`). Če UI nit **blokiramo** z `.Result`/`.Wait()`, nadaljevanje ne more
priti na vrsto – Task se nikoli ne konča → **deadlock**, aplikacija zamrzne.

**V kodi:**
- Demo 2a (zamrzne!) / 2b: `Forms/DemoErrorsForm.cs` → `btnDeadlock_Click`, `btnAwait_Click`,
  `GetValueAsync`
- `ConfigureAwait(false)` v vseh repozitorijih (komentar nad razredom `CustomerRepository`)
- `async void` samo v event handlerjih, drugje `async Task` (npr. `MainForm.EditSelectedCustomerAsync`)

**Vprašanja:**
- *Zakaj nastane deadlock?* `.Result` blokira UI nit; `await` brez `ConfigureAwait(false)` želi
  nadaljevati na UI niti; obe strani čakata druga na drugo.
- *Kaj naredi `ConfigureAwait(false)` in kje ga (ne) uporabiš?* Nadaljevanje teče na niti iz poola.
  Uporabi v knjižnicah/repozitorijih; NE v obrazcih, kjer po `await` dostopaš do kontrol.
- *Zakaj je `async void` nevaren?* Klicatelj ga ne more počakati in ne more ujeti izjeme – ta gre
  neposredno na `SynchronizationContext` (v WinForms do `Application.ThreadException`).
- *Ali `Task.Run(...).Result` reši problem?* Prepreči deadlock, a UI nit je še vedno blokirana –
  obvoz, ne rešitev. Prava rešitev je "async all the way".

---

## 12. IProgress&lt;T&gt; in CancellationToken

**Razlaga.** Uvoz CSV teče asinhrono v paketih po 25 vrstic (vsak paket svoja transakcija).
Napredek servis sporoča prek `IProgress<ImportProgress>`; `Progress<T>`, ustvarjen v obrazcu, si
zapomni UI kontekst in poročila sam prenese na UI nit. Preklic je **sodelovalen**: gumb Prekliči
pokliče `CancellationTokenSource.Cancel()`, servis med paketi kliče `ThrowIfCancellationRequested()`.

**V kodi:**
- `Services/CsvImportService.cs` → `ImportAsync`
- `Forms/ImportForm.cs` → `btnImportAsync_Click`, `OnProgress`, `btnCancel_Click`
- `CustomerRepository.InsertBatchAsync` (token gre tudi v Dapper prek `CommandDefinition`)
- Zakasnitev za demo: `App.config` → `ImportDemoDelayMs`

**Vprašanja:**
- *Zakaj Progress&lt;T&gt; namesto neposrednega klica `progressBar.Value = ...` v servisu?* Servis ne
  pozna UI in teče na drugi niti; `Progress<T>` poskrbi za prenos na UI nit.
- *Kako se preklic izrazi?* `OperationCanceledException` (ali izpeljana `TaskCanceledException`);
  klicatelj jo ujame posebej in je ne prikaže kot napako.
- *Kaj ostane v bazi po preklicu?* Paketi, ki so bili že potrjeni (COMMIT). Če bi želel "vse ali nič",
  bi bil cel uvoz ena transakcija (dolga transakcija = dolgi zaklepi).
- *Zakaj `CancellationTokenSource` dispose-amo?* Implementira `IDisposable` (lahko ima časovnik in
  WaitHandle).

---

## 13. BackgroundWorker (legacy)

**Razlaga.** Isti uvoz še s `BackgroundWorker`: `DoWork` teče na delovni niti (brez dostopa do
kontrol), `ProgressChanged` in `RunWorkerCompleted` na UI niti. Preklic: `CancelAsync()` postavi
zastavico `CancellationPending`, ki jo `DoWork` preverja.

**V kodi:**
- `Forms/ImportForm.cs` → `btnImportBgw_Click`, `bgwImport_DoWork`, `bgwImport_ProgressChanged`,
  `bgwImport_RunWorkerCompleted` (komponenta `bgwImport` v Designerju)
- `CsvImportService.Import` (sinhrona različica)

**Vprašanja:**
- *Kje pride izjema iz DoWork?* V `RunWorkerCompleted` kot `e.Error`.
- *Na kaj paziti v RunWorkerCompleted?* Najprej `e.Error`, nato `e.Cancelled`, šele nato `e.Result` –
  branje `e.Result` ob napaki ali preklicu vrže izjemo.
- *BackgroundWorker ali async/await?* Za novo kodo async/await (berljivo, sestavljivo, deluje tudi
  zunaj WinForms). BackgroundWorker srečaš v starejši kodi – moraš ga razumeti.

---

## 14. Invoke / InvokeRequired

**Razlaga.** Kontrole smeš spreminjati le z niti, ki jih je ustvarila (UI nit). Z druge niti
uporabiš `control.Invoke(...)` (počaka) ali `BeginInvoke(...)` (ne čaka). `InvokeRequired` pove,
ali si na drugi niti.

**V kodi:**
- `ImportForm.AppendLog` (klican tudi iz `bgwImport_DoWork`)
- Demo 5a/5b: `DemoErrorsForm.btnCrossThread_Click` (InvalidOperationException),
  `btnInvoke_Click`, `SetThreadTargetText`

**Vprašanja:**
- *Zakaj napačna koda včasih "deluje"?* Preverjanje (`CheckForIllegalCrossThreadCalls`) je privzeto
  vklopljeno le z razhroščevalnikom. Brez njega so napake nenapovedljive.
- *Invoke ali BeginInvoke?* `Invoke` blokira delovno nit, dokler UI ne izvede klica – nevarno, če
  UI nit čaka na delovno nit (deadlock). `BeginInvoke` le pošlje sporočilo.
- *Alternative?* `IProgress<T>`, `SynchronizationContext.Post`, ali preprosto `await` (nadaljevanje
  je na UI niti).

---

## 15. BindingSource, BindingList&lt;T&gt; in master-detail

**Razlaga.** Mreže so vezane prek `BindingSource` na `BindingList<T>`. BindingList sporoča
spremembe (`ListChanged`), zato zamenjava elementa (`_customers[i] = edited`) takoj osveži vrstico.
Master-detail: `bsOrderItems.DataSource = bsOrders`, `DataMember = "Items"` – postavke sledijo
izbranemu naročilu brez kode. `OrderItem` implementira `INotifyPropertyChanged`, da se stolpec
*Znesek* in vsota osvežita med urejanjem.

**V kodi:**
- `Forms/MainForm.cs` → konstruktor (vezave), `FillBindingList`, `EditSelectedCustomerAsync`,
  `bsCustomers_CurrentChanged`
- `Forms/OrderEditForm.cs` → `Items_AddingNew`, `bsItems_ListChanged`, `dgvItems_DataError`
- `Models/OrderItem.cs` (INotifyPropertyChanged)

**Vprašanja:**
- *List&lt;T&gt; ali BindingList&lt;T&gt;?* `List<T>` ne sporoča sprememb – po dodajanju moraš mrežo
  ponovno vezati. `BindingList<T>` sproži `ListChanged`.
- *Čemu služi BindingSource?* Posrednik med podatki in kontrolami: trenutni element (`Current`,
  `Position`), filtriranje/urejanje (če vir to podpira), master-detail, `CurrentChanged` dogodek.
- *Zakaj `RaiseListChangedEvents = false` pri polnjenju?* Da se mreža ne riše ob vsakem `Add`; na
  koncu en `ResetBindings()`.
- *Zakaj `AutoGenerateColumns = false`?* Sicer mreža doda stolpce za vse javne lastnosti (Id,
  UpdatedAt ...).

---

## 16. Validacija z ErrorProvider

**Razlaga.** V obrazcu stranke ima polje dogodek `Validating`, ki nastavi `ErrorProvider`
(ikona z opisom) in `e.Cancel`. `AutoValidate = EnableAllowFocusChange` dovoli premik na drugo
polje; ob Shrani `ValidateChildren()` preveri vsa polja. Gumb Prekliči ima `CausesValidation = false`.
Pravila so v `CustomerValidator`/`OrderValidator` (Services), servis jih preveri še enkrat.

**V kodi:**
- `Forms/CustomerEditForm.cs` → `txtFirstName_Validating`, `txtEmail_Validating`, `btnOk_Click`,
  `CustomerEditForm_FormClosing`
- `Forms/OrderEditForm.cs` → `ShowValidationErrors` (ErrorProvider + `Row.ErrorText` v mreži)
- `Services/CustomerValidator.cs`, `Services/OrderValidator.cs`

**Vprašanja:**
- *Zakaj validacija v servisu, če jo že naredi obrazec?* Obrazec je za uporabnika (hiter odziv),
  servis je varovalka – do servisa lahko pride tudi uvoz, drug obrazec ali test.
- *Kaj je past pri `e.Cancel = true` v Validating?* S privzetim `AutoValidate` uporabnik ne more
  zapustiti polja in včasih niti zapreti okna – rešitev: `EnableAllowFocusChange`,
  `CausesValidation = false` na Prekliči, `e.Cancel = false` v `FormClosing`.
- *Validating ali Validated?* Validating se zgodi prej in ga lahko prekličeš; Validated po uspehu.

---

## 17. Izvoz v CSV s SaveFileDialog

**Razlaga.** *Datoteka → Izvoz strank v CSV* odpre `SaveFileDialog` (komponenta `sfdExport` v
Designerju), servis zapiše CSV s podpičjem (slovenski Excel) v UTF-8 **z BOM** (da Excel prav prikaže
šumnike). Vrednosti s podpičjem ali narekovajem so v narekovajih.

**V kodi:**
- `MainForm.mnuExport_Click`
- `CustomerService.ExportToCsvAsync`, `Services/CustomerCsv.cs` → `Write`, `Escape`
- Uvoz uporablja isti razred: `CustomerCsv.Parse`, `ParseLine`

**Vprašanja:**
- *Zakaj BOM?* Excel brez BOM bere CSV v sistemski kodni strani (cp1250) – č, š, ž se pokvarijo.
- *Kaj je "CSV injection"?* Vrednost, ki se začne z `=`, `+`, `-`, `@`, Excel izvede kot formulo.
  Zaščita: takim vrednostim dodaj vodilni `'`.
- *Zakaj razred CSV dela s TextReaderjem in nizi, ne z datotekami?* Da ga testiraš s `StringReader`
  brez diska.

---

## 18. Globalna obravnava napak in log

**Razlaga.** V `Program.Main` so pred prvim obrazcem nastavljeni:
`Application.SetUnhandledExceptionMode(CatchException)`, `Application.ThreadException` (UI nit –
aplikacija teče naprej), `AppDomain.CurrentDomain.UnhandledException` (druge niti – proces se
konča) in `TaskScheduler.UnobservedTaskException`. Vse gre v `logs\app-LLLLMMDD.log` poleg .exe.

**V kodi:**
- `Program.cs` → `Main`, `Application_ThreadException`, `CurrentDomain_UnhandledException`,
  `TaskScheduler_UnobservedTaskException`
- `Services/AppLogger.cs` (`lock`, ker pišejo različne niti)
- Demo 7a (UI nit – aplikacija preživi) in 7b (delovna nit – aplikacija se zapre)

**Vprašanja:**
- *Razlika med ThreadException in UnhandledException?* Prvi ujame izjeme na UI niti (sporočilna
  zanka) in aplikacija lahko nadaljuje. Drugi je zadnje obvestilo za izjeme na drugih nitih – proces
  se konča, lahko le zapišeš log.
- *Zakaj je napaka pred `Application.Run` posebej?* Še ni sporočilne zanke, zato je ThreadException
  ne ujame – v `Main` je zato `try/catch` okoli `CreateMainForm`.
- *Zakaj v log `exception.ToString()` in ne `Message`?* ToString vsebuje tip, stack trace in vse
  InnerException.
- *Je globalni handler nadomestek za try/catch?* Ne – je zadnja varovalka. Pričakovane napake
  (baza ni dosegljiva, konflikt) obravnavaš lokalno z razumljivim sporočilom.

---

## 19. Odzivnost UI: debounce in "race condition"

**Razlaga.** Iskanje se ne izvede ob vsaki črki, ampak 400 ms po zadnji (WinForms `Timer` –
*debounce*). Ker so klici asinhroni, se lahko odgovor starejše zahteve vrne za novejšim; vsaka
zahteva zato dobi zaporedno številko in prikaže se samo zadnja.

**V kodi:**
- `MainForm.txtSearch_TextChanged`, `tmrSearch_Tick`
- `MainForm.ReloadCustomersAsync`, `LoadOrdersAsync` (`_customersRequestId`, `_ordersRequestId`)

**Vprašanja:**
- *Katere tri vrste Timerjev pozna .NET?* `System.Windows.Forms.Timer` (Tick na UI niti),
  `System.Timers.Timer` in `System.Threading.Timer` (na niti iz poola – za UI potrebuješ Invoke).
- *Druga rešitev za zastarele odgovore?* `CancellationTokenSource` – ob novi zahtevi prekličeš
  prejšnjo.

---

## 20. Testi enot

**Razlaga.** Projekt `WindowsFormsApp1.Tests` (MSTest, .NET Framework 4.7.2) testira servise z
lažnima repozitorijema v pomnilniku – brez baze. 24 testov: validacija in poenotenje strank,
prevod konflikta v `ConcurrencyException`, naročila (simulirana napaka ne spremeni objekta
klicatelja, napaka repozitorija se prenese naprej), CSV (narekovaji, round-trip), uvoz (paketi,
napredek, preklic v obeh različicah).

**V kodi:**
- `WindowsFormsApp1.Tests/Fakes/FakeCustomerRepository.cs`, `FakeOrderRepository.cs`
- `CustomerServiceTests.cs`, `OrderServiceTests.cs`, `CustomerCsvTests.cs`, `CsvImportServiceTests.cs`

**Vprašanja:**
- *Unit test ali integracijski test?* Unit test preveri en razred izolirano (lažne odvisnosti),
  hitro in deterministično. Integracijski preveri sodelovanje s pravo bazo (npr. da rollback res
  deluje) – počasnejši, potrebuje okolje.
- *Fake, stub, mock?* Stub vrne pripravljene odgovore; fake je delujoča poenostavljena implementacija
  (seznam namesto baze); mock (npr. Moq) preverja tudi, *kako* je bil klican.
- *Kako testiraš async metodo?* Testna metoda je `async Task` (nikoli `async void`), izjemo preveriš
  z `Assert.ThrowsExceptionAsync<T>`.
- *Zakaj v testu uvoza lasten `IProgress` namesto `Progress<T>`?* Brez UI konteksta `Progress<T>`
  pošlje poročilo na nit iz poola – test bi lahko preveril rezultat, preden poročilo prispe.

---

## 21. Hitri pregled: kje je kaj

| Tema | Datoteka → metoda |
|---|---|
| Composition root | `Program.cs` → `CreateMainForm` |
| Globalne napake | `Program.cs` → `Application_ThreadException`, `CurrentDomain_UnhandledException` |
| Transakcija | `OrderRepository.CreateAsync` |
| Multi-mapping | `OrderRepository.GetByCustomerAsync` |
| QueryMultiple | `OrderRepository.GetByIdAsync`, `CustomerRepository.SearchPagedAsync` |
| LIMIT/OFFSET + LIKE | `CustomerRepository.SearchPagedAsync`, `BuildLikePattern` |
| Shranjena procedura | `CustomerRepository.GetStatsAsync`, `Database/04_procedures.sql` |
| Optimistično zaklepanje | `CustomerRepository.UpdateAsync`, `CustomerService.UpdateAsync` |
| Uvoz async + IProgress + preklic | `CsvImportService.ImportAsync`, `ImportForm.btnImportAsync_Click` |
| BackgroundWorker | `ImportForm.bgwImport_*`, `CsvImportService.Import` |
| Invoke/InvokeRequired | `ImportForm.AppendLog`, `DemoErrorsForm.SetThreadTargetText` |
| BindingSource/BindingList, master-detail | `MainForm` konstruktor, `FillBindingList` |
| ErrorProvider | `CustomerEditForm.*_Validating`, `OrderEditForm.ShowValidationErrors` |
| Izvoz CSV | `MainForm.mnuExport_Click`, `CustomerService.ExportToCsvAsync` |
| Demo napak | `Forms/DemoErrorsForm.cs`, `Services/DemoService.cs`, `Repositories/DemoRepository.cs` |
| Testi | `WindowsFormsApp1.Tests/` |
