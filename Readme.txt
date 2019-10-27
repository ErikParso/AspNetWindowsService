Projekt HgProductionServer
==========================
Asp.Net core služba. Simulácia servera ktorý poskytuje inštalačku poslednej verzie Client Managera na stiahnutie.
Admin v rámci inštalácie alebo Client Manager služba v rámci samoaktualizácie odtiaľto stiahne inštalačku.
Nasadenie:

	1. build + publish do priečinka bin\Release\netcoreapp2.2\win7-x64\publish
	2. v priečinku bin\Release\netcoreapp2.2\win7-x64\publish ako admin spustiť script registerService.ps1
	   
	   !!! spustiť script v priečinku publish. Služba musí bežať v priečinku, kde bude nakopýrovaný .msi balíček po vybuildovaní projektom ServiceSetup !!!

	3. Vo windows services by mala bežať služba HgProductionServer (port 5100)

Projekt ClientManagerService
============================
Asp.Net core služba pre správu inštalácii Helios Green clientov. 
Stačí vybuildovať do debug priečinka.
Script na nasadenie neexistuje, službu bude nasadená v rámci inštalácie pomocou .msi balíčka.

Projekt ClientManagerApp
========================
Angular+Electron client aplikácia.
Build:
	1. Otvoriť priečinok vo Visual Studio Code
	2. npm install
	3. príkaz ng serve -o (spustí client aplikáciu v prehliadači, funguje live reload a debug)
	4. ríkaz npm run lelectron (spustí projekt ako desktop aplikáciu)
	4. npm run electron-package (vypublikuje desktop aplikáciu)
	5. vypublikovaná aplikácia sa nachádza v priečinku ./ClientManagerApp-win32-x64

CredentialsListener
===================
V rámci komunikácie s aplikačným serverom je niekedy potrebné nastaviť prihlasovacie údaje.
Jedná sa o clientov s nastavením "IntegratedWindowsAuthentication".

Priebeh:
	1. Služba spustí inštanciu CredentialsListenera
	2. Služba použije RpcHub pre kontaktovanie CredentialsListenera.
	   Použije metódu RequestAllClients, čím dá aplikácii CredentialsListener pár sekúnd na pripojenie sa na hub.
	3. V momente keď sa CredentialsListener pripojí na hub, služba ho notifikuje požiadavkou o prihlásenie.
	4. CredentialsListener zareaguje na požiadavku a zobrazí login okno.
	5. Užívateľ sa prihlási.
	6. CredentialsListener odpovie službe prihlasovacími údajmi.
	7. Služba obdrží odpoveď a pokračuje v kontaktovaní servera s prihlasovacími údajmi.

	viac info v sekcii RpcHub.cs =>

Projekt ServiceSetup
====================
Zabalí ClientManagerService službu a vypublikovanú ClientManagerApp do .msi balíčka (urobí inštalačku).
Inštalačku nakopíruje do adresára služby HgProductionServer.
Konfigurácia:
	0. Nainštalovať wix toolset (https://wixtoolset.org/).
	1. V program.cs nastaviť konštanty
	   serviceFiles: cesta k buildu Client Manager služby (projekt ClientManagerService)
	   electronClientFiles: cesta k vypublikovanej client aplikácii (ClientManagerApp)
	   credentialsListenerFiles: cesta k outputu CredentialsListener projektu
	   msiDeployPath: publish priečinok služby HgProductionServer (tu do priečinka Installer sa nakopíruje .msi balíček)
	2. Spustiť projekt
	3. Výsledný .msi balíček bude nakopírovaný do priečinku Installer služby HgProductionServer

Stiahnutie a inštalácia Client Managera
=======================================
0a. beží služba HgProductionServer
0b. v priečinku služby HgProductionServer je adresár Installer s nakopírovaným .msi balíčkom
3. v prehliadači na adrese localhost:5100/api/installer/download stiahntuť .msi balíček
4. nainštalovať Helios Green Client Manager
5. Vo Windows Services beží ClientManagerService služba (port 5000). V tray bare beží ClientManagerApp.exe.

Nová verzia Client Managera (samoaktualizácia)
==============================================
1. Urobíme zmeny v ClientManagerService, CredentialsListener alebo ClientManagerApp.
2. Ak sme urobili zmeny v ClientManagerService alebo CredentialsListener, inkrementujeme hodnoty Version, AssemblyVersion, FileVersion v .csproj.
   * Inštalačný .msi balíček inak neprepíše tieto dllky.
4. Ak sme robili zmeny v ClientManagerApp, aplikáciu vypublikujeme príkazom npm run electron-package
3. V ClientManagerService inkrementujeme verziu v súbore VersionInfo.json
5. Spustíme ServiceSetup projekt. Ten vytvorí .msi balíček a nakopíruje ho do priečinku služby HgProductionServer.
6. Spustíme novú inštanciu ClientManagerApp (exe nájdeme v ProgramFiles/Asseco Solutions/Client manager/Client Manager App)
7. V hlavičke bude možnosť (šípka) aktualizovať program Helios Green Client Manager.

RpcHub.cs
=========
Rozšírenie SignalR hub. Umožňuje službe kontaktovať klienta a čakať na jeho odpoveď.
Princíp: 
	0. v Startup.cs zaregistrujeme a namapujeme RpcHub<Request, Response>
	1. Klient sa pripojí na rpcHub, počúva na metódu "request(methodId, Request)"
	2. služba kontaktuje klienta metódou "request(methodId, Request)" a čaká na odpoveď takto:
	
		 var result = await rpcHub.request(methodId, Request)

	3. Klient reaguje na metódu "request(methodId, Request)" spracuje požiadavku a odpovie metódou "methodResponse(methodId, Response)"
	4. Služba obdrží odpoveď a pokračuje

Update:

	* Inšpirácia z článku https://www.tpeczek.com/2019/03/server-to-client-rpc-calls-with-results.html
	  Implementácia je vlastná, rozšírená o generiká <Request, Response>, cieľ a princíp je však rovnaký
	* V prípade, že sa klient odpojí a služba čaká na jeho odpoveď, Task sa uvoľní a služba dostane ako odpoveď NULL.
	* Možno kontaktovať klienta, od ktorého pochádza request. Služba čaká na jeho odpoveď (metóda RequestClient)
	* Možno kontaktovať všetkých klientov, služba čaká na odpoveď prvého (metóda RequstAllClients)

	parametre metódy RpcHub<Q, S>.RequestClient:
		- request: Q - request objekt na klienta

	parametre metódy RpcHub<Q, S>.RequestAllClients:
		- request: Q - request objekt na klientov
		- (optional) clientConnectTimeout: int - v prípade, že žiaden klient nie je pripojený, parameter určuje, ako dlho bude služba čakať na pripojenie sa klienta.		

	RpcHub je použitý na:
		- získanie prihlasovacích údajov pomocou CredentialsListenera (použitá metóda RequestAllClients)
		- schválenie certifikátu užívateľom na strane ClientManagerApp (použitá metóda RequestClient)

Odporúčania pre prácu s FE
==========================
* Ctrl+Shift+i - otvorí konzolu v electron aplikácii.
* V electron aplikácii nefunguje Live Reload odporúčam pracovať v Browseri kým je to možné.
* Naštudovať si základy RxJs a dodržiavať pattern, v prehliadači nainštalovať plugin Redux DevTools.
* Swagger dokumentácia služby na http://localhost:5000/swagger

Načítanie zoznamu nainštalovaných HG clientov a zistenie dostupnosti aktualizácie
=================================================================================
Pri spustení ClientManagerApp vidno spinner indikujúci, že prebieha načítanie klientov.
V tomto momente služba spracováva zoznam nainštalovaných klientov. Súčasťou tohto spracovania je aj kontrola dostupnosti aktualizácie daného HG klienta.
Výnimkou sú klienti, ktorí majú špecifikované nastavenie "UseIntegratedWindowsCrenetials". 
V takom prípade musí užívateľ zadať prihlasovacie údaje a z toho dôvodu sa kontrola dostupnosti aktualizácie nevykonáva.
Takýto klient bude mať v aplikácii príznak "upgrade may be available" a užívateľ musí vyvolať kontrolu dostupnosti aktualizácie ručne, klikom na tlačidlo v dolnom panely.

Validácia aplikačného servera
=============================
V kroku 1 inštaláce je pole pre adresu Aplikačného Servera. Užívateľ tu musí zadať validnú adresu Version Manager servera.
Validácia prebieha na strane Client Manager služby. 
Služba sa v rámci validácie pokúsi na tento server pripojiť, presmerovať sa na Aplikačný Server a odtiaľ zistiť dostupné jazyky.
Do validácie vstupujú aj konfiguračné parametre týkajúce sa pripojenia. Sú to nastavenia UseDefaultProxy, Proxy, UseIntegratedWindowsCredentials.
Tieto parametre možno nastaviť iba v prvom kroku inštalácie a nedajú sa odstrániť/editovať v kroku 2 inštalácie.
Zmena týchto nastavení vyvolá validáciu aplikačného servera.

V prípade nastavenia UseIntegratedWindowsCredentials, ak dôjde k zmene adresy aplikačného servera,
zobrazi sa špeciálna hláška informujúca o nutnosti zadať prihlasovacie údaje. 
Kliknutím na odkaz "Log In" užívateľ znovuvaliduje adresu, ale tentokrát si služba vyžiada od užívateľa prihlasovacie údaje (pozri CredentialsListener).
(Toto riešenie je hlavne kôli tomu, aby sa užívateľ nemusel prihlasovať za každým písmenom čo zadá do textboxu...)

V rámci úspešnej validácie služba vráti aj zoznam dostupných jazykov. Tie sa potom nastavia ako možnosti comboboxu.
(Toto by bolo dobre separovať do samotného requestu na službu).

Nové nastavenia do configu
==========================
section: Cient, name: ClientName, value: 	
	Win32 - stiahne sa verzia klienta "NorisWin32.exe"
	ClientWinForms - stiahne sa verzia klienta "ASOL.Green.Client.WinForms.exe"
	- jedná sa o parameter do funkcie "GetClientManifestInfoAsync"
	- na výsledné .exe sú správne navazané linky, odkazy a asociácia súborov
	- voliteľné, ako default je použitá hodnota Win32

section: Client, name: ClientAuthor, value: string
	- jedná sa o parameter do funkcie "GetClientManifestInfoAsync"
	- voliteľné, ako default sa použije string.empty

Schvaľovanie problematických certifikátov
=========================================
V prípade že komunikácia s Client Managerom alebo Aplikačným Serverom prebieha cez https, je nastavený custom validátor pre validáciu server certifikátu.
V rámci validácie, ak je certifikát chybný, Služba pomocou SinalR kontaktuje Aplikáciu, ktorá zobrazí dialógové okno s problémom. Služba čaká na odpoveď (použitý RpcHub).
Ak užívateľ certifikát schválil, zaradí sa do ignore listu a problémy s týmto certifikátom budú v rámci jedného requestu na službu ignorované.
Ak užívateľ certifikát schválil s možnosťou zapamätať voľbu, problémy s týmto certifikátom budú ignorované v rámci lifetimu ClientManagerService.
Ak užívateľ certifikát zamietol, zaradí sa do ignore listu s tým, že jeho ďalšia validácia v rámci requestu bude negatívna. 
(V tomto prípade by bolo vhodné tento ignore list ukladať niekde na disk). 

Bezobslužná aktualizácia
========================
ClientManagerService v pravidelných intervaloch kontroluje dostupnosť aktualizácii klientov a vykonáva ich automatickú aktualizáciu.
Kontrola a samotná aktualizácia klientov prebieha paralelne. 

V rámci bezobslužnej aktualizácie služba pomocou SignalR notifikuje o jej priebehu. 
ClientManagerApp takto môže v reálnom čase reagovať na akcie bezobslužnej aktualizácie a to sú:
	- clientUpgradeCheck(clientId) - bola spustená kontrola dostupnosti aktualizácie klienta.
	- clientUpgradeCheckResult(clientId, upgradeInfo, message) - bola dokončená kontrola dostupnosti aktualizácie klienta.
	- clientAutoUpgrade(clientId, processId*) - bola spustená aktualizácia klienta.
	- cleintAutoUpgradeResult(result, clientInfo) - notifikácia o dokončení aktualizácie s výsledkom a clientInfo objektom.

Bezobslužná aktualizácia neaktualizuje klientov kde:
	- treba prehlasovacie údaje: výsledok kontroly aktualizácie je UpgradeInfo.NotChecked, aktualizácie neprebehne
	- treba schválenie certifikátu: výsledok kontroly aktualizácie je UpgradeInfo.UpgradeCheckFailed, aktualizácia neprebehne
	- aktualizácia zahrňa plugin akcie: výsledok kontroly aktualizácie je UpgradeInfo.UpgradeAvailable, aktualizácia neprebehne

Synchronizácia pomocou ClientLockService: 
	- Každý klient má pridelený valstný semafor a v jednom momente môže s klientom pracovať len jeden proces.
	- Novší proces čaká a bude pokračovať až vtedy, keď iný proces dokončí prácu a klienta uvoľní.
	- ClientLockService musí byť registrovaná ako singleton. Použitie:
		
		using (var clientLock = clientLockService.GetClientLockContext(clientId))
        {
			await clientLock.Lock();
			práca sa klientom...
		}
	
* Premenná processId slúži pre spárovanie bežiaceho procesu aktualizácie s objektom procesu na klientovi kvôli notifikáciam o progrese aktualizácie.
	1. Služba notifikuje štart aktualizácie a pošle processId.
	2. CleintManagerApp, ak beží, obdrží správu o štarte aktualizácie. Vytvorí objekt reprezentujúci process a nastaví mu processId.
	3. Služba notifikuje o progresse aktualizácie, v správe zahrnie processId.
	4. ClientManagerApp obdrží správu o progresse, pomocou processId nájde správny proces ktorému aktualizuje progress.

TimeSpan
	- v súbore ClientManagerSettings.json možno nastaviť v akých intervaloch sa bude aktualizovať.

Aktualizácia ClientManager riešenia
===================================
1. ClientManagerApp sa pri spustení dotáže služby o version info
2. ClientManagerApp obdrží lokálnu verziu client managera a verziu dostupnú na produkčnom serveri.
3. Ak sa verzie líšia, znamená to, že je k dispozícii aktualizácia. ClientManagerApp zobrazí v hlavičke tlačidlo upgrade.
4. Užívateľ klikne na tlačidlo, ClientManagerApp kontaktuje ClientManagerService aby sziahla novú verziu.
5. Služba stiahne msi balíček a uloží ho na disku. ClientManagerApp obdrží cestu k balíčku.
6. ClientManagerApp spustí inštalačný balíček a po sekunde sa vypne.
7. Užívateľ pokračuje krokmi inštalácie čím aktualizuje ClientManager riešenie.

HttpResponseException
=====================
Pre uľahčenie spracovania chýb na klientovi.
Exception bude odchytená filtrom a prekonvertovaná na odpoveď s definovanou správou a result kódom.
príklad: pozri TestController.GetException()


