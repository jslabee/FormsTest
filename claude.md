# CLAUDE.md — WinForms aplikacija na .NET Framework 4.7

## Projekt
Testna namizna aplikacija v **C# / Windows Forms** na **.NET Framework 4.7** (klasični .NET Framework,
NE .NET Core / .NET 5+). Namen: vaja za tehnični intervju, zato naj bo koda čista, razumljiva in
napisana tako, kot se piše v obstoječih (legacy) podjetniških projektih.

Razvoj: **Visual Studio 2022/2026 Community** z WinForms Designerjem. Claude Code teče v terminalu
v mapi projekta.

## Omejitve .NET Framework 4.7 (pomembno!)
- Jezik je **C# 7.3** (privzeto za 4.7). NE uporabljaj:
  - `ImplicitUsings`, file-scoped namespace (`namespace X;`), global usings
  - nullable reference types (`string?`), records, `init`, pattern `is not`, switch expressions
  - target-typed `new()`, `using var` deklaracij
  - `ApplicationConfiguration.Initialize()` (to je samo v .NET 6+)
- Vstopna točka (`Program.cs`) je klasična:
  ```csharp
  [STAThread]
  static void Main()
  {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
  }
  ```
- Nastavitve v `App.config` (`ConfigurationManager`), ne `appsettings.json`.
- JSON: `Newtonsoft.Json` (NuGet), ker `System.Text.Json` ni del Frameworka.
- `async`/`await` in `Task` sta na voljo — uporabljaj ju za dolgotrajna opravila, da UI ne zamrzne.

## Designer
- Obrazci se urejajo v Visual Studio Designerju. Vsak Form ima dve datoteki:
  `MainForm.cs` (tvoja koda) in `MainForm.Designer.cs` (generira Designer).
- **`*.Designer.cs` datotek ne urejaj ročno**, razen če te izrecno prosim. Če dodaš kontrolo
  v kodi, jo dodaj v `MainForm.cs` (npr. v konstruktorju po `InitializeComponent()`)
  ali mi povej, naj jo dodam v Designerju.
- Ne spreminjaj `.resx` datotek.

## Struktura
```
MyApp/
  MyApp.sln
  MyApp/
    MyApp.csproj        // star (non-SDK) format, TargetFrameworkVersion v4.7
    App.config
    Program.cs
    Forms/              // MainForm.cs + MainForm.Designer.cs
    Services/           // poslovna logika, brez referenc na WinForms
    Models/             // podatkovni razredi
```

## Baza: MySQL + Dapper
- Lokalna testna baza MySQL 8.4, baza `testapp`, uporabnik `testapp_user`.
- Paketi (NuGet): `Dapper` in `MySqlConnector` (pravi async). Ne mešaj z `MySql.Data`.
- Connection string je v `App.config` pod `<connectionStrings name="TestDb">`, bere se z
  `ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString`. Nikoli ga ne piši v kodo.
  Primer: `Server=localhost;Port=3306;Database=testapp;Uid=testapp_user;Pwd=...;CharSet=utf8mb4;`
- Dostop do baze samo v `Repositories/` (npr. `CustomerRepository`), UI ne vidi SQL-a.
- Vsaka metoda odpre svojo povezavo z `using (var conn = new MySqlConnection(...))` — povezave
  se ne hranijo v poljih (pooling to uredi sam).
- **Vedno parametri** (`@Name`), nikoli lepljenje stringov v SQL.
- Več povezanih zapisov → `MySqlTransaction` in `transaction:` parameter v Dapper klicih.
- V WinForms nikoli `.Result` ali `.Wait()` na Task (deadlock na UI niti) — uporabljaj
  `async void` samo v event handlerjih, drugje `async Task`.
- Tabele: `utf8mb4` / `utf8mb4_0900_ai_ci` (šumniki), `DATETIME` za datume.
- Shema in testni podatki v `Database/schema.sql` in `Database/seed.sql`.

## Gradnja
- V Visual Studiu: **F5** (zagon z razhroščevanjem), **Ctrl+Shift+B** (build).
- Iz terminala (Developer PowerShell for VS): `msbuild MyApp.sln /p:Configuration=Debug`
- Ko spremeniš kodo, preveri, da se projekt zgradi brez napak in opozoril.

## Slog kode
- Imena: PascalCase za razrede, metode, lastnosti; `_camelCase` za zasebna polja.
- Kontrole poimenuj s predpono tipa: `btnSave`, `txtName`, `lstItems`, `dgvCustomers`.
- Event handlerji: `btnSave_Click` (Designer konvencija).
- Logike ne piši v event handlerje — pokliči metodo iz `Services/`.
- Napake ujemi in prikaži z `MessageBox.Show(...)`.
- **Vsa koda mora biti komentirana v slovenščini** (namen je učenje za intervju):
  - nad vsakim razredom in metodo `/// <summary>` z razlago, kaj dela in zakaj,
  - v metodah kratki komentarji pred vsakim pomembnim korakom,
  - pri vzorcih, ki so tipična intervju tema (using, async/await, transakcije, Invoke,
    parametri v SQL ...), komentar `// INTERVJU:` z razlago, zakaj se dela tako
    in kaj gre narobe, če se ne.
- Ne komentiraj samoumevnih vrstic (`i++`), komentarji naj razlagajo "zakaj", ne le "kaj".
- `*.Designer.cs` datotek ne komentiraj (Designer jih prepiše).

## Ne
- Ne nadgrajuj projekta na novejši .NET in ne spreminjaj `TargetFrameworkVersion` brez vprašanja.
- Ne dodajaj NuGet paketov brez razloga.