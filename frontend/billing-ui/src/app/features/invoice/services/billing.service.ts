import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../../../models/invoice';

@Injectable({ providedIn: 'root' })
export class BillingService {
  private readonly apiUrl = 'http://localhost:5121/api';

  constructor(private readonly http: HttpClient) {}

  seedDemoData(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/seed/demo`, {});
  }

  getInvoice(userId: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.apiUrl}/billing/invoices/${userId}`);
  }
}
