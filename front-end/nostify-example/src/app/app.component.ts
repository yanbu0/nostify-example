import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators'

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {

    constructor(public http: HttpClient) { }

    title = 'nostify-example';

    username: string = null;
    createMessage: string = null;

    createUserClick() {
        this.createMessage = null;
        if (this.username){
            this.http.post<any>("http://localhost:7071/api/CreateUser",{ userName: this.username, id: null, tenantId: 1 }).pipe(take(1)).subscribe(r => {
                this.createMessage = r.message;
            },
            error => this.createMessage = JSON.stringify(error));
        }
    }

}
