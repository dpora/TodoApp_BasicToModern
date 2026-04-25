# [TODO-101] Scale Quartz.NET Jobs Horizontally via Clustered Persistent JobStore

**Type:** Feature / Tech Debt Resolution
**Assignee:** Daniel Pora
**Status:** In Progress 
**Labels:** `Backend`, `Scaling`, `Quartz`, `Database`

## 📝 Background / Context
Leadership has flagged the internal Todo application as a scaling concern. Usage is growing rapidly, and the current architecture cannot support multiple horizontal instances. 

Upon codebase review, the root cause preventing horizontal scaling is the Quartz.NET configuration: `quartz.jobStore.type` is currently set to `Quartz.Simpl.RAMJobStore, Quartz`. 

In a multi-node environment, `RAMJobStore` causes each application instance to maintain its own isolated scheduler memory. This results in severe duplication of background jobs (e.g., multiple instances firing the same daily reports simultaneously) and prevents load distribution.

## 🎯 Objective
Transition the Quartz.NET implementation from an in-memory store to a clustered, database-backed JobStore (ADO.NET) to allow the application to scale horizontally safely.

## 🛠 Scope of Implementation
The following components were identified within the scope of this feature implementation:
* **Configuration:** `appsettings.json`
* **Startup/DI:** `Program.cs`, `QuartzServiceExtensions.cs`
* **Controllers:** `JobsController.cs`
* **Jobs:** `DailyTodoItemReportJob.cs`, `TaskReminderJob.cs`, `WeeklyTaskSummaryJob.cs`

## ✅ Acceptance Criteria
- [x] **Database Schema Setup:** Quartz system tables (e.g., `QRTZ_JOB_DETAILS`, `QRTZ_TRIGGERS`) are added to the application's database migrations.
- [x] **Configuration Update:** `appsettings.json` is updated to utilize the ADO.NET JobStore and a valid connection string.
- [x] **Clustering Enabled:** Quartz clustering properties (e.g., `quartz.jobStore.isClustered: true`) are enabled so multiple nodes respect database locks.
- [x] **Service Registration:** `Program.cs` / `QuartzServiceExtensions.cs` are updated to enforce persistent storage via the DI container.
- [x] **Local Verification:** Application successfully boots with a local MySQL server and successfully executes the Quartz migrations.

---

## 📂 Files Changed

* **`TodoApp.Server/src/Todo.API/appsettings.json`**
    * *Why:* Replaced the `RAMJobStore` configuration with `AdoJobStore` properties. Enabled clustering (`quartz.jobStore.clustered: true`) and mapped the MySQL driver delegate and connection settings.
* **`TodoApp.Server/src/Todo.API/Todo.API.csproj`**
    * *Why:* Added the `MySql.Data` NuGet package dependency required for ADO.NET to communicate with the MySQL database.
* **`TodoApp.Server/src/Todo.API/Program.cs`**
    * *Why:* Updated the `AddQuartzConfiguration` method call to pass the `IConfiguration` object, allowing the extension method to read the new database and cluster settings.
* **`TodoApp.Server/src/Todo.API/Extensions/QuartzServiceExtensions.cs`**
    * *Why:* Refactored the Quartz DI registration. Removed `.UseInMemoryStore()` and implemented `.UsePersistentStore()` using the MySQL provider, pulling connection strings and clustering thresholds from the injected configuration.
* **`TodoApp.Server/src/Todo.Models/Migrations/20260425120000_AddQuartzAdoJobStoreSchema.cs`**
    * *Why:* Generated a new Entity Framework migration containing the official Quartz.NET SQL initialization scripts to create the necessary `QRTZ_` tables and indexes for the cluster's shared persistent storage.

---

## 🚨 Architectural Risk: Idempotency & "At-Least-Once" Execution

### The Core Issue
By migrating Quartz.NET from an in-memory `RAMJobStore` to a clustered persistent database store, we have solved the immediate horizontal scaling issue. However, we have fundamentally shifted the system's execution guarantees from **"Exactly-Once"** (single node) to **"At-Least-Once"** (distributed cluster). 

In a clustered environment, if a node experiences a brief network partition or crash while executing a job, the database lock on that job may time out. A secondary node will then pick up the "orphaned" job and execute it. If the original node's network recovers, both nodes will complete the job, meaning the job runs twice.

### Current State Findings
An audit of our existing background jobs (`DailyTodoItemReportJob.cs`, `TaskReminderJob.cs`, `WeeklyTaskSummaryJob.cs`) yielded the following findings:
* ✅ **Statelessness:** **Passed.** Jobs do not rely on local server memory variables.
* ✅ **Serialization:** **Passed.** Data passed into jobs via `JobDataMap` is strictly serializable and safe for DB storage.
* ❌ **Idempotency (General):** **Failed.** The current `Execute` methods are not designed to be idempotent.
* ❌ **External Email API Integration:** **Failed.** The external SMTP service used within our reminder and report jobs is inherently non-idempotent. It does not support idempotency keys. If Quartz fires a job twice due to a cluster failover, the SMTP service will be called twice, and the user will receive duplicate emails.

### Impact Analysis
Because the jobs are not currently idempotent, duplicate execution during cluster failovers will cause noticeable harm:
* **User Impact:** Users will receive duplicate daily reports or task reminder emails due to the non-idempotent external SMTP API.
* **Data Impact:** Depending on the implementation of `WeeklyTaskSummaryJob`, duplicate processing might result in skewed metrics or duplicate database entries.

### Proposed Solutions (Action Items for [TODO-102])
To resolve this technical debt and ensure data integrity, the job logic must be refactored using the following patterns before scaling the application to production:

1.  **The Transactional Outbox Pattern (For External SMTP API):** Because our external email service is non-idempotent, we cannot safely invoke it directly from the Quartz job. Instead, the Quartz job must save an "EmailIntent" record to a new database Outbox table *within the exact same database transaction* that updates the business state (e.g., marking a reminder as sent). A separate, lightweight background worker will then read from this Outbox table and handle the actual dispatch to the SMTP server. This decouples the volatile external call from the job's database transaction, providing exactly-once delivery semantics for the email side-effects.

2.  **Idempotency Key Tracking (For Internal Tasks):** For internal state updates and jobs that don't involve the non-idempotent SMTP API, we will use the unique Quartz `context.FireInstanceId` as an idempotency key. We can store and validate these keys using either our **existing Redis cache** (configured with an appropriate TTL for job lock duration) or by creating a new dedicated **SQL table (e.g., `JobIdempotencyKeys`)**. The job's `Execute` method will first check this store; if the `FireInstanceId` exists, the execution is a duplicate and will be safely skipped.

3.  **Absolute Database Updates:** Ensure any database mutations use absolute state updates (e.g., `SET Status = 'Processed'`) rather than relative math (e.g., `SET Points = Points + 1`), which is highly vulnerable to double-counting.