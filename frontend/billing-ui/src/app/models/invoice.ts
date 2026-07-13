export interface InvoiceLineItem {
  resourceId: string;
  serviceType: string;
  unit: string;
  quantity: number;
  amount: number;
}

export interface ServiceSubtotal {
  serviceType: string;
  amount: number;
}

export interface Invoice {
  userId: string;
  start: string;
  end: string;
  lineItems: InvoiceLineItem[];
  serviceSubtotals: ServiceSubtotal[];
  totalAmount: number;
}
