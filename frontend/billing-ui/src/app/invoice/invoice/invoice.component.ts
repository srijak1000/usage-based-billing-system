import { Component } from '@angular/core';
import { InvoiceService } from '../../features/invoice/services/invoice.service';
import { Invoice } from '../../models/invoice.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class InvoiceComponent {

  userId = '';
  startDate = '';
  endDate = '';

  invoiceData?: Invoice;
  loading = false;
  error = '';

  constructor(private invoiceService: InvoiceService) {}

  fetchInvoice() {
    this.loading = true;
    this.error = '';

    this.invoiceService.getInvoice(this.userId, this.startDate, this.endDate)
      .subscribe({
        next: (res) => {
          this.invoiceData = res;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to fetch invoice';
          this.loading = false;
        }
      });
  }
}