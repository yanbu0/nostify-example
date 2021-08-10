import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { delay, take, takeUntil } from 'rxjs/operators'
import { BehaviorSubject, Subject } from 'rxjs';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

class Transaction {
    amount: number
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
    aggregateId: string = null;

    public transactions: BehaviorSubject<Array<Transaction>> = new BehaviorSubject([]);
    private _hub: HubConnection;

    generateTransactions(){
        for(let i = 0; i < 10; i++){
            let amt = Math.floor(Math.random() * 2001) - 1000;
            this.http.post(`http://localhost:7071/api/ProcessTransaction?id=${this.aggregateId}&amount=${amt}`, null).pipe(delay(100), take(1)).subscribe();
        }
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
            .withUrl(info.url+`&bankAccountId=${this.accountId}`, options)
            .configureLogging(LogLevel.Information)
            .build();
    
          this._hub.on("transactionAdded", (transaction:Transaction) =>  {
              let tArr = [].concat(this.transactions.value);
              tArr.unshift(transaction);
              this.transactions.next(tArr);
          });

          this._hub.on('initialConnect',(currentTransactions: Array<Transaction>) => this.transactions.next(currentTransactions));
    
    
          console.log('connecting...');
          this._hub.start()
            .then(() => {
                console.log('connected!');
            })
            .catch(console.error);
    
    
        });
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
