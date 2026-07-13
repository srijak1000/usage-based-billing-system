export interface Invoice {
  userId: string;
  totalCost: number;
  breakdown: InvoiceItem[];
}

export interface InvoiceItem {
  date: string;
  unitsConsumed: number;
  costPerUnit: number;
  total: number;
}