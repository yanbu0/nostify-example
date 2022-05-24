import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { delay, take, takeUntil } from 'rxjs/operators'
import { BehaviorSubject, Subject } from 'rxjs';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

class BankAccountDetails {
    id: string;
    accountManagerId: string;
    accountManagerName: string;
    currentBalance: number;
    tenantId: number;
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

    constructor(public http: HttpClient) { }

    destroy$: Subject<boolean> = new Subject();

    title = 'nostify-example';

    customerName: string = null;
    accountId: number = 0;
    createMessage: string = null; 

    accountManagerName: string = null;
    createManagerMessage: string = null;

    assManagerId: string = null;
    assAccountId: string = null;
    assignManagerMessage: string = null;

    managerId: string = null;
    newManagerName: string = null;
    updateManagerMessage:string = null;

    bankAccountAggregateId: string = null;

    baDetailsId: string = null;

    public baDetails: BehaviorSubject<BankAccountDetails> = new BehaviorSubject(null);
    private _hub: HubConnection;

    rebuildMessage: string = null;

    rebuildDetailsContainerClick(){
        this.http.post(`http://localhost:7071/api/RebuildContainer?containerName=BankAccountDetails`,null)
            .pipe(take(1))
            .subscribe((ret:any) => this.rebuildMessage = ret.message,
                error => this.rebuildMessage = JSON.stringify(error));
    }

    generateTransactions(){
        for(let i = 0; i < 10; i++){
            let amt = Math.floor(Math.random() * 2001) - 1000;
            this.http.post(`http://localhost:7071/api/ProcessTransaction?id=${this.bankAccountAggregateId}&amount=${amt}`, null).pipe(delay(100), take(1)).subscribe();
        }
    }

    showRealTimeClick() {
        this.http.post(`http://localhost:7071/api/AccountSelected?bankAccountId=${this.baDetailsId}`,null)
            .pipe(take(1))
            .subscribe();
    }

    initHub(){
        this.http.post(`http://localhost:7071/api/negotiate`, null, {}).pipe(takeUntil(this.destroy$)).subscribe((info: any) => {
            // make compatible with old and new SignalRConnectionInfo
            info.accessToken = info.accessToken || info.accessKey;
            info.url = info.url || info.endpoint;

            console.log(info);

            const options = {
            accessTokenFactory: () => info.accessToken
            };


            this._hub = new HubConnectionBuilder()
            .withUrl(info.url+`&bankAccountId=${this.baDetailsId}`, options)
            .configureLogging(LogLevel.Information)
            .build();

            this._hub.on('bankAccountDetailUpdated', (deets:BankAccountDetails) =>  {
                this.baDetails.next(deets);
            });

            console.log('connecting...');
            this._hub.start()
            .catch(console.error);
    
    
        });
      }

    refreshMgrButtonText: string = "Refresh Manager List";   
    refreshMgrButtonDisabled: boolean = false; 
    accountManagersArr: Array<any> = [];  
    getAccountManagersClick(){
        this.refreshMgrButtonDisabled = true; 
        this.refreshMgrButtonText = "Loading Manager List";
        this.http.get("http://localhost:7072/api/GetAllAccountManagers").pipe(take(1)).subscribe(
            (mgrList: Array<any>) => {
                this.accountManagersArr = mgrList;
                this.refreshMgrButtonDisabled = false;
                this.refreshMgrButtonText = "Refresh Manager List";
            },
            error => this.assignManagerMessage = JSON.stringify(error));
    }  

    refreshAcctButtonText: string = "Refresh Account List";   
    refreshAcctButtonDisabled: boolean = false; 
    accountsArr: Array<any> = [];  
    getAccountsClick(){
        this.refreshAcctButtonDisabled = true; 
        this.refreshAcctButtonText = "Loading Account List";
        this.http.get("http://localhost:7071/api/GetAllAccounts").pipe(take(1)).subscribe(
            (acctList: Array<any>) => {
                this.accountsArr = acctList;
                this.refreshAcctButtonDisabled = false;
                this.refreshAcctButtonText = "Refresh Account List";
            },
            error => this.assignManagerMessage = JSON.stringify(error));
    }  

    assignManagerClick() {
        this.createMessage = null;
        if (this.assAccountId && this.assManagerId){
            this.http.post<any>("http://localhost:7071/api/UpdateBankAccount",{ id: this.assAccountId, accountManagerId: this.assManagerId }).pipe(take(1)).subscribe(r => {
                this.assignManagerMessage = r.message;
            },
            error => this.assignManagerMessage = JSON.stringify(error));
        }
    }

    updateManagerClick(){
        this.updateManagerMessage = null;
        if (this.managerId && this.newManagerName){
            this.http.post<any>("http://localhost:7072/api/UpdateManagerName",{ id: this.managerId, name: this.newManagerName  }).pipe(take(1)).subscribe(r => {
                this.updateManagerMessage = r.message;
            },
            error => this.updateManagerMessage = JSON.stringify(error));
        }
    }

    createManagerClick(){
        this.createManagerMessage = null;
        if (this.accountManagerName){
            this.http.post<any>("http://localhost:7072/api/CreateAccountManager",{ name: this.accountManagerName  }).pipe(take(1)).subscribe(r => {
                this.createManagerMessage = r.message;
            },
            error => this.createManagerMessage = JSON.stringify(error));
        }
    }

    createAccountClick() {
        this.createMessage = null;
        if (this.customerName){
            this.http.post<any>("http://localhost:7071/api/CreateAccount",{ customerName: this.customerName, accountId: this.accountId  }).pipe(take(1)).subscribe(r => {
                this.createMessage = r.message;
            },
            error => this.createMessage = JSON.stringify(error));
        }
    }

    ngOnInit(){
        console.log('init hub');
        this.initHub();
    }

    ngOnDestroy(){
        this.destroy$.next(true);
        this.destroy$.unsubscribe();
    }

}
