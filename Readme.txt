Deploy:

	1. Publish AspWinService.
	2. Build AspWinServiceClient.
	3. Set serviceFiles and clientFiles constants in ServiceSetup.Program class.
	4. Run ServiceSetup project.
	6a. Run MyProduct.msi lacated in ServiceSetup project directory.

Test:

	7. Check ..ProgramFiles/My Company content.
	8. Check Windows Services for AspWinService.
	9. try http://localhost:5000/api/values in browser.
	10. Run ..ProgramFiles\My Company\My Product\Client\AspWinService.Exe

Upgrade:

	1. Make changes in AspWinService or AspWinServiceClient
	2. Increase Assembly version in ChangedProject -> Properties -> Package
	3. Increase version in ServcieSetup.Program class
	4. Deploy and test

Quiet install

	1. open cmd and execute > start msiexec /i MyProduct.msi /q