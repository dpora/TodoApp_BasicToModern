# Take-Home Technical Assessment Notes

## Task & Scope Assessment

**Prompt:** "I have been assigned the following task: 'Take-Home Technical Assessment
- You've just joined the team that owns this application: https://github.com/jin3107/TodoApp_BasicToModern
- It's a full-stack Todo application currently running in a small internal deployment
- Leadership has flagged it as a scaling concern usage is growing rapidly and the current architecture won't hold up.

Your Task
- Review the codebase and produce a Jira-style ticket in markdown for: scaling out quartz jobs horizontally
- Within the scope of the feature implementation identify issues and improve the quality of design and code as high as practicable.' Help me identify what my scope would be here."

**Reasoning:** "I wasn't sure what code I should take into consideration for this assessment, I didn't want to get stuck on an issue that wasn't even my scope."

**Plan:**
1. Look into quartz.net, find some examples and documentation
   a. Maybe look into direct examples of horizontal scaling
2. Find all implementation of quartz.net within our project
   a. Look into configuration settings
   b. Match our code with official documentation

**Our current scope (based on quartz implementation):** `appsettings.json`, `program.cs`, `JobsController.cs`, `QuartzService Extensions.cs`, `Daily Todoltem ReportJob.cs`, `TaskReminderJob.cs`, `Weekly TaskSummaryJob.cs`

**Findings:** In `appsettings.json` `"quartz.jobStore.type": "Quartz.Simpl.RAMJobStore, Quartz"`

---

## Issue Identification

**Prompt:** Our config has `quartz.jobStore.type` equal to "Quartz.Simpl.RAMJobStore, Quartz", could this be causing our scaling issue?

**Reasoning:** I am very positive that this is contributing to the scaling issue, I just want to confirm my suspicion with Al.

**Findings:** Quartz is not configured for horizontal scaling, we will need to fix this.

**Next steps:** Figure out how much change will be needed for this

---

## Clustered Quartz Requirements

**Prompt:** What change will be needed to convert Quartz to be clustered?

**Reasoning:** I want to get an idea of how much will need to be changed in order to convert Quartz to be clustered.

**Findings:**
1. **Database setup:** Need to set up our existing DB to include the schema needed for Quartz job storage
2. **Config:** Configuration files will need to be modified to handle the new storage type, and we will need to enable the clustered property.
3. **App startup:** Ensure Quartz is configured to use persistent storage
   a. i.e., add `builder.Configuration` for Quartz
4. **Job refactoring:** Jobs will need to be refactored to have the following
   a. **Stateless:** Jobs cannot rely on variables stored in memory on a specific server
   b. **Serialization:** All data passed into a job must be serializable since it is written to a DB
   c. **Idempotency (our biggest roadblock):** Ensure a job request is run exactly once
      i.
      ii.
      - Quartz ensures a job is run at least once but not exactly once
      - EX: Job A runs, stops for some reason, B picks it up, then A starts running it again.
      - Here, the same job is being run twice

---

## Job Refactoring Analysis

**Prompt:** Based on the jobs we already have in our code, what will need to be changed for the new scaling functionality?

**Reasoning:** I want to get a clear idea of what needs to be changed so I can plan generate a plan.

**Findings:**
- All jobs are currently stateless
- All data passed into jobs is serializable
- We are not ensuring idempotency

**Next steps:** Let's start by changing Quartz to allow the ability to run clustered. Then we can think about making the jobs idempotent

---

## AI Implementation & Testing

**Prompt:** "We need to configure quartz.net to scale horizontally. This will require us to change the job store type, add Quartz DB schema to migrations, and change configurations for the new DB job store, along with the new clustered settings."

**Reason:** "This seems like a good opportunity to use Al to implement these changes. Once implemented, I will review its changes."

**Notes:** I was able to get a local MySQL server running to test the DB migrations. The migrations work; however, I need to make some tweaks to what Al produced to have the program run.

**Next steps:** Let's do some research into idempotency.

---

## Idempotency Deep Dive

**Prompt:** Will we be able to handle idempotency using the default Quartz schema to track jobs?

**Reasoning:** Maybe Quartz has the ability to handle idempotency out of the box. I don't want to add a new table to track job progress if I can just use what's already available.

**Findings:** I will need to either make a new table for the idempotency key tracking of jobs.

**Prompt:** Is idempotency crucial for the jobs we're dealing with?

**Reasoning:** I'd hate to make a lot of unnecessary changes; maybe the jobs don't need this added complexity.

**Findings:** These jobs perform crucial operations that would result in visible anomalies if idempotency weren't ensured. For example, metrics could be skewed, or multiple emails could be sent to a user.

**Prompt:** Does our external email api SMPT ensure idempotency?

**Reason:** If there are calls that aren't idempotent, even if we implement a new table for the idempotency key tracking of jobs, there is a chance emails can be duplicated or not even sent at all.

---

## Conclusion

**Closing thoughts:** I have gone a bit over my two-hour limit so I will stop here. Next steps would be to ensure the internal and external jobs are idempotent. For now, I will just make a note of this in my ticket.