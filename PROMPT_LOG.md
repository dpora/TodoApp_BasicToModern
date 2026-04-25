# Take-Home Assessment: Prompts and Reasoning

**Prompt 1:** "I have been assigned the following task: 'Take-Home Technical Assessment
- You've just joined the team that owns this application: https://github.com/jin3107/TodoApp_BasicToModern
- It's a full-stack Todo application currently running in a small internal deployment
- Leadership has flagged it as a scaling concern usage is growing rapidly and the current architecture won't hold up.

Your Task
- Review the codebase and produce a Jira-style ticket in markdown for: scaling out quartz jobs horizontally
- Within the scope of the feature implementation identify issues and improve the quality of design and code as high as practicable.' Help me identify what my scope would be here."
**Reasoning:** "I wasn't sure what code I should take into consideration for this assessment, I didn't want to get stuck on an issue that wasn't even my scope."

---

**Prompt 2:** Our config has `quartz.jobStore.type` equal to "Quartz.Simpl.RAMJobStore, Quartz", could this be causing our scaling issue?
**Reasoning:** I am very positive that this is contributing to the scaling issue, I just want to confirm my suspicion with Al.

---

**Prompt 3:** What change will be needed to convert Quartz to be clustered?
**Reasoning:** I want to get an idea of how much will need to be changed in order to convert Quartz to be clustered.

---

**Prompt 4:** Based on the jobs we already have in our code, what will need to be changed for the new scaling functionality?
**Reasoning:** I want to get a clear idea of what needs to be changed so I can plan generate a plan.

---

**Prompt 5:** "We need to configure quartz.net to scale horizontally. This will require us to change the job store type, add Quartz DB schema to migrations, and change configurations for the new DB job store, along with the new clustered settings."
**Reasoning:** "This seems like a good opportunity to use Al to implement these changes. Once implemented, I will review its changes."

---

**Prompt 6:** Will we be able to handle idempotency using the default Quartz schema to track jobs?
**Reasoning:** Maybe Quartz has the ability to handle idempotency out of the box. I don't want to add a new table to track job progress if I can just use what's already available.

---

**Prompt 7:** Is idempotency crucial for the jobs we're dealing with?
**Reasoning:** I'd hate to make a lot of unnecessary changes; maybe the jobs don't need this added complexity.

---

**Prompt 8:** Does our external email api SMPT ensure idempotency?
**Reasoning:** If there are calls that aren't idempotent, even if we implement a new table for the idempotency key tracking of jobs, there is a chance emails can be duplicated or not even sent at all.
