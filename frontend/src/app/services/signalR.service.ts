import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalService {
  private orderHubConnection!: signalR.HubConnection;
  private productViewHubConnection!: signalR.HubConnection;

  public orderData: BehaviorSubject<any[]>;
  public viewEvent: BehaviorSubject<any[]>;

  constructor() {
    const savedOrders = localStorage.getItem('orders');
    this.orderData = new BehaviorSubject<any[]>(savedOrders ? JSON.parse(savedOrders) : []);

    const savedViewEvent = localStorage.getItem('viewEvent');
    this.viewEvent = new BehaviorSubject<any[]>(savedViewEvent ? JSON.parse(savedViewEvent) : []);

    this.startOrderHubConnection();
    this.startProductViewHubConnection();
  }

  private startOrderHubConnection(): void {
    this.orderHubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5081/orderhub', { transport: signalR.HttpTransportType.WebSockets, withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.orderHubConnection
      .start()
      .then(() => console.log('SignalR orderhub bağlantısı başladı'))
      .catch(err => console.error('SignalR orderhub bağlantı hatası:', err));

    this.orderHubConnection.on('ReceiveOrder', (message: any) => {
      console.log('SignalR ile order mesajı alındı:', message);

      const currentOrders = this.orderData.value;
      const updatedOrders = [message, ...currentOrders];
      localStorage.setItem('orders', JSON.stringify(updatedOrders));
      this.orderData.next(updatedOrders);
    });
  }


  private startProductViewHubConnection(): void {
    this.productViewHubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5081/productviewhub', { transport: signalR.HttpTransportType.WebSockets, withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.productViewHubConnection
      .start()
      .then(() => console.log('SignalR productviewhub bağlantısı başladı'))
      .catch(err => console.error('SignalR productviewhub bağlantı hatası:', err));

    this.productViewHubConnection.on('ReceiveProductView', (viewData: any) => {
      console.log('SignalR ile view verisi alındı:', viewData);
      const updatedView = [viewData, ...this.viewEvent.value];
      console.log('Güncellenmiş view verisi:', updatedView);
      localStorage.setItem('viewEvent', JSON.stringify(updatedView));
      this.viewEvent.next(updatedView);
    });

  }
}


