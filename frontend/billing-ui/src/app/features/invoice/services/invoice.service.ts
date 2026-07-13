import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../../../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {

  private baseUrl = 'https://localhost:5001/api';

  constructor(private http: HttpClient) {}

  getInvoice(userId: string, start: string, end: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.baseUrl}/invoice`, {
      params: { userId, start, end }
    });
  }
}