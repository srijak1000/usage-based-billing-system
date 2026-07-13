import { Routes } from '@angular/router';
import { InvoicePageComponent } from './features/invoice/pages/invoice-page.component';

export const routes: Routes = [
  { path: '', component: InvoicePageComponent },
  { path: '**', redirectTo: '' }
];
