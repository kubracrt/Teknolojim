import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class ProcessedOrdersService {

    baseUrl: string = "http://localhost:5248/";
    
    constructor(private http: HttpClient) { }

    getProcessedOrders(): Observable<any[]> {
        return this.http.get<any[]>(this.baseUrl + "api/ProcessedOrders/GetProcessedOrders");
    }


}