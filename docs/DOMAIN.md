# Domain

## Context

A private investor builds a single-family house, usually financed partly by a construction mortgage paid out in tranches. They need to know at any moment: how much was planned, how much was spent, what is left, which stage is late or over budget, and what the bank needs before the next tranche.

MVP scope: one user owns their construction projects. Sharing a project (e.g. with a spouse) comes later.

## Ubiquitous language (PL → EN)

| Polish | English (code) | Meaning |
|--------|----------------|---------|
| Budowa | `ConstructionProject` | The whole house build; root of the planning. |
| Etap | `Stage` | A phase of the build (foundations, shell, roof, installations, finishing). |
| Pozycja budżetu | `BudgetItem` | A planned cost within a stage (e.g. "Concrete", "Roof tiles"). |
| Kosztorys / budżet | `Budget` | Sum of planned amounts. |
| Wydatek | `Expense` | Actual money spent (invoice, receipt, transfer). |
| Faktura / paragon | `Document` | Proof of an expense (attachment). |
| Wykonawca | `Contractor` | A company or person doing work or supplying materials. |
| Branża | `Trade` | Contractor's specialisation (plumbing, electrical, roofing…). |
| NIP | `Nip` | Polish tax identification number (10 digits with checksum). |
| Kredyt budowlany | `ConstructionLoan` | Mortgage paid out in tranches. |
| Transza | `LoanTranche` | A single loan payout, usually tied to completing a stage. |
| Inspekcja banku | `BankInspection` | Bank's verification of progress before a tranche. |
| Środki własne | `OwnFunds` | Money not coming from the loan. |
| Kwota brutto / netto | `GrossAmount` / `NetAmount` | Amount with / without VAT. |
| Stawka VAT | `VatRate` | 0%, 5%, 8% or 23%. |

## Aggregates

### ConstructionProject (aggregate root)
Contains `Stage` entities, each containing `BudgetItem` entities.

- `Name`, `Address`, `Status` (`Planning`, `InProgress`, `OnHold`, `Completed`), `PlannedPeriod` (`DateRange`), `OwnerId`.
- `TotalBudget` is computed as the sum of stage budgets (not stored separately).
- Stages have `Name`, `Order`, `PlannedPeriod`, `Status` (`NotStarted`, `InProgress`, `Completed`), `CompletedOn`.
- Budget items have `Name`, `Category`, `PlannedAmount` (`Money`).

Invariants:
- Stage names are unique within a project; `Order` values are unique and contiguous (reordering is a project operation).
- A stage can be completed only if it is `InProgress`; `CompletedOn` cannot be in the future.
- A stage's planned period must lie within the project's planned period (when both are set).
- A `Completed` project cannot be modified, except to reopen it explicitly.
- Planned amounts are non-negative.

### Expense (aggregate root)
Separate aggregate – there are many expenses, and they change independently of the plan. References the project, stage and optionally a budget item and contractor **by ID**.

- `ProjectId`, `StageId`, `BudgetItemId?`, `ContractorId?`
- `GrossAmount` (`Money`), `VatRate?`, `IncurredOn` (`DateOnly`), `Description`
- `FundingSource` (`OwnFunds`, `Loan`), `PaymentStatus` (`Unpaid`, `Paid`), `PaidOn?`
- `Documents` – attachments (file name, content type, storage key)

Invariants:
- `GrossAmount` > 0.
- `PaidOn` is set if and only if `PaymentStatus == Paid`. It cannot be in the future, but it may be earlier than `IncurredOn` (prepayments happen).
- Stage and budget item must belong to the same project – checked in the Application layer (cross-aggregate rule).

Over-budget is **allowed** (it happens in real life) but raises the `StageBudgetExceeded` domain event so the UI can warn.

### Contractor (aggregate root)
- `Name`, `Nip?`, `Trade`, `Phone?`, `Email?`, `Notes?`
- `Nip` is unique per owner when provided.

### ConstructionLoan (aggregate root, later phase)
- `ProjectId`, `BankName`, `TotalAmount`, `Tranches`
- Tranche: `Number`, `Amount`, `PlannedDate`, `RequiredStageId?`, `Status` (`Planned`, `Requested`, `Disbursed`), `DisbursedOn?`

Invariants:
- Sum of tranche amounts equals `TotalAmount`.
- Tranches are disbursed in order.
- A tranche with `RequiredStageId` can be requested only when that stage is `Completed` (cross-aggregate – checked in Application).

## Value objects

- `Money(decimal Amount, Currency Currency)` – PLN only in MVP; arithmetic only between equal currencies; rounded to 2 decimal places.
- `Nip` – 10 digits, validated with the official checksum (weights 6,5,7,2,3,4,5,6,7; sum mod 11 equals the last digit; result 10 is invalid). Accepts input with dashes/spaces, stores digits only.
- `DateRange(DateOnly Start, DateOnly? End)` – `End >= Start`.
- `Address` – street, building number, postal code (`NN-NNN`), city; or plot number (`działka`) when the address does not exist yet.
- `VatRate` – one of 0, 5, 8, 23.

## Key read models (queries)

- **Budget overview** per project: for each stage – planned, spent, remaining, % used, over-budget flag; totals for the project; split by funding source.
- **Expense list** with filters: stage, contractor, date range, payment status, funding source.
- **Upcoming**: unpaid expenses, next tranche and its required stage.

## Domain events

- `StageBudgetExceeded(ProjectId, StageId, Planned, Spent)`
- `StageCompleted(ProjectId, StageId, CompletedOn)` – later used to suggest requesting a tranche.
- `ExpenseRecorded(ExpenseId, ProjectId, StageId, Amount)`

## Out of scope (for now)

Multi-currency, multiple users per project, accounting/tax reporting, scheduling dependencies between stages (Gantt), offline mode.
