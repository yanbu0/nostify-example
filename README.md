# nostify-example

Example repo for the nostify framework.

Find more information here: https://github.com/yanbu0/nostify

To run this example you will need Azure Storage Emulator (part of the Azure SDK or available here: https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) and the Cosmos DB Emulator (https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator-release-notes).

The example is designed to run in VS Code.  Right click on the BankAccount folder and run in Code:
![image](https://user-images.githubusercontent.com/26099646/172683853-e1a7b99f-5112-408a-9c75-820a08b2b1ec.png)

Click the Run and Debug icon in the left hand navigation and then click the play button to start that microservice.
![image](https://user-images.githubusercontent.com/26099646/172684127-adcb5da0-c63d-49ff-b454-22861c804f34.png)

Do the same for AccountManager, then open the `front-end` folder and right click on the `nostify-example` folder and select Open with Code.  The example front end is done with Angular.  The first time you run the project you will need to run `npm install` in the terminal to download and install the supporting libraries for the front end.  Once that is complete run `ng serve` and open a browser to http://localhost:4200 to view the example front end.


