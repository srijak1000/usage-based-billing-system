import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BillingService } from '../services/billing.service';
import { Invoice } from '../../../models/invoice';

@Component({
  selector: 'app-invoice-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice-page.component.html',
  styleUrl: './invoice-page.component.scss'
})
export class InvoicePageComponent {
  private readonly billingService = inject(BillingService);

  userId = 'user-1';
  invoice: Invoice | null = null;
  loading = false;
  message = '';

  loadInvoice(): void {
    this.loading = true;
    this.message = '';

    this.billingService.getInvoice(this.userId).subscribe({
      next: (invoice) => {
        this.invoice = invoice;
        this.loading = false;
      },
      error: () => {
        this.message = 'Unable to fetch the invoice right now.';
        this.loading = false;
      }
    });
  }

  seedDemoData(): void {
    this.loading = true;
    this.message = '';

    this.billingService.seedDemoData().subscribe({
      next: () => {
        this.message = 'Demo data loaded successfully.';
        this.loadInvoice();
      },
      error: () => {
        this.message = 'Unable to seed demo data.';
        this.loading = false;
      }
    });
  }
}
